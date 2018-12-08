using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.Textures;
using System.Drawing;

namespace MikuMikuLibrary.Sprites
{
    /// <summary>
    /// A drawable subsection of a <see cref="Texture"/>.
    /// </summary>
    public class Sprite
    {
        /// <summary>
        /// The name of the sprite.
        /// </summary>
        public string Name { get; set; }

        public int Field00 { get; set; }
        public int Field01 { get; set; }

        /// <summary>
        /// The index of the parent <see cref="TextureSet"/> object in which the sprite is stored in.
        /// </summary>
        public int TextureIndex { get; set; }

        public float Field02 { get; set; }
        public float Field03 { get; set; }
        public float Field04 { get; set; }
        public float Field05 { get; set; }
        public float Field06 { get; set; }

        /// <summary>
        /// The top left x-position of the sprite's source rectangle.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// The top left y-position of the sprite's source rectangle.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// The width of the sprite's source rectangle.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// The height of the sprite's source rectangle.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// A reference to the parent <see cref="TextureSet"/> object.
        /// </summary>
        public TextureSet ParentTextureSet { get; set; }

        /// <summary>
        /// A reference to the <see cref="Texture"/> object indexed by the <see cref="TextureIndex"/> property.
        /// </summary>
        public Texture ParentTexture
        {
            get
            {
                if (TextureIndex >= 0 && TextureIndex < ParentTextureSet?.Textures?.Count)
                {
                    return ParentTextureSet?.Textures?[TextureIndex];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// The texture's name of the <see cref="ParentTexture"/> object.
        /// </summary>
        public string ParentTextureName
        {
            // To make the SpriteNode more comprehensive.
            get => ParentTexture?.Name;
        }

        /// <summary>
        /// Constructs and returns a <see cref="RectangleF"/> source rectangle from the source rectangle properties.
        /// </summary>
        public RectangleF GetSourceRectangle()
        {
            return new RectangleF(X, Y, Width, Height);
        }

        internal void ReadFirst(EndianBinaryReader reader)
        {
            TextureIndex = reader.ReadInt32();
            Field02 = reader.ReadSingle();
            Field03 = reader.ReadSingle();
            Field04 = reader.ReadSingle();
            Field05 = reader.ReadSingle();
            Field06 = reader.ReadSingle();
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Width = reader.ReadSingle();
            Height = reader.ReadSingle();
        }

        internal void WriteFirst(EndianBinaryWriter writer)
        {
            writer.Write(TextureIndex);
            writer.Write(Field02);
            writer.Write(Field03);
            writer.Write(Field04);
            writer.Write(Field05);
            writer.Write(Field06);
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Width);
            writer.Write(Height);
        }

        internal void ReadSecond(EndianBinaryReader reader)
        {
            Field00 = reader.ReadInt32();
            Field01 = reader.ReadInt32();
        }

        internal void WriteSecond(EndianBinaryWriter writer)
        {
            writer.Write(Field00);
            writer.Write(Field01);
        }
    }
}
