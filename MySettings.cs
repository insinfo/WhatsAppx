using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WhatsAppx
{
    public class MySettings
    {
        private const string UserSettingsFilename = "settings.xml";
        private string _UserSettingsPath = AppDomain.CurrentDomain.BaseDirectory +
            "\\Settings\\" + UserSettingsFilename;

        /******* Settings *********/
        public string Server { get; set; } = "192.168.16.113";//"127.0.0.1" localhost
        public string Port { get; set; } = "3306";//3306
        public string User { get; set; } = "insinfo";//"sisadmin";//"postgres"
        public string Password { get; set; } = "Ins257257";//"Ins257257"
        public string Database { get; set; } = "whatsappx";
        public string Device { get; set; } = "Bernardo Oliveira";
        public int Limit { get; set; } = 1;
        public int SendIntervalMin { get; set; } = 60000;
        public int SendIntervalMax { get; set; } = 120000;
        public string[] UserAgents = {
        
          "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0",
          "Mozilla/5.0 (Macintosh; Intel Mac OS X x.y; rv:42.0) Gecko/20100101 Firefox/42.0",
          "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36",
          "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36 OPR/38.0.2220.41",
          "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3_1 like Mac OS X) AppleWebKit/603.1.30 (KHTML, like Gecko) Version/10.0 Mobile/14E304 Safari/602.1",
          "Mozilla/5.0 (compatible; MSIE 9.0; Windows Phone OS 7.5; Trident/5.0; IEMobile/9.0)",
          "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36",
          "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.181 Safari/537.36",
          "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:61.0) Gecko/20100101 Firefox/61.0",
          "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko",
          "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0"
        };
        public string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";

        public void Save()
        {
            using (StreamWriter sw = new StreamWriter(_UserSettingsPath))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(MySettings));
                xmls.Serialize(sw, this);
            }
        }
        public MySettings Read()
        {
            using (StreamReader sw = new StreamReader(_UserSettingsPath))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(MySettings));
                return xmls.Deserialize(sw) as MySettings;
            }
        }
    }
}
