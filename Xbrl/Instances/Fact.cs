using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Instances
{
    public class Fact
    {
        public Fact(XName conceptName, Entity entity, Period period)
        {
            ConceptName = conceptName;
            Entity = entity;
            Period = period;
            ExplicitMembers = new List<ExplicitMember>();

            Context = new Context(entity, period, ExplicitMembers);
        }

        internal Context Context { get; }

        public string GetContextId(XElement namespaceDeclarationsElement)
        {
            return Context.GetContextId(namespaceDeclarationsElement);
        }

        public XElement ToXml(XElement namespaceDeclarationsElement)
        {
            var factElement = new XElement(ConceptName,
                new XAttribute("contextRef", GetContextId(namespaceDeclarationsElement)),
                Value);

            if (Unit != null)
            {
                factElement.Add(new XAttribute("unitRef", Unit.Id));
            }

            if (Unit is CurrencyUnit)
            {
                factElement.Add(new XAttribute("decimals", Decimals));
            }

            return factElement;
        }

        public double GetAccountingValue(string balanceType)
        {
            if (BalanceMethod==BalanceMethod.Unknown)
            {
                throw new InvalidOperationException($"The balance method of the fact for {ConceptName} is not set.");
            }

            var d = double.Parse(Value, CultureInfo.InvariantCulture);

            // Below line also considers the around 150 monetary IFRS concepts without a balance type, by treating them as debit.
            return BalanceMethod == BalanceMethod.Algebraic && balanceType == BalanceTypes.Credit ? -d : d;
        }

        #region Required aspects

        public XName ConceptName { get; }

        public Period Period { get; }

        public Entity Entity { get; }

        #endregion

        #region Optional aspects

        /// <summary>
        ///     According to the XBRL specifications, tuples and non-numerics MUST NOT have a unit,
        ///     while numerics MUST have a unit.
        /// </summary>
        public Unit Unit { get; set; }

        public IList<ExplicitMember> ExplicitMembers { get; }

        #endregion

        #region Value

        public string Value { get; set; }

        /// <summary>
        ///     Either the Decimals or the Precision must be present for numerical items.
        ///     (See XBRL specifications, chapter 4.6.3)
        /// </summary>
        /// <remarks>
        ///     This API only supports Decimals.
        /// </remarks>
        public int Decimals { get; set; }

        public BalanceMethod BalanceMethod { get; set; }

        #endregion
    }
}

