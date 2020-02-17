using System.IO;
using kRPG.GameObjects.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.GameObjects.Modifiers
{
    public class ElusiveModifier : NpcModifier
    {
        private kNPC kNpc { get; set; }

        public ElusiveModifier() { }

        public ElusiveModifier(kNPC kNpc, NPC npc) : base(kNpc, npc)
        {
            this.npc = npc;
            this.kNpc = kNpc;
        }

        private float DodgeModifier { get; set; }

        public override void Initialize()
        {
            kNpc.SpeedModifier *= 1.25f;
            DodgeModifier = 1f + Main.rand.NextFloat(.3f);
            kRPG.LogMessage("Initializing Dodge Modifier: " + DodgeModifier);
        }

        public override void Apply()
        {
            npc.GivenName = "Elusive " + npc.FullName;
        }

        public new static NpcModifier New(kNPC kNpc, NPC npc)
        {
            return new ElusiveModifier(kNpc, npc);
        }

        public override int Unpack(BinaryReader reader)
        {
            DodgeModifier = reader.ReadSingle();
#if DEBUG
            kRPG.LogMessage("Reading DodgeModifier: " + DodgeModifier.ToString("F"));
#endif
            return 4;
        }

        public override float StrikeNpc(NPC oNpc, double damage, int defense, float knockBack, int hitDirection, bool crit)
        {
            return DodgeModifier;
        }

        public override int Pack(ModPacket packet)
        {
            packet.Write(DodgeModifier);
            return 4;
        }
    }
}