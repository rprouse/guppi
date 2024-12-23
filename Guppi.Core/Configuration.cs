using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Guppi.Core.Attributes;
using Guppi.Core.Extensions;
using Spectre.Console;

namespace Guppi.Core
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
            var configurationFile = Path.Combine(ConfigurationDirectory, $"{name}.json");

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
                AnsiConsole.MarkupLine($"[red][[:cross_mark: Failed to load the configuration file for {name}]][/]");
                AnsiConsole.WriteException(ex,
                    ExceptionFormats.ShortenPaths | ExceptionFormats.ShortenTypes |
                    ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks);
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
                AnsiConsole.MarkupLine($"[red][[:cross_mark: Failed to save the configuration file for {Name}]][/]");
                AnsiConsole.WriteException(ex,
                    ExceptionFormats.ShortenPaths | ExceptionFormats.ShortenTypes |
                    ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks);
            }
        }

        public void RunConfiguration(string name, string description)
        {
            AnsiConsoleHelper.TitleRule($":wrench: Configure {name}");
            AnsiConsole.MarkupLine($"[silver][[{description}]][/]");
            AnsiConsole.WriteLine();

            foreach (PropertyInfo prop in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.PropertyType == typeof(string) && p.GetCustomAttribute<HideAttribute>() == null))
            {
                ConfigureProperty(prop);
            }

            ConfigureCustomProperties();

            Enabled = true;
            Configured = true;
            Save();
            AnsiConsoleHelper.Rule("white");
        }

        /// <summary>
        /// Override this property to configure non-string properties
        /// </summary>
        protected virtual void ConfigureCustomProperties() { }

        private void ConfigureProperty(PropertyInfo prop)
        {
            var attr = prop.GetCustomAttribute<DisplayAttribute>();
            string name = attr != null ? attr.Description : prop.Name;
            string value = prop.GetValue(this) as string ?? "";

            string newValue = AnsiConsole.Prompt(
                new TextPrompt<string>($"[green]{name}[/] [silver][[{value}]][/]")
                    .AllowEmpty()
                );
            if (!string.IsNullOrWhiteSpace(newValue))
                prop.SetValue(this, newValue);
        }

        public static string ConfigurationDirectory =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create), "Guppi");

        /// <summary>
        /// The name of the configuration file without the extension
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetConfigurationFile(string name, string extension = "json") =>
            Path.Combine(ConfigurationDirectory, $"{name}.{extension}");
    }
}
