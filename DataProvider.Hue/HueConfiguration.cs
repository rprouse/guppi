using System;
using System.IO;
using ColoredConsole;

namespace DataProvider.Hue
{
    class HueConfiguration
    {
        string _ip;

        public HueConfiguration(string ip)
        {
            _ip = ip;
        }

        public string LoadKey()
        {
            string keyfile = KeyFilename;

            if (!File.Exists(keyfile))
                return null;

            try
            {
                var key = File.ReadAllText(keyfile);
                if (!string.IsNullOrWhiteSpace(key))
                    return key;
            }
            catch { }

            return null;
        }

        public bool SaveKey(string key)
        {
            try
            {
                File.WriteAllText(KeyFilename, key);
            }
            catch (Exception e)
            {
                ColorConsole.WriteLine("Failed to save bridge registration.".Red());
                ColorConsole.WriteLine(e.Message.Red());
                return false;
            }
            return true;
        }

        string KeyFilename =>
            Path.Combine(GetConfigDirectory(), $"{_ip}.key");

        string GetConfigDirectory()
        {
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create), "Guppi", "hue");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
