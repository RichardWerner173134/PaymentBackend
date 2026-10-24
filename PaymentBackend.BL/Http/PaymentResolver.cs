using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using PaymentBackend.Common.Exceptions;
using PaymentBackend.Database.DatabaseServices;
using PaymentBackend.Settings;
using Newtonsoft.Json;

namespace PaymentBackend.BL.Http
{
    public interface IPaymentResolver
    {
        Task<HttpResponseData> GetPaymentsAsync(HttpRequestData req, long paymentContext);
        Task<HttpResponseData> GetPaymentByIdAsync(HttpRequestData req, long paymentContext, long paymentId);
        Task<HttpResponseData> ProcessNewPaymentAsync(HttpRequestData req, long paymentContext);
        Task<HttpResponseData> DeletePaymentByIdAsync(HttpRequestData req, long paymentContext, long paymentId);
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

        public async Task<HttpResponseData> GetPaymentsAsync(HttpRequestData req, long paymentContext)
        {
            var allPayments = await _paymentDbService.SelectAllPaymentsAsync(paymentContext);

            var mappedPayments = allPayments.Select(payment => new Common.Generated.Payment
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

            return await BuildOkResponseAsync(req, response);
        }

        public async Task<HttpResponseData> GetPaymentByIdAsync(HttpRequestData req, long paymentContext, long paymentId)
        {
            var resolvedPayment = await _paymentDbService.SelectPaymentByIdAsync(paymentContext, paymentId);

            if (resolvedPayment == null)
            {
                return await BuildNotFoundResponseAsync(req);
            }

            var mappedPayment = new Common.Generated.Payment
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
                Payments = new List<Common.Generated.Payment> { mappedPayment }
            };

            return await BuildOkResponseAsync(req, response);
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
                    return await BuildBadRequestResponseAsync(req);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Bad request: " + e.Message);
                return await BuildBadRequestResponseAsync(e, req);
            }

            try
            {
                ValidatePayment(paymentContext, postPayment);
            }
            catch (PaymentValidationException e)
            {
                _logger.LogError($"Validation failed for payment: {e.Message}");
                return await BuildBadRequestResponseAsync(e, req);
            }

            /*
             * Map request object into internal dto
             */
            Common.Model.Dto.InsertPaymentDto dto;
            try
            {
                dto = await BuildInsertPaymentDtoAsync(paymentContext, postPayment);
            }
            catch (UserNotFoundException e)
            {
                _logger.LogError($"User not found: " + e.Message);
                return await BuildBadRequestResponseAsync(e, req);
            }
            catch (PaymentContextNotFoundException e)
            {
                _logger.LogError($"PaymentContext not found: {e.Message}");
                return await BuildBadRequestResponseAsync(e, req);
            }

            /*
             * process the new dto
             * might fail when there are users that cannot be resolved via username
             */
            long paymentId;
            try
            {
                paymentId = await _postPaymentDbService.InsertPaymentAsync(dto);
            }
            catch (Exception e)
            {
                _logger.LogError($"Unexpected Error: {e.Message}");
                return await BuildInternalServerErrorResponseAsync(e, req);
            }

            return await BuildOkResponseAsync(req);
        }

        public async Task<HttpResponseData> DeletePaymentByIdAsync(HttpRequestData req, long paymentContext, long paymentId)
        {
            Common.Model.Dto.FullPaymentDto? payment = await _paymentDbService.SelectPaymentByIdAsync(paymentContext, paymentId);

            if (payment == null)
            {
                return await BuildNotFoundResponseAsync(req);
            }

            await _paymentDbService.MarkPaymentAsDeletedAsync(paymentContext, paymentId);

            return await BuildOkResponseAsync(req);
        }


        private async Task<Common.Model.Dto.InsertPaymentDto> BuildInsertPaymentDtoAsync(long paymentContext, Common.Generated.PostPaymentRequest postPayment)
        {
            // resolve the payment context
            Common.Model.PaymentContext? resolvedPaymentContext = await _paymentContextDatabaseService.SelectPaymentContextByIdAsync(paymentContext);
            if (resolvedPaymentContext == null)
            {
                throw new PaymentContextNotFoundException($"Can´t resolve PaymentContext [{paymentContext}]");
            }

            if (resolvedPaymentContext.IsClosed)
            {
                throw new PaymentContextClosedException($"Can´t create new payment. Payment context already closed.");
            }

            // resolve the author 
            Common.Model.User? author = await _userDatabaseService.SelectUserByUsernameAsync(postPayment.Payment.Author);
            if (author == null)
            {
                throw new UserNotFoundException($"Can´t resolve author [{postPayment.Payment.Author}].");
            }

            // resolve the creditor
            Common.Model.User? creditor = await _userDatabaseService.SelectUserByUsernameAsync(postPayment.Payment.Creditor);
            if (creditor == null)
            {
                throw new UserNotFoundException($"Can´t resolve author [{postPayment.Payment.Creditor}].");
            }

            // resolve all debitors
            List<Common.Model.User> debitors = [];
            foreach (var debitorUsername in postPayment.Payment.Debitors)
            {
                Common.Model.User? debitor = await _userDatabaseService.SelectUserByUsernameAsync(debitorUsername);
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
