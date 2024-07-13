using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaymentBackend.Common.Exceptions;
using PaymentBackend.Database;
using PaymentBackend.Database.DatabaseServices;
using PaymentBackend.Settings;

namespace PaymentBackend.BL.Http
{
    public interface IPaymentResolver
    {
        Task<IActionResult> GetPayments();
        Task<IActionResult> GetPaymentById(long paymentId);
        Task<IActionResult> ProcessNewPaymentAsync(HttpRequest req);
    }

    public class PaymentResolver : AbstractDatabaseService, IPaymentResolver
    {
        private readonly IPaymentDatabaseService _paymentDbService;
        private readonly IUserDatabaseService _userDatabaseService;
        private readonly IPostPaymentDatabaseService _postPaymentDbService;

        public PaymentResolver(IPaymentDatabaseService paymentDbService,
            IUserDatabaseService userDatabaseService,
            IPostPaymentDatabaseService postPaymentDbService,
            ISqlExceptionHandler sqlExceptionHandler,
            IFunctionSettingsResolver functionSettingsResolver,
            ILogger<PaymentResolver> logger) : base(sqlExceptionHandler, functionSettingsResolver, logger)
        {
            _paymentDbService = paymentDbService;
            _userDatabaseService = userDatabaseService;
            _postPaymentDbService = postPaymentDbService;
        }

        public Task<IActionResult> GetPayments()
        {
            var allPayments = _paymentDbService.SelectAllPayments();

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

            return Task.FromResult<IActionResult>(new JsonResult(response));
        }

        public Task<IActionResult> GetPaymentById(long paymentId)
        {
            var resolvedPayment = _paymentDbService.SelectPaymentById(paymentId);

            if (resolvedPayment == null)
            {
                return Task.FromResult<IActionResult>(new NotFoundResult());
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

            return Task.FromResult<IActionResult>(new JsonResult(response));
        }

        public async Task<IActionResult> ProcessNewPaymentAsync(HttpRequest req)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Common.Generated.PostPaymentRequest postPayment;
            try
            {
                postPayment = JsonConvert.DeserializeObject<Common.Generated.PostPaymentRequest>(requestBody);
            }
            catch (Exception e)
            {
                _logger.LogError($"Bad request: " + e.Message);
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }

            try
            {
                ValidatePayment(postPayment);
            }
            catch (PaymentValidationException e)
            {
                _logger.LogError($"Validation failed for payment: {e.Message}");
            }

            /*
             * Map request object into internal dto
             */
            Common.Model.Dto.InsertPaymentDto dto;
            try
            {
                dto = BuildInsertPaymentDto(postPayment);
            }
            catch (UserNotFoundException e)
            {
                _logger.LogError($"User not found: " + e.Message);
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
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
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return new OkObjectResult(paymentId);
        }


        private Common.Model.Dto.InsertPaymentDto BuildInsertPaymentDto(Common.Generated.PostPaymentRequest postPayment)
        {
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
                Author = author,
                Creditor = creditor,
                Debitors = debitors,
                Price = Convert.ToDecimal(postPayment.Payment.Price),
                PaymentDate = postPayment.Payment.PaymentDate.DateTime,
                UpdateTime = DateTime.UtcNow,
                Description = postPayment.Payment.PaymentDescription
            };
        }

        private void ValidatePayment(Common.Generated.PostPaymentRequest postPayment)
        {
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
        }
    }
}
