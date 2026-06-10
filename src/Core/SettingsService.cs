/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psycho-core]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;
using System.IO;
using System.Text.Json;

namespace PsychoBuddy.Core
{
    public class SettingsService
    {
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        public string SettingsPath { get; }

        public SettingsService(string? settingsPath = null)
        {
            SettingsPath = string.IsNullOrWhiteSpace(settingsPath)
                ? Path.Combine(AppContext.BaseDirectory, "settings.json")
                : settingsPath;
        }

        public AppSettings Load()
        {
            try
            {
                if (!File.Exists(SettingsPath))
                {
                    return new AppSettings();
                }

                string json = File.ReadAllText(SettingsPath);
                return JsonSerializer.Deserialize<AppSettings>(json, _jsonOptions) ?? new AppSettings();
            }
            catch
            {
                return new AppSettings();
            }
        }

        public void Save(AppSettings settings)
        {
            string? directory = Path.GetDirectoryName(SettingsPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonSerializer.Serialize(settings, _jsonOptions);
            File.WriteAllText(SettingsPath, json);
        }
    }
}
