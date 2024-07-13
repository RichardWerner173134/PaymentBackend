using PaymentBackend.BL.Core;
using PaymentBackend.Common.Model.Dto;
using FluentAssertions;

namespace PaymentBackend.BL.Test.Core
{
    public class PaymentOverviewCalculatorTest
    {
#pragma warning disable IDE0044
        private IPaymentOverviewCalculator _classUnderTest;
#pragma warning restore IDE0044

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new PaymentOverviewCalculator();
        }

        #region GetPaymentOverviewForCreditor

        [Test]
        public void GetPaymentOverviewForCreditor_ShouldSumAllTotalsInBaseCase_WhenCreditorDidntPayForHimself()
        {
            // Arrange
            string creditor = "Richard";
            List<FullPaymentDto> payments = new()
            {
                new FullPaymentDto
                {
                    Id = 1,
                    Price = 20,
                    Creditor = creditor,
                    Debitors = new List<string>(){ "Günther" }, // no creditor in here
                    Author = creditor,
                    PaymentDate = new(),
                    PaymentDescription = "blubablub",
                    UpdateTime = new()
                },
                new FullPaymentDto
                {
                    Id = 1,
                    Price = 40,
                    Creditor = creditor,
                    Debitors = new List<string>(){ "Günther", "Dieter" }, // no creditor in here
                    Author = "",
                    PaymentDate = new(),
                    PaymentDescription = "",
                    UpdateTime = new()
                }
            };

            // Act
            var result = _classUnderTest.GetPaymentOverviewForCreditor(payments, creditor);

            // Assert
            result.Should().NotBeNull();
            result.Payments.Count.Should().Be(2);
            result.TotalWithCreditor.Should().Be(20 + 40);
            var expectedTotalWithoutCreditor = Decimal.ToDouble(20 + 40);
            result.TotalWithoutCreditor.Should().Be(expectedTotalWithoutCreditor);
        }

        [Test]
        public void GetPaymentOverviewForCreditor_ShouldRemovePortionFromTotalWithoutCreditor_WhenCreditorPayedForHimself()
        {
            // Arrange
            string creditor = "Richard";
            List<FullPaymentDto> payments = new()
            {
                new FullPaymentDto
                {
                    Id = 1,
                    Price = 20,
                    Creditor = creditor,
                    Debitors = new List<string>(){ creditor, "Günther" }, // 1/2 of this price will be removed
                    Author = creditor,
                    PaymentDate = new(),
                    PaymentDescription = "blubablub",
                    UpdateTime = new()
                },
                new FullPaymentDto
                {
                    Id = 1,
                    Price = 40,
                    Creditor = creditor,
                    Debitors = new List<string>(){ creditor, "Günther", "Dieter" }, // 1/3 of this price will be removed
                    Author = "",
                    PaymentDate = new(),
                    PaymentDescription = "",
                    UpdateTime = new()
                }
            };

            // Act
            var result = _classUnderTest.GetPaymentOverviewForCreditor(payments, creditor);

            // Assert
            result.Should().NotBeNull();
            result.Payments.Count.Should().Be(2);
            result.TotalWithCreditor.Should().Be(20 + 40);
            var expectedTotalWithoutCreditor = Decimal.ToDouble((20 + 40) - ((decimal)20 / (decimal)2) - ((decimal)40 / (decimal)3));
            result.TotalWithoutCreditor.Should().Be(expectedTotalWithoutCreditor);
        }

        #endregion

        #region GetPaymentOverviewForDebitor

        [Test]
        public void GetPaymentOverviewForDebitor_ShouldCalculateTotalDebitorOnly()
        {
            // Arrange
            string debitor = "Günther";
            List<FullPaymentDto> payments = new()
            {
                new FullPaymentDto
                {
                    Id = 1,
                    Price = 20,
                    Creditor = "Richard",
                    Debitors = new List<string>(){ debitor, "Richard" }, // 1/2 of this price is the portion Günther wants to know
                    Author = "Richard",
                    PaymentDate = new(),
                    PaymentDescription = "blubablub",
                    UpdateTime = new()
                },
                new FullPaymentDto
                {
                    Id = 1,
                    Price = 40,
                    Creditor = "Richard",
                    Debitors = new List<string>(){ debitor, "Dieter", "Uschi" }, // 1/3 of this price is the portion Günther wants to know
                    Author = "Richard",
                    PaymentDate = new(),
                    PaymentDescription = "",
                    UpdateTime = new()
                }
            };

            // Act
            var result = _classUnderTest.GetPaymentOverviewForDebitor(payments, debitor);

            // Assert
            result.Should().NotBeNull();
            result.Payments.Count.Should().Be(2);
            var expectedTotalDebitorOnly = Decimal.ToDouble(((decimal)20 / (decimal)2) + ((decimal)40 / (decimal)3));
            result.TotalDebitorOnly.Should().Be(expectedTotalDebitorOnly);
        }

        #endregion

        #region CaseSensitivity

        [Test]
        public void GetPaymentOverviewForCreditor_ShouldRemovePortionFromTotalWithoutCreditor_WhenCreditorPayedForHimself_WithWeirdCase()
        {
            // Arrange
            string creditor = "Richard";
            List<FullPaymentDto> payments = new()
            {
                new FullPaymentDto
                {
                    Id = 1,
                    Price = 20,
                    Creditor = creditor,
                    Debitors = new List<string>(){ "rICHarD", "Günther" }, // 1/2 of this price will be removed
                    Author = creditor,
                    PaymentDate = new(),
                    PaymentDescription = "blubablub",
                    UpdateTime = new()
                },
                new FullPaymentDto
                {
                    Id = 1,
                    Price = 40,
                    Creditor = creditor,
                    Debitors = new List<string>(){ "RICHARd", "Günther", "Dieter" }, // 1/3 of this price will be removed
                    Author = "",
                    PaymentDate = new(),
                    PaymentDescription = "",
                    UpdateTime = new()
                }
            };

            // Act
            var result = _classUnderTest.GetPaymentOverviewForCreditor(payments, creditor);

            // Assert
            result.Should().NotBeNull();
            result.Payments.Count.Should().Be(2);
            result.TotalWithCreditor.Should().Be(20 + 40);
            var expectedTotalWithoutCreditor = Decimal.ToDouble((20 + 40) - ((decimal)20 / (decimal)2) - ((decimal)40 / (decimal)3));
            result.TotalWithoutCreditor.Should().Be(expectedTotalWithoutCreditor);
        }

        [Test]
        public void GetPaymentOverviewForDebitor_ShouldCalculateTotalDebitorOnly_WithWeirdCase()
        {
            // Arrange
            string debitor = "gÜntHER";
            List<FullPaymentDto> payments = new()
            {
                new FullPaymentDto
                {
                    Id = 1,
                    Price = 20,
                    Creditor = "Richard",
                    Debitors = new List<string>(){ "günther", "Richard" }, // 1/2 of this price is the portion Günther wants to know
                    Author = "Richard",
                    PaymentDate = new(),
                    PaymentDescription = "blubablub",
                    UpdateTime = new()
                },
                new FullPaymentDto
                {
                    Id = 1,
                    Price = 40,
                    Creditor = "Richard",
                    Debitors = new List<string>(){ "GünTHer", "Dieter", "Uschi" }, // 1/3 of this price is the portion Günther wants to know
                    Author = "Richard",
                    PaymentDate = new(),
                    PaymentDescription = "",
                    UpdateTime = new()
                }
            };

            // Act
            var result = _classUnderTest.GetPaymentOverviewForDebitor(payments, debitor);

            // Assert
            result.Should().NotBeNull();
            result.Payments.Count.Should().Be(2);
            var expectedTotalDebitorOnly = Decimal.ToDouble(((decimal)20 / (decimal)2) + ((decimal)40 / (decimal)3));
            result.TotalDebitorOnly.Should().Be(expectedTotalDebitorOnly);
        }


        #endregion
    }
}
