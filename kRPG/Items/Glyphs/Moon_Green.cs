using System;
using System.Linq;
using kRPG.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Items.Glyphs
{
    public class Moon_Green : Moon
    {
        public const int RotTimeLeft = 3600;

        public override float BaseDamageModifier()
        {
            return 1.24f - projCount * 0.08f;
        }

        public override Action<ProceduralSpellProj> GetAiAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                try
                {
                    int rotDistance = spell.minion ? 72 : 96;
                    if (RotTimeLeft - spell.projectile.timeLeft >= rotDistance * 2 / 3)
                    {
                        var unitRelativePos = spell.RelativePos(spell.caster.Center);
                        unitRelativePos.Normalize();
                        spell.projectile.Center = spell.caster.Center + unitRelativePos * rotDistance;
                        spell.displacementVelocity =
                            new Vector2(1.5f, 0f).RotatedBy(spell.RelativePos(spell.caster.Center).ToRotation() + (float) API.Tau / 4f);

                        float angle = spell.displacementAngle + 0.04f * (RotTimeLeft - spell.projectile.timeLeft - rotDistance * 2 / 3);
                        spell.projectile.Center = spell.caster.Center + new Vector2(0f, -rotDistance).RotatedBy(angle);
                    }
                    else
                    {
                        spell.projectile.Center = spell.caster.Center +
                                                  new Vector2(0f, -1.5f).RotatedBy(spell.displacementAngle) * (RotTimeLeft - spell.projectile.timeLeft);
                    }

                    spell.projectile.velocity = spell.displacementVelocity + spell.caster.velocity;
                    spell.basePosition = spell.caster.position;
                }
                catch (SystemException e)
                {
                    ModLoader.GetMod("kRPG").Logger.InfoFormat(e.ToString());
                }
            };
        }

        public override Action<ProceduralSpell, Player, Vector2, Vector2, Entity> GetCastAction()
        {
            return delegate(ProceduralSpell spell, Player player, Vector2 origin, Vector2 target, Entity caster)
            {
                switch (caster)
                {
                    case Player p:
                    {
                        var character = p.GetModPlayer<PlayerCharacter>();
                        foreach (var proj in character.circlingProtection.Where(proj => proj.projectile.modProjectile is ProceduralSpellProj))
                            proj.projectile.Kill();
                        character.circlingProtection.Clear();
                        break;
                    }
                    case Projectile pj:
                    {
                        var minion = (ProceduralMinion) pj.modProjectile;
                        foreach (var proj in minion.circlingProtection.Where(proj => proj.projectile.modProjectile is ProceduralSpellProj))
                            proj.projectile.Kill();
                        minion.circlingProtection.Clear();
                        break;
                    }
                }

                float spread = GetSpread(spell.projCount);
                var velocity = new Vector2(0f, -1.5f);
                for (int i = 0; i < spell.projCount; i += 1)
                {
                    var proj = spell.CreateProjectile(player, Vector2.Zero, spread * i, origin, caster);
                    proj.projectile.timeLeft = RotTimeLeft;
                    proj.displacementVelocity = velocity.RotatedBy(i * spread * API.Tau);
                    proj.displacementAngle = i * spread * (float) API.Tau;
                    switch (caster)
                    {
                        case Player _:
                            player.GetModPlayer<PlayerCharacter>().circlingProtection.Add(proj);
                            break;
                        case Projectile pj:
                            ((ProceduralMinion) pj.modProjectile).circlingProtection.Add(proj);
                            break;
                    }
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
            projCount = Main.rand.Next(3, 11);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Green Moon Glyph");
            Tooltip.SetDefault("Casts projectiles to orbit around you");
        }
    }
}