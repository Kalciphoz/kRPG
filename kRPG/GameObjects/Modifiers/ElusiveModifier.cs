using System.IO;
using kRPG.GameObjects.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.GameObjects.Modifiers
{
    public class ElusiveModifier : NpcModifier
    {
        public ElusiveModifier() { }
        public ElusiveModifier(kNPC kNpc, NPC npc, float dodgeModifier = 1.2f) : base(kNpc, npc)
        {
            this.npc = npc;
            npc.GivenName = "Elusive " + npc.GivenName;
            DodgeModifier = dodgeModifier;
        }

        private float DodgeModifier { get; set; }

        public override void Apply()
        {
            npc.GetGlobalNPC<kNPC>().SpeedModifier *= 1.25f;
        }

        public new static NpcModifier New(kNPC kNpc, NPC npc)
        {
            return new ElusiveModifier(kNpc, npc);
        }

        public static NpcModifier Random(kNPC kNpc, NPC npc)
        {
            return new ElusiveModifier(kNpc, npc, 1f + Main.rand.NextFloat(.3f));
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