/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psycho-core]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System.Collections.Generic;

namespace PsychoBuddy.Core
{
    public enum GameVersion
    {
        Vanilla_1_12,
        TBC_2_4_3,
        WotLK_3_3_5a,
        Cataclysm_4_3_4,
        MoP_5_4_8,
        Legion_7_3_5,
        BFA_8_3_7
    }

    public static class GameVersionCatalog
    {
        public static IReadOnlyList<string> All { get; } = new[]
        {
            "Vanilla_1_12",
            "TBC_2_4_3",
            "WotLK_3_3_5a",
            "Cataclysm_4_3_4",
            "MoP_5_4_8",
            "Legion_7_3_5",
            "BFA_8_3_7"
        };

        public static string Normalize(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "BFA_8_3_7";

            foreach (string version in All)
            {
                if (version.Equals(value, System.StringComparison.OrdinalIgnoreCase))
                {
                    return version;
                }
            }

            return "BFA_8_3_7";
        }
    }
}
