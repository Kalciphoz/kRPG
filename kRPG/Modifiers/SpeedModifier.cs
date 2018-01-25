using System;
using Microsoft.Xna.Framework;
using Terraria;

namespace kRPG.Modifiers
{
    public class SpeedModifier : NPCModifier
    {
        public float speedModifier = 1f;
        
        public SpeedModifier(kNPC kNPC, NPC npc, float speedModifier = 2f) : base(kNPC, npc)
        {
            npc.GivenName = "Speedy " + npc.GivenName;
            this.speedModifier = speedModifier;
        }

        public override void Update(NPC npc)
        {
            base.Update(npc);
            if (npc.aiStyle == 3 && npc.velocity.Y == 0f)
                npc.velocity.X = MathHelper.Lerp(npc.velocity.X, npc.direction * Math.Max(Math.Abs(npc.velocity.X), 8f), 1f * speedModifier / 20f);
        }
        
        public new static NPCModifier Random(kNPC kNPC, NPC npc)
        {
            return new SpeedModifier(kNPC, npc, 1f+ Main.rand.NextFloat(2));
        }
    }
}