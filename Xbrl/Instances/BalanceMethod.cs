namespace Banan.Tools.Xbrl.Instances
{
    public enum BalanceMethod
    {
        Unknown = 0,

        /// <summary>
        /// Quoting from "The XBRL Book": "Monetary concepts, that is, concepts that express amounts of money in any currency,
        /// are stamped with a balance which can be either credit or debit." This extra information leads to a human-friendly
        /// display of mainly positive numbers, e.g. a revenue of $1000 and an expense of $750.
        ///
        /// XBRL interprets monetary fact values with the Accounting method.
        /// </summary>
        /// <remarks>
        /// The balance type is reflected in calculation networks by the +1 and -1 weights.
        /// </remarks>
        Accounting,

        /// <summary>
        /// The Algebraic method disregards balance types and uses addition to debit and subtraction to credit, e.g. a
        /// revenue of $1000 would be +1000 and an expense of $750 would be -750, with the Revenue fact resulting in a value of
        /// +1000 and the Expense fact resulting in a value of -750.
        /// </summary>
        /// <remarks>
        /// 1. Knowing the balance type of a monetary concept, one can easily convert between the Algebraic and Accounting methods, namely:
        /// for Debit balance type: accounting value = algebraic value
        /// for Credit balance type: accounting value = -(algebraic value)
        ///
        /// 2. The algebraic method allows summing up balances without the need for any weights.
        /// </remarks>
        Algebraic,
    }
}