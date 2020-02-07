using System;
using Microsoft.Xna.Framework;
using Terraria;

namespace kRPG.Modifiers
{
    public class LifeRegenModifier : NPCModifier
    {
        private float regenTimer;
        
        public LifeRegenModifier(kNPC kNPC, NPC npc) : base(kNPC, npc)
        {
            this.npc = npc;
            npc.GivenName = "Shimmering " + npc.FullName;
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

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            int i = Math.Abs(((int)(Main.time * 13) % 255) - 127);
            drawColor = new Color(127 + i, 127 + i, 127 + i);
        }

        public new static NPCModifier Random(kNPC kNPC, NPC npc)
        {
            return new LifeRegenModifier(kNPC, npc);
        }

        public new static NPCModifier New(kNPC kNPC, NPC npc)
        {
            return new LifeRegenModifier(kNPC, npc);
        }
    }
}