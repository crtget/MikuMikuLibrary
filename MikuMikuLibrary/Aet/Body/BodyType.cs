namespace MikuMikuLibrary.Aet.Body
{
    /// <summary>
    /// The different kinds of <see cref="AetObjBody"/>s an <see cref="AetObj"/> can contain.
    /// </summary>
    public enum BodyType : byte
    {
        /// <summary>
        /// <see cref="NopBody"/>; Does nothing.
        /// </summary>
        NOP = 0x00,

        /// <summary>
        /// <see cref="PicBody"/>; Draws/animates a <see cref="Sprites.Sprite"/>.
        /// </summary>
        PIC = 0x01,

        /// <summary>
        /// <see cref="AifBody"/>; Plays a sound effect.
        /// </summary>
        AIF = 0x02,

        /// <summary>
        /// <see cref="EffBody"/>; Draws/animates a <see cref="PicBody"/>.
        /// </summary>
        EFF = 0x03,
    }
}