/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psycho-core]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using PsychoBuddy.Senses;

namespace PsychoBuddy.Brain
{
    public class ProfileService
    {
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public string ProfilesPath { get; }

        public ProfileService(string? profilesPath = null)
        {
            ProfilesPath = string.IsNullOrWhiteSpace(profilesPath)
                ? Path.Combine(AppContext.BaseDirectory, "profiles")
                : profilesPath;
        }

        public List<ProfileDefinition> LoadProfiles()
        {
            EnsureDefaultProfilesExist();

            List<ProfileDefinition> profiles = new List<ProfileDefinition>();
            foreach (string file in Directory.GetFiles(ProfilesPath, "*.json", SearchOption.AllDirectories))
            {
                try
                {
                    string json = File.ReadAllText(file);
                    ProfileDefinition? profile = JsonSerializer.Deserialize<ProfileDefinition>(json, _jsonOptions);
                    if (profile != null && profile.IsEnabled && !string.IsNullOrWhiteSpace(profile.DisplayName))
                    {
                        EnsureExecutableRuleFallback(profile);
                        profiles.Add(profile);
                    }
                }
                catch
                {
                    // Ignore malformed profile files for now. A future Profiles page can surface validation errors.
                }
            }

            if (profiles.Count == 0)
            {
                profiles.AddRange(GetDefaultProfiles());
            }

            return profiles
                .OrderBy(profile => profile.DisplayName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public ProfileDefinition? GetProfileByDisplayName(string? displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                return LoadProfiles().FirstOrDefault();
            }

            return LoadProfiles()
                .FirstOrDefault(profile => profile.DisplayName.Equals(displayName, StringComparison.OrdinalIgnoreCase));
        }

        public RotationProfile BuildRotationProfile(string? displayName)
        {
            ProfileDefinition profile = GetProfileByDisplayName(displayName) ?? GetDefaultProfiles().First();
            return BuildRotationProfile(profile);
        }

        public RotationProfile BuildRotationProfile(ProfileDefinition profile)
        {
            EnsureExecutableRuleFallback(profile);
            RotationProfile rotationProfile = new RotationProfile { ProfileName = profile.DisplayName };

            foreach (ProfileRuleDefinition rule in profile.Rules.OrderBy(rule => rule.Priority))
            {
                rotationProfile.Rules.Add(new PriorityRule
                {
                    AbilityToCast = new Ability
                    {
                        Name = rule.AbilityName,
                        SpellID = rule.SpellId,
                        KeyCode = rule.KeyCode,
                        Cooldown = rule.CooldownSeconds
                    },
                    Condition = BuildCondition(rule)
                });
            }

            return rotationProfile;
        }

        public void EnsureDefaultProfilesExist()
        {
            Directory.CreateDirectory(ProfilesPath);

            foreach (ProfileDefinition profile in GetDefaultProfiles())
            {
                string fileName = ToSafeFileName(profile.DisplayName) + ".json";
                string path = Path.Combine(ProfilesPath, fileName);
                if (File.Exists(path)) continue;

                string json = JsonSerializer.Serialize(profile, _jsonOptions);
                File.WriteAllText(path, json);
            }
        }

        private static Func<UnitData, bool> BuildCondition(ProfileRuleDefinition rule)
        {
            string condition = rule.Condition?.Trim() ?? "Always";
            float threshold = rule.Threshold;

            return condition.ToLowerInvariant() switch
            {
                "healthbelow" => state => state.HealthPercentage < threshold,
                "healthabove" => state => state.HealthPercentage > threshold,
                "manabelow" => state => state.ManaPercentage < threshold,
                "manaabove" => state => state.ManaPercentage > threshold,
                "isdead" => state => state.IsDead,
                "isalive" => state => !state.IsDead,
                _ => _ => true
            };
        }

        private static void EnsureExecutableRuleFallback(ProfileDefinition profile)
        {
            if (profile.Rules.Count > 0) return;

            profile.Rules.Add(new ProfileRuleDefinition
            {
                Priority = 100,
                AbilityName = "Basic Action",
                SpellId = 0,
                KeyCode = 0x31,
                CooldownSeconds = 1.0f,
                Condition = "Always"
            });
        }

        private static List<ProfileDefinition> GetDefaultProfiles()
        {
            return new List<ProfileDefinition>
            {
                new ProfileDefinition
                {
                    Id = "basic-rotation",
                    DisplayName = "Basic Rotation",
                    Description = "Starter profile used for dashboard testing. Uses a basic action placeholder until real rotations are implemented.",
                    Role = "DPS",
                    Version = "Any",
                    Specialization = "Generic",
                    Tags = new List<string> { "starter", "test", "generic" },
                    Rules = new List<ProfileRuleDefinition>
                    {
                        new ProfileRuleDefinition { Priority = 100, AbilityName = "Basic Action", KeyCode = 0x31, CooldownSeconds = 1.0f, Condition = "Always" }
                    }
                },
                new ProfileDefinition
                {
                    Id = "tank-assist",
                    DisplayName = "Tank Assist",
                    Description = "Tank-oriented placeholder profile for future threat, mitigation, and pull-control behavior.",
                    Role = "Tank",
                    Version = "Legion/BFA",
                    Specialization = "Generic Tank",
                    Tags = new List<string> { "tank", "assist", "placeholder" },
                    Rules = new List<ProfileRuleDefinition>
                    {
                        new ProfileRuleDefinition { Priority = 10, AbilityName = "Defensive Action", KeyCode = 0x32, CooldownSeconds = 5.0f, Condition = "HealthBelow", Threshold = 45 },
                        new ProfileRuleDefinition { Priority = 100, AbilityName = "Tank Filler", KeyCode = 0x31, CooldownSeconds = 1.0f, Condition = "Always" }
                    }
                },
                new ProfileDefinition
                {
                    Id = "healer-assist",
                    DisplayName = "Healer Assist",
                    Description = "Healer-oriented placeholder profile for future party triage and tank-priority healing behavior.",
                    Role = "Healer",
                    Version = "Legion/BFA",
                    Specialization = "Generic Healer",
                    Tags = new List<string> { "healer", "assist", "placeholder" },
                    Rules = new List<ProfileRuleDefinition>
                    {
                        new ProfileRuleDefinition { Priority = 10, AbilityName = "Emergency Heal", KeyCode = 0x32, CooldownSeconds = 3.0f, Condition = "HealthBelow", Threshold = 55 },
                        new ProfileRuleDefinition { Priority = 100, AbilityName = "Healer Filler", KeyCode = 0x31, CooldownSeconds = 1.5f, Condition = "Always" }
                    }
                },
                new ProfileDefinition
                {
                    Id = "dps-assist",
                    DisplayName = "DPS Assist",
                    Description = "Damage-dealer placeholder profile for future target assist, cooldown, and rotation behavior.",
                    Role = "DPS",
                    Version = "Legion/BFA",
                    Specialization = "Generic DPS",
                    Tags = new List<string> { "dps", "assist", "placeholder" },
                    Rules = new List<ProfileRuleDefinition>
                    {
                        new ProfileRuleDefinition { Priority = 20, AbilityName = "Resource Dump", KeyCode = 0x32, CooldownSeconds = 2.0f, Condition = "ManaAbove", Threshold = 80 },
                        new ProfileRuleDefinition { Priority = 100, AbilityName = "DPS Filler", KeyCode = 0x31, CooldownSeconds = 1.0f, Condition = "Always" }
                    }
                },
                new ProfileDefinition
                {
                    Id = "manual-follow",
                    DisplayName = "Manual Follow",
                    Description = "Utility placeholder profile intended for follow/assist behavior without autonomous combat decisions.",
                    Role = "Utility",
                    Version = "Any",
                    Specialization = "Follow",
                    Tags = new List<string> { "utility", "follow", "placeholder" },
                    Rules = new List<ProfileRuleDefinition>
                    {
                        new ProfileRuleDefinition { Priority = 100, AbilityName = "Follow Pulse", KeyCode = 0x30, CooldownSeconds = 5.0f, Condition = "Always" }
                    }
                }
            };
        }

        private static string ToSafeFileName(string name)
        {
            char[] invalid = Path.GetInvalidFileNameChars();
            string safe = new string(name.Select(ch => invalid.Contains(ch) ? '_' : ch).ToArray());
            return safe.Replace(" ", string.Empty);
        }
    }
}
