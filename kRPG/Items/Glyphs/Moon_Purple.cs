using System;
using kRPG.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;

namespace kRPG.Items.Glyphs
{
    public class Moon_Purple : Moon
    {
        public override float BaseDamageModifier()
        {
            return 2.6f - projCount * 0.06f;
        }

        public override float BaseManaModifier()
        {
            return 1.2f;
        }

        public override Action<ProceduralSpellProj> GetAiAction()
        {
            return delegate(ProceduralSpellProj spell) { spell.projectile.velocity.Y += 0.3f; };
        }

        public override Action<ProceduralSpell, Player, Vector2, Vector2, Entity> GetCastAction()
        {
            return delegate(ProceduralSpell spell, Player player, Vector2 origin, Vector2 target, Entity caster)
            {
                new SpellEffect(spell, target, projCount * 10, delegate(ProceduralSpell ability, int timeLeft)
                {
                    if (timeLeft % 10 != 0)
                        return;
                    var proj = spell.CreateProjectile(player, new Vector2(0, -9f), Main.rand.NextFloat(-0.07f, 0.07f), caster.Center + new Vector2(0, -16f),
                        caster);
                    if (proj.alpha < 1f) proj.alpha = 0.5f;
                    proj.projectile.tileCollide = true;
                });
            };
        }

        public override void Randomize()
        {
            base.Randomize();
            projCount = Main.rand.Next(10, 21);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Purple Moon Glyph");
            Tooltip.SetDefault("Shoots up a fountain of projectiles to rain down around you");
        }
    }
}