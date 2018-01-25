using System;
using Terraria;

namespace kRPG.Modifiers
{
    public class LifeRegenModifier : NPCModifier
    {
        private float regenTimer;
        
        public LifeRegenModifier(kNPC kNPC, NPC npc) : base(kNPC, npc)
        {
            npc.GivenName = "Regenerative " + npc.GivenName;
        }

        public override void Update(NPC npc)
        {
            regenTimer += 1;
            int amount = npc.lifeMax / 20;
            if (regenTimer > 60f / amount)
            {
                npc.life = Math.Min(npc.life + (int)(regenTimer / (60f / amount)), npc.lifeMax);
                regenTimer = regenTimer % (60 / amount);
            }
        }
        
        
        public new static NPCModifier Random(kNPC kNPC, NPC npc)
        {
            return new LifeRegenModifier(kNPC, npc);
        }
    }
}