namespace MikuMikuLibrary.Aet
{
    /// <summary>
    /// The different predefined fragment shaders a <see cref="Body.PicBody"/> can draw a <see cref="Sprites.Sprite"/> with.
    /// </summary>
    public enum ShaderType : short
    {
        UNK_0x0000 = 0x0000,
        UNK_0x0001 = 0x0001,
        UNK_0x0002 = 0x0002,
        UNK_0x0004 = 0x0004,
        UNK_0x0009 = 0x0009,

        DEFAULT = 0x0003,
        BLACK_ALPHA = 0x0005,
        BLUE_TINT = 0x0006,
        INVERT = 0x0007,
        TRANSPARENT = 0x0008,
    }
}
