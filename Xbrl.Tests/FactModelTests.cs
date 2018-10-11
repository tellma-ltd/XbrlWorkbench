using System;
using System.Linq;
using Banan.Tools.Xbrl.Instances;
using Banan.Tools.Xbrl.Instances.Export.Presentation;
using Banan.Tools.Xbrl.Taxonomies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Banan.Tools.Xbrl.Tests
{
    [TestClass]
    public class FactModelTests
    {
        [TestMethod]
        public void Aspects_Contains()
        {
            // Arrange
            var entity = new Entity {IdentifierScheme = new Uri("http://www.sec.gov/CIK"), Identifier = "WSI",Id="wsi", Name = "Walia Steel Industry PLC"};
            var unit = new CurrencyUnit("CHF");
            var period1 = new DurationPeriod(new DateTime(2017, 1, 1), new DateTime(2017, 12, 31));
            var period2 = new DurationPeriod(new DateTime(2018, 1, 1), new DateTime(2018, 12, 31));

            var concept1 = new Item { Name = "concept1" };
            var concept2 = new Item { Name = "concept2" };

            var fact1 = new Fact(concept1.Name, entity, period1) { Unit = unit, Value = "2017", BalanceMethod = BalanceMethod.Algebraic};
            var fact2 = new Fact (concept1.Name, entity, period2) {Unit = unit, Value = "2018", BalanceMethod = BalanceMethod.Algebraic};
            var facts = new[] {fact1, fact2};

            var factSet = new FactSet(facts);

            var period1Aspect = new PeriodAspect(period1);

            // Act
            var factModel1 = factSet.FactModels.Single(fm => fm.Fact==fact1);
            var containsPeriod1Aspect = factModel1.Aspects.Contains(period1Aspect);

            // Assert
            Assert.AreEqual(true, containsPeriod1Aspect);
        }
    }
}
