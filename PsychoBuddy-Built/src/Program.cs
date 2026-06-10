using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using PsychoBuddy.Core;
using PsychoBuddy.Senses;
using PsychoBuddy.Muscles;
using PsychoBuddy.Brain;

namespace PsychoBuddy
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== PsychoBuddy Foundation Initialized ===");
            Console.WriteLine("Starting Attachment System scan...");

            AttachmentManager attachmentManager = new AttachmentManager();
            List<ClientBinding> clients = attachmentManager.ScanForClients();

            if (clients.Count == 0)
            {
                Console.WriteLine("No WoW clients found. Please ensure the game is running.");
            }
            else
            {
                Console.WriteLine($"Found {clients.Count} potential client(s). Initializing Fleet Orchestrator...");
                
                InputManager sharedInput = new InputManager();
                Orchestrator orchestrator = new Orchestrator(sharedInput);

                foreach (var client in clients)
                {
                    BotRole role = BotRole.DPS;
                    if (client.CharacterName.Contains("Atlas")) role = BotRole.Tank;
                    else if (client.CharacterName.Contains("Luna")) role = BotRole.Healer;

                    RotationProfile profile = new RotationProfile { ProfileName = $"{client.CharacterName}_Profile" };
                    profile.Rules.Add(new PriorityRule { 
                        AbilityToCast = new Ability { Name = "Basic Attack", KeyCode = 0x31, Cooldown = 1.0f }, 
                        Condition = (s) => true 
                    });

                    orchestrator.RegisterBot(client, profile, role);
                    Console.WriteLine($"Registered {client.CharacterName} as {role}");
                }

                // Demonstrate MODE SWITCHING
                Console.WriteLine("\n--- Testing Mode: POWER MODE ---");
                orchestrator.UpdateFleet();

                Console.WriteLine("\n--- Testing Mode: STEALTH MODE ---");
                // In a real app, we would switch the Senses implementation in the Orchestrator
                foreach (var client in clients)
                {
                    StealthModeSenses stealthSenses = new StealthModeSenses(client);
                    UnitData data = stealthSenses.GetLocalPlayerData();
                    Console.WriteLine($"[{client.CharacterName}] Stealth Read -> Health: {data.HealthPercentage:F1}%");
                }

                Console.WriteLine("\nProject Foundation, Senses (Both Modes), Muscles, Brain, and Orchestration setup complete.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }
    }
}
