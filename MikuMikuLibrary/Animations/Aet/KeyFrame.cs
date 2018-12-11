namespace MikuMikuLibrary.Animations.Aet
{
    public struct KeyFrame
    {
        public float Frame { get; set; }

        public float Value { get; set; }

        public float Bounciness { get; set; }

        public KeyFrame(float value)
        {
            Value = value;
            Frame = 0;
            Bounciness = 0;
        }

        public KeyFrame(float frame, float value, float bounciness)
        {
            Frame = frame;
            Value = value;
            Bounciness = bounciness;
        }

        public override string ToString()
        {
            return $"Frame: {Frame}; Value: {Value}; {Bounciness}";
        }
    }
}
