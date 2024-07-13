using FluentAssertions;
using PaymentBackend.Common.Model;
using PaymentBackend.Common.Model.Dto;

namespace PaymentBackend.Common.Test.Model
{
    public class BillCompositeTest
    {
        [Test]
        public void BillComposite_ShouldGetInitialized_WithoutDirectionChange_WithPayment()
        {
            // Arrange
            FullPaymentDto payment = new()
            {
                Debitors = new List<string>() { "d1", "d2" },
                Price = (decimal)42.9
            };

            // Act
            BillComposite composite = new(payment, true);

            // Assert
            composite.Should().NotBeNull();
            composite.FullPayment.Should().Be(payment);
            composite.AmountPerDebitor.Should().Be((decimal)42.9 / (decimal)2);
        }

        [Test]
        public void BillComposite_ShouldGetInitialized_WithDirectionChange_WithPayment()
        {
            // Arrange
            FullPaymentDto payment = new()
            {
                Debitors = new List<string>() { "d1", "d2" },
                Price = (decimal)42.9
            };

            // Act
            BillComposite composite = new(payment, false);

            // Assert
            composite.Should().NotBeNull();
            composite.FullPayment.Should().Be(payment);
            composite.AmountPerDebitor.Should().Be(-1 * (decimal)42.9 / (decimal)2);
        }

        [Test]
        public void InvertAmount_ShouldInvert()
        {
            // Arrange
            FullPaymentDto payment = new()
            {
                Debitors = new List<string>() { "d1", "d2" },
                Price = (decimal)42.9
            };

            BillComposite composite = new(payment, true);

            composite.Should().NotBeNull();
            composite.FullPayment.Should().Be(payment);
            decimal expectedAmountPerDebitor = (decimal)42.9 / (decimal)2;
            composite.AmountPerDebitor.Should().Be(expectedAmountPerDebitor);

            // Act 
            composite.InvertAmount();

            // Assert
            composite.Should().NotBeNull();
            composite.FullPayment.Should().Be(payment);
            composite.AmountPerDebitor.Should().Be(-1 * expectedAmountPerDebitor);
        }
    }
}
