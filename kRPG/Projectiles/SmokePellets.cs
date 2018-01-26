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
            projectile.width = 192;
            projectile.height = 128;
            projectile.timeLeft = 57;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.ownerHitCheck = true;
            projectile.scale = 1f;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 19;
            DisplayName.SetDefault("Smoke Pellets");
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            projectile.frame = 19 - (int)Math.Ceiling(projectile.timeLeft / 3.0);
            Texture2D text = Main.projectileTexture[projectile.type];
            int height = text.Height / Main.projFrames[projectile.type];
            spriteBatch.Draw(text, projectile.position - Main.screenPosition, new Rectangle(0, projectile.frame * height, text.Width, height), Color.White);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Confused, 210);
        }
    }
}
