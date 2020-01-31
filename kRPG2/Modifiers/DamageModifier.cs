using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace kRPG2.Modifiers
{
    public class DamageModifier : NpcModifier
    {
        private float DmgModifier { get; set; }

        public DamageModifier(kNPC kNpc, NPC npc, float dmgModifier = 1.2f) : base(kNpc, npc)
        {
            this.npc = npc;
            npc.GivenName = "Brutal " + npc.GivenName;
            this.DmgModifier = dmgModifier;
            if (Main.netMode != 1) 
                Apply();
        }

        public override void Apply()
        {
            npc.damage = (int) Math.Round(npc.damage * DmgModifier);
            npc.defense = 1;
        }

        public new static NpcModifier New(kNPC kNpc, NPC npc)
        {
            return new DamageModifier(kNpc, npc);
        }

        public new static NpcModifier Random(kNPC kNpc, NPC npc)
        {
            return new DamageModifier(kNpc, npc, 1f + Main.rand.NextFloat(1));
        }

        public override void Read(BinaryReader reader)
        {
            DmgModifier = reader.ReadSingle();
        }

        public override void Write(ModPacket packet)
        {
            packet.Write(DmgModifier);
        }
    }
}