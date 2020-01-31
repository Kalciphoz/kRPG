using System;
using Microsoft.Xna.Framework;
using Terraria;

namespace kRPG.Items.Glyphs
{
    public class Moon_Blue : Moon
    {
        public override float BaseDamageModifier()
        {
            return 1.2f;
        }

        public override float BaseManaModifier()
        {
            return 0.72f + projCount * 0.06f;
        }

        public override Action<ProceduralSpell, Player, Vector2, Vector2, Entity> GetCastAction()
        {
            return delegate(ProceduralSpell spell, Player player, Vector2 origin, Vector2 target, Entity caster)
            {
                float spread = GetSpread(spell.projCount);
                Vector2 unitVelocity = target - origin;
                unitVelocity.Normalize();
                Vector2 velocity = unitVelocity * 6f;
                for (int i = 0; i < spell.projCount; i += 1)
                    spell.CreateProjectile(player, velocity, spell.projCount * -spread / 2f + i * spread + spread / 2f, origin, caster);
            };
        }

        private static float GetSpread(int projCount)
        {
            return 0.020f - projCount * 0.001f;
        }

        public override void Randomize()
        {
            base.Randomize();
            projCount = Main.rand.Next(3, 8);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blue Moon Glyph");
            Tooltip.SetDefault("Fires an array of projectiles outwards");
        }
    }
}