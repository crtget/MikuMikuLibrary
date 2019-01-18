namespace MikuMikuLibrary.Scripts
{
    public abstract class FormatDescriptor
    {
        public abstract uint[] ScriptFormats { get; }

        public abstract CommandInfo[] CommandData { get; }

        protected virtual bool IsValid(int opcode)
        {
            return opcode >= 0 && opcode < CommandData.Length;
        }

        public virtual bool TryGetCommandInfo(int opcode, out CommandInfo commandInfo)
        {
            bool valid = IsValid(opcode);
            commandInfo = valid ? CommandData[opcode] : CommandInfo.Unknown;

            return valid;
        }

        protected FormatDescriptor()
        {
            return;
        }
    }
}
