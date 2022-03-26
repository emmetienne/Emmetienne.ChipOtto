namespace Emmetienne.ChipOtto.Model
{
    public class DisplaySettings
    {
        public int Scale { get; set; }
        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }

        public DisplaySettings(int scale, Color background, Color foreground)
        {
            Scale = scale;
            BackgroundColor = background;
            ForegroundColor = foreground;
        }

        public DisplaySettings()
        {
            this.Scale = 10;
            this.BackgroundColor = new Color(0, 52, 5, 255);
            this.ForegroundColor = new Color(3, 205, 45, 255);
        }
    }
}
