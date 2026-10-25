using FluentAssertions;
using PaymentBackend.BL.Mapper;
using PaymentBackend.Common.Model;

namespace PaymentBackend.BL.Test.Mapper
{
    public class BillHttpMapperTest
    {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private IBillHttpMapper _classUnderTest;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new BillHttpMapper(new FullPaymentDto2HttpPaymentMapper());
        }

        #region MapMinimizedBills

        [Test]
        public void MapMinimizedBills_ShouldReturnEmptyList_WhenThereAreNoBills()
        {
            // Act
            var result = _classUnderTest.MapMinimizedBills([]);

            // Assert
            result.Should().BeEmpty();
        }

        [Test]
        public void MapMinimizedBills_ShouldMapReceiverPayerAndAmount()
        {
            // Arrange: Ben pays Anna 12.34 €
            List<MinimizedBill> minimizedBills = [new MinimizedBill(issuedBy: "Anna", issuedFor: "Ben", amount: 12.34m)];

            // Act
            var result = _classUnderTest.MapMinimizedBills(minimizedBills);

            // Assert
            result.Should().ContainSingle();
            result[0].IssuedBy.Should().Be("Anna");
            result[0].IssuedFor.Should().Be("Ben");
            result[0].Amount.Should().Be(12.34);
        }

        [Test]
        public void MapMinimizedBills_ShouldKeepTheOrder()
        {
            // Arrange
            List<MinimizedBill> minimizedBills =
            [
                new MinimizedBill("Anna", "Ben", 1),
                new MinimizedBill("Anna", "Carl", 2),
                new MinimizedBill("Dora", "Emil", 3)
            ];

            // Act
            var result = _classUnderTest.MapMinimizedBills(minimizedBills);

            // Assert
            result.Select(bill => bill.Amount).Should().Equal(1, 2, 3);
        }

        #endregion
    }
}
