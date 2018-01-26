using System;
using Terraria;

namespace kRPG.Modifiers
{
    public class DamageModifier : NPCModifier
    {
        public DamageModifier(kNPC kNPC, NPC npc, float damageModifier = 1.2f) : base(kNPC, npc)
        {
            npc.GivenName = "Brutal " + npc.GivenName;
            npc.damage = (int)Math.Round(npc.damage * damageModifier);
            npc.defense = 1;
        }

        public new static NPCModifier Random(kNPC kNPC, NPC npc)
        {
            return new DamageModifier(kNPC, npc, 1f+ Main.rand.NextFloat(1));
        }

        public new static NPCModifier New(kNPC kNPC, NPC npc)
        {
            return new DamageModifier(kNPC, npc);
        }
    }
}