﻿using MikuMikuLibrary.Processing.Textures;
using MikuMikuLibrary.Sprites;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MikuMikuLibrary.Processing.Sprites
{
    public static class SpriteCropper
    {
        public static Bitmap Crop( Sprite sprite, SpriteSet parentSet )
        {
            var bitmap = TextureDecoder.Decode(
                parentSet.TextureSet.Textures[ sprite.TextureIndex ] );
            bitmap.RotateFlip( RotateFlipType.Rotate180FlipX );

            var croppedBitmap = bitmap.Clone(
                new RectangleF( sprite.X, sprite.Y, sprite.Width, sprite.Height ), bitmap.PixelFormat );

            bitmap.Dispose();
            return croppedBitmap;
        }

        public static List<Tuple<Sprite, Bitmap>> Crop( SpriteSet spriteSet )
        {
            var bitmaps = new List<Bitmap>( spriteSet.TextureSet.Textures.Count );
            foreach ( var texture in spriteSet.TextureSet.Textures )
            {
                var bitmap = TextureDecoder.Decode( texture );
                bitmap.RotateFlip( RotateFlipType.Rotate180FlipX );
                bitmaps.Add( bitmap );
            }

            var sprites = new List<Tuple<Sprite, Bitmap>>( spriteSet.Sprites.Count );
            foreach ( var sprite in spriteSet.Sprites )
            {
                var sourceBitmap = bitmaps[ sprite.TextureIndex ];
                var bitmap = sourceBitmap.Clone(
                    new RectangleF( sprite.X, sprite.Y, sprite.Width, sprite.Height ), sourceBitmap.PixelFormat );
                sprites.Add( new Tuple<Sprite, Bitmap>( sprite, bitmap ) );
            }

            return sprites;
        }
    }
}
