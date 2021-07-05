using MagicConchQQRobot.Modules.SecretProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCode.DataAccessLayer;

namespace MagicConchQQRobot
{
    public class DBAccess
    {
        public static void InitDataBase()
        {
            foreach(var item in SecretData.GetChildren("ConnectionStrings"))
            {
                DAL.AddConnStr(item.Key, item.GetSection("connectionString").Value, null, item.GetSection("providerName").Value);
            }
        }
    }
}
