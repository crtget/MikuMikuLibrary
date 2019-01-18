using System;
using System.IO;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Scripts;

namespace MikuMikuModel.FormatModules
{
    class PvScriptFormatModule : FormatModule<PvScript>
    {
        public override FormatModuleFlags Flags
        {
            get => FormatModuleFlags.Import | FormatModuleFlags.Export;
        }

        public override string Name => "PV Script";

        public override string[] Extensions => new string[] { "dsc" };

        protected override bool CanImportCore(Stream source, string fileName)
        {
            string extension = Path.GetExtension(fileName);

            if (extension.Equals(".dsc", StringComparison.InvariantCultureIgnoreCase))
                return true;

            return false;
        }

        protected override PvScript ImportCore(Stream source, string fileName)
        {
            return BinaryFile.Load<PvScript>(source, true);
        }

        protected override void ExportCore(PvScript obj, Stream destination, string fileName)
        {
            obj.Save(destination);
        }
    }
}
