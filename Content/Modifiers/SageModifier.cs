using System;
using System.IO;
using kRPG.Content.Items.Glyphs;
using kRPG.Content.Items.Projectiles;
using kRPG.Content.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Content.Modifiers
{
    public class SageModifier : NpcModifier
    {

        public SageModifier() { }
        private int Cross1Id { get; set; } = 0;
        private int Cross2Id { get; set; } = 0;

        
        public SageModifier(kNPC kNpc, NPC npc) : base(kNpc, npc)
        {
            this.npc = npc;

        }

        public ProceduralSpellProj RotMissile { get; set; }
        public ProceduralSpellProj RotSecondary { get; set; }

        public new static NpcModifier New(kNPC kNpc, NPC npc)
        {
            return new SageModifier(kNpc, npc);
        }

        //public new static NpcModifier Random(kNPC kNpc, NPC npc)
        //{
        //    return new SageModifier(kNpc, npc);
        //}

        public override void Update(NPC kNpc)
        {
            try
            {
                const int rotDistance = 64;
                const int rotTimeLeft = 36000;

                if (RotMissile != null)
                    if (RotMissile.projectile.active && kNpc.active)
                        goto Secondary;
                    else
                        RotMissile.projectile.Kill();

                Projectile proj1 = Main.projectile[Projectile.NewProjectile(kNpc.Center, new Vector2(0f, -1.5f), ModContent.ProjectileType<ProceduralSpellProj>(), kNpc.damage, 3f)];
                proj1.hostile = true;
                proj1.friendly = false;
                ProceduralSpellProj ps1 = (ProceduralSpellProj)proj1.modProjectile;
                ps1.Origin = proj1.position;

                Cross cross1 = Cross1Id == 0 ? (Cross)new Cross_Red() : new Cross_Violet();

                ps1.Ai.Add(delegate (ProceduralSpellProj spell)
                {
                    cross1.GetAiAction()(spell);

                    float displacementAngle = (float)Constants.Tau / 4f;
                    Vector2 displacementVelocity = Vector2.Zero;
                    if (rotTimeLeft - spell.projectile.timeLeft >= rotDistance * 2 / 3)
                    {
                        Vector2 unitRelativePos = spell.RelativePos(spell.Caster.Center);
                        unitRelativePos.Normalize();
                        spell.projectile.Center = spell.Caster.Center + unitRelativePos * rotDistance;
                        displacementVelocity = new Vector2(-2f, 0f).RotatedBy(spell.RelativePos(spell.Caster.Center).ToRotation() + (float)Constants.Tau / 4f);

                        float angle = displacementAngle - 0.06f * (rotTimeLeft - spell.projectile.timeLeft - rotDistance * 2 / 3);
                        spell.projectile.Center = spell.Caster.Center + new Vector2(0f, -rotDistance).RotatedBy(angle);
                    }
                    else
                    {
                        spell.projectile.Center = spell.Caster.Center +
                                                  new Vector2(0f, -1.5f).RotatedBy(displacementAngle) * (rotTimeLeft - spell.projectile.timeLeft);
                    }

                    spell.projectile.velocity = displacementVelocity + spell.Caster.velocity;
                    spell.BasePosition = spell.Caster.position;
                });
                ps1.Inits.Add(cross1.GetInitAction());
                ps1.Caster = kNpc;
                ps1.Initialize();
                ps1.projectile.penetrate = -1;
                ps1.projectile.timeLeft = rotTimeLeft;
                RotMissile = ps1;

            Secondary:

                if (RotSecondary != null)
                    if (RotSecondary.projectile.active && kNpc.active)
                        return;
                    else
                        RotSecondary.projectile.Kill();

                Projectile proj2 = Main.projectile[Projectile.NewProjectile(kNpc.Center, new Vector2(0f, 1.5f), ModContent.ProjectileType<ProceduralSpellProj>(), kNpc.damage, 3f)];
                proj2.hostile = true;
                proj2.friendly = false;
                ProceduralSpellProj ps2 = (ProceduralSpellProj)proj2.modProjectile;

                //Null Check to prevent crash.
                if (ps2 == null)
                    return;

                ps2.Origin = proj2.position;
                Cross cross2 = Cross2Id == 0 ? (Cross)new Cross_Blue() : new Cross_Purple();
                ps2.Ai.Add(delegate (ProceduralSpellProj spell)
                {
                    cross2.GetAiAction()(spell);

                    float displacementAngle = (float)Constants.Tau / 4f + (float)Math.PI;
                    Vector2 displacementVelocity = Vector2.Zero;
                    if (rotTimeLeft - spell.projectile.timeLeft >= rotDistance * 2 / 3)
                    {
                        Vector2 unitRelativePos = spell.RelativePos(spell.Caster.Center);
                        unitRelativePos.Normalize();
                        spell.projectile.Center = spell.Caster.Center + unitRelativePos * rotDistance;
                        displacementVelocity = new Vector2(-2f, 0f).RotatedBy(spell.RelativePos(spell.Caster.Center).ToRotation() + (float)Constants.Tau / 4f);

                        float angle = displacementAngle - 0.06f * (rotTimeLeft - spell.projectile.timeLeft - rotDistance * 2 / 3);
                        spell.projectile.Center = spell.Caster.Center + new Vector2(0f, -rotDistance).RotatedBy(angle);
                    }
                    else
                    {
                        spell.projectile.Center = spell.Caster.Center +
                                                  new Vector2(0f, 1.5f).RotatedBy(displacementAngle) * (rotTimeLeft - spell.projectile.timeLeft);
                    }

                    spell.projectile.velocity = displacementVelocity + spell.Caster.velocity;
                    spell.BasePosition = spell.Caster.position;
                });
                ps2.Inits.Add(cross2.GetInitAction());
                ps2.Caster = kNpc;
                ps2.Initialize();
                ps2.projectile.penetrate = -1;
                ps2.projectile.timeLeft = rotTimeLeft;
                RotSecondary = ps2;



            }
            catch (SystemException e)
            {
                Main.NewText(e.ToString());
                ModLoader.GetMod(Constants.ModName).Logger.InfoFormat(e.ToString());
            }
        }

        public override int Pack(ModPacket packet)
        {
            packet.Write(Cross1Id);
            packet.Write(Cross2Id);
            return 0;
        }

        public override void Initialize()
        {
            
            Cross1Id = Main.rand.Next(2);
            Cross2Id = Main.rand.Next(2);
            kRPG.LogMessage($"Initializing Sage Modifier: Cross 1 = {Cross1Id} Cross 2 ={Cross2Id}");
        }

        public override void Apply()
        {
            AddNamePrefix("Sagely");
            
        }

        public override int Unpack(BinaryReader reader)
        {
            Cross1Id = reader.ReadInt32();
            Cross2Id = reader.ReadInt32();
            return 0;
        }
    }
}