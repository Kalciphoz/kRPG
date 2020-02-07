using Terraria;
using Terraria.ModLoader;
using System.IO;

namespace kRPG.Modifiers
{
    public class ElusiveModifier : NPCModifier
    {
        private float dodgeModifier = 1.2f;

        public ElusiveModifier(kNPC kNPC, NPC npc, float dodgeModifier = 1.2f) : base(kNPC, npc)
        {
            this.npc = npc;
            npc.GivenName = "Elusive " + npc.FullName;
            this.dodgeModifier = dodgeModifier;
        }

        public override void Apply()
        {
            npc.GetGlobalNPC<kNPC>().speedModifier *= 1.25f;
        }

        public override float StrikeNPC(NPC npc, double damage, int defense, float knockback, int hitDirection, bool crit)
        {
            return dodgeModifier;
        }

        public override void Write(ModPacket packet)
        {
            packet.Write(dodgeModifier);
        }

        public override void Read(BinaryReader reader)
        {
            dodgeModifier = reader.ReadSingle();
        }

        public new static NPCModifier Random(kNPC kNPC, NPC npc)
        {
            return new ElusiveModifier(kNPC, npc, 1f+ Main.rand.NextFloat(.3f));
        }

        public new static NPCModifier New(kNPC kNPC, NPC npc)
        {
            return new ElusiveModifier(kNPC, npc);
        }
    }
}