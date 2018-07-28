namespace MikuMikuLibrary.Textures
{
    public enum TextureFormat
    {
        RGB = 1,
        RGBA = 2,
        DXT1 = 6,
        DXT3 = 7,
        DXT5 = 9,
        ATI1 = 10,
        ATI2 = 11,
    }

    public static class TextureFormatUtilities
    {
        public static int GetBitsPerPixel( TextureFormat format )
        {
            switch ( format )
            {
                case TextureFormat.RGB:
                    return 24;

                case TextureFormat.RGBA:
                    return 32;

                case TextureFormat.DXT1:
                case TextureFormat.ATI1:
                    return 4;

                case TextureFormat.DXT3:
                case TextureFormat.DXT5:
                case TextureFormat.ATI2:
                    return 8;
            }

            return 0;
        }

        public static int CalculateDataSize( int width, int height, TextureFormat format )
        {
            return ( ( width * height ) * GetBitsPerPixel( format ) ) / 8;
        }
    }
}
