using System.Collections.Generic;

namespace Banan.Tools.Xbrl.Instances.Export.Presentation
{
    /// <summary>
    /// The classic builder design pattern.
    /// Modelled after existing specialised writers in the .NET framework, e.g. DocumentXmlWriter, HtmlTextWriter
    /// or the almost forgotten XpsDocumentWriter.
    /// </summary>
    public interface IPresenterWriter
    {
        /// <summary>
        /// The presenter calls this method once at the start of the export,
        /// handing the writer the context of the export.
        /// </summary>
        void WriteBeginExport(Instance instance);

        /// <summary>
        /// The current export requires a single entity, a single currency and a main duration period,
        /// that together define the context for the entire export. The presenter calls this method once.
        /// </summary>
        /// <remarks>
        /// The purpose is to simplify the writer, as it can use a simple substitution in the template.
        /// It is not a limitation of the underlying data structure.
        /// </remarks>
        void WriteIntro(Entity entity, CurrencyUnit unit);
        
        /// <summary>
        /// The presenter calls this method at the start of each presentation network. A writer would typically write
        /// out the name of the network.
        /// </summary>
        void WriteBeginPresentationNetwork(PresentationNetwork presentationNetwork);

        /// <summary>
        /// The presenter calls this method at the start of each table. A writer typically maintains a stack of active tables.
        /// It is up to the writer how to visualise nested tables.
        /// </summary>
        void WriteBeginTable(Axis horizontalAxis);

        /// <summary>
        /// The presenter calls this method for each concept member in a concept tree and for each line item in a table.
        /// A writer would typically writes the label of the concept member together and aligns the facts with the
        /// members of the vertical axis.
        /// The writer is also responsible for adding a header row when appropriate (e.g. with the presence of monetary facts).
        /// </summary>
        void WriteConcept(ConceptMember conceptMember, IDictionary<Member, FactModel> facts);

        /// <summary>
        /// The presenter calls this method at the end of each table. A writer would typically reduce the stack of active tables and
        /// continue with the parent table, if any.
        /// </summary>
        void WriteEndTable();

        /// <summary>
        /// The presenter calls this method at the end of each presentation network. A writer would typically modify some aspect
        /// of its internal context and close the presentation block for the network.
        /// </summary>
        void WriteEndPresentationNetwork();

        /// <summary>
        /// The presenter calls this method once at the end of the export.
        /// </summary>
        void WriteEndExport();
    }
}