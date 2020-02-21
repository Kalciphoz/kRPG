using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kRPG.GameObjects.Modifiers;
using kRPG.GameObjects.NPCs;
using Terraria;
using System.Threading;
using Terraria.ModLoader;

namespace kRPG.Enums
{
    public  class ModiferFunctions
    {
        private ModiferFunctions()
        {
            Modifiers = new List<Entry>()
            {
                DamageModifer,
                ElusiveModifier,
                ExplosiveModifier,
                LifeRegenModifier,
                SageModifier,
                SizeModifier,
                SpeedModifier
            };
        }

        private static readonly Lazy<ModiferFunctions> LazyInstance = new Lazy<ModiferFunctions>(CreateInstanceOfT, LazyThreadSafetyMode.ExecutionAndPublication);
        private static ModiferFunctions CreateInstanceOfT() { return Activator.CreateInstance(typeof(ModiferFunctions), true) as ModiferFunctions; }


        public static ModiferFunctions Instance => LazyInstance.Value;

        public class Entry
        {
            public int Id { get; set; }
            public string ClassName { get; set; }
            public Func<kNPC, NPC, NpcModifier> Function { get; set; }
        }

        public  Entry DamageModifer { get; } = new Entry()
        {
            Id = 0,
            ClassName = typeof(DamageModifier).AssemblyQualifiedName,
            Function = DamageModifier.New
        };

        public  Entry ElusiveModifier { get; } = new Entry()
        {
            Id = 1,
            ClassName = typeof(ElusiveModifier).AssemblyQualifiedName,
            Function = GameObjects.Modifiers.ElusiveModifier.New
        };

        public  Entry ExplosiveModifier { get; } = new Entry()
        {
            Id = 2,
            ClassName = typeof(ExplosiveModifier).AssemblyQualifiedName,
            Function = GameObjects.Modifiers.ExplosiveModifier.New
        };

        public  Entry LifeRegenModifier { get; } = new Entry()
        {
            Id = 3,
            Function = GameObjects.Modifiers.LifeRegenModifier.New,
            ClassName = typeof(LifeRegenModifier).AssemblyQualifiedName
        };

        public  Entry SageModifier { get; } = new Entry()
        {
            Id = 4,
            Function = GameObjects.Modifiers.SageModifier.New,
            ClassName = typeof(SageModifier).AssemblyQualifiedName

        };

        public  Entry SizeModifier { get; } = new Entry()
        {
            Id = 5,
            Function = GameObjects.Modifiers.SizeModifier.New,
            ClassName = typeof(SizeModifier).AssemblyQualifiedName
        };

        public  Entry SpeedModifier { get; } = new Entry()
        {
            Id = 6,
            ClassName = typeof(SpeedModifier).AssemblyQualifiedName,
            Function = GameObjects.Modifiers.SpeedModifier.New
        };

        public List<Entry> Modifiers { get; }
        









    }
}
