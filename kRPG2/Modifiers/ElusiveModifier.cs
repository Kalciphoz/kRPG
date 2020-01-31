using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace kRPG2.Modifiers
{
    public class ElusiveModifier : NpcModifier
    {
        private float DodgeModifier { get; set; } = 1.2f;

        public ElusiveModifier(kNPC kNpc, NPC npc, float dodgeModifier = 1.2f) : base(kNpc, npc)
        {
            this.npc = npc;
            npc.GivenName = "Elusive " + npc.GivenName;
            this.DodgeModifier = dodgeModifier;
        }

        public override void Apply()
        {
            npc.GetGlobalNPC<kNPC>().SpeedModifier *= 1.25f;
        }

        public new static NpcModifier New(kNPC kNpc, NPC npc)
        {
            return new ElusiveModifier(kNpc, npc);
        }

        public new static NpcModifier Random(kNPC kNpc, NPC npc)
        {
            return new ElusiveModifier(kNpc, npc, 1f + Main.rand.NextFloat(.3f));
        }

        public override void Read(BinaryReader reader)
        {
            DodgeModifier = reader.ReadSingle();
        }

        public override float StrikeNPC(NPC npc, double damage, int defense, float knockback, int hitDirection, bool crit)
        {
            return DodgeModifier;
        }

        public override void Write(ModPacket packet)
        {
            packet.Write(DodgeModifier);
        }
    }
}