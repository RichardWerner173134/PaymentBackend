using FluentAssertions;
using PaymentBackend.BL.Core;
using PaymentBackend.Common.Model;
using PaymentBackend.Common.Model.Dto;

namespace PaymentBackend.BL.Test.Core
{
    /*
     * Reading guide: Owes("Ben", "Anna", 10) means "Ben owes Anna 10 €", i.e. a bill issued by Anna for Ben.
     * A minimized bill is a transfer: IssuedFor pays Amount to IssuedBy.
     */
    public class BillMinimizationServiceTest
    {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private IBillMinimizationService _classUnderTest;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private long _nextPaymentId;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new BillMinimizationService();
            _nextPaymentId = 1;
        }

        #region MinimizeBills - basics

        [Test]
        public void MinimizeBills_ShouldReturnNoTransfers_WhenThereAreNoBills()
        {
            // Act
            var result = _classUnderTest.MinimizeBills([]);

            // Assert
            result.Should().BeEmpty();
        }

        [Test]
        public void MinimizeBills_ShouldKeepTheBill_WhenThereIsOnlyOne()
        {
            // Arrange
            List<Bill> bills = [Owes("Ben", "Anna", 25)];

            // Act
            var result = _classUnderTest.MinimizeBills(bills);

            // Assert
            result.Should().ContainSingle();
            ShouldContainTransfer(result, from: "Ben", to: "Anna", amount: 25);
        }

        [Test]
        public void MinimizeBills_ShouldReturnNoTransfers_WhenAllDebtsCancelEachOtherOut()
        {
            // Arrange: everybody owes the next one 10 € in a circle, so nobody has to pay anything
            List<Bill> bills =
            [
                Owes("Ben", "Anna", 10),
                Owes("Carl", "Ben", 10),
                Owes("Anna", "Carl", 10)
            ];

            // Act
            var result = _classUnderTest.MinimizeBills(bills);

            // Assert
            result.Should().BeEmpty();
        }

        [Test]
        public void MinimizeBills_ShouldOffsetBillsInOppositeDirections()
        {
            // Arrange: Ben owes Anna 10 €, Anna owes Ben 4 € -> Ben pays the difference
            List<Bill> bills =
            [
                Owes("Ben", "Anna", 10),
                Owes("Anna", "Ben", 4)
            ];

            // Act
            var result = _classUnderTest.MinimizeBills(bills);

            // Assert
            result.Should().ContainSingle();
            ShouldContainTransfer(result, from: "Ben", to: "Anna", amount: 6);
        }

        #endregion

        #region MinimizeBills - fewer transfers

        [Test]
        public void MinimizeBills_ShouldReplaceAChainWithADirectTransfer()
        {
            // Arrange: Carl owes Ben 10 €, Ben owes Anna 10 € -> Ben only passes the money on, Carl can pay Anna directly
            List<Bill> bills =
            [
                Owes("Carl", "Ben", 10),
                Owes("Ben", "Anna", 10)
            ];

            // Act
            var result = _classUnderTest.MinimizeBills(bills);

            // Assert
            result.Should().ContainSingle();
            ShouldContainTransfer(result, from: "Carl", to: "Anna", amount: 10);
            ShouldKeepEveryBalance(bills, result);
        }

        [Test]
        public void MinimizeBills_ShouldNeedOneTransferLessThanUsers_WhenNoSubgroupBalancesOut()
        {
            // Arrange: 4 users whose debts are tangled up in 4 bills
            // balances: Anna +30, Ben -5, Carl -10, Dora -15 -> only Anna receives money, so 3 transfers are needed
            List<Bill> bills =
            [
                Owes("Ben", "Anna", 10),
                Owes("Carl", "Anna", 10),
                Owes("Dora", "Anna", 10),
                Owes("Dora", "Ben", 5)
            ];

            // Act
            var result = _classUnderTest.MinimizeBills(bills);

            // Assert
            result.Should().HaveCount(3);
            ShouldKeepEveryBalance(bills, result);
        }

        [Test]
        public void MinimizeBills_ShouldSettleIndependentGroupsSeparately()
        {
            // Arrange: Anna and Ben only have to do with each other, Carl and Dora as well
            List<Bill> bills =
            [
                Owes("Ben", "Anna", 20),
                Owes("Dora", "Carl", 20)
            ];

            // Act
            var result = _classUnderTest.MinimizeBills(bills);

            // Assert: no mixed transfers like Ben -> Carl
            result.Should().HaveCount(2);
            ShouldContainTransfer(result, from: "Ben", to: "Anna", amount: 20);
            ShouldContainTransfer(result, from: "Dora", to: "Carl", amount: 20);
        }

        [Test]
        public void MinimizeBills_ShouldFindTheMinimum_WhereASimpleSettlementNeedsMoreTransfers()
        {
            // Arrange: balances Anna +4, Ben +3, Carl -3, Dora -2, Emil -2
            // Matching the largest receiver with the largest payer needs 4 transfers.
            // The minimum is 3: {Carl -> Ben} and {Dora -> Anna, Emil -> Anna}
            List<Bill> bills =
            [
                Owes("Carl", "Anna", 3),
                Owes("Dora", "Anna", 1),
                Owes("Dora", "Ben", 1),
                Owes("Emil", "Ben", 2)
            ];

            // Act
            var result = _classUnderTest.MinimizeBills(bills);

            // Assert
            result.Should().HaveCount(3);
            ShouldContainTransfer(result, from: "Carl", to: "Ben", amount: 3);
            ShouldContainTransfer(result, from: "Dora", to: "Anna", amount: 2);
            ShouldContainTransfer(result, from: "Emil", to: "Anna", amount: 2);
        }

        [Test]
        public void MinimizeBills_ShouldKeepEveryBalance_ForARealisticVacation()
        {
            // Arrange: a few vacation payments, turned into bills by the regular bill calculation
            List<FullPaymentDto> payments =
            [
                Payment(creditor: "Anna", price: 300, "Anna", "Ben", "Carl", "Dora"),  // hotel
                Payment(creditor: "Ben", price: 80, "Ben", "Carl"),                    // dinner
                Payment(creditor: "Carl", price: 45, "Anna", "Dora"),                  // museum
                Payment(creditor: "Dora", price: 120, "Anna", "Ben", "Carl", "Dora"),  // rental car
                Payment(creditor: "Ben", price: 30, "Dora")                            // taxi
            ];
            List<Bill> bills = new BillCalculationService().GetBills(payments);

            // Act
            var result = _classUnderTest.MinimizeBills(bills);

            // Assert
            result.Count.Should().BeLessThanOrEqualTo(3, "4 users never need more than 3 transfers");
            result.Count.Should().BeLessThan(bills.Count);
            ShouldKeepEveryBalance(bills, result);
        }

        #endregion

        #region MinimizeBills - rounding

        [Test]
        public void MinimizeBills_ShouldRoundToCents()
        {
            // Arrange: Anna pays 10 € for Anna, Ben and Carl -> Ben and Carl owe 3.333... € each
            List<Bill> bills = new BillCalculationService().GetBills(
            [
                Payment(creditor: "Anna", price: 10, "Anna", "Ben", "Carl")
            ]);

            // Act
            var result = _classUnderTest.MinimizeBills(bills);

            // Assert
            result.Should().HaveCount(2);
            ShouldContainTransfer(result, from: "Ben", to: "Anna", amount: 3.33m);
            ShouldContainTransfer(result, from: "Carl", to: "Anna", amount: 3.33m);
        }

        [Test]
        public void MinimizeBills_ShouldChangeNoBalanceByMoreThanOneCent_WhenRoundingLeavesSeveralCentsOver()
        {
            // Arrange: two payments split into thirds. Rounding every balance to cents leaves 2 cents over,
            // they must be spread so that no balance moves by more than one cent
            List<Bill> bills = new BillCalculationService().GetBills(
            [
                Payment(creditor: "Anna", price: 10, "Anna", "Ben", "Carl"),
                Payment(creditor: "Dora", price: 10, "Dora", "Emil", "Finn")
            ]);

            // Act
            var result = _classUnderTest.MinimizeBills(bills);

            // Assert
            result.Sum(transfer => transfer.Amount).Should().BeGreaterThan(0);
            ShouldKeepEveryBalance(bills, result, tolerance: 0.01m);
        }

        #endregion

        #region MinimizeBills - usernames and limits

        [Test]
        public void MinimizeBills_ShouldTreatUsernamesCaseInsensitive()
        {
            // Arrange: "Ben" and "ben" are the same user, so this is a chain like Carl -> Ben -> Anna
            List<Bill> bills =
            [
                Owes("Carl", "ben", 10),
                Owes("Ben", "Anna", 10)
            ];

            // Act
            var result = _classUnderTest.MinimizeBills(bills);

            // Assert
            result.Should().ContainSingle();
            ShouldContainTransfer(result, from: "Carl", to: "Anna", amount: 10);
        }

        [Test]
        public void MinimizeBills_ShouldReturnTheSameTransfers_WhenCalledTwice()
        {
            // Arrange
            List<Bill> bills =
            [
                Owes("Carl", "Anna", 3),
                Owes("Dora", "Anna", 1),
                Owes("Dora", "Ben", 1),
                Owes("Emil", "Ben", 2)
            ];

            // Act
            var first = _classUnderTest.MinimizeBills(bills);
            var second = _classUnderTest.MinimizeBills(bills);

            // Assert
            second.Should().BeEquivalentTo(first, options => options.WithStrictOrdering());
        }

        [Test]
        public void MinimizeBills_ShouldStillSettleEverything_WhenThereAreTooManyUsersForTheExactSearch()
        {
            // Arrange: 26 users with an open balance, more than the exact search handles (20)
            List<Bill> bills = [];
            for (int i = 1; i <= 25; i++)
            {
                bills.Add(Owes($"User{i}", "Anna", i));
                bills.Add(Owes($"User{i + 1}", $"User{i}", 1));
            }

            // Act
            var result = _classUnderTest.MinimizeBills(bills);

            // Assert: the result may not be minimal anymore, but it has to be correct
            result.Count.Should().BeLessThan(bills.Count);
            ShouldKeepEveryBalance(bills, result);
        }

        #endregion

        /// <summary>A bill saying that <paramref name="payer"/> owes <paramref name="receiver"/> the amount.</summary>
        private Bill Owes(string payer, string receiver, decimal amount)
        {
            Bill bill = new(receiver, payer);
            bill.AddBillComposite(Payment(creditor: receiver, price: amount, payer), receiver, payer);
            return bill;
        }

        private FullPaymentDto Payment(string creditor, decimal price, params string[] debitors)
        {
            return new FullPaymentDto
            {
                Id = _nextPaymentId++,
                Author = creditor,
                Creditor = creditor,
                Debitors = [.. debitors],
                Price = price,
                PaymentDescription = "test payment",
                PaymentContextIsClosed = true
            };
        }

        private static void ShouldContainTransfer(List<MinimizedBill> result, string from, string to, decimal amount)
        {
            result.Should().ContainSingle(
                transfer => transfer.IssuedFor == from && transfer.IssuedBy == to && transfer.Amount == amount,
                $"{from} should transfer {amount} to {to}");
        }

        /// <summary>
        /// The core promise of the minimization: everybody pays or receives the same total as with the original bills.
        /// Also every transfer is a real one: a positive amount between two different users.
        /// </summary>
        private static void ShouldKeepEveryBalance(List<Bill> bills, List<MinimizedBill> transfers, decimal tolerance = 0)
        {
            Dictionary<string, decimal> expected = BalancesOf(bills.Select(bill => (bill.IssuedBy, bill.IssuedFor, bill.Amount)));
            Dictionary<string, decimal> actual = BalancesOf(transfers.Select(bill => (bill.IssuedBy, bill.IssuedFor, bill.Amount)));

            foreach (var (user, balance) in expected)
            {
                actual.GetValueOrDefault(user).Should().BeApproximately(balance, tolerance, $"the balance of {user} must not change");
            }

            transfers.Should().OnlyContain(transfer => transfer.Amount > 0);
            transfers.Should().OnlyContain(transfer => !transfer.IssuedBy.Equals(transfer.IssuedFor, StringComparison.OrdinalIgnoreCase));
        }

        private static Dictionary<string, decimal> BalancesOf(IEnumerable<(string Receiver, string Payer, decimal Amount)> bills)
        {
            Dictionary<string, decimal> balances = new(StringComparer.OrdinalIgnoreCase);

            foreach (var (receiver, payer, amount) in bills)
            {
                balances[receiver] = balances.GetValueOrDefault(receiver) + amount;
                balances[payer] = balances.GetValueOrDefault(payer) - amount;
            }

            return balances;
        }
    }
}
