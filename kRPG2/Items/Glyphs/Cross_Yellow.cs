using System;
using kRPG2.Projectiles;
using Terraria;
using Terraria.ID;

namespace kRPG2.Items.Glyphs
{
    public class Cross_Yellow : Cross
    {
        public override float BaseDamageModifier()
        {
            return 1.1f;
        }

        public override Action<ProceduralSpellProj> GetInitAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                spell.LocalTexture = GFX.ProjectileBoulder;
                spell.projectile.width = spell.LocalTexture.Width;
                spell.projectile.height = spell.LocalTexture.Height;
                spell.projectile.magic = true;
                spell.Alpha = 1f;
                spell.DrawTrail = true;
                spell.projectile.knockBack = 11f;
                spell.projectile.scale = spell.Minion ? 0.8f : 1f;
            };
        }

        public override Action<ProceduralSpellProj> GetKillAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                for (int k = 0; k < 10; k++)
                    Dust.NewDust(spell.projectile.position + spell.projectile.velocity, spell.projectile.width, spell.projectile.height, DustID.Stone,
                        spell.projectile.oldVelocity.X * 0.5f, spell.projectile.oldVelocity.Y * 0.5f, 0, default, 2f);
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yellow Cross Glyph");
            Tooltip.SetDefault("Conjures large magical boulders");
        }
    }
}