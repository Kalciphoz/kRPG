using System;
using kRPG.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;

namespace kRPG.Items.Glyphs
{
    public class Moon_Yellow : Moon
    {
        public override float BaseDamageModifier()
        {
            return 1f - projCount * 0.05f;
        }

        public override Action<ProceduralSpellProj> GetAiAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                int rotDistance = spell.minion ? 32 : 48;
                spell.basePosition += spell.baseVelocity;
                Vector2 unitRelativePos = spell.RelativePos(spell.basePosition);
                unitRelativePos.Normalize();
                spell.projectile.Center = spell.basePosition + unitRelativePos * rotDistance;
                spell.displacementVelocity =
                    new Vector2(12f / spell.source.projCount, 0f).RotatedBy(spell.RelativePos(spell.basePosition).ToRotation() + (float) API.Tau / 4f);

                float angle = spell.displacementAngle + 0.24f * (-spell.projectile.timeLeft - rotDistance) / projCount;
                spell.projectile.Center = spell.basePosition + new Vector2(0f, -rotDistance).RotatedBy(angle);

                spell.projectile.velocity = spell.displacementVelocity + spell.baseVelocity;
            };
        }

        public override Action<ProceduralSpell, Player, Vector2, Vector2, Entity> GetCastAction()
        {
            return delegate(ProceduralSpell spell, Player player, Vector2 origin, Vector2 target, Entity caster)
            {
                int rotDistance = spell.minion ? 32 : 48;
                float spread = GetSpread(spell.projCount);
                for (int i = 0; i < spell.projCount; i += 1)
                {
                    float angle = i * spread * (float) API.Tau;
                    ProceduralSpellProj proj = spell.CreateProjectile(player, Vector2.Zero, spread * i, origin + new Vector2(0f, -rotDistance).RotatedBy(angle), caster);
                    proj.basePosition = origin;
                    Vector2 unitVelocity = target - origin;
                    unitVelocity.Normalize();
                    proj.baseVelocity = unitVelocity * 8f;
                    proj.displacementAngle = angle;
                    proj.projectile.penetrate = 3;
                }
            };
        }

        private static float GetSpread(int projCount)
        {
            return 1f / projCount;
        }

        public override void Randomize()
        {
            base.Randomize();
            projCount = Main.rand.Next(2, 5);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yellow Moon Glyph");
            Tooltip.SetDefault("Throws whirling projectiles that pierce the enemies");
        }
    }
}