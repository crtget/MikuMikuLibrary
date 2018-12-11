namespace MikuMikuLibrary.Animations.Aet
{
    /// <summary>
    /// The different predefined fragment shaders a <see cref="Body.PicBody"/> can draw a <see cref="Sprites.Sprite"/> with.
    /// </summary>
    public enum ShaderType : short
    {
        DEFAULT = 0x0003,
        BLACK_ALPHA = 0x0005,
        BLUE_TINT = 0x0006,
        INVERT = 0x0007,
        TRANSPARENT = 0x0008,
    }
}
