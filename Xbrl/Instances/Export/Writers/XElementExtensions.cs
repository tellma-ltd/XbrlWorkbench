using System.Linq;
using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Instances.Export.Writers
{
    public static class XElementExtensions
    {
        public static XElement FindById(this XElement context, string id)
        {
            return context.DescendantsAndSelf().SingleOrDefault(e => e.Attributes().Any(a => a.Name == "id" && a.Value == id));
        }

        public static void Substitute(this XElement context, string id, string text)
        {
            var element = FindById(context, id);
            if (element != null)
            {
                element.Value = text;
            }
        }

        public static void AddClass(this XElement element, string className)
        {
            var classAttribute = element.Attribute("class");
            if (classAttribute == null)
            {
                classAttribute = new XAttribute("class", className);
                element.Add(classAttribute);
                return;
            }

            classAttribute.Value += $" {className}";
        }

        /// <summary>
        /// Adds a data attribute to the DOM. The name parameter is the short name without the data- prefix.
        /// </summary>
        public static void AddData(this XElement element, string name, string value)
        {
            element.SetAttributeValue($"data-{name}", value);
        }
    }
}