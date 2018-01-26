using Terraria;

namespace kRPG.Modifiers
{
    public class ElusiveModifier : NPCModifier
    {
        private float dodgeModifier = 1.2f;

        public ElusiveModifier(kNPC kNPC, NPC npc, float dodgeModifier = 1.2f) : base(kNPC, npc)
        {
            npc.GivenName = "Elusive " + npc.GivenName;
            this.dodgeModifier = dodgeModifier;
        }

        public override float StrikeNPC(NPC npc, double damage, int defense, float knockback, int hitDirection, bool crit)
        {
            return dodgeModifier;
        }
        
        public new static NPCModifier Random(kNPC kNPC, NPC npc)
        {
            return new ElusiveModifier(kNPC, npc, 1f+ Main.rand.NextFloat(.3f));
        }
    }
}