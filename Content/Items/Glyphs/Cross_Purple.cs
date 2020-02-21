using System;
using System.Collections.Generic;
using kRPG.Content.Items.Projectiles;
using kRPG.Enums;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.Content.Items.Glyphs
{
    public class Cross_Purple : Cross
    {
        public override Dictionary<Element, float> EleDmg { get; set; } =
            new Dictionary<Element, float> {{Element.Fire, 0}, {Element.Cold, 0}, {Element.Lightning, 1f}, {Element.Shadow, 0}};

        public override Action<ProceduralSpellProj> GetAiAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                ProceduralSpellProj.aiRotateToVelocity(spell);
                if (!(Main.rand.NextFloat(0f, 2f) <= spell.Alpha))
                    return;
                int dust = Dust.NewDust(spell.projectile.position, spell.projectile.width, spell.projectile.height, DustID.Electric,
                    spell.projectile.velocity.X * 0.2f, spell.projectile.velocity.Y * 0.2f, 63, Color.White, 0.2f + spell.Alpha);
                Main.dust[dust].noGravity = true;
            };
        }

        public override Action<ProceduralSpellProj> GetInitAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                spell.LocalTexture = GFX.GFX.ProjectileThunderbolt;

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
                    Dust.NewDust(spell.projectile.position + spell.projectile.velocity, spell.projectile.width, spell.projectile.height, DustID.Electric,
                        spell.projectile.oldVelocity.X * 0.5f, spell.projectile.oldVelocity.Y * 0.5f, 0, default, 0.5f);
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Purple Cross Glyph");
            Tooltip.SetDefault("Casts magical lightning orbs");
        }
    }
}