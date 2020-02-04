//  Fairfield Tek L.L.C.
//  Copyright (c) 2016, Fairfield Tek L.L.C.
// 
// 
// THIS SOFTWARE IS PROVIDED BY FairfieldTek LLC ''AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL FAIRFIELDTEK LLC BE LIABLE FOR ANY DIRECT, INDIRECT,
// INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
// OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
// DAMAGE.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using kRPG.Enums;
using kRPG.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.Items.Glyphs
{
    public class GlyphModifier
    {
        public GlyphModifier(int id, string tooltip, Func<Glyph, bool> match, Func<bool> odds, float damageModifier = 1f, float manaModifier = 1f)
        {
            Id = id;
            Tooltip = tooltip;
            Match = match;
            Odds = odds;
            DamageModifier = damageModifier;
            ManaModifier = manaModifier;
            if (!Modifiers.Contains(this)) Modifiers.Insert(id, this);
        }

        public static GlyphModifier Attack { get; set; }
        public static GlyphModifier Bounce { get; set; }
        public static GlyphModifier CrossChains { get; set; }
        public float DamageModifier { get; set; }
        public Action<ProceduralSpellProj, SpriteBatch, Color> Draw { get; set; }
        public static GlyphModifier Explosions { get; set; }

        public Func<GlyphModifier> Group { get; set; }

        public static GlyphModifier GroupImpactEffects { get; set; }

        public int Id { get; set; }
        public Action<ProceduralSpellProj, NPC, int> Impact { get; set; }
        public Action<ProceduralSpellProj> Init { get; set; }
        public static GlyphModifier LifeLeech { get; set; }
        public static GlyphModifier ManaLeech { get; set; }
        public float ManaModifier { get; set; }
        public Func<Glyph, bool> Match { get; set; }
        public Action<ProceduralMinion> MinionAi { get; set; }

        public static List<GlyphModifier> Modifiers { get; set; }
        public Func<bool> Odds { get; set; }
        public static GlyphModifier Pierce { get; set; }
        public static GlyphModifier SmallProt { get; set; }
        public static GlyphModifier SmokePellets { get; set; }
        public static GlyphModifier ThornChains { get; set; }
        public string Tooltip { get; set; }
        public static GlyphModifier Vanish { get; set; }

        public GlyphModifier DefineGroup(Func<GlyphModifier> group)
        {
            Group = group;
            return this;
        }

        public static void Initialize(Mod mod)
        {
            Modifiers = new List<GlyphModifier>();

            Attack = new GlyphModifier(0, "Attack", glyph => glyph.Minion, () => Main.rand.Next(2) == 0, 0.6f).SetMinionAi(delegate(ProceduralMinion minion)
            {
                if (minion.projectile.timeLeft % 50 != 0)
                    return;

                Projectile proj = minion.projectile;
                NPC target = minion.GetTarget();
                Vector2 unitVelocity = target.Center - proj.Center;
                if (!minion.Attack) return;
                if (Vector2.Distance(minion.projectile.Center, target.Center) < 400f)
                    unitVelocity.Normalize();
                Vector2 velocity = unitVelocity * 5f;
                ProceduralSpellProj spell = minion.Source.CreateProjectile(Main.player[minion.projectile.owner], velocity, 0, proj.Center, proj);
                spell.projectile.minion = true;
                Moon moon = (Moon) minion.Source.Glyphs[(int) GLYPHTYPE.MOON].modItem;
                spell.ai.Remove(moon.GetAiAction());
            });
            SmallProt = new GlyphModifier(1, "Orbiting fire", glyph => glyph.Minion, () => Main.rand.Next(4) == 0, 0.95f).SetMinionAi(
                delegate(ProceduralMinion minion)
                {
                    if (minion.SmallProt != null || minion.projectile.timeLeft % 180 != 0)
                        // ReSharper disable once PossibleNullReferenceException
                        if (minion.SmallProt.projectile.active)
                            return;
                        else
                            minion.SmallProt.projectile.Kill();

                    Projectile m = minion.projectile;
                    Player player = Main.player[m.owner];
                    Projectile projectile =
                        Main.projectile[
                            Projectile.NewProjectile(m.Center, new Vector2(0f, -1.5f), ModContent.ProjectileType<ProceduralSpellProj>(),
                                minion.Source.ProjectileDamage(player.GetModPlayer<PlayerCharacter>()), 3f, m.owner)];
                    projectile.minion = true;
                    ProceduralSpellProj ps = (ProceduralSpellProj) projectile.modProjectile;
                    ps.Origin = projectile.position;
                    Cross cross = new Cross_Red();
                    ps.ai.Add(delegate(ProceduralSpellProj spell)
                    {
                        cross.GetAiAction()(spell);

                        int rotDistance = 40;
                        int rotTimeLeft = 3600;
                        float displacementAngle = (float) API.Tau / 4f;
                        Vector2 displacementVelocity = Vector2.Zero;
                        if (rotTimeLeft - spell.projectile.timeLeft >= rotDistance * 2 / 3)
                        {
                            Vector2 unitRelativePos = spell.RelativePos(spell.Caster.Center);
                            unitRelativePos.Normalize();
                            spell.projectile.Center = spell.Caster.Center + unitRelativePos * rotDistance;
                            displacementVelocity = new Vector2(-2f, 0f).RotatedBy(spell.RelativePos(spell.Caster.Center).ToRotation() + (float) API.Tau / 4f);

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
                    ps.Inits.Add(delegate(ProceduralSpellProj spell)
                    {
                        cross.GetInitAction()(spell);
                        spell.projectile.scale = 0.9f;
                        ProceduralSpellProj.aiRotateToVelocity(spell);
                        if (!(Main.rand.NextFloat(0f, 1.5f) <= spell.Alpha))
                            return;
                        int dust = Dust.NewDust(spell.projectile.position, spell.projectile.width, spell.projectile.height, DustID.Fire,
                            spell.projectile.velocity.X * 0.2f, spell.projectile.velocity.Y * 0.2f, 63, Color.White, 1f + spell.Alpha * 2f);
                        Main.dust[dust].noGravity = true;
                    });
                    ps.Impacts.Add(cross.GetImpactAction());
                    ps.Kills.Add(cross.GetKillAction());
                    ps.Caster = m;
                    ps.Initialize();
                    minion.SmallProt = ps;
                });
            GroupImpactEffects = new GlyphModifier(2, "", glyph => glyph is Cross, () => Main.rand.Next(2) == 0).DefineGroup(delegate
            {
                switch (Main.rand.Next(2))
                {
                    default:
                        return SmokePellets;
                    case 1:
                        return Explosions;
                }
            });
            SmokePellets = new GlyphModifier(3, "Smoke Pellets", glyph => false, () => false, 0.9f).SetImpact(
                delegate(ProceduralSpellProj spell, NPC npc, int damage)
                {
                    Main.PlaySound(new LegacySoundStyle(2, 14).WithVolume(0.5f), spell.projectile.Center);
                    Projectile proj = Main.projectile[
                        Projectile.NewProjectile(npc.Center - new Vector2(24, 48), Vector2.Zero, ModContent.ProjectileType<SmokePellets>(), 2, 0f,
                            spell.projectile.owner)];
                    proj.minion = true;
                });
            Explosions = new GlyphModifier(4, "Explosive", glyph => false, () => false, 0.85f).SetImpact(
                delegate(ProceduralSpellProj spell, NPC npc, int damage)
                {
                    Main.PlaySound(new LegacySoundStyle(2, 14).WithVolume(0.5f), spell.projectile.Center);
                    Projectile proj = Main.projectile[
                        Projectile.NewProjectile(npc.Center - new Vector2(16, 32), Vector2.Zero, ModContent.ProjectileType<Explosion>(),
                            spell.projectile.damage, 0f, spell.projectile.owner)];
                    proj.minion = true;
                });
            Vanish = new GlyphModifier(5, "Discord", glyph => glyph is Star, () => Main.rand.Next(3) == 0, 1.2f);
            CrossChains = new GlyphModifier(6, "", glyph => glyph is Cross, () => Main.rand.Next(3) == 0).DefineGroup(delegate
            {
                switch (Main.rand.Next(2))
                {
                    default:
                        return LifeLeech;
                    case 1:
                        return ManaLeech;
                }
            });
            LifeLeech = new GlyphModifier(7, "Leeches life", glyph => false, () => false, 0.9f, 1.2f).SetImpact(
                delegate(ProceduralSpellProj spell, NPC npc, int damage)
                {
                    if (Main.rand.Next(3) != 0)
                        return;
                    PlayerCharacter character = Main.player[spell.projectile.owner].GetModPlayer<PlayerCharacter>();
                    float distance = Vector2.Distance(npc.Center, character.player.Center);
                    int count = (int) (distance / 20);
                    Trail trail = new Trail(npc.Center, 60, delegate(SpriteBatch spriteBatch, Player player, Vector2 end, Vector2[] displacement, float scale)
                    {
                        for (int i = 0; i < count; i += 1)
                        {
                            Vector2 position = (npc.position - player.Center) * i / count + player.Center;
                            spriteBatch.Draw(GFX.Heart, position - Main.screenPosition + displacement[i], null, Color.White, 0f, Vector2.Zero, scale,
                                SpriteEffects.None, 0f);
                        }
                    }) {Displacement = new Vector2[count]};
                    for (int i = 0; i < count; i += 1)
                        trail.Displacement[i] = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
                    character.Trails.Add(trail);
                    int heal = damage / 5;
                    if (heal < 1) return;
                    character.player.HealEffect(heal);
                    character.player.statLife += heal;
                });
            ManaLeech = new GlyphModifier(8, "Steals mana", glyph => false, () => false, 0.8f).SetImpact(
                delegate(ProceduralSpellProj spell, NPC npc, int damage)
                {
                    if (Main.rand.Next(2) != 0)
                        return;
                    PlayerCharacter character = Main.player[spell.projectile.owner].GetModPlayer<PlayerCharacter>();
                    float distance = Vector2.Distance(npc.Center, character.player.Center);
                    int count = (int) (distance / 20);
                    Trail trail = new Trail(npc.Center, 60, delegate(SpriteBatch spriteBatch, Player player, Vector2 end, Vector2[] displacement, float scale)
                    {
                        for (int i = 0; i < count; i += 1)
                        {
                            Vector2 position = (npc.position - player.Center) * i / count + player.Center;
                            spriteBatch.Draw(GFX.Star, position - Main.screenPosition + displacement[i], null, Color.White, 0f, Vector2.Zero, scale,
                                SpriteEffects.None, 0f);
                        }
                    }) {Displacement = new Vector2[count]};
                    for (int i = 0; i < count; i += 1)
                        trail.Displacement[i] = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
                    character.Trails.Add(trail);
                    int mana = damage / 6;
                    if (mana < 1) return;
                    character.player.ManaEffect(mana);
                    character.Mana += mana;
                    character.player.statMana += mana;
                });
            ThornChains = new GlyphModifier(9, "Thorny Chains", glyph => glyph is Moon && !(glyph is Moon_Violet), () => Main.rand.Next(4) == 0, 0.9f).SetDraw(
                delegate(ProceduralSpellProj spell, SpriteBatch spriteBatch, Color color)
                {
                    Projectile proj = spell.projectile;
                    Player player = Main.player[proj.owner];
                    Entity caster = spell.Caster;
                    List<NPC> npcs = new List<NPC>();
                    for (int i = 0; i < Main.npc.Length; i += 1)
                        if (new Rectangle((int) Math.Min(caster.Center.X, proj.Center.X), (int) Math.Min(caster.Center.Y, proj.Center.Y),
                                (int) Math.Abs(proj.Center.X - caster.Center.X), (int) Math.Abs(proj.Center.Y - caster.Center.Y))
                            .Intersects(new Rectangle((int) Main.npc[i].position.X, (int) Main.npc[i].position.Y, Main.npc[i].width, Main.npc[i].height)))
                            npcs.Add(Main.npc[i]);
                    Vector2 relativePos = proj.Center - caster.Center;
                    if (relativePos.Length() > 480f) return;
                    int count = (int) (relativePos.Length() / 24);
                    for (int i = 0; i < count; i += 1)
                    {
                        Vector2 chainpos = caster.Center + relativePos * (i + 0.5f) / count;
                        Rectangle bounds = new Rectangle((int) Math.Round(chainpos.X) - 12, (int) Math.Round(chainpos.Y) - 12, 24, 24);
                        foreach (NPC npc in npcs)
                            if (npc.active)
                                if (bounds.Intersects(new Rectangle((int) npc.position.X, (int) npc.position.Y, npc.width, npc.height)) && !npc.friendly &&
                                    !npc.townNPC)
                                {
                                    kNPC kn = npc.GetGlobalNPC<kNPC>();
                                    bool canHit = !(kn.ImmuneTime > 0);
                                    if (kn.InvincibilityTime.ContainsKey(spell.Source))
                                        if (kn.InvincibilityTime[spell.Source] > 0)
                                            canHit = false;
                                    if (!canHit)
                                        continue;
                                    player.ApplyDamageToNPC(npc, proj.damage, 0f, 0, false);
                                    npc.AddBuff(BuffID.Poisoned, proj.damage + 60);
                                    kn.InvincibilityTime[spell.Source] = 30;
                                }

                        spriteBatch.Draw(GFX.ThornChain, chainpos - Main.screenPosition, null, spell.Lighted ? Color.White : color,
                            relativePos.ToRotation() - (float) API.Tau / 4f, new Vector2(10f, 16f), 1f, SpriteEffects.None, 0.1f);
                    }
                });
            Pierce = new GlyphModifier(10, "Pierces two additional enemies", glyph => glyph is Moon, () => Main.rand.Next(3) == 0, 0.8f, 1.1f).SetInit(
                delegate(ProceduralSpellProj spell)
                {
                    if (spell.projectile.penetrate > 0) spell.projectile.penetrate += 2;
                });
            Bounce = new GlyphModifier(11, "Bouncing", glyph => glyph is Moon_Blue || glyph is Moon_Purple, () => Main.rand.Next(7) < 2, 0.8f).SetInit(
                delegate(ProceduralSpellProj spell)
                {
                    spell.projectile.tileCollide = true;
                    spell.Bounce = true;
                });
        }

        public GlyphModifier SetDraw(Action<ProceduralSpellProj, SpriteBatch, Color> draw)
        {
            Draw = draw;
            return this;
        }

        public GlyphModifier SetImpact(Action<ProceduralSpellProj, NPC, int> impact)
        {
            Impact = impact;
            return this;
        }

        public GlyphModifier SetInit(Action<ProceduralSpellProj> init)
        {
            Init = init;
            return this;
        }

        public GlyphModifier SetMinionAi(Action<ProceduralMinion> minionAi)
        {
            MinionAi = minionAi;
            return this;
        }
    }
}