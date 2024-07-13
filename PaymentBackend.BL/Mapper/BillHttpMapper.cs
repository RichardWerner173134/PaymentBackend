namespace PaymentBackend.BL.Mapper
{
    public interface IBillHttpMapper
    {
        List<Common.Generated.Bill> MapBills(List<Common.Model.Bill> internalBills);
        List<Common.Generated.ShortBill> MapShortBills(List<Common.Model.Bill> internalBills);
    }

    public class BillHttpMapper : IBillHttpMapper
    {
        private readonly IFullPaymentDto2HttpPaymentMapper _paymentHttpMapper;

        public BillHttpMapper(IFullPaymentDto2HttpPaymentMapper paymentHttpMapper)
        {
            _paymentHttpMapper = paymentHttpMapper;
        }

        public List<Common.Generated.Bill> MapBills(List<Common.Model.Bill> internalBills)
        {
            return internalBills.Select(MapBill).ToList();
        }

        public List<Common.Generated.ShortBill> MapShortBills(List<Common.Model.Bill> internalBills)
        {
            return internalBills.Select(MapShortBill).ToList();
        }

        private Common.Generated.Bill MapBill(Common.Model.Bill bill)
        {
            List<Common.Generated.BillComposite> mappedComposites = MapBillComposites(bill.GetBillComposites());

            Common.Generated.Bill result = new()
            {
                IssuedBy = bill.IssuedBy,
                IssuedFor = bill.IssuedFor,
                Amount = Decimal.ToDouble(bill.Amount),
                BillComposites = mappedComposites
            };

            return result;
        }

        private Common.Generated.ShortBill MapShortBill(Common.Model.Bill bill)
        {
            List<Common.Generated.BillComposite> mappedComposites = MapBillComposites(bill.GetBillComposites());

            Common.Generated.ShortBill result = new()
            {
                IssuedBy = bill.IssuedBy,
                IssuedFor = bill.IssuedFor,
                Amount = Decimal.ToDouble(bill.Amount)
            };

            return result;
        }

        private List<Common.Generated.BillComposite> MapBillComposites(List<Common.Model.BillComposite> composites)
        {
            List<Common.Generated.BillComposite> result = new();

            foreach (var composite in composites)
            {
                Common.Generated.Payment mappedPayment = _paymentHttpMapper.ConvertPayment(composite.FullPayment);
                Common.Generated.BillComposite mappedComposite = new()
                {
                    Payment = mappedPayment,
                    Amount = Decimal.ToDouble(composite.AmountPerDebitor)
                };

                result.Add(mappedComposite);
            }

            return result;
        }
    }
}
