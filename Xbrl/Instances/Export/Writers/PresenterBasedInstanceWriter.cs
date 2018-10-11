using System.Collections.Generic;
using Banan.Tools.Xbrl.Instances.Export.Presentation;

namespace Banan.Tools.Xbrl.Instances.Export.Writers
{
    public abstract class PresenterBasedInstanceWriter: IInstanceWriter, IPresenterWriter
    {
        public ScopeSettings Scope { get; set; }

        #region IInstanceWriter

        public void Write(Instance instance)
        {
            var presenter = new Presenter(instance);
            presenter.Present(this, Scope);
        }

        #endregion

        #region IPresenterWriter

        public abstract void WriteBeginExport(Instance instance);
        public abstract void WriteIntro(Entity entity, CurrencyUnit unit);
        public abstract void WriteBeginPresentationNetwork(PresentationNetwork presentationNetwork);
        public abstract void WriteBeginTable(Axis horizontalAxis);
        public abstract void WriteConcept(ConceptMember conceptMember, IDictionary<Member, FactModel> facts);
        public abstract void WriteEndTable();
        public abstract void WriteEndPresentationNetwork();
        public abstract void WriteEndExport();

        #endregion
    }
}