using MikuMikuLibrary.Sprites;
using MikuMikuLibrary.Textures;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MikuMikuModel.GUI.Controls
{
    public partial class TextureViewControl : UserControl
    {
        private static TextureViewControl instance;

        public static TextureViewControl Instance
        {
            get
            {
                if (instance == null)
                    instance = new TextureViewControl();

                return instance;
            }
        }

        public static void DisposeInstance()
        {
            instance?.Dispose();
        }

        private Texture texture;
        private Sprite sprite;

        private Bitmap[,] bitmaps;
        private Bitmap ycbcrBitmap;

        private int mipMapIndex;
        private int levelIndex;

        public int CurrentMipMapIndex
        {
            get { return mipMapIndex; }
            set
            {
                if (texture.IsYCbCr)
                    return;

                value = Math.Max(0, Math.Min(value, texture.MipMapCount - 1));

                if (value == mipMapIndex)
                    return;

                mipMapIndex = value;
                SetAllTextureInfoText();
            }
        }

        public int CurrentLevelIndex
        {
            get { return levelIndex; }
            set
            {
                if (texture.IsYCbCr)
                    return;

                value = Math.Max(0, Math.Min(value, texture.Depth - 1));

                if (value == levelIndex)
                    return;

                levelIndex = value;
                SetAllTextureInfoText();
            }
        }

        private void DecodeAndSetBitmaps()
        {
            if (texture.IsYCbCr)
            {
                ycbcrBitmap = TextureDecoder.Decode(texture);
            }
            else
            {
                bitmaps = new Bitmap[texture.Depth, texture.MipMapCount];
                for (int i = 0; i < texture.Depth; i++)
                    for (int j = 0; j < texture.MipMapCount; j++)
                        bitmaps[i, j] = TextureDecoder.Decode(texture[i, j]);
            }
        }

        private void SetFormatText()
        {
            if (texture.IsYCbCr)
                formatLabel.Text = "Format: YCbCr";
            else
                formatLabel.Text = $"Format: {Enum.GetName(typeof(TextureFormat), texture.Format)}";
        }

        private void SetSizeText()
        {
            if (texture.IsYCbCr)
                sizeLabel.Text = $"Size: {texture.Width}x{texture.Height}";
            else
                sizeLabel.Text = $"Size: {texture[levelIndex, mipMapIndex].Width}x{texture[levelIndex, mipMapIndex].Height}";
        }

        private void SetMipMapText()
        {
            if (texture.IsYCbCr)
                mipMapLabel.Text = "MipMap: 1/1";
            else
                mipMapLabel.Text = $"MipMap: {mipMapIndex + 1}/{texture.MipMapCount}";
        }

        private void SetLevelText()
        {
            if (texture.IsYCbCr)
                levelLabel.Text = "Level: 1/1";
            else
                levelLabel.Text = $"Level: {levelIndex + 1}/{texture.Depth}";
        }

        private void SetControlTextureBackground()
        {
            var bitmap = texture.IsYCbCr ? ycbcrBitmap : bitmaps[levelIndex, mipMapIndex];
            bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);

            BackgroundImage = bitmap;

            SetBackgroundImageLayout();
        }

        private void SetControlSpriteBackground()
        {
            var bitmap = texture.IsYCbCr ? ycbcrBitmap : bitmaps[levelIndex, mipMapIndex];
            bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);

            BackgroundImage = SpriteCropper.Crop(bitmap, sprite.GetSourceRectangle());

            SetBackgroundImageLayout();
        }

        private void SetBackgroundImageLayout()
        {
            if (ClientSize.Width < BackgroundImage.Width || ClientSize.Height < BackgroundImage.Height)
                BackgroundImageLayout = ImageLayout.Zoom;
            else
                BackgroundImageLayout = ImageLayout.Center;

            Refresh();
        }

        private void SetAllTextureInfoText()
        {
            SetFormatText();
            SetSizeText();
            SetMipMapText();
            SetLevelText();
        }

        public void SetTexture(Texture texture)
        {
            DisposeBitmaps();

            this.texture = texture;
            sprite = null;

            DecodeAndSetBitmaps();

            SetAllTextureInfoText();
            SetControlTextureBackground();
        }

        public void SetSprite(Sprite sprite)
        {
            // This is not the most efficient way of doing this
            // so we might wanna use an openGL control instead 
            // which we'll have to do anyway if we want to render aets cirComfy

            DisposeBitmaps();

            texture = sprite.ParentTexture;
            this.sprite = sprite;

            DecodeAndSetBitmaps();

            SetAllTextureInfoText();
            SetControlSpriteBackground();
        }

        private void DisposeBitmaps()
        {
            if (bitmaps != null)
            {
                foreach (var bitmap in bitmaps)
                    bitmap.Dispose();
            }

            ycbcrBitmap?.Dispose();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up:
                    CurrentLevelIndex++;
                    return true;

                case Keys.Down:
                    CurrentLevelIndex--;
                    return true;

                case Keys.Left:
                    CurrentMipMapIndex--;
                    return true;

                case Keys.Right:
                    CurrentMipMapIndex++;
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                DisposeBitmaps();
            }
            base.Dispose(disposing);
        }

        public TextureViewControl()
        {
            InitializeComponent();
        }
    }
}
