using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Projectiles
{
    public class Obelisk : ProceduralMinion
    {
        public override void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale)
        {
            Texture2D t = Main.projectileTexture[ModContent.ProjectileType<Obelisk>()];
            spriteBatch.Draw(t, position + t.Bounds.Center(), null, Color.White, rotation, t.Bounds.Center(), scale, SpriteEffects.None, 0f);
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.scale = 1f;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 1800;
            projectile.tileCollide = false;
            projectile.knockBack = 0f;
        }
    }
}