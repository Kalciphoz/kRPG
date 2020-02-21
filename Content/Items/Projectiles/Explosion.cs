﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Content.Items.Projectiles
{
    public class Explosion : ModProjectile
    {
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Lighting.AddLight(projectile.position, 0.7f, 0.4f, 0.1f);
            projectile.frame = 9 - (int) Math.Ceiling(projectile.timeLeft / 3.0);
            spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.position - Main.screenPosition,
                new Rectangle(0, projectile.frame * 128, 128, 128), Color.White);
            return false;
        }

        public override void SetDefaults()
        {
            projectile.width = 128;
            projectile.height = 128;
            projectile.timeLeft = 27;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.ownerHitCheck = true;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 9;
            DisplayName.SetDefault("Explosion");
        }
    }
}