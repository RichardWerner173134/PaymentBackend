using FluentAssertions;
using PaymentBackend.Common.Model;
using PaymentBackend.Common.Model.Dto;

namespace PaymentBackend.Common.Test.Model
{
    public class BillTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Bill_ShouldInitializeBillCorrectly()
        {
            // Arrange
            string creditor = "creditor";
            string debitor = "debitor";

            // Act
            Bill bill = new(creditor, debitor);

            // Assert
            bill.Should().NotBeNull();
            bill.Amount.Should().Be(0);
            bill.IssuedBy.Should().Be(creditor);
            bill.IssuedFor.Should().Be(debitor);
            bill.GetBillComposites().Should().BeEmpty();
        }

        [Test]
        public void AddBillComposite_ShouldAddCorrectly_WhenInitializedEmpty()
        {
            // Arrange
            string creditor = "creditor";
            string debitor = "debitor";

            FullPaymentDto payment1 = new()
            {
                Id = 1,
                Creditor = creditor,
                Debitors = new List<string> { creditor, debitor },
                Price = 42
                // ... other fields dont matter
            };

            FullPaymentDto payment2 = new()
            {
                Id = 2,
                Creditor = creditor,
                Debitors = new List<string> { creditor, debitor },
                Price = 24
                // ... other fields dont matter
            };

            // Act
            Bill bill = new(creditor, debitor);
            bill.AddBillComposite(payment1, creditor, debitor);

            // Assert
            bill.Should().NotBeNull();
            bill.IssuedBy.Should().Be(creditor);
            bill.IssuedFor.Should().Be(debitor);
            bill.Amount.Should().Be((decimal)21);
            
            List<BillComposite> billComposites = bill.GetBillComposites();
            billComposites.Should().NotBeNullOrEmpty();
            
            BillComposite billComposite1 = billComposites[0];
            billComposite1.Should().NotBeNull();
            billComposite1.AmountPerDebitor.Should().Be((decimal)42 / (decimal)2);
            billComposite1.FullPayment.Should().Be(payment1);

            // Act
            bill.AddBillComposite(payment2, creditor, debitor);
            
            // Assert
            bill.Should().NotBeNull();
            bill.IssuedBy.Should().Be(creditor);
            bill.IssuedFor.Should().Be(debitor);
            bill.Amount.Should().Be((decimal)21 + 12);

            billComposites = bill.GetBillComposites();
            billComposites.Should().NotBeNullOrEmpty();
            
            billComposite1 = billComposites[0];
            billComposite1.Should().NotBeNull();
            billComposite1.AmountPerDebitor.Should().Be((decimal)42 / (decimal)2);
            billComposite1.FullPayment.Should().Be(payment1);

            BillComposite billComposite2 = billComposites[1];
            billComposite2.Should().NotBeNull();
            billComposite2.AmountPerDebitor.Should().Be((decimal)24 / (decimal)2);
            billComposite2.FullPayment.Should().Be(payment2);
        }

        [Test]
        public void AddBillToComposite_ShouldDoNothing_WhenTryingToAddSameCompositeTwice()
        {
            // Arrange
            string creditor = "creditor";
            string debitor = "debitor";

            FullPaymentDto payment1 = new()
            {
                Id = 1,
                Creditor = creditor,
                Debitors = new List<string> { creditor, debitor },
                Price = 42
                // ... other fields dont matter
            };

            FullPaymentDto payment2 = new()
            {
                Id = 1, // must be the same as the Id of payment1
                Creditor = creditor,
                Debitors = new List<string> { creditor, debitor },
                Price = 24
                // ... other fields dont matter
            };

            // Act
            Bill bill = new(creditor, debitor);
            bill.AddBillComposite(payment1, creditor, debitor);
            bill.AddBillComposite(payment2, creditor, debitor);

            // Assert
            bill.Should().NotBeNull();
            bill.IssuedBy.Should().Be(creditor);
            bill.IssuedFor.Should().Be(debitor);
            bill.Amount.Should().Be((decimal)21);

            List<BillComposite> billComposites = bill.GetBillComposites();
            billComposites.Should().NotBeNullOrEmpty();

            BillComposite billComposite1 = billComposites[0];
            billComposite1.Should().NotBeNull();
            billComposite1.AmountPerDebitor.Should().Be((decimal)42 / (decimal)2);
            billComposite1.FullPayment.Should().Be(payment1);
        }

        [Test]
        public void AddBillToComposite_ShouldSwapCreditorAndDebitor_WhenAmountBelowZero()
        {
            // Arrange
            string creditor = "creditor";
            string debitor = "debitor";

            FullPaymentDto payment1 = new()
            {
                Id = 1,
                Creditor = creditor,
                Debitors = new List<string> { debitor },
                Price = 42
                // ... other fields dont matter
            };

            FullPaymentDto payment2 = new()
            {
                Id = 2,
                Creditor = debitor,
                Debitors = new List<string> { creditor},
                Price = (decimal)42.01
                // ... other fields dont matter
            };

            // Act
            Bill bill = new(creditor, debitor);
            bill.AddBillComposite(payment1, creditor, debitor);
            bill.AddBillComposite(payment2, debitor, creditor);

            // Assert
            bill.Should().NotBeNull();
            bill.IssuedBy.Should().Be(debitor);
            bill.IssuedFor.Should().Be(creditor);
            bill.Amount.Should().Be((decimal)42.01 - (decimal)42);

            List<BillComposite> billComposites = bill.GetBillComposites();
            billComposites.Should().NotBeNullOrEmpty();
            billComposites.Count.Should().Be(2);

            BillComposite billComposite1 = billComposites[0];
            billComposite1.Should().NotBeNull();
            billComposite1.AmountPerDebitor.Should().Be(-1 * (decimal)42);
            billComposite1.FullPayment.Should().Be(payment1);

            BillComposite billComposite2 = billComposites[1];
            billComposite2.Should().NotBeNull();
            billComposite2.AmountPerDebitor.Should().Be((decimal)42.01);
            billComposite2.FullPayment.Should().Be(payment2);
        }


        [Test]
        public void AddBillToComposite_ShouldThrowArgumentException_WhenTryingToWrongBill()
        {
            // Arrange
            string creditor = "creditor";
            string debitor = "debitor";

            FullPaymentDto payment1 = new()
            {
                Id = 1,
                Creditor = creditor,
                Debitors = new List<string> { creditor, debitor },
                Price = 42
                // ... other fields dont matter
            };

            FullPaymentDto payment2 = new()
            {
                Id = 1, // must be the same as the Id of payment1
                Creditor = creditor,
                Debitors = new List<string> { creditor, debitor },
                Price = 24
                // ... other fields dont matter
            };

            // Act
            Bill bill = new(creditor, debitor);
            Action act = () => bill.AddBillComposite(payment1, "falscherCreditor", "falscherDebitor");
            act.Should().Throw<ArgumentException>().WithMessage(
                $"Invalid combination of creditor=[falscherCreditor] and debitor=[falscherDebitor]. Cant add payment to bill with IssuedBy=[{creditor}] and IssuedFor=[{debitor}]");

            // Assert
            bill.Should().NotBeNull();
            bill.IssuedBy.Should().Be(creditor);
            bill.IssuedFor.Should().Be(debitor);
            bill.Amount.Should().Be(0);

            List<BillComposite> billComposites = bill.GetBillComposites();
            billComposites.Should().BeEmpty();
        }
    }
}