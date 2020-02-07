using System;
using kRPG.Items.Glyphs;
using kRPG.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Modifiers
{
    public class WizardModifier : NPCModifier
    {
        private int timer = 0;
        private const int cooldown = 140;
        private int spell = 0;

        private kNPC kNPC;

        public WizardModifier(kNPC kNPC, NPC npc) : base(kNPC, npc)
        {
            this.npc = npc;
            npc.GivenName = "Wizardly " + npc.FullName;
            this.kNPC = kNPC;
        }

        public override void Update(NPC npc)
        {
            if (timer > 0)
            {
                timer -= 1;
                return;
            }
            else
                try
                {
                    float dist = Vector2.Distance(npc.position, npc.NearestPlayer().position);
                    if (dist > 640f || dist < 128f) return;

                    Vector2 velocity = npc.NearestPlayer().position - npc.position;
                    velocity.Normalize();
                    velocity *= 6f;
                    Projectile projectile =
                        Main.projectile[
                            Projectile.NewProjectile(npc.Center, velocity,
                                kNPC.mod.ProjectileType<ProceduralSpellProj>(), npc.damage * 2 / 3, 3f)];
                    projectile.hostile = true;
                    projectile.friendly = false;
                    ProceduralSpellProj psp = (ProceduralSpellProj) projectile.modProjectile;
                    psp.origin = projectile.position;
                    Cross cross;
                    switch (spell % 4)
                    {
                        default:
                            cross = new Cross_Red();
                            break;
                        case 1:
                            cross = new Cross_Blue();
                            break;
                        case 2:
                            cross = new Cross_Purple();
                            break;
                        case 3:
                            cross = new Cross_Violet();
                            break;
                    }
                    psp.ai.Add(cross.GetAIAction());
                    psp.init.Add(cross.GetInitAction());
                    psp.caster = npc;
                    psp.Initialize();

                    timer = cooldown;
                    spell += 1;
                }
                catch (SystemException e)
                {
                    Main.NewText(e.ToString());
                    ErrorLogger.Log(e.ToString());
                }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            int i = Math.Abs(((int)(Main.time * 11) % 511) - 255);
            drawColor = new Color(i, i, 255);
        }

        public new static NPCModifier Random(kNPC kNPC, NPC npc)
        {
            return new WizardModifier(kNPC, npc);
        }

        public new static NPCModifier New(kNPC kNPC, NPC npc)
        {
            return new WizardModifier(kNPC, npc);
        }
    }
}