using System.IO;
using kRPG.GameObjects.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.GameObjects.Modifiers
{
    public abstract class NpcModifier
    {
        public NPC npc;

        public NpcModifier(kNPC kNpc, NPC npc)
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

        public static NpcModifier New(kNPC kNpc, NPC npc)
        {
            return null;
        }

        public virtual void NPCLoot(NPC npc)
        {
        }

        public virtual void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
        }

        public virtual void PostAI(NPC npc)
        {
        }

        public static NpcModifier Random(kNPC kNpc, NPC npc)
        {
            return null;
        }

        public virtual void Read(BinaryReader reader)
        {
        }

        public virtual float StrikeNPC(NPC npc, double damage, int defense, float knockback, int hitDirection, bool crit)
        {
            return 1f;
        }

        public virtual void Update(NPC kNpc)
        {
        }

        public virtual void Write(ModPacket packet)
        {
        }
    }
}