using PaymentBackend.Common.Model;
using PaymentBackend.Common.Model.Dto;
using static System.Decimal;

namespace PaymentBackend.BL.Core
{
    public interface IPaymentOverviewCalculator
    {
        PaymentOverviewForCreditor GetPaymentOverviewForCreditor(List<FullPaymentDto> payments, string creditor);
        PaymentOverviewForDebitor GetPaymentOverviewForDebitor(List<FullPaymentDto> payments, string username);
    }

    public class PaymentOverviewCalculator : IPaymentOverviewCalculator
    {
        /// <summary>
        /// contains all the payments, that the creditor payed for
        /// totalWithCreditor is the sum of all payment prices
        /// totalWithoutCreditor is the sum without the amount of money the creditor payed for himself
        ///
        /// p1 = {10, u1, [u2, u3]}
        /// p2 = {20, u1, [u1, u3]}
        /// p3 = {12, u1, [u1, u2, u3, u4]}
        ///
        /// totalWithoutCreditor =
        /// (10 + 20 + 12) - (
        ///     20 * (1/2)
        ///     12 * (1/4)
        /// )
        /// = 42 - (10 + 3)
        /// = 42 - 13
        /// = 29
        /// 
        /// </summary>
        /// <param name="payments">a list of payments for one creditor</param>
        /// <param name="creditor"></param>
        /// <returns>PaymentOverviewForCreditor</returns>
        public PaymentOverviewForCreditor GetPaymentOverviewForCreditor(List<FullPaymentDto> payments, string creditor)
        {
            decimal totalWithCreditor = payments.Sum(payment => payment.Price);
            decimal totalCreditorOnly = 0;
            foreach (var payment in payments)
            {
                if (payment.Debitors.Select(debitor => debitor.ToLower()).Contains(creditor.ToLower()))
                {
                    totalCreditorOnly += payment.Price * ((decimal)1 / (decimal)payment.Debitors.Count);
                }
            }

            decimal totalWithoutCreditor = totalWithCreditor - totalCreditorOnly;

            PaymentOverviewForCreditor result = new()
            {
                Payments = payments,
                TotalWithCreditor = ToDouble(totalWithCreditor),
                TotalWithoutCreditor = ToDouble(totalWithoutCreditor)
            };

            return result;
        }

        /// <summary>
        /// totalDebitorOnly is the sum of all portions of payments for a debitor
        ///
        /// p1 = {20, u1, [u1, u2]}
        /// p2 = {30, u4, [u1, u3, u4]}
        ///
        /// totalDebitorOnly = 20 / ()
        /// 
        /// </summary>
        /// <param name="payments">a list of payments for a debitor</param>
        /// <param name="username"></param>
        /// <returns></returns>
        public PaymentOverviewForDebitor GetPaymentOverviewForDebitor(List<FullPaymentDto> payments, string username)
        {
            decimal totalDebitorOnly = payments.Sum(payment => payment.Price * ((decimal)1 / (decimal)payment.Debitors.Count));

            PaymentOverviewForDebitor result = new()
            {
                Payments = payments,
                TotalDebitorOnly = ToDouble(totalDebitorOnly)
            };

            return result;
        }
    }
}
