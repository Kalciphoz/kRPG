using System;
using kRPG.Content.Items.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.Content.Items.Glyphs
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
                spell.LocalTexture = GFX.GFX.ProjectileBoulder;
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