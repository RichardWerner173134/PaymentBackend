using FluentAssertions;
using PaymentBackend.BL.Core;
using PaymentBackend.Common.Model;
using PaymentBackend.Common.Model.Dto;

namespace PaymentBackend.BL.Test.Core
{
    public class Tests
    {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private IBillCalculationService _classUnderTest;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [SetUp]
        public void Setup()
        {

            _classUnderTest = new BillCalculationService();
        }

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
                    Debitors = new List<string>() { "FirstDebitor", "SecondDebitor" },
                    PaymentDate = new(),
                    Price = 100,
                    UpdateTime = new(),
                    PaymentDescription = "XXX"
                }
            };

            // Act
            var result = _classUnderTest.GetBills(data);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(2);

            var result1 = result[0];
            var result2 = result[1];

            result1.Should().NotBeNull();
            result1.IssuedBy.Should().Be("Gelehrter");
            result1.IssuedFor.Should().Be("FirstDebitor");
            result1.Amount.Should().Be(50);
            result1.GetIncludedPayments().Should().HaveCount(1);

            result2.Should().NotBeNull();
            result2.IssuedBy.Should().Be("Gelehrter");
            result2.IssuedFor.Should().Be("SecondDebitor");
            result2.Amount.Should().Be(50);
            result2.GetIncludedPayments().Should().HaveCount(1);
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
            result1.GetIncludedPayments().Should().HaveCount(1);
            result1.GetIncludedPayments()[0].AmountPerDebitor.Should().Be(-100);
            result1.GetIncludedPayments()[1].AmountPerDebitor.Should().Be(120);
        }

    }
}