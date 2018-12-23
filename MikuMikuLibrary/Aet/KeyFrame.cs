namespace MikuMikuLibrary.Aet
{
    public class KeyFrame
    {
        public float Frame { get; set; }

        public float Value { get; set; }

        public float Bounciness { get; set; }

        public KeyFrame(float value) : this(0, value)
        {
            return;
        }

        public KeyFrame(float frame, float value) : this(frame, value, 0)
        {
            return;
        }

        public KeyFrame(float frame, float value, float bounciness)
        {
            Frame = frame;
            Value = value;
            Bounciness = bounciness;
        }

        public override string ToString()
        {
            return $"{nameof(Frame)}: {Frame}; {nameof(Value)}: {Value}; {Bounciness}";
        }
    }
}
