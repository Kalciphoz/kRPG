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

        protected NpcModifier(kNPC kNpc, NPC oNpc)
        {
        }

        public virtual void Apply()
        {
        }

        public virtual void DrawEffects(NPC oNpc, ref Color drawColor)
        {
        }

        public virtual void ModifyHitPlayer(NPC oNpc, Player target, ref int damage, ref bool crit)
        {
        }

        public static NpcModifier New(kNPC oNpc, NPC npc)
        {
            return null;
        }

        public virtual void NpcLoot(NPC oNpc)
        {
        }

        public virtual void OnHitByProjectile(NPC oNpc, Projectile projectile, int damage, float knockBack, bool crit)
        {
        }

        public virtual void PostAi(NPC oNpc)
        {
        }

        //public static NpcModifier Random(kNPC kNpc, NPC oNpc)
        //{
        //    return null;
        //}

        public virtual void Read(BinaryReader reader)
        {
        }

        public virtual float StrikeNpc(NPC oNpc, double damage, int defense, float knockBack, int hitDirection, bool crit)
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