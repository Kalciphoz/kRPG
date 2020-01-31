using System;
using System.Collections.Generic;
using kRPG2.Enums;
using kRPG2.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace kRPG2.Items.Glyphs
{
    public class Cross_Purple : Cross
    {
        public override Dictionary<ELEMENT, float> EleDmg { get; set; } = new Dictionary<ELEMENT, float> { { ELEMENT.FIRE, 0 }, { ELEMENT.COLD, 0 }, { ELEMENT.LIGHTNING, 1f }, { ELEMENT.SHADOW, 0 } };

        public override Action<ProceduralSpellProj> GetAiAction()
        {
            return delegate (ProceduralSpellProj spell)
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
            return delegate (ProceduralSpellProj spell)
            {
                spell.LocalTexture = GFX.ProjectileThunderbolt;
                spell.projectile.width = spell.LocalTexture.Width;
                spell.projectile.height = spell.LocalTexture.Height;
                spell.projectile.magic = true;
                spell.Lighted = true;
            };
        }

        public override Action<ProceduralSpellProj> GetKillAction()
        {
            return delegate (ProceduralSpellProj spell)
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