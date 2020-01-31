using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace kRPG2.Modifiers
{
    public class SpeedModifier : NpcModifier
    {
        public SpeedModifier(kNPC kNpc, NPC npc, float speedModifierAdj = 1.8f) : base(kNpc, npc)
        {
            this.npc = npc;
            Kn = kNpc;
            npc.GivenName = "Swift " + npc.GivenName;
            SpeedModifierAdj = speedModifierAdj;
        }

        private kNPC Kn { get; }
        private float SpeedModifierAdj { get; set; }

        public override void Apply()
        {
            Kn.SpeedModifier = SpeedModifierAdj;
        }

        public new static NpcModifier New(kNPC kNpc, NPC npc)
        {
            return new SpeedModifier(kNpc, npc);
        }

        public new static NpcModifier Random(kNPC kNpc, NPC npc)
        {
            return new SpeedModifier(kNpc, npc, 1f + Main.rand.NextFloat(2));
        }

        public override void Read(BinaryReader reader)
        {
            SpeedModifierAdj = reader.ReadSingle();
        }

        public override void Write(ModPacket packet)
        {
            packet.Write(SpeedModifierAdj);
        }
    }
}