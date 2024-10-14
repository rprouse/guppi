namespace Guppi.Core.Entities.Ascii
{
    public readonly struct AsciiData(int value, string character, string description)
    {
        public int Value { get; } = value;
        public string Character { get; } = character;
        public string Description { get; } = description;
    }
}
