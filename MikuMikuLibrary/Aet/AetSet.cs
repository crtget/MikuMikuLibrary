using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using MikuMikuLibrary.Sprites;

namespace MikuMikuLibrary.Aet
{
    /// <summary>
    /// An <see cref="AetSet"/> is an <see cref="AetObj"/> container stored inside an aet file.
    /// </summary>
    public class AetSet : BinaryFile
    {
        public override BinaryFileFlags Flags
        {
            get => BinaryFileFlags.Load | BinaryFileFlags.Save;
        }

        /// <summary>
        /// The <see cref="AetSection"/> storing normal <see cref="AetObj"/>s.
        /// </summary>
        public AetSection MainSection { get; set; }

        /// <summary>
        /// The <see cref="AetSection"/> storing PDA touch panel related <see cref="AetObj"/>s.
        /// <remarks>
        /// Does not use sprites, used for touch point intersection calculations (?).
        /// </remarks>
        /// </summary>
        public AetSection TouchSection { get; set; }

        /// <summary>
        /// The <see cref="SpriteSet"/> associated with this <see cref="AetSet"/>.
        /// <see cref="AetSet"/>s can also draw <see cref="Sprite"/>s outside their scope.
        /// </summary>
        public SpriteSet AssociatedSpriteSet { get; set; }

        public AetSet()
        {
            return;
        }

        public override void Read(EndianBinaryReader reader, Section section = null)
        {
            // header
            int mainMetadataOffset = reader.ReadInt32();
            int touchMetadataOffset = reader.ReadInt32();

            reader.ReadInt32(); // unused
            reader.ReadInt32(); // unused

            // MAIN metadata
            if (mainMetadataOffset > 0)
            {
                MainSection = new AetSection()
                {
                    ThisOffset = mainMetadataOffset,
                    SectionType = AetSectionType.MAIN,
                };

                MainSection.Read(reader);
            }

            // TOUCH metadata
            if (touchMetadataOffset > 0)
            {
                TouchSection = new AetSection()
                {
                    ThisOffset = touchMetadataOffset,
                    SectionType = AetSectionType.TOUCH,
                };

                TouchSection.Read(reader);
            }
        }

        public override void Write(EndianBinaryWriter writer, Section section = null)
        {
            writer.Write(0x0); // MAIN
            writer.Write(0x0); // TOUCH
            writer.Write(0x0); // unused
            writer.Write(0x0); // unused

            MainSection?.Write(writer);
            TouchSection?.Write(writer);
        }
    }
}
