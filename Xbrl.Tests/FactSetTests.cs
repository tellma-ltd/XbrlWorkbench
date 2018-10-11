using System;
using Banan.Tools.Xbrl.Instances;
using Banan.Tools.Xbrl.Instances.Export.Presentation;
using Banan.Tools.Xbrl.Taxonomies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Banan.Tools.Xbrl.Tests
{
    [TestClass]
    public class FactSetTests
    {
        [TestMethod]
        public void Slice()
        {
            // Arrange
            var entity = new Entity { IdentifierScheme = new Uri("http://www.sec.gov/CIK"), Identifier = "WSI", Id = "wsi", Name = "Walia Steel Industry PLC" };
            var unit = new CurrencyUnit("CHF");
            var duration1 = new DurationPeriod(new DateTime(2017, 1, 1), new DateTime(2017, 12, 31));
            var instant1Start = new InstantPeriod(new DateTime(2016, 12, 31));
            var duration2 = new DurationPeriod(new DateTime(2018, 1, 1), new DateTime(2018, 12, 31));

            var concept1 = new Item { Name = "concept1" };

            var duration1Fact = new Fact(concept1.Name, entity, duration1) { Unit = unit, Value = "2017", BalanceMethod = BalanceMethod.Algebraic };
            var instant1StartFact = new Fact(concept1.Name, entity, instant1Start) { Unit = unit, Value = "2017.1", BalanceMethod = BalanceMethod.Algebraic };
            var duration2Fact = new Fact (concept1.Name, entity, duration2) { Unit = unit, Value = "2018", BalanceMethod = BalanceMethod.Algebraic };
            var facts = new[] {duration1Fact, instant1StartFact, duration2Fact };

            var factSet = new FactSet(facts);

            var duration1Aspect = new PeriodAspect(duration1);
            var instant1StartAspect = new PeriodAspect(instant1Start);

            var duration1Member = new Member(duration1Aspect);
            duration1Member.RelatedAspects.Add(new RelatedAspect(instant1StartAspect, RelatedAspectRoles.PeriodStart));

            // Act
            var slicedFactSetIncludingRelatedAspects = factSet.Slice(duration1Member, true);
            var slicedFactSetExcludingRelatedAspects = factSet.Slice(duration1Member, false);

            // Assert
            Assert.AreEqual(facts.Length, factSet.FactModels.Count);
            Assert.AreEqual(2, slicedFactSetIncludingRelatedAspects.FactModels.Count);
            Assert.AreEqual(1, slicedFactSetExcludingRelatedAspects.FactModels.Count);
        }
    }
}
