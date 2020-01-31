using System;
using System.Linq;
using kRPG2.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace kRPG2.Items.Glyphs
{
    public class Moon_Green : Moon
    {
        public const int RotTimeLeft = 3600;

        public override float BaseDamageModifier()
        {
            return 1.24f - ProjCount * 0.08f;
        }

        public override Action<ProceduralSpellProj> GetAiAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                try
                {
                    int rotDistance = spell.Minion ? 72 : 96;
                    if (RotTimeLeft - spell.projectile.timeLeft >= rotDistance * 2 / 3)
                    {
                        Vector2 unitRelativePos = spell.RelativePos(spell.Caster.Center);
                        unitRelativePos.Normalize();
                        spell.projectile.Center = spell.Caster.Center + unitRelativePos * rotDistance;
                        spell.DisplacementVelocity =
                            new Vector2(1.5f, 0f).RotatedBy(spell.RelativePos(spell.Caster.Center).ToRotation() + (float) API.Tau / 4f);

                        float angle = spell.DisplacementAngle + 0.04f * (RotTimeLeft - spell.projectile.timeLeft - rotDistance * 2 / 3);
                        spell.projectile.Center = spell.Caster.Center + new Vector2(0f, -rotDistance).RotatedBy(angle);
                    }
                    else
                    {
                        spell.projectile.Center = spell.Caster.Center +
                                                  new Vector2(0f, -1.5f).RotatedBy(spell.DisplacementAngle) * (RotTimeLeft - spell.projectile.timeLeft);
                    }

                    spell.projectile.velocity = spell.DisplacementVelocity + spell.Caster.velocity;
                    spell.BasePosition = spell.Caster.position;
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
                        PlayerCharacter character = p.GetModPlayer<PlayerCharacter>();
                        foreach (ProceduralSpellProj proj in character.CirclingProtection.Where(proj => proj.projectile.modProjectile is ProceduralSpellProj))
                            proj.projectile.Kill();
                        character.CirclingProtection.Clear();
                        break;
                    }
                    case Projectile pj:
                    {
                        ProceduralMinion minion = (ProceduralMinion) pj.modProjectile;
                        foreach (ProceduralSpellProj proj in minion.CirclingProtection.Where(proj => proj.projectile.modProjectile is ProceduralSpellProj))
                            proj.projectile.Kill();
                        minion.CirclingProtection.Clear();
                        break;
                    }
                }

                float spread = GetSpread(spell.ProjCount);
                Vector2 velocity = new Vector2(0f, -1.5f);
                for (int i = 0; i < spell.ProjCount; i += 1)
                {
                    ProceduralSpellProj proj = spell.CreateProjectile(player, Vector2.Zero, spread * i, origin, caster);
                    proj.projectile.timeLeft = RotTimeLeft;
                    proj.DisplacementVelocity = velocity.RotatedBy(i * spread * API.Tau);
                    proj.DisplacementAngle = i * spread * (float) API.Tau;
                    switch (caster)
                    {
                        case Player _:
                            player.GetModPlayer<PlayerCharacter>().CirclingProtection.Add(proj);
                            break;
                        case Projectile pj:
                            ((ProceduralMinion) pj.modProjectile).CirclingProtection.Add(proj);
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
            ProjCount = Main.rand.Next(3, 11);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Green Moon Glyph");
            Tooltip.SetDefault("Casts projectiles to orbit around you");
        }
    }
}