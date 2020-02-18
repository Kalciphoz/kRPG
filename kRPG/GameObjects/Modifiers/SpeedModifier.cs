using System.IO;
using kRPG.GameObjects.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.GameObjects.Modifiers
{
    public class SpeedModifier : NpcModifier
    {
        public SpeedModifier() { }
        public SpeedModifier(kNPC kNpc, NPC npc) : base(kNpc, npc)
        {
            this.npc = npc;
            Knpc = kNpc;
            
            
        }

        private kNPC Knpc { get; }
        private float SpeedModifierAdj { get; set; }

        public override void Initialize()
        {
            SpeedModifierAdj = 1f + Main.rand.NextFloat(.8f);
            
            kRPG.LogMessage("Initializing Speed Modifer: " + SpeedModifierAdj);
        }

        public override void Apply()
        {
            Knpc.SpeedModifier = SpeedModifierAdj;
            npc.GivenName = "Swift " + npc.FullName;
        }

        public new static NpcModifier New(kNPC kNpc, NPC npc)
        {
            return new SpeedModifier(kNpc, npc);
        }

        //public new static NpcModifier Random(kNPC kNpc, NPC npc)
        //{
        //    return new SpeedModifier(kNpc, npc, 1f + Main.rand.NextFloat(2));
        //}

        public override int Unpack(BinaryReader reader)
        {
            SpeedModifierAdj = reader.ReadSingle();
#if DEBUG
            kRPG.LogMessage("Reading SpeedModifierAdj: " + SpeedModifierAdj.ToString("F"));
#endif
            return 4;
        }

        public override int Pack(ModPacket packet)
        {
            packet.Write(SpeedModifierAdj);
            return 4;
        }
    }
}