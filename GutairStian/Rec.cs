namespace GutairStian
{
    public class Rec
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string WavPath { get; set; }
        public Rec(string name, string imagePath, string wavPath)
        {
            Name = name;
            ImagePath = imagePath;
            WavPath = wavPath;
        }
        public Rec(string imagePath, string wavPath)
        {
            ImagePath = imagePath;
            WavPath = wavPath;
        }
    }
}
