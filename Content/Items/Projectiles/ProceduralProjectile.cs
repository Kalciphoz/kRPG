using kRPG.Content.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Content.Items.Projectiles
{
    public class ProceduralProjectile : ModProjectile
    {
        public Texture2D LocalTexture { get; set; }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale)
        {
        }

        public virtual void Initialize()
        {
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.owner != Main.myPlayer && !(this is ProceduralSpellProj) && !(this is WingedEyeball))
                return true;
            Draw(spriteBatch, projectile.position - Main.screenPosition, lightColor, projectile.rotation, projectile.scale);
            return false;
        }

        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 48;
            projectile.scale = 1f;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Procedurally Generated Projectile; Please Ignore");
        }
    }

    //public class ProceduralSwordThrow : ProceduralProjectile
    //{
    //    public SwordHilt hilt;
    //    public SwordBlade blade;
    //    public SwordAccent accent;
    //    public Item sword;

    //    public override void Initialize()
    //    {
    //        this.LocalTexture = GFX.CombineTextures(new List<Texture2D>(){
    //            { blade.LocalTexture },
    //            { hilt.LocalTexture },
    //            { accent.LocalTexture }
    //        }, new List<Point>(){
    //            { new Point(CombinedTextureSize().X - blade.LocalTexture.Width, 0) },
    //            { new Point(0, CombinedTextureSize().Y - hilt.LocalTexture.Height) },
    //            { new Point((int)hilt.origin.X - (int)accent.origin.X, CombinedTextureSize().Y - hilt.LocalTexture.Height + (int)hilt.origin.Y - (int)accent.origin.Y) }
    //        }, CombinedTextureSize());
    //        projectile.width = LocalTexture.Width;
    //        projectile.height = LocalTexture.Height;
    //    }

    //    public override void ModifyDamageHitbox(ref Rectangle hitbox)
    //    {
    //        hitbox = new Rectangle((int)projectile.playerPosition.X - LocalTexture.Width / 2, (int)projectile.playerPosition.Y - LocalTexture.Height / 2, LocalTexture.Width, LocalTexture.Height);
    //    }

    //    public override bool? CanHitNPC(NPC target)
    //    {
    //        Player owner = Main.player[projectile.owner];
    //        if ((target.playerPosition.X - owner.playerPosition.X) * owner.direction > -1f)
    //            return base.CanHitNPC(target);
    //        else return false;
    //    }

    //    public Point CombinedTextureSize()
    //    {
    //        return new Point(blade.LocalTexture.Width - (int)blade.origin.X + (int)hilt.origin.X, (int)blade.origin.Y + hilt.LocalTexture.Height - (int)hilt.origin.Y);
    //    }

    //    public override void Draw(SpriteBatch spriteBatch, Vector2 playerPosition, Color color, float rotation, float scale)
    //    {
    //        if (LocalTexture == null)
    //        {
    //            Initialize();
    //            return;
    //        }
    //        spriteBatch.Draw(LocalTexture, playerPosition + LocalTexture.Size() / 2f, null, blade.lighted ? Color.White : color, rotation, LocalTexture.Bounds.Center(), scale, projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
    //    }

    //    public override void SetDefaults()
    //    {
    //        projectile.width = 40;
    //        projectile.height = 40;
    //        projectile.scale = 1f;
    //        projectile.friendly = true;
    //        projectile.hostile = false;
    //        projectile.penetrate = -1;
    //        projectile.timeLeft = 3600;
    //        projectile.tileCollide = false;
    //        projectile.aiStyle = 3;
    //    }

    //    public override void SetStaticDefaults()
    //    {
    //        DisplayName.SetDefault("Procedurally Generated Sword Projectile; Please Ignore");
    //    }

    //    public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
    //    {
    //        Player owner = Main.player[projectile.owner];
    //        //if (accent.onHit != null) accent.onHit(owner, target, (ProceduralSword)owner.inventory[owner.selectedItem].modItem, damage, crit);
    //    }
    //}
}