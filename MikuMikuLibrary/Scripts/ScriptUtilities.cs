using MikuMikuLibrary.IO;
using MikuMikuLibrary.Scripts.Descriptors;
using System;
using System.Linq;
using System.Text;

namespace MikuMikuLibrary.Scripts
{
    public static class ScriptUtilities
    {
        private const double MS_TIME_FACTOR = 100.0;

        public static byte[] PVSC_MAGIC { get; } = Encoding.ASCII.GetBytes("PVSC");
        
        public static FormatDescriptor GetFormatDescriptor(BinaryFormat format)
        {
            switch (format)
            {
                case BinaryFormat.DT:
                    return DtDescriptor.Instance;

                case BinaryFormat.F:
                    return FDescriptor.Instance;

                case BinaryFormat.FT:
                    return FtDescriptor.Instance;

                case BinaryFormat.F2nd:
                    return F2ndDescriptor.Instance;

                case BinaryFormat.X:
                    return XDescriptor.Instance;

                default:
                    throw new Exception($"Invalid format: {nameof(BinaryFormat)}.{format}");
            }
        }

        public static uint GetDefaultScriptFormat(BinaryFormat format)
        {
            return GetScriptFormats(format).FirstOrDefault();
        }

        public static uint[] GetScriptFormats(BinaryFormat format)
        {
            return GetFormatDescriptor(format).ScriptFormats;
        }

        public static int GetDivaTime(TimeSpan timeSpan)
        {
            return (int)(timeSpan.TotalMilliseconds * MS_TIME_FACTOR);
        }

        public static TimeSpan GetTimeSpan(int divaTime)
        {
            return TimeSpan.FromMilliseconds(divaTime / MS_TIME_FACTOR);
        }
    }
}
