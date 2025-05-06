using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using PaymentBackend.Common.Exceptions;
using PaymentBackend.Database;
using PaymentBackend.Database.DatabaseServices;
using PaymentBackend.Settings;
using Newtonsoft.Json;

namespace PaymentBackend.BL.Http
{
    public interface IPaymentResolver
    {
        Task<HttpResponseData> GetPayments(HttpRequestData req, long paymentContext);
        Task<HttpResponseData> GetPaymentById(HttpRequestData req, long paymentContext, long paymentId);
        Task<HttpResponseData> ProcessNewPaymentAsync(HttpRequestData req, long paymentContext);
        Task<HttpResponseData> DeletePaymentById(HttpRequestData req, long paymentContext, long paymentId);
    }

    public class PaymentResolver : AbstractHttpResolver, IPaymentResolver
    {
        private readonly IPaymentDatabaseService _paymentDbService;
        private readonly IUserDatabaseService _userDatabaseService;
        private readonly IPostPaymentDatabaseService _postPaymentDbService;
        private readonly IPaymentContextDatabaseService _paymentContextDatabaseService;
        private readonly ILogger _logger;

        public PaymentResolver(IPaymentDatabaseService paymentDbService,
            IUserDatabaseService userDatabaseService,
            IPostPaymentDatabaseService postPaymentDbService,
            ISqlExceptionHandler sqlExceptionHandler,
            IFunctionSettingsResolver functionSettingsResolver,
            ILogger<PaymentResolver> logger,
            IPaymentContextDatabaseService paymentContextDatabaseService
        ) 
        {
            _paymentDbService = paymentDbService;
            _userDatabaseService = userDatabaseService;
            _postPaymentDbService = postPaymentDbService;
            _paymentContextDatabaseService = paymentContextDatabaseService;
            _logger = logger;
        }

        public async Task<HttpResponseData> GetPayments(HttpRequestData req, long paymentContext)
        {
            var allPayments = _paymentDbService.SelectAllPayments(paymentContext);

            var mappedPayments = allPayments.Select(payment => new Common.Generated.Payment()
            {
                PaymentId = payment.Id,
                Price = decimal.ToDouble(payment.Price),
                Creditor = payment.Creditor,
                Debitors = payment.Debitors,
                Author = payment.Author,
                PaymentDate = payment.PaymentDate,
                UpdateTime = payment.UpdateTime,
                PaymentDescription = payment.PaymentDescription
            }).ToList();

            Common.Generated.GetPaymentsResponse response = new()
            {
                Payments = mappedPayments
            };

            return await BuildOkResponse(req, response);
        }

        public async Task<HttpResponseData> GetPaymentById(HttpRequestData req, long paymentContext, long paymentId)
        {
            var resolvedPayment = _paymentDbService.SelectPaymentById(paymentContext, paymentId);

            if (resolvedPayment == null)
            {
                return await BuildNotFoundResponse(req);
            }

            var mappedPayment = new Common.Generated.Payment()
            {
                PaymentId = resolvedPayment.Id,
                Price = decimal.ToDouble(resolvedPayment.Price),
                Creditor = resolvedPayment.Creditor,
                Debitors = resolvedPayment.Debitors,
                Author = resolvedPayment.Author,
                PaymentDate = resolvedPayment.PaymentDate,
                UpdateTime = resolvedPayment.UpdateTime,
                PaymentDescription = resolvedPayment.PaymentDescription
            };

            Common.Generated.GetPaymentsResponse response = new()
            {
                Payments = new List<Common.Generated.Payment>() { mappedPayment }
            };

            return await BuildOkResponse(req, response);
        }

        public async Task<HttpResponseData> ProcessNewPaymentAsync(HttpRequestData req, long paymentContext)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Common.Generated.PostPaymentRequest? postPayment;
            try
            {
                postPayment = JsonConvert.DeserializeObject<Common.Generated.PostPaymentRequest>(requestBody);

                if (postPayment == null)
                {
                    return await BuildBadRequestResponse(req);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Bad request: " + e.Message);
                return await BuildBadRequestResponse(e, req);
            }

            try
            {
                ValidatePayment(paymentContext, postPayment);
            }
            catch (PaymentValidationException e)
            {
                _logger.LogError($"Validation failed for payment: {e.Message}");
                return await BuildBadRequestResponse(e, req);
            }

            /*
             * Map request object into internal dto
             */
            Common.Model.Dto.InsertPaymentDto dto;
            try
            {
                dto = BuildInsertPaymentDto(paymentContext, postPayment);
            }
            catch (UserNotFoundException e)
            {
                _logger.LogError($"User not found: " + e.Message);
                return await BuildBadRequestResponse(e, req);
            }
            catch (PaymentContextNotFoundException e)
            {
                _logger.LogError($"PaymentContext not found: {e.Message}");
                return await BuildBadRequestResponse(e, req);
            }

            /*
             * process the new dto
             * might fail when there are users that cannot be resolved via username
             */
            long paymentId;
            try
            {
                paymentId = _postPaymentDbService.InsertPayment(dto);
            }
            catch (Exception e)
            {
                _logger.LogError($"Unexpected Error: {e.Message}");
                return await BuildInternalServerErrorResponse(e, req);
            }

            return await BuildOkResponse(req);
        }

        public async Task<HttpResponseData> DeletePaymentById(HttpRequestData req, long paymentContext, long paymentId)
        {
            Common.Model.Dto.FullPaymentDto? payment = _paymentDbService.SelectPaymentById(paymentContext, paymentId);

            if (payment == null)
            {
                return await BuildNotFoundResponse(req);
            }

            _paymentDbService.MarkPaymentAsDeleted(paymentContext, paymentId);

            return await BuildOkResponse(req);
        }


        private Common.Model.Dto.InsertPaymentDto BuildInsertPaymentDto(long paymentContext, Common.Generated.PostPaymentRequest postPayment)
        {
            // resolve the payment context
            Common.Model.PaymentContext? resolvedPaymentContext = _paymentContextDatabaseService.SelectPaymentContextById(paymentContext);
            if (resolvedPaymentContext == null)
            {
                throw new PaymentContextNotFoundException($"Can´t resolve PaymentContext [{paymentContext}]");
            }

            if (resolvedPaymentContext.IsClosed)
            {
                throw new PaymentContextClosedException($"Can´t create new payment. Payment context already closed.");
            }

            // resolve the author 
            Common.Model.User? author = _userDatabaseService.SelectUserByUsername(postPayment.Payment.Author);
            if (author == null)
            {
                throw new UserNotFoundException($"Can´t resolve author [{postPayment.Payment.Author}].");
            }

            // resolve the creditor
            Common.Model.User? creditor = _userDatabaseService.SelectUserByUsername(postPayment.Payment.Creditor);
            if (creditor == null)
            {
                throw new UserNotFoundException($"Can´t resolve author [{postPayment.Payment.Creditor}].");
            }

            // resolve all debitors
            List<Common.Model.User> debitors = new();
            foreach (var debitorUsername in postPayment.Payment.Debitors)
            {
                Common.Model.User? debitor = _userDatabaseService.SelectUserByUsername(debitorUsername);
                if (debitor == null)
                {
                    throw new UserNotFoundException($"Can´t resolve a debitor [{debitorUsername}].");
                }

                debitors.Add(debitor);
            }

            return new Common.Model.Dto.InsertPaymentDto
            {
                PaymentContext = paymentContext,
                Author = author,
                Creditor = creditor,
                Debitors = debitors,
                Price = Convert.ToDecimal(postPayment.Payment.Price),
                PaymentDate = postPayment.Payment.PaymentDate.DateTime,
                UpdateTime = DateTime.UtcNow,
                Description = postPayment.Payment.PaymentDescription,
                IsDeleted = 0
            };
        }

        private void ValidatePayment(long paymentContext, Common.Generated.PostPaymentRequest postPayment)
        {
#pragma warning disable CS0472 // Das Ergebnis des Ausdrucks lautet immer gleich, da ein Wert dieses Typs niemals 'null' entspricht
            if (paymentContext == null || paymentContext < 1)
#pragma warning restore CS0472 // Das Ergebnis des Ausdrucks lautet immer gleich, da ein Wert dieses Typs niemals 'null' entspricht
            {
                throw new PaymentValidationException("PaymentContext was null");
            }

            Common.Generated.PostPayment newPayment = postPayment.Payment;

            if (newPayment.Price <= 0)
            {
                throw new PaymentValidationException("Price must be greater than zero");
            }

            if (newPayment.Debitors.Any() == false)
            {
                throw new PaymentValidationException("Debitors cant be empty");
            }

            if (newPayment.Debitors.Count == 1 && newPayment.Debitors.ElementAt(0).ToLower().Equals(newPayment.Creditor.ToLower()))
            {
                throw new PaymentValidationException("The only debitor cant be the creditor");
            }

            if (newPayment.PaymentDate > DateTime.UtcNow)
            {
                throw new PaymentValidationException("PaymentDate cant be in the future");
            }

            bool hasDuplicate = postPayment.Payment.Debitors
                .GroupBy(d => d.ToLower())
                .Any(g => g.Count() > 1);

            if (hasDuplicate)
            {
                throw new PaymentValidationException("Each Debitor must be unique");
            }
        }
    }
}
