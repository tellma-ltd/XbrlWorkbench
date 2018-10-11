using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Instances.Export.Writers;

namespace Banan.Tools.Xbrl.Instances
{
    /// <summary>
    /// XBRL instances combine an entity, a period and the list of explicit members to a context.
    /// This is an internal detail that is not exposed to the public Instance API.
    /// </summary>
    internal class Context        
    {
        private readonly Entity _entity;
        private readonly Period _period;
        private readonly IList<ExplicitMember> _explicitMembers;

        internal Context(Entity entity, Period period, IList<ExplicitMember> explicitMembers)
        {
            _entity = entity;
            _period = period;
            _explicitMembers = explicitMembers;
        }

        /// <summary>
        /// Explicit members appear in the scenario element, which is what IFRS uses and also in line
        /// with the ESEF (European Single Electronic Format).
        /// </summary>
        internal XElement ToXml(XElement namespaceDeclarationsElement)
        {
            var contextId = GetContextId(namespaceDeclarationsElement);

            var xEntity = _entity.ToXml();


            var xContext = new XElement(Namespaces.Xbrli + "context", 
                new XAttribute("id", contextId),
                xEntity, 
                _period.ToXml());

            if (_explicitMembers.Any())
            {
                var xScenario = new XElement(Namespaces.Xbrli + "scenario");
                foreach (var explicitMember in _explicitMembers)
                {
                    xScenario.Add(explicitMember.ToXml(namespaceDeclarationsElement));
                }
                xContext.Add(xScenario);
            }

            return xContext;
        }

        /// <summary>
        /// Creates a unique context id by collating the entity id, period id and all explicit members.
        /// This seems to be common practice and makes sense, even though the context ids tends to become long.
        /// </summary>
        /// <remarks>
        /// We consistently use underscores to separate parts and dashes inside parts. Deplorably, colons are not allowed
        /// because IDs are NCNames, see e.g. https://stackoverflow.com/questions/1631396/what-is-an-xsncname-type-and-when-should-it-be-used
        /// </remarks>
        internal string GetContextId(XElement namespaceDeclarationsElement)
        {
            var sb = new StringBuilder($"{_entity.Id}_{_period.Id}");

            foreach (var explicitMember in _explicitMembers)
            {
                sb.Append($"_{explicitMember.DimensionName.ToColonSeparated(namespaceDeclarationsElement)}_{explicitMember.MemberName.ToColonSeparated(namespaceDeclarationsElement)}");
            }

            return sb.ToString().Replace(":","-");
        }

    }
}