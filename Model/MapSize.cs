namespace ConsoleGame.Model
{
    class MapSize
    {
        public byte Width { get; set; }
        public byte Height { get; set; }

        public MapSize(byte width, byte height)
        {
            Width = width;
            Height = height;
        }
    }
}
