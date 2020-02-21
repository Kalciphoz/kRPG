using System;
using System.Collections.Generic;
using kRPG.Content.Items.Dusts;
using kRPG.Content.Items.Projectiles;
using kRPG.Enums;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Content.Items.Glyphs
{
    public class Cross_Blue : Cross
    {
        public override Dictionary<Element, float> EleDmg { get; set; } =
            new Dictionary<Element, float> {{Element.Fire, 0}, {Element.Cold, 1f}, {Element.Lightning, 0}, {Element.Shadow, 0}};

        public override float BaseDamageModifier()
        {
            return 1.05f;
        }

        public override Action<ProceduralSpellProj> GetAiAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                if (Main.rand.NextFloat(0f, 2f) <= spell.Alpha)
                {
                    int dust = Dust.NewDust(spell.projectile.position, spell.projectile.width, spell.projectile.height, ModContent.GetInstance<Ice>().Type, 0f,
                        0f, 100, Color.White, 0.5f + spell.Alpha);
                    Main.dust[dust].noGravity = true;
                }

                Lighting.AddLight(spell.projectile.Center, 0f, 0.4f, 1f);
            };
        }

        public override Action<ProceduralSpellProj> GetInitAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                spell.LocalTexture = GFX.GFX.ProjectileFrostbolt;

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

                spell.projectile.magic = true;
                spell.Lighted = true;
            };
        }

        public override Action<ProceduralSpellProj> GetKillAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                for (int k = 0; k < 8; k++)
                    try
                    {
                        Dust.NewDust(spell.projectile.position + spell.projectile.velocity, spell.projectile.width, spell.projectile.height,
                            ModContent.DustType<Ice>(), spell.projectile.oldVelocity.X * 0.5f, spell.projectile.oldVelocity.Y * 0.5f);
                    }
                    catch (SystemException e)
                    {
                        ModLoader.GetMod(Constants.ModName).Logger.InfoFormat(e.ToString());
                    }
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blue Cross Glyph");
            Tooltip.SetDefault("Casts magical ice cubes");
        }
    }
}