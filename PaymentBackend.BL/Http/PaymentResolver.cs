using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaymentBackend.Common.Exceptions;
using PaymentBackend.Common.Generated;
using PaymentBackend.Common.Model.Dto;
using PaymentBackend.Database;
using PaymentBackend.Database.DatabaseServices;
using PaymentBackend.Settings;
using User = PaymentBackend.Common.Model.User;

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

            GetPaymentsResponse response = new()
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

            GetPaymentsResponse response = new()
            {
                Payments = new List<Payment>() { mappedPayment }
            };

            return Task.FromResult<IActionResult>(new JsonResult(response));
        }

        public async Task<IActionResult> ProcessNewPaymentAsync(HttpRequest req)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            PostPaymentRequest postPayment;
            try
            {
                postPayment = JsonConvert.DeserializeObject<Common.Generated.PostPaymentRequest>(requestBody);
            }
            catch (Exception e)
            {
                _logger.LogError($"Bad request: " + e.Message);
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }

            /*
             * Map request object into internal dto
             */

            InsertPaymentDto dto;
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

        private InsertPaymentDto BuildInsertPaymentDto(PostPaymentRequest postPayment)
        {
            // resolve the author 
            var author = _userDatabaseService.SelectUserByUsername(postPayment.Payment.Author);
            if (author == null)
            {
                throw new UserNotFoundException($"Can´t resolve author [{postPayment.Payment.Author}].");
            }

            // resolve the creditor
            var creditor = _userDatabaseService.SelectUserByUsername(postPayment.Payment.Creditor);
            if (creditor == null)
            {
                throw new UserNotFoundException($"Can´t resolve author [{postPayment.Payment.Creditor}].");
            }

            // resolve all debitors
            List<User> debitors = new();
            foreach (var debitorUsername in postPayment.Payment.Debitors)
            {
                var debitor = _userDatabaseService.SelectUserByUsername(debitorUsername);
                if (debitor == null)
                {
                    throw new UserNotFoundException($"Can´t resolve a debitor [{debitorUsername}].");
                }

                debitors.Add(debitor);
            }

            return new InsertPaymentDto
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
    }
}
