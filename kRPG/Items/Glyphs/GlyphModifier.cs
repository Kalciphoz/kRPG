using kRPG.Buffs;
using kRPG.GUI;
using kRPG.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.Items.Glyphs
{
    public class GlyphModifier
    {
        public static GlyphModifier attack;
        public static GlyphModifier smallProt;
        public static GlyphModifier smokePellets;
        public static GlyphModifier explosions;
        public static GlyphModifier vanish;
        public static GlyphModifier crosschains;
        public static GlyphModifier lifeleech;
        public static GlyphModifier manaleech;
        public static GlyphModifier thornchains;
        public static GlyphModifier pierce;
        public static GlyphModifier bounce;

        public static GlyphModifier group_impactEffects;

        public static List<GlyphModifier> modifiers;

        public int id = 0;
        public string tooltip = "";
        public Func<Glyph, bool> match;
        public Func<bool> odds;
        public float dmgModifier = 1f;
        public float manaModifier = 1f;
        public Action<ProceduralMinion> minionAI;
        public Action<ProceduralSpellProj, NPC, int> impact;
        public Action<ProceduralSpellProj> init;
        public Action<ProceduralSpellProj, SpriteBatch, Color> draw;

        public Func<GlyphModifier> group;

        public GlyphModifier(int id, string tooltip, Func<Glyph, bool> match, Func<bool> odds, float dmgModifier = 1f, float manaModifier = 1f)
        {
            this.id = id;
            this.tooltip = tooltip;
            this.match = match;
            this.odds = odds;
            this.dmgModifier = dmgModifier;
            this.manaModifier = manaModifier;
            if (!modifiers.Contains(this)) modifiers.Insert(id, this);
        }

        public GlyphModifier SetMinionAI(Action<ProceduralMinion> minionAI)
        {
            this.minionAI = minionAI;
            return this;
        }

        public GlyphModifier SetImpact(Action<ProceduralSpellProj, NPC, int> impact)
        {
            this.impact = impact;
            return this;
        }

        public GlyphModifier SetInit(Action<ProceduralSpellProj> init)
        {
            this.init = init;
            return this;
        }

        public GlyphModifier SetDraw(Action<ProceduralSpellProj, SpriteBatch, Color> draw)
        {
            this.draw = draw;
            return this;
        }

        public GlyphModifier DefineGroup(Func<GlyphModifier> group)
        {
            this.group = group;
            return this;
        }

        public static void Initialize(Mod mod)
        {
            modifiers = new List<GlyphModifier>();

            attack = new GlyphModifier(0, "Attack", glyph => glyph.minion, () => Main.rand.Next(2) == 0, 0.6f).SetMinionAI(delegate (ProceduralMinion minion)
            {
                if (minion.projectile.timeLeft % 50 != 0)
                    return;
                
                Projectile proj = minion.projectile;
                NPC target = minion.GetTarget();
                Vector2 unitVelocity = (target.Center - proj.Center);
                if (!minion.attack) return;
                if (Vector2.Distance(minion.projectile.Center, target.Center) < 400f)
                unitVelocity.Normalize();
                Vector2 velocity = unitVelocity * 5f;
                ProceduralSpellProj spell = minion.source.CreateProjectile(Main.player[minion.projectile.owner], velocity, 0, proj.Center, proj);
                spell.projectile.minion = true;
                Moon moon = (Moon)minion.source.glyphs[(int)GLYPHTYPE.MOON].modItem;
                spell.ai.Remove(moon.GetAIAction());
            });
            smallProt = new GlyphModifier(1, "Orbiting fire", glyph => glyph.minion, () => Main.rand.Next(4) == 0, 0.95f).SetMinionAI(delegate (ProceduralMinion minion)
            {
                if (minion.smallProt != null || minion.projectile.timeLeft % 180 != 0)
                    if (minion.smallProt.projectile.active)
                        return;
                    else
                        minion.smallProt.projectile.Kill();

                Projectile m = minion.projectile;
                Player player = Main.player[m.owner];
                Projectile projectile = Main.projectile[Projectile.NewProjectile(m.Center, new Vector2(0f, -1.5f), ModContent.ProjectileType<ProceduralSpellProj>(), minion.source.ProjectileDamage(player.GetModPlayer<PlayerCharacter>()), 3f, m.owner)];
                projectile.minion = true;
                ProceduralSpellProj ps = (ProceduralSpellProj)projectile.modProjectile;
                ps.origin = projectile.position;
                Cross cross = new Cross_Red();
                ps.ai.Add(delegate (ProceduralSpellProj spell)
                {
                    cross.GetAIAction()(spell);

                    int rotDistance = 40;
                    int rotTimeLeft = 3600;
                    float displacementAngle = (float)API.Tau / 4f;
                    Vector2 displacementVelocity = Vector2.Zero;
                    if (rotTimeLeft - spell.projectile.timeLeft >= rotDistance * 2 / 3)
                    {
                        Vector2 unitRelativePos = spell.RelativePos(spell.caster.Center);
                        unitRelativePos.Normalize();
                        spell.projectile.Center = spell.caster.Center + unitRelativePos * rotDistance;
                        displacementVelocity = new Vector2(-2f, 0f).RotatedBy((spell.RelativePos(spell.caster.Center)).ToRotation() + (float)API.Tau / 4f);

                        float angle = displacementAngle - 0.06f * (float)(rotTimeLeft - spell.projectile.timeLeft - rotDistance * 2 / 3);
                        spell.projectile.Center = spell.caster.Center + new Vector2(0f, -rotDistance).RotatedBy(angle);
                    }
                    else
                    {
                        spell.projectile.Center = spell.caster.Center + new Vector2(0f, -1.5f).RotatedBy(displacementAngle) * (rotTimeLeft - spell.projectile.timeLeft);
                    }
                    spell.projectile.velocity = displacementVelocity + spell.caster.velocity;
                    spell.basePosition = spell.caster.position;
                });
                ps.init.Add(delegate (ProceduralSpellProj spell)
                {
                    cross.GetInitAction()(spell);
                    spell.projectile.scale = 0.9f;
                    ProceduralSpellProj.AI_RotateToVelocity(spell);
                    if (Main.rand.NextFloat(0f, 1.5f) <= spell.alpha)
                    {
                        int dust = Dust.NewDust(spell.projectile.position, spell.projectile.width, spell.projectile.height, DustID.Fire, spell.projectile.velocity.X * 0.2f, spell.projectile.velocity.Y * 0.2f, 63, Color.White, 1f + spell.alpha * 2f);
                        Main.dust[dust].noGravity = true;
                    }
                });
                ps.impact.Add(cross.GetImpactAction());
                ps.kill.Add(cross.GetKillAction());
                ps.caster = m;
                ps.Initialize();
                minion.smallProt = ps;
            });
            group_impactEffects = new GlyphModifier(2, "", glyph => glyph is Cross, () => Main.rand.Next(2) == 0).DefineGroup(delegate
            {
                switch(Main.rand.Next(2))
                {
                    default:
                        return smokePellets;
                    case 1:
                        return explosions;
                }
            });
            smokePellets = new GlyphModifier(3, "Smoke Pellets", glyph => false, () => false, 0.9f).SetImpact(delegate (ProceduralSpellProj spell, NPC npc, int damage)
            {
                Main.PlaySound(new LegacySoundStyle(2, 14, Terraria.Audio.SoundType.Sound).WithVolume(0.5f), spell.projectile.Center);
                Projectile proj = Main.projectile[Projectile.NewProjectile(npc.Center - new Vector2(24, 48), Vector2.Zero, ModContent.ProjectileType<SmokePellets>(), 2, 0f, spell.projectile.owner)];
                proj.minion = true;
            });
            explosions = new GlyphModifier(4, "Explosive", glyph => false, () => false, 0.85f).SetImpact(delegate (ProceduralSpellProj spell, NPC npc, int damage)
            {
                Main.PlaySound(new LegacySoundStyle(2, 14, Terraria.Audio.SoundType.Sound).WithVolume(0.5f), spell.projectile.Center);
                Projectile proj = Main.projectile[Projectile.NewProjectile(npc.Center - new Vector2(16, 32), Vector2.Zero, ModContent.ProjectileType<Explosion>(), spell.projectile.damage, 0f, spell.projectile.owner)];
                proj.minion = true;
            });
            vanish = new GlyphModifier(5, "Discord", glyph => glyph is Star, () => Main.rand.Next(3) == 0, 1.2f, 1f);
            crosschains = new GlyphModifier(6, "", glyph => glyph is Cross, () => Main.rand.Next(3) == 0).DefineGroup(delegate
            {
                switch(Main.rand.Next(2))
                {
                    default:
                        return lifeleech;
                    case 1:
                        return manaleech;
                }
            });
            lifeleech = new GlyphModifier(7, "Leeches life", glyph => false, () => false, 0.9f, 1.2f).SetImpact(delegate (ProceduralSpellProj spell, NPC npc, int damage)
            {
                if (Main.rand.Next(3) != 0)
                    return;
                PlayerCharacter character = Main.player[spell.projectile.owner].GetModPlayer<PlayerCharacter>();
                float distance = Vector2.Distance(npc.Center, character.player.Center);
                int count = (int)(distance / 20);
                Trail trail = new Trail(npc.Center, 60, delegate (SpriteBatch spriteBatch, Player player, Vector2 end, Vector2[] displacement, float scale)
                {
                    for (int i = 0; i < count; i += 1)
                    {
                        Vector2 position = (npc.position - player.Center) * i / count + player.Center;
                        spriteBatch.Draw(GFX.heart, position - Main.screenPosition + displacement[i], null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    }
                });
                trail.displacement = new Vector2[count];
                for (int i = 0; i < count; i += 1)
                    trail.displacement[i] = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
                character.trails.Add(trail);
                int heal = damage / 5;
                if (heal < 1) return;
                character.player.HealEffect(heal);
                character.player.statLife += heal;
            });
            manaleech = new GlyphModifier(8, "Steals mana", glyph => false, () => false, 0.8f).SetImpact(delegate (ProceduralSpellProj spell, NPC npc, int damage)
            {
                if (Main.rand.Next(2) != 0)
                    return;
                PlayerCharacter character = Main.player[spell.projectile.owner].GetModPlayer<PlayerCharacter>();
                float distance = Vector2.Distance(npc.Center, character.player.Center);
                int count = (int)(distance / 20);
                Trail trail = new Trail(npc.Center, 60, delegate (SpriteBatch spriteBatch, Player player, Vector2 end, Vector2[] displacement, float scale)
                {
                    for (int i = 0; i < count; i += 1)
                    {
                        Vector2 position = (npc.position - player.Center) * i / count + player.Center;
                        spriteBatch.Draw(GFX.star, position - Main.screenPosition + displacement[i], null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    }
                });
                trail.displacement = new Vector2[count];
                for (int i = 0; i < count; i += 1)
                    trail.displacement[i] = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
                character.trails.Add(trail);
                int mana = damage / 6;
                if (mana < 1) return;
                character.player.ManaEffect(mana);
                character.mana += mana;
                character.player.statMana += mana;
            });
            thornchains = new GlyphModifier(9, "Thorny Chains", glyph => glyph is Moon && !(glyph is Moon_Violet), () => Main.rand.Next(4) == 0, 0.9f).SetDraw(delegate (ProceduralSpellProj spell, SpriteBatch spriteBatch, Color color)
            {
                Projectile proj = spell.projectile;
                Player player = Main.player[proj.owner];
                Entity caster = spell.caster;
                List<NPC> npcs = new List<NPC>();
                for (int i = 0; i < Main.npc.Length; i += 1)
                    if (new Rectangle((int)Math.Min(caster.Center.X, proj.Center.X), (int)Math.Min(caster.Center.Y, proj.Center.Y), (int)Math.Abs(proj.Center.X - caster.Center.X), (int)Math.Abs(proj.Center.Y - caster.Center.Y)).Intersects(new Rectangle((int)Main.npc[i].position.X, (int)Main.npc[i].position.Y, Main.npc[i].width, Main.npc[i].height)))
                        npcs.Add(Main.npc[i]);
                Vector2 relativePos = proj.Center - caster.Center;
                if (relativePos.Length() > 480f) return;
                int count = (int)(relativePos.Length() / 24);
                for (int i = 0; i < count; i += 1)
                {
                    Vector2 chainpos = caster.Center + relativePos * (i + 0.5f) / count;
                    Rectangle bounds = new Rectangle((int)Math.Round(chainpos.X) - 12, (int)Math.Round(chainpos.Y) - 12, 24, 24);
                    foreach (NPC npc in npcs)
                        if (npc.active)
                            if (bounds.Intersects(new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height)) && !npc.friendly && !npc.townNPC)
                            {
                                kNPC kn = npc.GetGlobalNPC<kNPC>();
                                bool canHit = true;
                                if (kn.immuneTime > 0) canHit = false;
                                if (kn.invincibilityTime.ContainsKey(spell.source))
                                    if (kn.invincibilityTime[spell.source] > 0)
                                        canHit = false;
                                if (canHit)
                                {
                                    player.ApplyDamageToNPC(npc, proj.damage, 0f, 0, false);
                                    npc.AddBuff(BuffID.Poisoned, proj.damage + 60);
                                    kn.invincibilityTime[spell.source] = 30;
                                }
                            }
                    spriteBatch.Draw(GFX.thornChain, chainpos - Main.screenPosition, null, spell.lighted ? Color.White : color, relativePos.ToRotation() - (float)API.Tau / 4f, new Vector2(10f, 16f), 1f, SpriteEffects.None, 0.1f);
                }
            });
            pierce = new GlyphModifier(10, "Pierces two additional enemies", glyph => glyph is Moon, () => Main.rand.Next(3) == 0, 0.8f, 1.1f).SetInit(delegate (ProceduralSpellProj spell)
            {
                if (spell.projectile.penetrate > 0) spell.projectile.penetrate += 2;
            });
            bounce = new GlyphModifier(11, "Bouncing", glyph => glyph is Moon_Blue || glyph is Moon_Purple, () => Main.rand.Next(7) < 2, 0.8f, 1f).SetInit(delegate (ProceduralSpellProj spell)
            {
                spell.projectile.tileCollide = true;
                spell.bounce = true;
            });
        }
    }
}
