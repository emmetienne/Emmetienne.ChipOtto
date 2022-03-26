namespace Emmetienne.ChipOtto.Model
{
    public class ConfigurationModel
    {
        public DisplaySettings DisplaySettings { get; set; }
        public string DefaultRomPath { get; set; }
        public string BeepSoundPath { get; set; }
        public bool Debug { get; set; }

        public ConfigurationModel GetDefault()
        {
            var configurationModel = new ConfigurationModel();
            configurationModel.DisplaySettings = new DisplaySettings();
            configurationModel.Debug = true;
            configurationModel.DefaultRomPath = string.Empty;
            configurationModel.BeepSoundPath = string.Empty;

            return configurationModel;
        }
    }
}
