using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Modifiers
{
    public abstract class NPCModifier
    {
        public NPC npc;

        public NPCModifier(kNPC kNPC, NPC npc)
        {
        }

        public virtual void Apply()
        {
        }

        public virtual void DrawEffects(NPC npc, ref Color drawColor)
        {
        }

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

        public virtual void Write(ModPacket packet)
        {
        }

        public virtual void Read(BinaryReader reader)
        {
        }

        public static NPCModifier Random(kNPC kNPC, NPC npc)
        {
            return null;
        }

        public static NPCModifier New(kNPC kNPC, NPC npc)
        {
            return null;
        }
    }
}