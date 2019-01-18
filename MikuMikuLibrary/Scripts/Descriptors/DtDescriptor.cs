namespace MikuMikuLibrary.Scripts.Descriptors
{
    internal class DtDescriptor : FormatDescriptor
    {
        private static DtDescriptor instance;

        public static DtDescriptor Instance => instance ?? (instance = new DtDescriptor());

        public override uint[] ScriptFormats { get; } =
        {
            0x10120116,
            0x11032818,
            0x11021719,
        };

        public override CommandInfo[] CommandData { get; } =
        {
            new CommandInfo(0x00, -1, 000, "END"),
            new CommandInfo(0x01, -1, 001, "TIME"),
            new CommandInfo(0x02, -1, 004, "MIKU_MOVE"),
            new CommandInfo(0x03, -1, 002, "MIKU_ROT"),
            new CommandInfo(0x04, -1, 002, "MIKU_DISP"),
            new CommandInfo(0x05, -1, 002, "MIKU_SHADOW"),
            new CommandInfo(0x06, -1, 011, "TARGET"),
            new CommandInfo(0x07, -1, 004, "SET_MOTION"),
            new CommandInfo(0x08, -1, 002, "SET_PLAYDATA"),
            new CommandInfo(0x09, -1, 006, "EFFECT"),
            new CommandInfo(0x0A, -1, 002, "FADEIN_FIELD"),
            new CommandInfo(0x0B, -1, 001, "EFFECT_OFF"),
            new CommandInfo(0x0C, -1, 006, "SET_CAMERA"),
            new CommandInfo(0x0D, -1, 002, "DATA_CAMERA"),
            new CommandInfo(0x0E, -1, 001, "CHANGE_FIELD"),
            new CommandInfo(0x0F, -1, 001, "HIDE_FIELD"),
            new CommandInfo(0x10, -1, 003, "MOVE_FIELD"),
            new CommandInfo(0x11, -1, 002, "FADEOUT_FIELD"),
            new CommandInfo(0x12, -1, 003, "EYE_ANIM"),
            new CommandInfo(0x13, -1, 005, "MOUTH_ANIM"),
            new CommandInfo(0x14, -1, 005, "HAND_ANIM"),
            new CommandInfo(0x15, -1, 004, "LOOK_ANIM"),
            new CommandInfo(0x16, -1, 004, "EXPRESSION"),
            new CommandInfo(0x17, -1, 005, "LOOK_CAMERA"),
            new CommandInfo(0x18, -1, 002, "LYRIC"),
            new CommandInfo(0x19, -1, 000, "MUSIC_PLAY"),
            new CommandInfo(0x1A, -1, 002, "MODE_SELECT"),
            new CommandInfo(0x1B, -1, 004, "EDIT_MOTION"),
            new CommandInfo(0x1C, -1, 002, "BAR_TIME_SET"),
            new CommandInfo(0x1D, -1, 002, "SHADOWHEIGHT"),
            new CommandInfo(0x1E, -1, 001, "EDIT_FACE"),
            new CommandInfo(0x1F, -1, 021, "MOVE_CAMERA"),
            new CommandInfo(0x20, -1, 000, "PV_END"),
            new CommandInfo(0x21, -1, 003, "SHADOWPOS"),
            new CommandInfo(0x22, -1, 002, "EDIT_LYRIC"),
            new CommandInfo(0x23, -1, 005, "EDIT_TARGET"),
            new CommandInfo(0x24, -1, 001, "EDIT_MOUTH"),
            new CommandInfo(0x25, -1, 001, "SET_CHARA"),
            new CommandInfo(0x26, -1, 007, "EDIT_MOVE"),
            new CommandInfo(0x27, -1, 001, "EDIT_SHADOW"),
            new CommandInfo(0x28, -1, 001, "EDIT_EYELID"),
            new CommandInfo(0x29, -1, 002, "EDIT_EYE"),
            new CommandInfo(0x2A, -1, 001, "EDIT_ITEM"),
            new CommandInfo(0x2B, -1, 002, "EDIT_EFFECT"),
            new CommandInfo(0x2C, -1, 001, "EDIT_DISP"),
            new CommandInfo(0x2D, -1, 002, "EDIT_HAND_ANIM"),
            new CommandInfo(0x2E, -1, 003, "AIM"),
            new CommandInfo(0x2F, -1, 003, "HAND_ITEM"),
            new CommandInfo(0x30, -1, 001, "EDIT_BLUSH"),
            new CommandInfo(0x31, -1, 002, "NEAR_CLIP"),
            new CommandInfo(0x32, -1, 002, "CLOTH_WET"),
            new CommandInfo(0x33, -1, 003, "LIGHT_ROT"),
            new CommandInfo(0x34, -1, 006, "SCENE_FADE"),
            new CommandInfo(0x35, -1, 006, "TONE_TRANS"),
            new CommandInfo(0x36, -1, 001, "SATURATE"),
            new CommandInfo(0x37, -1, 001, "FADE_MODE"),
            new CommandInfo(0x38, -1, 002, "AUTO_BLINK"),
            new CommandInfo(0x39, -1, 003, "PARTS_DISP"),
            new CommandInfo(0x3A, -1, 001, "TARGET_FLYING_TIME"),
            new CommandInfo(0x3B, -1, 002, "CHARA_SIZE"),
            new CommandInfo(0x3C, -1, 002, "CHARA_HEIGHT_ADJUST"),
            new CommandInfo(0x3D, -1, 004, "ITEM_ANIM"),
            new CommandInfo(0x3E, -1, 004, "CHARA_POS_ADJUST"),
            new CommandInfo(0x3F, -1, 001, "SCENE_ROT"),
        };
    }
}
