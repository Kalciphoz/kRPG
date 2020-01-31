using System;
using Microsoft.Xna.Framework;
using Terraria;

namespace kRPG.Items.Glyphs
{
    public class Moon_Violet : Moon
    {
        public const float area = 192f;

        public override float BaseDamageModifier()
        {
            return 1.1f - projCount * 0.02f;
        }

        public override Action<ProceduralSpell, Player, Vector2, Vector2, Entity> GetCastAction()
        {
            return delegate(ProceduralSpell spell, Player player, Vector2 origin, Vector2 target, Entity caster)
            {
                new SpellEffect(spell, target, projCount * 8, delegate(ProceduralSpell ability, int timeLeft)
                {
                    if (timeLeft % 8 != 0)
                        return;
                    var proj = spell.CreateProjectile(player, new Vector2(0f, 8f), 0f,
                        new Vector2(target.X - area / 2f + Main.rand.NextFloat(area), target.Y - 240f), caster);
                    if (proj.alpha < 1f) proj.alpha = 0.5f;
                    proj.projectile.timeLeft = 60;
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
            DisplayName.SetDefault("Violet Moon Glyph");
            Tooltip.SetDefault("Rains down projectiles at the target location");
        }
    }
}