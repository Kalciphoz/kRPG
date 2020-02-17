using System;
using System.Collections.Generic;
using System.IO;
using kRPG.GameObjects.Items.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.GameObjects.Items.Weapons.Melee
{
    public class ProceduralSpear : ProceduralProjectile
    {
        /*
         * Little note here, I am using projectile.ai[1] to hold 3 numbers.
         * Since each number will always be less than 255 I can use bit shifting to store 3 numbers in one float.
         * This gets ride of extra network calls to pass these values.,
         * Better performance, and it actually works.
         *
         */

        private int Combined => (int)projectile.ai[1];

        public SwordBlade Blade {
            get => SwordBlade.Blades[(Combined & 0xFF)];
            set => projectile.ai[1] = (value.Type | (Accent.Type << 8) | (Hilt.Type << 16));
        }

        public SwordAccent Accent {
            get => SwordAccent.Accents[((Combined >> 8) & 0xFF)];
            set => projectile.ai[1] = (Blade.Type | (value.Type << 8) | (Hilt.Type << 16));
        }


        public SwordHilt Hilt {
            get => SwordHilt.Hilts[((Combined >> 16) & 0xFF)];
            set => projectile.ai[1] = (Blade.Type | (Accent.Type << 8) | (value.Type << 16));
        }


        public float MovementFactor // Change this value to alter how fast the spear moves
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        // It appears that for this AI, only the ai0 field is used!

        public override void AI()
        {
            try
            {
                //Sanity Check to make sure we have values for the hilt, blade and accent
                if (Math.Abs(projectile.ai[1]) < .00000000001)
                    return;

                int localTextureWidth = 48;
                int localTextureHeight = 48;

                //We need to load the local texture if we aren't running on the server.
                if (LocalTexture == null && Main.netMode != NetmodeID.Server)
                {
                    Initialize();
                    localTextureHeight = LocalTexture.Height;
                    localTextureWidth = LocalTexture.Width;
                }

                //kRPG.LogMessage($"########################################> Blade = {Blade.Type} Accent = {Accent.Type} Hilt: {Hilt.Type}");


                // Since we access the owner player instance so much, it's useful to create a helper local variable for this
                // Sadly, Projectile/ModProjectile does not have its own
                Player projOwner = Main.player[projectile.owner];
                // Here we set some of the projectile's owner properties, such as held item and itemtime, along with projectile directio and playerPosition based on the player
                //Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);

                projOwner.direction = projectile.velocity.X > 0 ? 1 : -1;
                projectile.direction = projOwner.direction;
                projectile.spriteDirection = projectile.direction;
                projOwner.heldProj = projectile.whoAmI;
                projOwner.itemTime = projOwner.itemAnimation;
                projectile.position.X = projOwner.Center.X - localTextureWidth / 2f /* + 2f*projOwner.direction*/;
                projectile.position.Y = projOwner.Center.Y - localTextureHeight / 2f /* + 4f*/;
                
                // As long as the player isn't frozen, the spear can move
                if (!projOwner.frozen)
                {
                    if (Math.Abs(MovementFactor) < .01) // When initially thrown out, the ai0 will be 0f
                    {
                        MovementFactor = 3f;
                        projectile.netUpdate = true;
                    }

                    if (projOwner.itemAnimation < projOwner.itemAnimationMax / 3)
                        MovementFactor -= 2.4f;
                    else
                        MovementFactor += 2.1f;
                }

                projectile.position += projectile.velocity * MovementFactor;
                Vector2 unitVelocity = projectile.velocity;
                unitVelocity.Normalize();
                projectile.position += unitVelocity * (Blade.Origin.Y * 2.8f + 8f);

                if (projOwner.itemAnimation == 1)
                    projectile.Kill();
                // Apply proper rotation, with an offset of 135 degrees due to the sprite's rotation, notice the usage of MathHelper, use this class!
                // MathHelper.ToRadians(xx degrees here)
                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.ToRadians(45f);
                // Offset by 90 degrees here
                if (projectile.spriteDirection == -1)
                    projectile.rotation += MathHelper.ToRadians(90f);

                Rectangle rect = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, localTextureWidth, localTextureHeight);
                


                Blade.Effect?.Invoke(rect, projOwner);
                Accent.Effect?.Invoke(rect, projOwner);
            }
            catch (SystemException e)
            {
                Main.NewText(e.ToString());
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            Player owner = Main.player[projectile.owner];
            return (target.position.X - owner.position.X) * owner.direction > -1f ? base.CanHitNPC(target) : false;
        }

        public Point CombinedTextureSize()
        {
            return new Point(Blade.Texture.Width - (int)Blade.Origin.X + (int)Hilt.SpearOrigin.X, (int)Blade.Origin.Y + Hilt.SpearTexture.Height - (int)Hilt.SpearOrigin.Y);
        }

        //public override void SendExtraAI(BinaryWriter writer)
        //{
        //    writer.Write(Blade.Type);
        //    writer.Write(Hilt.Type);
        //    writer.Write(Accent.Type);
        //}

        //public override void ReceiveExtraAI(BinaryReader reader)
        //{
        //    Blade = SwordBlade.Blades[reader.ReadInt32()];
        //    Hilt = SwordHilt.Hilts[reader.ReadInt32()];
        //    Accent = SwordAccent.Accents[reader.ReadInt32()];
        //    if (Main.netMode == NetmodeID.MultiplayerClient) 
        //        Initialize();
        //}

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale)
        {
            if (LocalTexture == null && Main.netMode != NetmodeID.Server)
            {
                Initialize();
                return;
            }

            switch (Main.netMode)
            {
                case NetmodeID.SinglePlayer:
                    spriteBatch.Draw(LocalTexture,
                        position + LocalTexture.Size() / 2f,
                        null,
                        Blade.Lighted ? Color.White : color,
                        rotation,
                        projectile.spriteDirection > 0 ? LocalTexture.Bounds.TopRight() : Vector2.Zero,
                        scale,
                        projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                        0f);
                    break;

                case NetmodeID.MultiplayerClient:
                    {
                        Texture2D t2d = Main.projectileTexture[projectile.type];
                        spriteBatch.Draw(t2d,
                            position + t2d.Size() / 2f,
                            null,
                            color,
                            rotation,
                            projectile.spriteDirection > 0 ? t2d.Bounds.TopRight() : Vector2.Zero,
                            scale,
                            projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                            0f);
                        break;
                    }
            }
        }

        public override void Initialize()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                if (Blade == null || Hilt == null || Accent == null)
                {
                    kRPG.LogMessage("Um, hilt, accent or Blade is null.");
                    return;

                }

                LocalTexture = GFX.GFX.CombineTextures(new List<Texture2D> { Blade.Texture, Hilt.SpearTexture, Accent.Texture },
                    new List<Point>
                    {
                        new Point(CombinedTextureSize().X - Blade.Texture.Width, 0),
                        new Point(0, CombinedTextureSize().Y - Hilt.SpearTexture.Height),
                        new Point((int) Hilt.SpearOrigin.X - (int) Accent.Origin.X, CombinedTextureSize().Y - Hilt.SpearTexture.Height + (int) Hilt.SpearOrigin.Y - (int) Accent.Origin.Y)
                    }, CombinedTextureSize());
                projectile.width = LocalTexture.Width;
                projectile.height = LocalTexture.Height;
            }
        }

        // ReSharper disable once IdentifierTypo
        public override void ModifyDamageHitbox(ref Rectangle hitBox)
        {
            Player owner = Main.player[projectile.owner];
            hitBox = new Rectangle((int)projectile.position.X - 2, (int)projectile.position.Y - 2, (int)(projectile.Right.X - projectile.Left.X) + 2,
                (int)(projectile.Bottom.Y - projectile.Top.Y + 2));
            if (owner.direction < 0) hitBox.X += hitBox.Width / 2;
            else hitBox.X -= hitBox.Width / 2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockBack, bool crit)
        {
            try
            {
                Player owner = Main.player[projectile.owner];
                Accent.OnHit?.Invoke(owner, target, (ProceduralSword)owner.inventory[owner.selectedItem].modItem, damage, crit);
            }
            catch (SystemException e)
            {
                Main.NewText(e.ToString());
                ModLoader.GetMod(Constants.ModName).Logger.InfoFormat(e.ToString());
            }
        }

        public override void SetDefaults()
        {
            try
            {
                projectile.width = 40;
                projectile.height = 40;
                projectile.scale = 1f;
                projectile.friendly = true;
                projectile.hostile = false;
                projectile.melee = true;
                projectile.penetrate = -1;
                projectile.timeLeft = 600;
                projectile.tileCollide = false;
            }
            catch (SystemException e)
            {
                Main.NewText(e.ToString());
                ModLoader.GetMod(Constants.ModName).Logger.InfoFormat(e.ToString());
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Procedurally Generated Spear; Please Ignore");
        }
    }
}