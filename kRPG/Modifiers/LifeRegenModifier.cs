using System;
using Terraria;

namespace kRPG.Modifiers
{
    public class LifeRegenModifier : NpcModifier
    {
        private float RegenTimer { get; set; }

        public LifeRegenModifier(kNPC kNpc, NPC npc) : base(kNpc, npc)
        {
            this.npc = npc;
            npc.GivenName = "Shimmering " + npc.GivenName;
        }

        public new static NpcModifier New(kNPC kNpc, NPC npc)
        {
            return new LifeRegenModifier(kNpc, npc);
        }

        public new static NpcModifier Random(kNPC kNpc, NPC npc)
        {
            return new LifeRegenModifier(kNpc, npc);
        }

        public override void Update(NPC kNpc)
        {
            RegenTimer += 1;
            int amount = kNpc.lifeMax / 20;
            if (!(RegenTimer > 60f / amount))
                return;
            kNpc.life = Math.Min(kNpc.life + (int) (RegenTimer / (60f / amount)), kNpc.lifeMax);
            RegenTimer %= 60f / amount;
        }
    }
}