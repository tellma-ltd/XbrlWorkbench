using System;

namespace Banan.Tools.Xbrl.Instances.Export
{
    public class InstanceExportException : Exception
    {
        public InstanceExportException()
        {
        }

        public InstanceExportException(string message)
            : base(message)
        {
        }

        public InstanceExportException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}