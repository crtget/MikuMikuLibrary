using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikuMikuLibrary.Animations.Aet;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Sprites;

namespace MikuMikuModel.FormatModules
{
    class AetFormatModule : FormatModule<Aet>
    {
        public override FormatModuleFlags Flags
        {
            get => FormatModuleFlags.Import;
        }

        public override string Name
        {
            get => "Aet Animation Table";
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
                int metadataPtr = ReadInt();

                if (metadataPtr > 0 && metadataPtr < source.Length)
                {
                    source.Seek(metadataPtr, SeekOrigin.Begin);

                    int mainStrPtr = ReadInt();

                    if (mainStrPtr > 0 && mainStrPtr < source.Length - Aet.MAIN_STRING.Length)
                    {
                        source.Seek(mainStrPtr, SeekOrigin.Begin);

                        byte[] mainStrBuffer = new byte[Aet.MAIN_STRING.Length];
                        source.Read(mainStrBuffer, 0, mainStrBuffer.Length);

                        if (Encoding.ASCII.GetString(mainStrBuffer) == Aet.MAIN_STRING)
                            return true;
                    }
                }
            }

            int ReadInt()
            {
                byte[] buffer = new byte[sizeof(int)];
                source.Read(buffer, 0, sizeof(int));

                return BitConverter.ToInt32(buffer, 0);
            }

            return false;
        }

        protected override Aet ImportCore(Stream source, string fileName)
        {
            return BinaryFile.Load<Aet>(source, true);
        }

        protected override void ExportCore(Aet obj, Stream destination, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
