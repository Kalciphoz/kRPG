using System;
using System.Collections.Generic;
using kRPG.Enums;
using kRPG.GameObjects.Items.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.GameObjects.Items.Glyphs
{
    public class Cross_Red : Cross
    {
        public override Dictionary<Element, float> EleDmg { get; set; } =
            new Dictionary<Element, float> {{Element.Fire, 1f}, {Element.Cold, 0}, {Element.Lightning, 0}, {Element.Shadow, 0}};

        public override Action<ProceduralSpellProj> GetAiAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                try
                {
                    ProceduralSpellProj.aiRotateToVelocity(spell);
                    if (!(Main.rand.NextFloat(0f, 1.5f) <= spell.Alpha))
                        return;
                    int dust = Dust.NewDust(spell.projectile.position, spell.projectile.width, spell.projectile.height, DustID.Fire,
                        spell.projectile.velocity.X * 0.2f, spell.projectile.velocity.Y * 0.2f, 63, Color.White, 1f + spell.Alpha * 2f);
                    Main.dust[dust].noGravity = true;
                }
                catch (SystemException e)
                {
                    ModLoader.GetMod(Constants.ModName).Logger.InfoFormat(e.ToString());
                }
            };
        }

        public override Action<ProceduralSpellProj> GetInitAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                try
                {
                    spell.LocalTexture = GFX.GFX.ProjectileFireball;
                    spell.projectile.width = spell.LocalTexture.Width;
                    spell.projectile.height = spell.LocalTexture.Height;
                    spell.projectile.magic = true;
                    spell.DrawTrail = true;
                    spell.Lighted = true;
                    spell.projectile.scale = spell.Minion ? 0.7f : 1f;
                }
                catch (SystemException e)
                {
                    ModLoader.GetMod(Constants.ModName).Logger.InfoFormat(e.ToString());
                }
            };
        }

        public override Action<ProceduralSpellProj> GetKillAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                try
                {
                    for (int k = 0; k < 20; k++)
                        Dust.NewDust(spell.projectile.position + spell.projectile.velocity, spell.projectile.width, spell.projectile.height, DustID.Fire,
                            spell.projectile.oldVelocity.X * 0.5f, spell.projectile.oldVelocity.Y * 0.5f, 0, default, 1.5f);
                }
                catch (SystemException e)
                {
                    ModLoader.GetMod(Constants.ModName).Logger.InfoFormat(e.ToString());
                }
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Red Cross Glyph");
            Tooltip.SetDefault("Casts magical fireballs");
        }
    }
}