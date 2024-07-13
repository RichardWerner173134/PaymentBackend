using FluentAssertions;
using PaymentBackend.BL.Core;
using PaymentBackend.Common.Model.Dto;

namespace PaymentBackend.BL.Test.Core
{
    public class BillCalculationServiceTest
    {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private IBillCalculationService _classUnderTest;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new BillCalculationService();
        }

        #region GetBills

        [Test]
        public void GetBills_ShouldReturnListOfData()
        {
            // Arrange
            List<FullPaymentDto> data = new()
            {
                new()
                {
                    Id = 1,
                    Author = "Gelehrter",
                    Creditor = "Gelehrter",
                    Debitors = new List<string>(){ "Gelehrter", "Richard" },
                    PaymentDate = new(),
                    Price = 100,
                    UpdateTime = new(),
                    PaymentDescription = "Payment with Creditor paying for himself"
                },
                new()
                {
                    Id = 1,
                    Author = "Gelehrter",
                    Creditor = "Gelehrter",
                    Debitors = new List<string>() { "Richard" },
                    PaymentDate = new(),
                    Price = 20,
                    UpdateTime = new(),
                    PaymentDescription = "Payment without Creditor paying for himself"
                }
            };

            // Act
            var result = _classUnderTest.GetBills(data);

            // Assert
            result.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void GetBills_ShouldCreateABillForEachDebitorInPayment()
        {
            // Arrange
            List<FullPaymentDto> data = new()
            {
                new()
                {
                    Id = 1,
                    Author = "Gelehrter",
                    Creditor = "Gelehrter",
                    Debitors = new List<string>() { "FirstDebitor", "SecondDebitor", "ThirdDebitor" },
                    PaymentDate = new(),
                    Price = 120,
                    UpdateTime = new(),
                    PaymentDescription = "XXX"
                }
            };

            // Act
            var result = _classUnderTest.GetBills(data);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(3);

            var result1 = result[0];
            var result2 = result[1];
            var result3 = result[2];

            result1.Should().NotBeNull();
            result1.IssuedBy.Should().Be("Gelehrter");
            result1.IssuedFor.Should().Be("FirstDebitor");
            result1.Amount.Should().Be(40);
            result1.GetBillComposites().Should().HaveCount(1);

            result2.Should().NotBeNull();
            result2.IssuedBy.Should().Be("Gelehrter");
            result2.IssuedFor.Should().Be("SecondDebitor");
            result2.Amount.Should().Be(40);
            result2.GetBillComposites().Should().HaveCount(1);

            result3.Should().NotBeNull();
            result3.IssuedBy.Should().Be("Gelehrter");
            result3.IssuedFor.Should().Be("ThirdDebitor");
            result3.Amount.Should().Be(40);
            result3.GetBillComposites().Should().HaveCount(1);
        }

        [Test]
        public void GetBills_ShouldCreateABillForEachDebitorInEachPayment()
        {
            // Arrange
            List<FullPaymentDto> data = new()
            {
                new()
                {
                    Id = 1,
                    Author = "Gelehrter",
                    Creditor = "Gelehrter",
                    Debitors = new List<string>() { "FirstDebitor", "SecondDebitor", "ThirdDebitor" },
                    PaymentDate = new(),
                    Price = 120,
                    UpdateTime = new(),
                    PaymentDescription = "XXX"
                },
                new()
                {
                    Id = 2,
                    Author = "Richard",
                    Creditor = "Richard",
                    Debitors = new List<string>() { "FourthDebitor", "FifthDebitor" },
                    PaymentDate = new(),
                    Price = 120,
                    UpdateTime = new(),
                    PaymentDescription = "XXX"
                }
            };

            // Act
            var result = _classUnderTest.GetBills(data);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(5);

            var result1 = result[0];
            var result2 = result[1];
            var result3 = result[2];
            var result4 = result[3];
            var result5 = result[4];

            result1.Should().NotBeNull();
            result1.IssuedBy.Should().Be("Gelehrter");
            result1.IssuedFor.Should().Be("FirstDebitor");
            result1.Amount.Should().Be(40);
            result1.GetBillComposites().Should().HaveCount(1);

            result2.Should().NotBeNull();
            result2.IssuedBy.Should().Be("Gelehrter");
            result2.IssuedFor.Should().Be("SecondDebitor");
            result2.Amount.Should().Be(40);
            result2.GetBillComposites().Should().HaveCount(1);

            result3.Should().NotBeNull();
            result3.IssuedBy.Should().Be("Gelehrter");
            result3.IssuedFor.Should().Be("ThirdDebitor");
            result3.Amount.Should().Be(40);
            result3.GetBillComposites().Should().HaveCount(1);

            result4.Should().NotBeNull();
            result4.IssuedBy.Should().Be("Richard");
            result4.IssuedFor.Should().Be("FourthDebitor");
            result4.Amount.Should().Be(60);
            result4.GetBillComposites().Should().HaveCount(1);

            result5.Should().NotBeNull();
            result5.IssuedBy.Should().Be("Richard");
            result5.IssuedFor.Should().Be("FifthDebitor");
            result5.Amount.Should().Be(60);
            result5.GetBillComposites().Should().HaveCount(1);
        }

        [Test]
        public void GetBills_ShouldChangeIssuedBy_WhenAmountGoesUnderZero()
        {
            // Arrange
            List<FullPaymentDto> data = new()
            {
                new()
                {
                    Id = 1,
                    Author = "Gelehrter",
                    Creditor = "Gelehrter",
                    Debitors = new List<string>() { "FirstDebitor" },
                    PaymentDate = new(),
                    Price = 100,
                    UpdateTime = new(),
                    PaymentDescription = "XXX"
                },
                new()
                {
                    Id = 2,
                    Author = "FirstDebitor",
                    Creditor = "FirstDebitor",
                    Debitors = new List<string>() { "Gelehrter" },
                    PaymentDate = new(),
                    Price = 120,
                    UpdateTime = new(),
                    PaymentDescription = "XXX"
                }
            };

            // Act
            var result = _classUnderTest.GetBills(data);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(1);

            var result1 = result[0];

            result1.Should().NotBeNull();
            result1.IssuedBy.Should().Be("FirstDebitor");
            result1.IssuedFor.Should().Be("Gelehrter");
            result1.Amount.Should().Be(20);
            result1.GetBillComposites().Should().HaveCount(2);
            result1.GetBillComposites()[0].AmountPerDebitor.Should().Be(-100);
            result1.GetBillComposites()[1].AmountPerDebitor.Should().Be(120);
        }

        [Test]
        public void GetBills_ShouldCreateABillForEachPair()
        {
            // Arrange
            List<FullPaymentDto> data = new()
            {
                new()
                {
                    Id = 1,
                    Author = "Gelehrter",
                    Creditor = "Gelehrter",
                    Debitors = new List<string>() { "Richard", "Florian" }, // 50 each
                    PaymentDate = new(),
                    Price = 100,
                    UpdateTime = new(),
                    PaymentDescription = "XXX"
                },
                new()
                {
                    Id = 2,
                    Author = "Richard",
                    Creditor = "Richard",
                    Debitors = new List<string>() { "Gelehrter", "Bombe", "Florian" }, // 40 each
                    PaymentDate = new(),
                    Price = 120,
                    UpdateTime = new(),
                    PaymentDescription = "XXX"
                }
            };

            // Act
            var result = _classUnderTest.GetBills(data);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(4);

            var result1 = result[0];
            var result2 = result[1];
            var result3 = result[2];
            var result4 = result[3];

            result1.Should().NotBeNull();
            result1.IssuedBy.Should().Be("Gelehrter");
            result1.IssuedFor.Should().Be("Richard");
            result1.Amount.Should().Be(50 - 40);
            result1.GetBillComposites().Should().HaveCount(2);

            result2.Should().NotBeNull();
            result2.IssuedBy.Should().Be("Gelehrter");
            result2.IssuedFor.Should().Be("Florian");
            result2.Amount.Should().Be(50);
            result2.GetBillComposites().Should().HaveCount(1);

            result3.Should().NotBeNull();
            result3.IssuedBy.Should().Be("Richard");
            result3.IssuedFor.Should().Be("Bombe");
            result3.Amount.Should().Be(40);
            result3.GetBillComposites().Should().HaveCount(1);

            result4.Should().NotBeNull();
            result4.IssuedBy.Should().Be("Richard");
            result4.IssuedFor.Should().Be("Florian");
            result4.Amount.Should().Be(40);
            result4.GetBillComposites().Should().HaveCount(1);
        }

        /*
         * the response uses the casing that appeared first when the respective pair is creeated
         * the function gets its input from the mapped db-objects so only the database usernames in original casing are used here
         */
        [Test]
        public void GetBills_ShouldCreateABillForEachPair_WithWeirdCase()
        {
            // Arrange
            List<FullPaymentDto> data = new()
            {
                new()
                {
                    Id = 1,
                    Author = "geLehrTer",
                    Creditor = "GelehRTEr",
                    Debitors = new List<string>() { "riCHard", "flORIan" }, // 50 each
                    PaymentDate = new(),
                    Price = 100,
                    UpdateTime = new(),
                    PaymentDescription = "XXX"
                },
                new()
                {
                    Id = 2,
                    Author = "Richard",
                    Creditor = "RichArd",
                    Debitors = new List<string>() { "GelehrTeR", "Bombe", "Florian" }, // 40 each
                    PaymentDate = new(),
                    Price = 120,
                    UpdateTime = new(),
                    PaymentDescription = "XXX"
                }
            };

            // Act
            var result = _classUnderTest.GetBills(data);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(4);

            var result1 = result[0];
            var result2 = result[1];
            var result3 = result[2];
            var result4 = result[3];

            result1.Should().NotBeNull();
            result1.IssuedBy.Should().Be("GelehRTEr");
            result1.IssuedFor.Should().Be("riCHard");
            result1.Amount.Should().Be(50 - 40);
            result1.GetBillComposites().Should().HaveCount(2);

            result2.Should().NotBeNull();
            result2.IssuedBy.Should().Be("GelehRTEr");
            result2.IssuedFor.Should().Be("flORIan");
            result2.Amount.Should().Be(50);
            result2.GetBillComposites().Should().HaveCount(1);

            result3.Should().NotBeNull();
            result3.IssuedBy.Should().Be("RichArd");
            result3.IssuedFor.Should().Be("Bombe");
            result3.Amount.Should().Be(40);
            result3.GetBillComposites().Should().HaveCount(1);

            result4.Should().NotBeNull();
            result4.IssuedBy.Should().Be("RichArd");
            result4.IssuedFor.Should().Be("Florian");
            result4.Amount.Should().Be(40);
            result4.GetBillComposites().Should().HaveCount(1);
        }

        #endregion
    }
}