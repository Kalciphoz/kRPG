using System;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Projectiles
{
    public class ChainHook : ModProjectile
    {
        public override void AI()
        {
            projectile.rotation = (float) Math.Atan2(projectile.position.Y - Main.player[projectile.owner].position.Y,
                                      projectile.position.X - Main.player[projectile.owner].position.X) + (float) Math.PI / 2f;
            projectile.spriteDirection = Main.player[projectile.owner].direction;
            base.AI();
        }

        public override void SetDefaults()
        {
            projectile.Name = "Chain Hook";
            projectile.width = 22;
            projectile.height = 22;
            projectile.aiStyle = 15;
            projectile.timeLeft = 1800;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = true;
            projectile.penetrate = -1;
            projectile.melee = true;
            projectile.hide = false;
            projectile.ownerHitCheck = true;
        }
    }
}