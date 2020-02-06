using System;
using kRPG.GameObjects.Items.Projectiles;
using kRPG.GameObjects.Spells;
using Microsoft.Xna.Framework;
using Terraria;

namespace kRPG.GameObjects.Items.Glyphs
{
    public class Moon_Yellow : Moon
    {
        public override float BaseDamageModifier()
        {
            return 1f - ProjCount * 0.05f;
        }

        public override Action<ProceduralSpellProj> GetAiAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                int rotDistance = spell.Minion ? 32 : 48;
                spell.BasePosition += spell.BaseVelocity;
                Vector2 unitRelativePos = spell.RelativePos(spell.BasePosition);
                unitRelativePos.Normalize();
                spell.projectile.Center = spell.BasePosition + unitRelativePos * rotDistance;
                spell.DisplacementVelocity =
                    new Vector2(12f / spell.Source.ProjCount, 0f).RotatedBy(spell.RelativePos(spell.BasePosition).ToRotation() + (float) API.Tau / 4f);

                float angle = spell.DisplacementAngle + 0.24f * (-spell.projectile.timeLeft - rotDistance) / ProjCount;
                spell.projectile.Center = spell.BasePosition + new Vector2(0f, -rotDistance).RotatedBy(angle);

                spell.projectile.velocity = spell.DisplacementVelocity + spell.BaseVelocity;
            };
        }

        public override Action<ProceduralSpell, Player, Vector2, Vector2, Entity> GetCastAction()
        {
            return delegate(ProceduralSpell spell, Player player, Vector2 origin, Vector2 target, Entity caster)
            {
                int rotDistance = spell.Minion ? 32 : 48;
                float spread = GetSpread(spell.ProjCount);
                for (int i = 0; i < spell.ProjCount; i += 1)
                {
                    float angle = i * spread * (float) API.Tau;
                    ProceduralSpellProj proj = spell.CreateProjectile(player, Vector2.Zero, spread * i, origin + new Vector2(0f, -rotDistance).RotatedBy(angle),
                        caster);
                    proj.BasePosition = origin;
                    Vector2 unitVelocity = target - origin;
                    unitVelocity.Normalize();
                    proj.BaseVelocity = unitVelocity * 8f;
                    proj.DisplacementAngle = angle;
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
            ProjCount = Main.rand.Next(2, 5);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yellow Moon Glyph");
            Tooltip.SetDefault("Throws whirling projectiles that pierce the enemies");
        }
    }
}