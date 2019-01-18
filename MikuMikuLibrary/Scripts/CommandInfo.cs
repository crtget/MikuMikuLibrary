namespace MikuMikuLibrary.Scripts
{
    public class CommandInfo
    {
        public static CommandInfo Unknown
        {
            get => new CommandInfo(-1, -1, -1, "UNKNOWN");
        }

        public int Opcode { get; }

        public int Dummy { get; }

        public int ArgumentCount { get; }

        public string Name { get; }

        internal CommandInfo(int opcode, int dummy, int argumentCount, string name)
        {
            Opcode = opcode;
            Dummy = dummy;
            ArgumentCount = argumentCount;
            Name = name;
        }
    }
}
