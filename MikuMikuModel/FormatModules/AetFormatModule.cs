using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Aet;
using MikuMikuLibrary.Sprites;

namespace MikuMikuModel.FormatModules
{
    class AetFormatModule : FormatModule<AetSet>
    {
        public override FormatModuleFlags Flags
        {
            get => FormatModuleFlags.Import | FormatModuleFlags.Export;
        }

        public override string Name
        {
            get => "Aet Object Container";
        }

        public override string[] Extensions
        {
            get => new string[] { "bin" };
        }

        protected override bool CanImportCore(Stream source, string fileName)
        {
            // 1) check for aet file name
            {
                if (Path.GetFileName(fileName).StartsWith("aet_", StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            // 2) check for MAIN string pointer
            {
                int metadataPtr = ReadInt32();

                if (metadataPtr > 0 && metadataPtr < source.Length)
                {
                    source.Seek(metadataPtr, SeekOrigin.Begin);

                    int mainStrPtr = ReadInt32();

                    if (mainStrPtr > 0 && mainStrPtr < source.Length - AetSet.MAIN_STRING.Length)
                    {
                        source.Seek(mainStrPtr, SeekOrigin.Begin);

                        byte[] mainStrBuffer = new byte[AetSet.MAIN_STRING.Length];
                        source.Read(mainStrBuffer, 0, mainStrBuffer.Length);

                        if (Encoding.ASCII.GetString(mainStrBuffer) == AetSet.MAIN_STRING)
                            return true;
                    }
                }
            }

            int ReadInt32()
            {
                byte[] buffer = new byte[sizeof(int)];
                source.Read(buffer, 0, sizeof(int));

                return BitConverter.ToInt32(buffer, 0);
            }

            return false;
        }

        protected override AetSet ImportCore(Stream source, string fileName)
        {
            return BinaryFile.Load<AetSet>(source, true);
        }

        protected override void ExportCore(AetSet obj, Stream destination, string fileName)
        {
            obj.Save(destination);
        }
    }
}
