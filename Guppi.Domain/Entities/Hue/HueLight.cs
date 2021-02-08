namespace Guppi.Domain.Entities.Hue
{
    public class HueLight
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public bool On { get; set; }

        public byte Brightness { get; set; }
    }
}
