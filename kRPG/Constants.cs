using System;
using System.Collections.Generic;
using kRPG.Enums;

namespace kRPG
{
    public static class Constants
    {
        public const float MaxScreenWidth = 3840f;
        public const string ModName = "kRPG";
        public static Dictionary<string, Ritual> ritualByName = new Dictionary<string, Ritual>
        {
            {"demon_pact", Ritual.DemonPact},
            {"warrior_oath", Ritual.WarriorOath},
            {"elan_vital", Ritual.ElanVital},
            {"stone_aspect", Ritual.StoneAspect},
            {"eldritch_fury", Ritual.EldritchFury},
            {"mind_fortress", Ritual.MindFortress},
            {"blood_drinking", Ritual.BloodDrinking}
        };
        public static class NetModes
        {
            public const int Server = 2;
            public const int Client = 1;
            public const int SinglePlayer = 0;
        }
        public static double Tau => Math.PI * 2;
    }
}