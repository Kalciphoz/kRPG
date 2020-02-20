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

        protected NpcModifier() { }

        protected NpcModifier(kNPC kNpc, NPC oNpc)
        {
        }

        /// <summary>
        /// This should be called from either the server or singleplayer
        /// </summary>
        public abstract void Initialize();
        
        /// <summary>
        /// This is called on both the server and client
        /// </summary>
        public virtual void Apply()
        {
        }

        /// <summary>
        /// Draws the effect of the modifieer
        /// </summary>
        /// <param name="oNpc"></param>
        /// <param name="drawColor"></param>
        public virtual void DrawEffects(NPC oNpc, ref Color drawColor)
        {
        }

        /// <summary>
        /// How to handle a player hitting a mob with this modifier
        /// </summary>
        /// <param name="oNpc"></param>
        /// <param name="target"></param>
        /// <param name="damage"></param>
        /// <param name="crit"></param>
        public virtual void ModifyHitPlayer(NPC oNpc, Player target, ref int damage, ref bool crit)
        {
        }

        /// <summary>
        /// Create new Modifier
        /// </summary>
        /// <param name="oNpc"></param>
        /// <param name="npc"></param>
        /// <returns></returns>
        public static NpcModifier New(kNPC oNpc, NPC npc)
        {
            return null;
        }

        /// <summary>
        /// Code to execute when a mob is killed
        /// </summary>
        /// <param name="oNpc"></param>
        public virtual void NpcLoot(NPC oNpc)
        {
        }

        /// <summary>
        /// Code to execute when hit by projectile
        /// </summary>
        /// <param name="oNpc"></param>
        /// <param name="projectile"></param>
        /// <param name="damage"></param>
        /// <param name="knockBack"></param>
        /// <param name="crit"></param>
        public virtual void OnHitByProjectile(NPC oNpc, Projectile projectile, int damage, float knockBack, bool crit)
        {
        }

        /// <summary>
        /// Code to execute in the NPC's postAI routine
        /// </summary>
        /// <param name="oNpc"></param>
        public virtual void PostAi(NPC oNpc)
        {
        }

        
        /// <summary>
        /// Unpacks the network stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public virtual int Unpack(BinaryReader reader)
        {
            return 0;
        }

        /// <summary>
        /// Code to execute when the npc is hit
        /// </summary>
        /// <param name="oNpc"></param>
        /// <param name="damage"></param>
        /// <param name="defense"></param>
        /// <param name="knockBack"></param>
        /// <param name="hitDirection"></param>
        /// <param name="crit"></param>
        /// <returns></returns>
        public virtual float StrikeNpc(NPC oNpc, double damage, int defense, float knockBack, int hitDirection, bool crit)
        {
            return 1f;
        }


        public virtual void Update(NPC kNpc)
        {
        }

        /// <summary>
        /// How to unpack the network stream
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public virtual int Pack(ModPacket packet)
        {
            return 0;
        }
    }
}