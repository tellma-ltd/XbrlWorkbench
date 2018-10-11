using System;
using System.IO;
using Unity.Attributes;
using System.Globalization;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Instances;
using Banan.Tools.Xbrl.Instances.Export.Presentation;
using Banan.Tools.Xbrl.Instances.Export.Writers;

namespace Banan.Tools.XbrlBench.UI.Commands
{
    [CommandExport(Name = "Save-Instance", CommandType = typeof(SaveInstanceCommand))]
    public class SaveInstanceCommand : ShellCommandBase
    {
        [Dependency]
        public Instance Instance { get; set; }


        public override void Invoke()
        {
            base.Invoke();

            if (!NamedParameters.ContainsKey("format"))
            {
                Context.Logger.WriteLine("Parameter 'format' is missing.");
                return;
            }
            var format = NamedParameters["format"];

            if (!NamedParameters.ContainsKey("file"))
            {
                Context.Logger.WriteLine("Parameter 'file' is missing.");
                return;
            }

            var filename = NamedParameters["file"];
            var fileInfo = new FileInfo(filename);

            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            if (format=="XML")
            {
                SaveAsXml(fileInfo);
            }
            else if (format == "XHTML")
            {
                SaveAsInlineXbrl(fileInfo);
            }
            else
            {
                Context.Logger.WriteLine($"Unknown format '{format}'.");
                return;
            }

            Context.Logger.WriteLine("Exported.");
        }

        private void SaveAsInlineXbrl(FileInfo fileInfo)
        {
            if (!NamedParameters.ContainsKey("template"))
            {
                Context.Logger.WriteLine("Parameter 'template' is missing.");
                return;
            }
            var template = NamedParameters["template"];

            var xTemplate = XDocument.Load(template);
            var writerSettings = new InlineXbrlWriterSettings
            {
                Title = "iXBRL Export",
                Culture = new CultureInfo("en-GB"),
                Scale = 3,
                IncludeDocumentation = true,
                IncludeReferences = true,
            };

            var networks = new[]
{
                new PresentationNetwork
                {
                    Name = "Statement of financial position, current/non-current",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_1_2017-03-09_role-210000")
                },
                new PresentationNetwork
                {
                    Name = "Statement of comprehensive income, profit or loss, by function of expense",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_1_2017-03-09_role-310000")
                },
                new PresentationNetwork
                {
                    Name = "Statement of cash flows, direct method",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_7_2017-03-09_role-510000")
                },
                new PresentationNetwork
                {
                    Name = "Statement of changes in equity",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_1_2017-03-09_role-610000")
                },
                new PresentationNetwork
                {
                    Name = "Notes - Subclassifications of assets, liabilities and equities",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_1_2017-03-09_role-800100")
                },
                new PresentationNetwork
                {
                    Name = "Notes - Analysis of income and expense",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_1_2017-03-09_role-800200")
                },
                new PresentationNetwork
                {
                    Name = "Notes - List of notes",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_1_2017-03-09_role-800500")
                },
                new PresentationNetwork
                {
                    Name = "Notes - List of accounting policies",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_1_2017-03-09_role-800600")
                },
                new PresentationNetwork
                {
                    Name = "Notes - Corporate information and statement of IFRS compliance",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_1_2017-03-09_role-810000")
                },
                new PresentationNetwork
                {
                    Name = "Notes - Property, plant and equipment",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_16_2017-03-09_role-822100")
                },
                new PresentationNetwork
                {
                    Name = "Notes - Fair value measurement",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ifrs_13_2017-03-09_role-823000")
                },
                new PresentationNetwork
                {
                    Name = "Notes - Investment property",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_40_2017-03-09_role-825100")
                },
                new PresentationNetwork
                {
                    Name = "Notes – Inventories",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_2_2017-03-09_role-826380")
                },
                new PresentationNetwork
                {
                    Name = "Notes – Revenue",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_18_2017-03-09_role-831110")
                },
                new PresentationNetwork
                {
                    Name = "Notes - Revenue from contracts with customers",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ifrs_15_2017-03-09_role-831150")
                },
                new PresentationNetwork
                {
                    Name = "Notes - Impairment of assets",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_36_2017-03-09_role-832410")
                },
                new PresentationNetwork
                {
                    Name = "Notes - Employee benefits",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_19_2017-03-09_role-834480")
                },
                new PresentationNetwork
                {
                    Name = "Notes - Operating segments",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ifrs_8_2017-03-09_role-871100")
                },
            };

            //var networks2 = new[]
            //{
            //    new PresentationNetwork
            //    {
            //        Name = "Statement of changes in equity",
            //        Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_1_2017-03-09_role-610000")
            //    },
            //};

            writerSettings.Scope = new ScopeSettings
            {
                PresentationNetworks = networks,
                IncludeEmptyConcepts = false,
                IncludeEmptyExplicitMembers = false
            };

            var inlineXbrlWriter = new InlineXbrlWriter(xTemplate, writerSettings);

            Instance.Save(inlineXbrlWriter);
            var result = inlineXbrlWriter.Document.ToString();

            File.WriteAllText(fileInfo.FullName, result);
        }


        private void SaveAsXml(FileInfo fileInfo)
        {
            var document = Instance.ToXml();
            using (var f = fileInfo.OpenWrite())
            {
                document.Save(f);
            }
        }
    }
}