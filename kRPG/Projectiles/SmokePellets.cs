using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace kRPG.Projectiles
{
    public class SmokePellets : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 96;
            projectile.height = 64;
            projectile.timeLeft = 57;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.ownerHitCheck = true;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 19;
            DisplayName.SetDefault("Smoke Pellets");
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            projectile.frame = 19 - (int)Math.Ceiling(projectile.timeLeft / 3.0);
            spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.position - Main.screenPosition, new Rectangle(0, projectile.frame * 64, 96, 64), Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Confused, 210);
        }
    }
}
