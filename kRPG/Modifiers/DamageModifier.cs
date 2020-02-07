using System;
using Terraria;
using Terraria.ModLoader;
using System.IO;
using Microsoft.Xna.Framework;

namespace kRPG.Modifiers
{
    public class DamageModifier : NPCModifier
    {
        private float damageModifier = 1.2f;

        public DamageModifier(kNPC kNPC, NPC npc, float damageModifier = 1.2f) : base(kNPC, npc)
        {
            this.npc = npc;
            npc.GivenName = "Brutal " + npc.FullName;
            this.damageModifier = damageModifier;
            if (Main.netMode != 1) Apply();
        }

        public override void Apply()
        {
            npc.damage = (int)Math.Round(npc.damage * this.damageModifier);
            npc.defense = 1;
        }

        public override void Write(ModPacket packet)
        {
            packet.Write(damageModifier);
        }

        public override void Read(BinaryReader reader)
        {
            damageModifier = reader.ReadSingle();
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            int i = Math.Abs(((int)(Main.time * 4) % 511) - 255);
            drawColor = new Color(255, i, i);
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