using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using ColoredConsole;
using MyDay.Core.Attributes;

namespace MyDay.Core
{
    public class Configuration
    {
        private string Name { get; set; }
        private string ConfigurationFile { get; set; }

        [Hide]
        public bool Enabled { get; set; }

        [Hide]
        public bool Configured { get; set; }

        public static T Load<T>(string name) where T : Configuration, new()
        {

            var configurationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create), "MyDay");
            var configurationFile = Path.Combine(configurationDirectory, $"{name}.json");

            try
            {
                if (File.Exists(configurationFile))
                {
                    string json = File.ReadAllText(configurationFile);
                    T config = JsonSerializer.Deserialize<T>(json);
                    config.Name = name;
                    config.ConfigurationFile = configurationFile;
                    return config;
                }
            }
            catch (Exception ex)
            {
                ColorConsole.WriteLine($"Failed to load the configuration file for {name}".Red());
                ColorConsole.WriteLine(ex.Message.Gray());
            }
            return new T()
            {
                Name = name,
                ConfigurationFile = configurationFile
            };
        }

        public void Save()
        {
            try
            {
                string json = JsonSerializer.Serialize(this, GetType());
                File.WriteAllText(ConfigurationFile, json);
            }
            catch (Exception ex)
            {
                ColorConsole.WriteLine($"Failed to save the configuration file for {Name}".Red());
                ColorConsole.WriteLine(ex.Message.Gray());
            }
        }

        public void RunConfiguration(string name, string description)
        {
            Console.Clear();
            ColorConsole.WriteLine($"Configure {name}".Yellow());
            Console.WriteLine();
            ColorConsole.WriteLine(description.White());
            Console.WriteLine();

            foreach (PropertyInfo prop in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.PropertyType == typeof(string) && p.GetCustomAttribute<HideAttribute>() == null))
            {
                ConfigureProperty(prop);
            }

            Enabled = true;
            Configured = true;
            Save();
        }

        private void ConfigureProperty(PropertyInfo prop)
        {
            var attr = prop.GetCustomAttribute<DisplayAttribute>();
            string name = attr != null ? attr.Description : prop.Name;
            string value = prop.GetValue(this) as string ?? "";

            ColorConsole.Write($"{name} ".Green(), $"[{value}]: ");
            string newValue = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newValue))
                prop.SetValue(this, newValue);
        }
    }
}
