using Microsoft.Xna.Framework;
using Terraria;

namespace kRPG.Modifiers
{
    public abstract class NPCModifier
    {
        public NPCModifier(kNPC kNPC, NPC npc)
        {
            
        }
        
        public virtual void DrawEffects(NPC npc, ref Color drawColor){}

        public virtual void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
        {
        }

        public virtual void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
        }

        public virtual float StrikeNPC(NPC npc, double damage, int defense, float knockback, int hitDirection, bool crit)
        {
            return 1f;
        }

        public virtual void PostAI(NPC npc)
        {
        }


        public virtual void Update(NPC npc)
        {
        }

        public virtual void NPCLoot(NPC npc)
        {
        }

        public static NPCModifier Random(kNPC kNPC, NPC npc)
        {
            return null;
        }
    }
}