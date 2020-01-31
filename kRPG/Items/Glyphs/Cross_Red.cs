using System;
using System.Collections.Generic;
using kRPG.Enums;
using kRPG.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.Items.Glyphs
{
    public class Cross_Red : Cross
    {
        public override Dictionary<ELEMENT, float> eleDmg =>
            new Dictionary<ELEMENT, float> {{ELEMENT.FIRE, 1f}, {ELEMENT.COLD, 0}, {ELEMENT.LIGHTNING, 0}, {ELEMENT.SHADOW, 0}};

        public override Action<ProceduralSpellProj> GetAiAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                try
                {
                    ProceduralSpellProj.aiRotateToVelocity(spell);
                    if (!(Main.rand.NextFloat(0f, 1.5f) <= spell.alpha))
                        return;
                    int dust = Dust.NewDust(spell.projectile.position, spell.projectile.width, spell.projectile.height, DustID.Fire,
                        spell.projectile.velocity.X * 0.2f, spell.projectile.velocity.Y * 0.2f, 63, Color.White, 1f + spell.alpha * 2f);
                    Main.dust[dust].noGravity = true;
                }
                catch (SystemException e)
                {
                    ModLoader.GetMod("kRPG").Logger.InfoFormat(e.ToString());
                }
            };
        }

        public override Action<ProceduralSpellProj> GetInitAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                try
                {
                    spell.texture = GFX.projectileFireball;
                    spell.projectile.width = spell.texture.Width;
                    spell.projectile.height = spell.texture.Height;
                    spell.projectile.magic = true;
                    spell.drawTrail = true;
                    spell.lighted = true;
                    spell.projectile.scale = spell.minion ? 0.7f : 1f;
                }
                catch (SystemException e)
                {
                    ModLoader.GetMod("kRPG").Logger.InfoFormat(e.ToString());
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
                    ModLoader.GetMod("kRPG").Logger.InfoFormat(e.ToString());
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