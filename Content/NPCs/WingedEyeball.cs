using System;
using kRPG.Content.Items.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Content.NPCs
{
    public class WingedEyeball : ProceduralMinion
    {
        public override void AI()
        {
            base.AI();
            Player player = Main.player[projectile.owner];

            float acceleration = 0.4f;
            projectile.tileCollide = false;
            Vector2 v = player.Center - projectile.Center;
            v.X += Main.rand.Next(-10, 21);
            v.X += Main.rand.Next(-10, 21);
            v.X += 60f * -player.direction;
            v.Y -= 60f;
            float someDist = (float) Math.Sqrt(v.X * v.X + v.Y * v.Y);
            float num22 = 14f;

            if (someDist < 100 && Math.Abs(player.velocity.Y) < .01 && projectile.Bottom.Y <= player.Bottom.Y &&
                !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                projectile.ai[0] = 0f;
                if (projectile.velocity.Y < -6f)
                    projectile.velocity.Y = -6f;
            }

            if (someDist < 50f)
            {
                if (Math.Abs(projectile.velocity.X) > 2f || Math.Abs(projectile.velocity.Y) > 2f)
                    projectile.velocity *= 0.99f;
                acceleration = 0.01f;
            }
            else
            {
                if (someDist < 100f)
                    acceleration = 0.1f;
                if (someDist > 300f)
                    acceleration = 0.6f;
                someDist = num22 / someDist;
                v.X *= someDist;
                v.Y *= someDist;
            }

            if (projectile.velocity.X < v.X)
            {
                projectile.velocity.X = projectile.velocity.X + acceleration;
                if (acceleration > 0.05f && projectile.velocity.X < 0f)
                    projectile.velocity.X = projectile.velocity.X + acceleration;
            }

            if (projectile.velocity.X > v.X)
            {
                projectile.velocity.X = projectile.velocity.X - acceleration;
                if (acceleration > 0.05f && projectile.velocity.X > 0f)
                    projectile.velocity.X = projectile.velocity.X - acceleration;
            }

            if (projectile.velocity.Y < v.Y)
            {
                projectile.velocity.Y = projectile.velocity.Y + acceleration;
                if (acceleration > 0.05f && projectile.velocity.Y < 0f)
                    projectile.velocity.Y = projectile.velocity.Y + acceleration * 2f;
            }

            if (projectile.velocity.Y > v.Y)
            {
                projectile.velocity.Y = projectile.velocity.Y - acceleration;
                if (acceleration > 0.05f && projectile.velocity.Y > 0f)
                    projectile.velocity.Y = projectile.velocity.Y - acceleration * 2f;
            }

            if (projectile.velocity.X > 0.25)
                projectile.direction = -1;
            else if (projectile.velocity.X < -0.25)
                projectile.direction = 1;
            projectile.spriteDirection = projectile.direction;
            projectile.rotation = projectile.velocity.X * 0.05f;
            int num9 = projectile.frameCounter;
            projectile.frameCounter = num9 + 1;
            if (projectile.frameCounter > 2)
            {
                num9 = projectile.frame;
                projectile.frame = num9 + 1;
                projectile.frameCounter = 0;
            }

            if (projectile.frame <= 3)
                return;
            projectile.frame = 0;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale)
        {
            Texture2D t = Main.projectileTexture[ModContent.ProjectileType<WingedEyeball>()];
            spriteBatch.Draw(t, position + t.Bounds.Center(), new Rectangle(0, projectile.frame * 40, 90, 40), color, rotation, t.Bounds.Center(), scale,
                projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        }
        //public Vector2 acceleration = new Vector2(1f, 0.5f);
        //public Vector2 maxSpeed = new Vector2(6f, 2f);

        public override void SetDefaults()
        {
            projectile.width = 90;
            projectile.height = 40;
            projectile.scale = 1f;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 7200;
            projectile.tileCollide = false;
            projectile.knockBack = 0f;
        }
    }
}