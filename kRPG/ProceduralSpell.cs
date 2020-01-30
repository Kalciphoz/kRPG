using System;
using System.Collections.Generic;
using System.Linq;
using kRPG.Enums;
using kRPG.GUI;
using kRPG.Items.Glyphs;
using kRPG.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG
{
    public class ProceduralSpell
    {
        private Mod mod;

        public Keys key = Keys.A;

        public int cooldown = 120;
        public int remaining = 0;

        public List<ProceduralSpellProj> circlingProtection = new List<ProceduralSpellProj>();
        public bool minion => !(glyphs[(byte) GLYPHTYPE.STAR].modItem is Star_Blue);

        public void Draw(SpriteBatch spriteBatch, Vector2 position, float scale)
        {
            try
            {
                bool full = true;
                for (int i = 0; i < glyphs.Length; i += 1)
                    if (glyphs[i].type == 0)
                        full = false;

                if (remaining > 0)
                    remaining -= 1;

                var bounds = new Rectangle((int) position.X, (int) position.Y, (int) (GFX.skillSlot.Width * scale), (int) (GFX.skillSlot.Height * scale));
                var character = Main.LocalPlayer.GetModPlayer<PlayerCharacter>();

                if (spriteBatch == null || character == null) return;

                if (bounds.Contains(new Point(Main.mouseX, Main.mouseY)))
                {
                    Main.LocalPlayer.mouseInterface = true;
                    if (Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        Main.PlaySound(SoundID.MenuTick);
                        character.CloseGuIs();
                        character.selectedAbility = this;
                    }
                }

                if (character.selectedAbility != null)
                {
                    if (character.selectedAbility.Equals(this))
                    {
                        spriteBatch.Draw(GFX.selectedSkillSlot, position, Color.White, scale);
                        for (var k = Keys.A; k <= Keys.Z; k += 1)
                            if (Main.keyState.IsKeyDown(k) && (Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift)))
                            {
                                for (int i = 0; i < character.abilities.Length; i += 1)
                                    if (character.abilities[i] != character.selectedAbility && character.abilities[i].key == k)
                                        character.abilities[i].key = key;

                                key = k;
                            }
                    }
                    else
                    {
                        spriteBatch.Draw(GFX.skillSlot, position, Color.White, scale);
                    }
                }
                else
                {
                    spriteBatch.Draw(GFX.skillSlot, position, Color.White, scale);
                }

                if (full && remaining == 0 && character.mana >= ManaCost(character)) spriteBatch.Draw(GFX.gothicLetter[key], position, Color.White, scale);
                else if (full || character.spellcraftingGUI.guiActive) spriteBatch.Draw(GFX.gothicLetter[key], position, new Color(143, 143, 151), scale);
            }
            catch (Exception e)
            {
                ModLoader.GetMod("kRPG").Logger.InfoFormat(e.ToString());
            }
        }

        public Item[] glyphs = new Item[3];

        public List<GlyphModifier> modifiers
        {
            get
            {
                if (modifierOverride != null) return modifierOverride;
                var list = new List<GlyphModifier>();
                for (int i = 0; i < glyphs.Length; i += 1)
                {
                    var glyph = (Glyph) glyphs[i].modItem;
                    for (int j = 0; j < glyph.modifiers.Count; j += 1)
                        list.Add(glyph.modifiers[j]);
                }

                return list;
            }
        }

        public List<GlyphModifier> modifierOverride = null;
        public int projCount => projCountOverride == -1 ? ((Moon) glyphs[(byte) GLYPHTYPE.MOON].modItem).projCount : projCountOverride;
        public int projCountOverride = -1;

        public ProceduralSpell(Mod mod)
        {
            this.mod = mod;
            for (int i = 0; i < glyphs.Length; i += 1)
                glyphs[i] = new Item();
        }

        public Action<ProceduralSpell, Player, Vector2> useAction
        {
            get
            {
                var glyph = (Glyph) glyphs[(byte) GLYPHTYPE.STAR].modItem;
                return glyph.GetUseAbility();
            }
        }

        public void UseAbility(Player player, Vector2 target)
        {
            bool vanish = modifiers.Contains(GlyphModifier.vanish);
            var oldCenter = player.Center;
            if (vanish)
            {
                Main.PlaySound(new LegacySoundStyle(2, 14).WithVolume(0.5f), oldCenter);
                Projectile.NewProjectile(oldCenter - new Vector2(24, 48), Vector2.Zero, ModContent.ProjectileType<SmokePellets>(), 2, 0f, player.whoAmI);
                Vector2 vector32;
                vector32.X = Main.mouseX + Main.screenPosition.X;
                if (Math.Abs(player.gravDir - 1f) < .01)
                    vector32.Y = Main.mouseY + Main.screenPosition.Y - player.height;
                else
                    vector32.Y = Main.screenPosition.Y + Main.screenHeight - Main.mouseY;
                vector32.X -= (float) (player.width / 2.0);
                if (vector32.X > 50f && vector32.X < Main.maxTilesX * 16 - 50 && vector32.Y > 50f && vector32.Y < Main.maxTilesY * 16 - 50)
                {
                    int num276 = (int) (vector32.X / 16f);
                    int num277 = (int) (vector32.Y / 16f);
                    if ((Main.tile[num276, num277].wall != 87 || num277 <= Main.worldSurface || NPC.downedPlantBoss) &&
                        !Collision.SolidCollision(vector32, player.width, player.height))
                    {
                        player.Teleport(vector32, 1);
                        NetMessage.SendData(65, -1, -1, null, 0, player.whoAmI, vector32.X, vector32.Y, 1);
                        if (player.chaosState)
                        {
                            player.statLife -= player.statLifeMax2 / 7;
                            var damageSource = PlayerDeathReason.ByOther(13);
                            if (Main.rand.Next(2) == 0)
                                damageSource = PlayerDeathReason.ByOther(player.Male ? 14 : 15);
                            if (player.statLife <= 0)
                                player.KillMe(damageSource, 1.0, 0);
                            player.lifeRegenCount = 0;
                            player.lifeRegenTime = 0;
                        }

                        player.AddBuff(88, 360);
                    }
                }
            }

            useAction(this, player, vanish ? oldCenter : target);
        }

        public Action<ProceduralSpell, Player, Vector2, Vector2, Entity> castAction
        {
            get
            {
                var glyph = (Glyph) glyphs[(byte) GLYPHTYPE.MOON].modItem;
                return glyph.GetCastAction();
            }
        }

        public void CastSpell(Player player, Vector2 origin, Vector2 target, Entity caster)
        {
            Main.PlaySound(SoundID.Item8, caster.position);
            castAction(this, player, origin, target, caster);
        }

        //angle from 0f to 1f
        public ProceduralSpellProj CreateProjectile(Player player, Vector2 velocity, float angle = 0f, Vector2? position = null, Entity caster = null)
        {
            if (caster == null) caster = player;
            var projectile =
                Main.projectile[
                    Projectile.NewProjectile(position ?? caster.Center, velocity.RotatedBy(API.Tau * angle),
                        ModContent.ProjectileType<ProceduralSpellProj>(), ProjectileDamage(player.GetModPlayer<PlayerCharacter>()), 3f, player.whoAmI)];
            var ps = (ProceduralSpellProj) projectile.modProjectile;
            ps.origin = projectile.position;
            foreach (var item in glyphs)
            {
                var glyph = (Glyph) item.modItem;
                if (glyph.GetAIAction() != null)
                    ps.ai.Add(glyph.GetAIAction());
                if (glyph.GetInitAction() != null)
                    ps.init.Add(glyph.GetInitAction());
                if (glyph.GetImpactAction() != null)
                    ps.impact.Add(glyph.GetImpactAction());
                if (glyph.GetKillAction() != null)
                    ps.kill.Add(glyph.GetKillAction());
            }

            foreach (var modifier in modifiers)
            {
                if (modifier.impact != null)
                    ps.impact.Add(modifier.impact);
                if (modifier.draw != null)
                    ps.draw.Add(modifier.draw);
                if (modifier.init != null)
                    ps.init.Add(modifier.init);
            }

            ps.caster = caster;
            ps.projectile.minion = minion;
            /*if (minion)
            {
                ps.projectile.melee = false;
                ps.projectile.ranged = false;
                ps.projectile.magic = false;
            }*/
            ps.source = this;
            ps.Initialize();
            if (Main.netMode != 1) return ps;
            var packet = mod.GetPacket();
            packet.Write((byte) Message.CreateProjectile);
            packet.Write(player.whoAmI);
            packet.Write(ps.projectile.whoAmI);
            packet.Write(glyphs[(byte) GLYPHTYPE.STAR].type);
            packet.Write(glyphs[(byte) GLYPHTYPE.CROSS].type);
            packet.Write(glyphs[(byte) GLYPHTYPE.MOON].type);
            packet.Write(ps.projectile.damage);
            packet.Write(minion);
            packet.Write(caster.whoAmI);
            var mods = modifiers;
            packet.Write(mods.Count);
            for (int j = 0; j < mods.Count; j += 1)
                packet.Write(mods[j].id);
            packet.Send();
            return ps;
        }

        public bool CompleteSkill()
        {
            bool complete = true;
            for (int i = 0; i < glyphs.Length; i += 1)
                if (glyphs[i].type == 0)
                    complete = false;

            return complete;
        }

        public int ProjectileDamage(PlayerCharacter character)
        {
            int constant = Main.hardMode ? character.level / 3 : 5;
            float multiplier = Main.expertMode ? 1f : 0.65f;
            multiplier = glyphs.Aggregate(multiplier, (current, item) => current * ((Glyph) item.modItem).DamageModifier());
            return (int) Math.Round(Math.Pow(1.04, Math.Min(130, character.level)) * 9f * multiplier) + constant;
        }

        // ReSharper disable once IdentifierTypo
        public int ManaCost(PlayerCharacter character)
        {
            float multiplier = glyphs.Aggregate(0.4f, (current, item) => current * ((Glyph) item.modItem).ManaModifier());
            return (int) Math.Round((20 + character.level) * multiplier);
        }
    }

    public class SpellEffect
    {
        private readonly ProceduralSpell ability;
        private Vector2 target;
        private int timeLeft;
        private readonly Action<ProceduralSpell, int> update;

        public SpellEffect(ProceduralSpell ability, Vector2 target, int timeLeft, Action<ProceduralSpell, int> update)
        {
            this.ability = ability;
            this.target = target;
            this.timeLeft = timeLeft;
            this.update = update;
            Main.player[Main.myPlayer].GetModPlayer<PlayerCharacter>().spellEffects.Add(this);
        }

        public void Update(PlayerCharacter character)
        {
            update(ability, timeLeft);
            timeLeft -= 1;
            if (timeLeft <= 0)
                character.spellEffects.Remove(this);
        }
    }
}