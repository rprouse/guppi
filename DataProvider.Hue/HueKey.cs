using System;
using System.IO;
using Spectre.Console;

namespace DataProvider.Hue
{
    class HueKey
    {
        string _ip;

        public HueKey(string ip)
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
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[red][[Failed to save bridge registration.]][/]");
                AnsiConsole.WriteException(ex,
                    ExceptionFormats.ShortenPaths | ExceptionFormats.ShortenTypes |
                    ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks);
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
