using System.Numerics;
using PaymentBackend.Common.Model;

namespace PaymentBackend.BL.Core
{
    public interface IBillMinimizationService
    {
        List<MinimizedBill> MinimizeBills(List<Bill> bills);
    }

    /*
     * Settles all bills with the smallest possible number of transfers.
     *
     * Every user ends up with the same balance as with the original bills. A group of k users whose balances
     * sum up to zero can always be settled with k - 1 transfers, so the number of transfers is minimal when the
     * users are split into as many zero-sum groups as possible.
     */
    public class BillMinimizationService : IBillMinimizationService
    {
        // the exact search is exponential in the number of users with an open balance
        private const int MaxUsersForExactSearch = 20;

        public List<MinimizedBill> MinimizeBills(List<Bill> bills)
        {
            List<Balance> balances = GetBalancesInCents(bills);

            if (balances.Count == 0)
            {
                return [];
            }

            // above the limit all users form one group, the greedy settlement stays correct but is not always minimal
            List<List<Balance>> groups = balances.Count <= MaxUsersForExactSearch
                ? SplitIntoMaximumNumberOfZeroSumGroups(balances)
                : [balances];

            return [.. groups.SelectMany(SettleGroup)];
        }

        private static List<Balance> GetBalancesInCents(List<Bill> bills)
        {
            Dictionary<string, (string Username, decimal Amount)> balancesByUser = new(StringComparer.OrdinalIgnoreCase);

            foreach (var bill in bills)
            {
                AddToBalance(balancesByUser, bill.IssuedBy, bill.Amount);
                AddToBalance(balancesByUser, bill.IssuedFor, -bill.Amount);
            }

            var exactBalances = balancesByUser.Values
                .OrderBy(balance => balance.Username, StringComparer.OrdinalIgnoreCase)
                .Select(balance => (balance.Username, ExactCents: balance.Amount * 100))
                .ToList();

            List<Balance> balances = [.. exactBalances
                .Select(balance => new Balance(balance.Username, (long)Math.Round(balance.ExactCents, MidpointRounding.AwayFromZero)))];

            // rounding every balance to cents can leave a few cents over, they go one by one to the users
            // whose balance was rounded the most in that direction, so no balance moves by more than one cent
            long roundingRest = balances.Sum(balance => balance.Cents);
            int direction = Math.Sign(roundingRest);

            List<int> mostRoundedUsers = [.. Enumerable.Range(0, balances.Count)
                .OrderByDescending(index => (balances[index].Cents - exactBalances[index].ExactCents) * direction)
                .Take((int)Math.Abs(roundingRest))];

            foreach (int index in mostRoundedUsers)
            {
                balances[index] = balances[index] with { Cents = balances[index].Cents - direction };
            }

            return [.. balances.Where(balance => balance.Cents != 0)];
        }

        private static void AddToBalance(Dictionary<string, (string Username, decimal Amount)> balancesByUser, string username, decimal amount)
        {
            if (balancesByUser.TryGetValue(username, out var balance))
            {
                balancesByUser[username] = (balance.Username, balance.Amount + amount);
            }
            else
            {
                balancesByUser[username] = (username, amount);
            }
        }

        /*
         * dp[mask] = maximum number of zero-sum groups when the users in mask are removed one after another.
         * Removing user i from mask leads to dp[mask without i], and mask itself counts as a group border when its
         * balances sum up to zero. Walking back from the full mask yields the users in removal order, and every
         * zero-sum mask on that way closes a group.
         */
        private static List<List<Balance>> SplitIntoMaximumNumberOfZeroSumGroups(List<Balance> balances)
        {
            int full = (1 << balances.Count) - 1;
            long[] sum = new long[full + 1];
            int[] dp = new int[full + 1];

            for (int mask = 1; mask <= full; mask++)
            {
                int lowestUser = BitOperations.TrailingZeroCount(mask);
                sum[mask] = sum[mask & (mask - 1)] + balances[lowestUser].Cents;

                int best = 0;
                for (int user = 0; user < balances.Count; user++)
                {
                    if ((mask & (1 << user)) != 0)
                    {
                        best = Math.Max(best, dp[mask ^ (1 << user)]);
                    }
                }

                dp[mask] = best + (sum[mask] == 0 ? 1 : 0);
            }

            List<List<Balance>> groups = [];
            List<Balance> currentGroup = [];
            int remaining = full;

            while (remaining != 0)
            {
                int groupBorder = sum[remaining] == 0 ? 1 : 0;

                int removedUser = 0;
                while ((remaining & (1 << removedUser)) == 0 || dp[remaining ^ (1 << removedUser)] + groupBorder != dp[remaining])
                {
                    removedUser++;
                }

                currentGroup.Add(balances[removedUser]);
                remaining ^= 1 << removedUser;

                if (sum[remaining] == 0)
                {
                    groups.Add(currentGroup);
                    currentGroup = [];
                }
            }

            return groups;
        }

        // every transfer settles at least one user completely, so a group of k users needs at most k - 1 transfers
        private static List<MinimizedBill> SettleGroup(List<Balance> group)
        {
            List<Balance> receivers = [.. group.Where(balance => balance.Cents > 0).OrderByDescending(balance => balance.Cents)];
            List<Balance> payers = [.. group.Where(balance => balance.Cents < 0).OrderBy(balance => balance.Cents)];

            List<MinimizedBill> result = [];
            int receiverIndex = 0;
            int payerIndex = 0;
            long receiverOpen = receivers.Count > 0 ? receivers[0].Cents : 0;
            long payerOpen = payers.Count > 0 ? -payers[0].Cents : 0;

            while (receiverIndex < receivers.Count && payerIndex < payers.Count)
            {
                long transfer = Math.Min(receiverOpen, payerOpen);
                result.Add(new MinimizedBill(receivers[receiverIndex].Username, payers[payerIndex].Username, transfer / 100m));

                receiverOpen -= transfer;
                payerOpen -= transfer;

                if (receiverOpen == 0 && ++receiverIndex < receivers.Count)
                {
                    receiverOpen = receivers[receiverIndex].Cents;
                }

                if (payerOpen == 0 && ++payerIndex < payers.Count)
                {
                    payerOpen = -payers[payerIndex].Cents;
                }
            }

            return result;
        }

        private record Balance(string Username, long Cents);
    }
}
