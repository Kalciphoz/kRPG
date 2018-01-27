using Terraria;

namespace kRPG.Modifiers
{
    public class SizeModifier : NPCModifier
    {
        
        public SizeModifier(kNPC kNPC, NPC npc, float scaleModifier = 1.2f, float lifeModifier = 1.4f) : base(kNPC, npc)
        { 
            npc.scale *= scaleModifier;
            npc.lifeMax = (int)(npc.lifeMax * lifeModifier);
            npc.life = (int)(npc.life * lifeModifier);
            if (scaleModifier < 1)
                npc.GivenName = "Small " + npc.GivenName;
            else
                npc.GivenName = "Massive " + npc.GivenName;
        }
        
        public new static NPCModifier Random(kNPC kNPC, NPC npc)
        {
            return new SizeModifier(kNPC, npc, .5f + Main.rand.NextFloat(2), .5f+ Main.rand.NextFloat(1));
        }

        public new static NPCModifier New(kNPC kNPC, NPC npc)
        {
            return new SizeModifier(kNPC, npc);
        }
    }
}