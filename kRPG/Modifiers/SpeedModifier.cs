using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Modifiers
{
    public class SpeedModifier : NPCModifier
    {
        private readonly kNPC kn;
        private float speedModifier = 1f;

        public SpeedModifier(kNPC kNPC, NPC npc, float speedModifier = 1.8f) : base(kNPC, npc)
        {
            this.npc = npc;
            kn = kNPC;
            npc.GivenName = "Swift " + npc.GivenName;
            this.speedModifier = speedModifier;
        }

        public override void Apply()
        {
            kn.speedModifier = speedModifier;
        }

        public new static NPCModifier New(kNPC kNPC, NPC npc)
        {
            return new SpeedModifier(kNPC, npc);
        }

        public new static NPCModifier Random(kNPC kNPC, NPC npc)
        {
            return new SpeedModifier(kNPC, npc, 1f + Main.rand.NextFloat(2));
        }

        public override void Read(BinaryReader reader)
        {
            speedModifier = reader.ReadSingle();
        }

        public override void Write(ModPacket packet)
        {
            packet.Write(speedModifier);
        }
    }
}