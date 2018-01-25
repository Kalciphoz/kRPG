using kRPG.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;

namespace kRPG.Modifiers
{
    public class ExplosiveModifier : NPCModifier
    {
        public float lifeModifier = 0.5f;
        private kNPC kNPC;
        
        public ExplosiveModifier(kNPC kNPC, NPC npc) : base(kNPC, npc)
        {
            npc.GivenName = "Explosive " + npc.GivenName;
            this.kNPC = kNPC;
            npc.lifeMax = (int)(npc.lifeMax * lifeModifier);
            npc.life = (int)(npc.life * lifeModifier);
        }

        public override void NPCLoot(NPC npc)
        {
            Main.PlaySound(new LegacySoundStyle(2, 14, Terraria.Audio.SoundType.Sound).WithVolume(0.5f), npc.Center);
            Projectile proj = Main.projectile[Projectile.NewProjectile(npc.Center - new Vector2(16, 32), Vector2.Zero, kNPC.mod.ProjectileType<NPC_Explosion>(), npc.damage * 3 / 2, 0f)];
        }
        
        
        public new static NPCModifier Random(kNPC kNPC, NPC npc)
        {
            return new ExplosiveModifier(kNPC, npc);
        }
    }
}