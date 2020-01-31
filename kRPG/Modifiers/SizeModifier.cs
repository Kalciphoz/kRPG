using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Modifiers
{
    public class SizeModifier : NPCModifier
    {
        private float lifeModifier;
        private float scaleModifier;

        public SizeModifier(kNPC kNPC, NPC npc, float scaleModifier = 1.1f, float lifeModifier = 1.4f) : base(kNPC, npc)
        {
            this.npc = npc;
            this.scaleModifier = scaleModifier;
            this.lifeModifier = lifeModifier;
            Apply();
        }

        public override void Apply()
        {
            npc.scale *= scaleModifier;
            npc.lifeMax = (int) (npc.lifeMax * lifeModifier);
            npc.life = (int) (npc.life * lifeModifier);
            if (scaleModifier < 1)
                npc.GivenName = "Small " + npc.GivenName;
            else
                npc.GivenName = "Massive " + npc.GivenName;
            npc.GetGlobalNPC<kNPC>().speedModifier *= (float) Math.Pow(scaleModifier, 0.9);
        }

        public new static NPCModifier New(kNPC kNPC, NPC npc)
        {
            return new SizeModifier(kNPC, npc);
        }

        public new static NPCModifier Random(kNPC kNPC, NPC npc)
        {
            return new SizeModifier(kNPC, npc, .5f + Main.rand.NextFloat(2), .5f + Main.rand.NextFloat(1));
        }

        public override void Read(BinaryReader reader)
        {
            scaleModifier = reader.ReadSingle();
            lifeModifier = reader.ReadSingle();
        }

        public override void Write(ModPacket packet)
        {
            packet.Write(scaleModifier);
            packet.Write(lifeModifier);
        }
    }
}