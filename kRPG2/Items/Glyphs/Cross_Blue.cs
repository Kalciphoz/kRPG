using System;
using System.Collections.Generic;
using kRPG2.Enums;
using kRPG2.Items.Dusts;
using kRPG2.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace kRPG2.Items.Glyphs
{
    public class Cross_Blue : Cross
    {
        public override Dictionary<ELEMENT, float> EleDmg { get; set; } =
            new Dictionary<ELEMENT, float> {{ELEMENT.FIRE, 0}, {ELEMENT.COLD, 1f}, {ELEMENT.LIGHTNING, 0}, {ELEMENT.SHADOW, 0}};

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
                spell.LocalTexture = GFX.ProjectileFrostbolt;
                spell.projectile.width = spell.LocalTexture.Width;
                spell.projectile.height = spell.LocalTexture.Height;
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
                        ModLoader.GetMod("kRPG2").Logger.InfoFormat(e.ToString());
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