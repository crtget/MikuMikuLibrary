namespace MikuMikuLibrary.Scripts
{
    public sealed class PvCommand
    {
        public int Time { get; set; }
        
        public int Opcode { get; set; }

        public int[] Arguments { get; set; }

        private PvCommand()
        {
            return;
        }

        public PvCommand(int time, int opcode, params int[] arguments)
        {
            Time = time;
            Opcode = opcode;
            Arguments = arguments;
        }

        private string FormatTime()
        {
            return $"{Time / 1000.0:0.000}";
        }

        private string FormatArgumentList()
        {
            return Arguments == null ? string.Empty : string.Join(", ", Arguments);
        }

        public string GetCommandName(FormatDescriptor formatDescriptor)
        {
            formatDescriptor.TryGetCommandInfo(Opcode, out var commandInfo);
            return commandInfo.Name;
        }

        public string FormatString(FormatDescriptor formatDescriptor)
        {
            return $"{GetCommandName(formatDescriptor)}({FormatArgumentList()})";
        }

        public override string ToString()
        {
            return $"[0x{Opcode:X2}] Command({FormatArgumentList()})";
        }
    }
}
