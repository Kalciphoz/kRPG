using System;
using kRPG.GameObjects.Items.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.GameObjects.Items.Glyphs
{
    public class Cross_Green : Cross
    {
        public override float BaseManaModifier()
        {
            return 0.9f;
        }

        public override Action<ProceduralSpellProj> GetAiAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                if (spell.projectile.velocity.X < 0 && spell.BasePosition == Vector2.Zero) spell.projectile.spriteDirection = -1;
                Vector2 v = spell.BasePosition != Vector2.Zero ? spell.BasePosition : spell.Origin;
                spell.projectile.rotation = (spell.projectile.Center - v).ToRotation() - (float) Constants.Tau / 4f;
            };
        }

        public override Action<ProceduralSpellProj> GetInitAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                spell.LocalTexture = Main.itemTexture[ItemID.WoodenArrow];
                if (spell.LocalTexture == null)
                {
                    ModLoader.GetMod(Constants.ModName).Logger.InfoFormat("GetInitAction, spell.localtexture = null.");
                    spell.projectile.width = 48;
                    spell.projectile.height = 48;
                }
                else
                {
                    spell.projectile.width = spell.LocalTexture.Width;
                    spell.projectile.height = spell.LocalTexture.Height;
                }
                spell.projectile.ranged = true;
                spell.DrawTrail = true;
                spell.Alpha = 1f;
                spell.Lighted = true;
                spell.projectile.scale = spell.Minion ? 1f : 1.5f;
            };
        }

        public override Action<ProceduralSpellProj> GetKillAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                for (int k = 0; k < 5; k++)
                    Dust.NewDust(spell.projectile.position + spell.projectile.velocity, spell.projectile.width, spell.projectile.height, DustID.Stone,
                        spell.projectile.oldVelocity.X * 0.5f, spell.projectile.oldVelocity.Y * 0.5f);
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Green Cross Glyph");
            Tooltip.SetDefault("Conjures giant arrows that deal ranged damage");
        }
    }
}