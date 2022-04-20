namespace Guppi.Domain.Entities.Ascii
{
    public struct AsciiData
    {
        public int Value { get; }
        public string Character { get; }
        public string Description { get; }

        public AsciiData(int value, string character, string description)
        {
            Value = value;
            Character = character;
            Description = description;
        }
    }
}
