using PaymentBackend.Common.Model;
using PaymentBackend.Common.Model.Dto;

namespace PaymentBackend.BL.Core
{
    public interface IBillCalculationService
    {
        List<Bill> GetBills(List<FullPaymentDto> payments);
        double GetBalanceForUser(List<Bill> bills, string username);
    }

    public class BillCalculationService : IBillCalculationService
    {
        public List<Bill> GetBills(List<FullPaymentDto> payments)
        {
            List<Bill> result = new();

            foreach (var payment in payments)
            {
                var pairs = payment.Debitors
                    .Where(debitor => payment.Creditor.ToLower().Equals(debitor.ToLower()) == false)
                    .Select(debitor => (Creditor: payment.Creditor, Debitor: debitor))
                    .ToList();

                foreach (var pair in pairs)
                {
                    Bill targetBill;

                    Bill? matchedByDebitor = result.Find(bill => 
                        bill.IssuedFor.ToLower().Equals(pair.Debitor.ToLower()) 
                        && 
                        bill.IssuedBy.ToLower().Equals(pair.Creditor.ToLower()));

                    Bill? matchedByCreditor = result.Find(bill => 
                        bill.IssuedFor.ToLower().Equals(pair.Creditor.ToLower()) 
                        && 
                        bill.IssuedBy.ToLower().Equals(pair.Debitor.ToLower()));

                    /*
                     * creditor is the current issuer of the bill
                     */
                    if (matchedByCreditor != null)
                    {
                        targetBill = matchedByCreditor;
                    }

                    /*
                     * debitor is the current issuer of the bill
                     */
                    else if (matchedByDebitor != null)
                    {
                        targetBill = matchedByDebitor;
                    }

                    /*
                     * dont know that pairing of creditor and debitor
                     */
                    else
                    {
                        targetBill = new(pair.Creditor, pair.Debitor);
                        result.Add(targetBill);
                    }

                    // add the payment to the matched bill
                    targetBill.AddIncludedPayment(payment, pair.Creditor, pair.Debitor);
                }
            }

            return result;
        }

        public double GetBalanceForUser(List<Bill> bills, string username)
        {
            return bills.Sum(bill => bill.IssuedBy.ToLower().Equals(username.ToLower()) ? bill.Amount : -bill.Amount);
        }
    }
}
