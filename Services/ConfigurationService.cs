using Emmetienne.ChipOtto.Model;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Emmetienne.ChipOtto.Services
{
    public class ConfigurationService
    {
        public ConfigurationModel GetConfiguration()
        {
            try
            {
                using (var streamReader = new StreamReader($"{AppContext.BaseDirectory}/Configurations/Configuration.json"))
                {
                    string configJson = streamReader.ReadToEnd();
                    var configuration = JsonConvert.DeserializeObject<ConfigurationModel>(configJson);

                    return configuration;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error in reading or deserializing configuration: {ex.Message}{Environment.NewLine}Loading default");
                var configurationModel = new ConfigurationModel();
                return configurationModel.GetDefault();
            }
        }
    }
}
