using MikuMikuLibrary.Sprites;
using MikuMikuLibrary.Textures;
using MikuMikuLibrary.Textures.DDS;
using MikuMikuModel.GUI.Controls;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MikuMikuModel.DataNodes
{
    public class SpriteNode : DataNode<Sprite>
    {
        public override DataNodeFlags Flags
        {
            get { return DataNodeFlags.Leaf; }
        }

        public override DataNodeActionFlags ActionFlags
        {
            get { return DataNodeActionFlags.Export | DataNodeActionFlags.Move | DataNodeActionFlags.Remove | DataNodeActionFlags.Rename | DataNodeActionFlags.Replace; }
        }

        public int Field00
        {
            get { return GetProperty<int>(); }
            set { SetProperty(value); }
        }
        public int Field01
        {
            get { return GetProperty<int>(); }
            set { SetProperty(value); }
        }
        [DisplayName("Texture Index")]
        public int TextureIndex
        {
            get { return GetProperty<int>(); }
            set { SetProperty(value); }
        }
        public float Field02
        {
            get { return GetProperty<float>(); }
            set { SetProperty(value); }
        }
        public float Field03
        {
            get { return GetProperty<float>(); }
            set { SetProperty(value); }
        }
        public float Field04
        {
            get { return GetProperty<float>(); }
            set { SetProperty(value); }
        }
        public float Field05
        {
            get { return GetProperty<float>(); }
            set { SetProperty(value); }
        }
        public float Field06
        {
            get { return GetProperty<float>(); }
            set { SetProperty(value); }
        }
        public float X
        {
            get { return GetProperty<float>(); }
            set { SetProperty(value); }
        }
        public float Y
        {
            get { return GetProperty<float>(); }
            set { SetProperty(value); }
        }
        public float Width
        {
            get { return GetProperty<float>(); }
            set { SetProperty(value); }
        }
        public float Height
        {
            get { return GetProperty<float>(); }
            set { SetProperty(value); }
        }

        public override Control Control
        {
            get
            {
                TextureViewControl.Instance.SetSprite(Data);
                return TextureViewControl.Instance;
            }
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Bitmap>((path) =>
            {
                var bitmap = TextureDecoder.Decode(Data.ParentTexture);
                bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                SpriteCropper.Crop(bitmap, Data.GetSourceRectangle()).Save(path);
            });

            RegisterReplaceHandler<Bitmap>((path) =>
            {
                using (var spriteBitmap = new Bitmap(path))
                {
                    // 1) decode the original texture
                    var fullBitmap = TextureDecoder.Decode(Data.ParentTexture);

                    // 2) insert the new sprite
                    if (spriteBitmap.Width <= Data.Width && spriteBitmap.Height <= Data.Height)
                    {
                        fullBitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                        {
                            SpriteCropper.Insert(spriteBitmap, fullBitmap, Data.GetSourceRectangle());
                        }
                        fullBitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                    }
                    else
                    {
                        // TODO: Rearrange sprite sheet and update sprite size property to accommodate the new size.
                        throw new NotImplementedException("Sprite texture region too large.");

                        Data.Width = spriteBitmap.Width;
                        Data.Height = spriteBitmap.Height;
                    }

                    Texture parentTexture;

                    // 3) encode the updated texture
                    if (Data.ParentTexture.IsYCbCr)
                    {
                        var format = DDSCodec.HasTransparency(fullBitmap) ? TextureFormat.RGBA : TextureFormat.RGB;

                        parentTexture = TextureEncoder.Encode(fullBitmap, format, false);
                    }
                    else
                    {
                        parentTexture = TextureEncoder.Encode(fullBitmap, Data.ParentTexture.Format, Data.ParentTexture.MipMapCount != 0);
                    }

                    // 4) update the texture object
                    {
                        //if (Data.ParentTexture.Format != parentTexture.Format)
                        //    throw new Exception($"Texture Format missmatch of type: {Data.ParentTexture.Format} and {parentTexture.Format}.");

                        string parentName = Data.ParentTextureName;
                        Data.ParentTextureSet.Textures[Data.TextureIndex] = parentTexture;
                        Data.ParentTextureSet.Textures[Data.TextureIndex].Name = parentName;
                    }

                    return Data;
                }
            });
        }

        protected override void InitializeViewCore()
        {
        }

        protected override void OnRename(string oldName)
        {
            SetProperty(Name, nameof(Data.Name));
            base.OnRename(oldName);
        }

        public SpriteNode(string name, Sprite data) :
            base(string.IsNullOrEmpty(data.Name) ? name : data.Name, data)
        {
        }
    }
}
