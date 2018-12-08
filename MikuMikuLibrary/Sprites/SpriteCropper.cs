using MikuMikuLibrary.Textures;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MikuMikuLibrary.Sprites
{
    public static class SpriteCropper
    {
        public static Bitmap Crop(Sprite sprite, SpriteSet parentSet)
        {
            var bitmap = TextureDecoder.Decode(
                parentSet.TextureSet.Textures[sprite.TextureIndex]);

            var croppedBitmap = Crop(bitmap, sprite.GetSourceRectangle());

            bitmap.Dispose();
            return croppedBitmap;
        }

        public static Bitmap Crop(Bitmap bitmap, RectangleF region)
        {
            if (region.IsEmpty || (region.X == 0 && region.Y == 0 && region.Width == bitmap.Width && region.Height == bitmap.Height))
                return bitmap;

            return bitmap.Clone(region, bitmap.PixelFormat);
        }

        public static Dictionary<Sprite, Bitmap> Crop(SpriteSet spriteSet)
        {
            var bitmaps = new List<Bitmap>(spriteSet.TextureSet.Textures.Count);
            foreach (var texture in spriteSet.TextureSet.Textures)
            {
                var bitmap = TextureDecoder.Decode(texture);
                bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                bitmaps.Add(bitmap);
            }

            var sprites = new Dictionary<Sprite, Bitmap>(spriteSet.Sprites.Count);
            foreach (var sprite in spriteSet.Sprites)
            {
                var sourceBitmap = bitmaps[sprite.TextureIndex];
                var bitmap = sourceBitmap.Clone(sprite.GetSourceRectangle(), sourceBitmap.PixelFormat);
                sprites.Add(sprite, bitmap);
            }

            return sprites;
        }

        /// <summary>
        /// Insert the <paramref name="source"/> <see cref="Bitmap"/> into the <paramref name="destination"/> <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="source">
        /// The <see cref="Bitmap"/> to be inserted into the <paramref name="destination"/>.
        /// </param>
        /// <param name="destination">
        /// The <see cref="Bitmap"/> in which the <paramref name="source"/> will be inserted.
        /// </param>
        /// <param name="region">
        /// The region of the <paramref name="destination"/> in which the <paramref name="source"/> will be inserted in.
        /// </param>
        public static void Insert(Bitmap source, Bitmap destination, RectangleF region)
        {
            using (Graphics graphics = Graphics.FromImage(destination))
            {
                graphics.DrawImage(source, Rectangle.Round(region));
            }
        }
    }
}
