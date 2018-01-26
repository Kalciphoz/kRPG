using System;
using kRPG.Items.Glyphs;
using kRPG.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Modifiers
{
    public class SageModifier : NPCModifier
    {
        public ProceduralSpellProj rotMissile = null;
        public ProceduralSpellProj rotSecondary = null;

        private kNPC kNPC;

        public SageModifier(kNPC kNPC, NPC npc) : base(kNPC, npc)
        {
            npc.GivenName = "Sagely " + npc.GivenName;
            this.kNPC = kNPC;
        }

        public override void Update(NPC npc)
        {
            try
            {
                int rotDistance = 64;
                int rotTimeLeft = 36000;

                if (rotMissile != null)
                    if (rotMissile.projectile.active && npc.active)
                        goto Secondary;
                    else
                        rotMissile.projectile.Kill();

                Projectile proj1 =
                    Main.projectile[
                        Projectile.NewProjectile(npc.Center, new Vector2(0f, -1.5f),
                            kNPC.mod.ProjectileType<ProceduralSpellProj>(), npc.damage, 3f)];
                proj1.hostile = true;
                proj1.friendly = false;
                ProceduralSpellProj ps1 = (ProceduralSpellProj) proj1.modProjectile;
                ps1.origin = proj1.position;
                Cross cross1 = Main.rand.Next(2) == 0 ? (Cross) new Cross_Red() : new Cross_Violet();
                ps1.ai.Add(delegate(ProceduralSpellProj spell)
                {
                    cross1.GetAIAction()(spell);

                    float displacementAngle = (float) API.Tau / 4f;
                    Vector2 displacementVelocity = Vector2.Zero;
                    if (rotTimeLeft - spell.projectile.timeLeft >= rotDistance * 2 / 3)
                    {
                        Vector2 unitRelativePos = spell.RelativePos(spell.caster.Center);
                        unitRelativePos.Normalize();
                        spell.projectile.Center = spell.caster.Center + unitRelativePos * rotDistance;
                        displacementVelocity =
                            new Vector2(-2f, 0f).RotatedBy((spell.RelativePos(spell.caster.Center)).ToRotation() +
                                                           (float) API.Tau / 4f);

                        float angle = displacementAngle -
                                      0.06f * (float) (rotTimeLeft - spell.projectile.timeLeft - rotDistance * 2 / 3);
                        spell.projectile.Center = spell.caster.Center + new Vector2(0f, -rotDistance).RotatedBy(angle);
                    }
                    else
                    {
                        spell.projectile.Center = spell.caster.Center +
                                                  new Vector2(0f, -1.5f).RotatedBy(displacementAngle) *
                                                  (rotTimeLeft - spell.projectile.timeLeft);
                    }

                    spell.projectile.velocity = displacementVelocity + spell.caster.velocity;
                    spell.basePosition = spell.caster.position;
                });
                ps1.init.Add(cross1.GetInitAction());
                ps1.caster = npc;
                ps1.Initialize();
                ps1.projectile.penetrate = -1;
                ps1.projectile.timeLeft = rotTimeLeft;
                rotMissile = ps1;

                Secondary:

                if (rotSecondary != null)
                    if (rotSecondary.projectile.active && npc.active)
                        return;
                    else
                        rotSecondary.projectile.Kill();

                Projectile proj2 =
                    Main.projectile[
                        Projectile.NewProjectile(npc.Center, new Vector2(0f, 1.5f),
                            kNPC.mod.ProjectileType<ProceduralSpellProj>(), npc.damage, 3f)];
                proj2.hostile = true;
                proj2.friendly = false;
                ProceduralSpellProj ps2 = (ProceduralSpellProj) proj2.modProjectile;
                ps2.origin = proj2.position;
                Cross cross2 = Main.rand.Next(2) == 0 ? (Cross) new Cross_Blue() : new Cross_Purple();
                ps2.ai.Add(delegate(ProceduralSpellProj spell)
                {
                    cross2.GetAIAction()(spell);

                    float displacementAngle = (float) API.Tau / 4f + (float) Math.PI;
                    Vector2 displacementVelocity = Vector2.Zero;
                    if (rotTimeLeft - spell.projectile.timeLeft >= rotDistance * 2 / 3)
                    {
                        Vector2 unitRelativePos = spell.RelativePos(spell.caster.Center);
                        unitRelativePos.Normalize();
                        spell.projectile.Center = spell.caster.Center + unitRelativePos * rotDistance;
                        displacementVelocity =
                            new Vector2(-2f, 0f).RotatedBy((spell.RelativePos(spell.caster.Center)).ToRotation() +
                                                           (float) API.Tau / 4f);

                        float angle = displacementAngle -
                                      0.06f * (float) (rotTimeLeft - spell.projectile.timeLeft - rotDistance * 2 / 3);
                        spell.projectile.Center = spell.caster.Center + new Vector2(0f, -rotDistance).RotatedBy(angle);
                    }
                    else
                    {
                        spell.projectile.Center = spell.caster.Center +
                                                  new Vector2(0f, 1.5f).RotatedBy(displacementAngle) *
                                                  (rotTimeLeft - spell.projectile.timeLeft);
                    }

                    spell.projectile.velocity = displacementVelocity + spell.caster.velocity;
                    spell.basePosition = spell.caster.position;
                });
                ps2.init.Add(cross2.GetInitAction());
                ps2.caster = npc;
                ps2.Initialize();
                ps2.projectile.penetrate = -1;
                ps2.projectile.timeLeft = rotTimeLeft;
                rotSecondary = ps2;
            }
            catch (SystemException e)
            {
                Main.NewText(e.ToString());
                ErrorLogger.Log(e.ToString());
            }
        }
        
        
        
        public new static NPCModifier Random(kNPC kNPC, NPC npc)
        {
            return new SageModifier(kNPC, npc);
        }

        public new static NPCModifier New(kNPC kNPC, NPC npc)
        {
            return new SageModifier(kNPC, npc);
        }
    }
}