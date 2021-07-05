using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicConchQQRobot.Modules.SecretProvider
{
    class SecretData
    {
        private static IConfigurationRoot SecretConfig;
        public static void LoadSecretData()
        {
            SecretConfig= new ConfigurationBuilder().AddUserSecrets<Program>().Build();
        }

        public static string GetSectionData(string sectionPath)
        {
            return SecretConfig.GetSection(sectionPath).Value;
        }

        public static IEnumerable<IConfigurationSection> GetChildren(string path)
        {
            return SecretConfig.GetSection(path).GetChildren();
        }
    }
}
