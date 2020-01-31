using System;
using System.Collections.Generic;
using System.Linq;
using kRPG2.Enums;
using kRPG2.Items.Glyphs;
using kRPG2.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG2
{
    public class ProceduralSpell
    {
        public List<ProceduralSpellProj> CirclingProtection { get; set; }= new List<ProceduralSpellProj>();

        public int Cooldown { get; set; } = 120;

        public Item[] Glyphs { get; set; } = new Item[3];

        public Keys Key { get; set; } = Keys.A;
        private  Mod Mod { get; }

        public List<GlyphModifier> ModifierOverride { get; set; } = null;
        public int ProjCountOverride { get; set; } = -1;
        public int Remaining { get; set; }

        public ProceduralSpell(Mod mod)
        {
            this.Mod = mod;
            for (int i = 0; i < Glyphs.Length; i += 1)
                Glyphs[i] = new Item();
        }

        public Action<ProceduralSpell, Player, Vector2, Vector2, Entity> castAction
        {
            get
            {
                Glyph glyph = (Glyph) Glyphs[(byte) GLYPHTYPE.MOON].modItem;
                return glyph.GetCastAction();
            }
        }

        public bool Minion => !(Glyphs[(byte) GLYPHTYPE.STAR].modItem is Star_Blue);

        public List<GlyphModifier> Modifiers
        {
            get
            {
                if (ModifierOverride != null) return ModifierOverride;
                List<GlyphModifier> list = new List<GlyphModifier>();
                for (int i = 0; i < Glyphs.Length; i += 1)
                {
                    Glyph glyph = (Glyph) Glyphs[i].modItem;
                    for (int j = 0; j < glyph.Modifiers.Count; j += 1)
                        list.Add(glyph.Modifiers[j]);
                }

                return list;
            }
        }

        public int ProjCount => ProjCountOverride == -1 ? ((Moon) Glyphs[(byte) GLYPHTYPE.MOON].modItem).ProjCount : ProjCountOverride;

        public Action<ProceduralSpell, Player, Vector2> UseAction
        {
            get
            {
                Glyph glyph = (Glyph) Glyphs[(byte) GLYPHTYPE.STAR].modItem;
                return glyph.GetUseAbility();
            }
        }

        public void CastSpell(Player player, Vector2 origin, Vector2 target, Entity caster)
        {
            Main.PlaySound(SoundID.Item8, caster.position);
            castAction(this, player, origin, target, caster);
        }

        public bool CompleteSkill()
        {
            bool complete = true;
            for (int i = 0; i < Glyphs.Length; i += 1)
                if (Glyphs[i].type == 0)
                    complete = false;

            return complete;
        }

        //angle from 0f to 1f
        public ProceduralSpellProj CreateProjectile(Player player, Vector2 velocity, float angle = 0f, Vector2? position = null, Entity caster = null)
        {
            if (caster == null) caster = player;
            Projectile projectile =
                Main.projectile[
                    Projectile.NewProjectile(position ?? caster.Center, velocity.RotatedBy(API.Tau * angle), ModContent.ProjectileType<ProceduralSpellProj>(),
                        ProjectileDamage(player.GetModPlayer<PlayerCharacter>()), 3f, player.whoAmI)];
            ProceduralSpellProj ps = (ProceduralSpellProj) projectile.modProjectile;
            ps.Origin = projectile.position;
            foreach (Item item in Glyphs)
            {
                Glyph glyph = (Glyph) item.modItem;
                if (glyph.GetAiAction() != null)
                    ps.ai.Add(glyph.GetAiAction());
                if (glyph.GetInitAction() != null)
                    ps.Inits.Add(glyph.GetInitAction());
                if (glyph.GetImpactAction() != null)
                    ps.Impacts.Add(glyph.GetImpactAction());
                if (glyph.GetKillAction() != null)
                    ps.Kills.Add(glyph.GetKillAction());
            }

            foreach (GlyphModifier modifier in Modifiers)
            {
                if (modifier.Impact != null)
                    ps.Impacts.Add(modifier.Impact);
                if (modifier.Draw != null)
                    ps.draw.Add(modifier.Draw);
                if (modifier.Init != null)
                    ps.Inits.Add(modifier.Init);
            }

            ps.Caster = caster;
            ps.projectile.minion = Minion;
            /*if (minion)
            {
                ps.projectile.melee = false;
                ps.projectile.ranged = false;
                ps.projectile.magic = false;
            }*/
            ps.Source = this;
            ps.Initialize();
            if (Main.netMode != 1) return ps;
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte) Message.CreateProjectile);
            packet.Write(player.whoAmI);
            packet.Write(ps.projectile.whoAmI);
            packet.Write(Glyphs[(byte) GLYPHTYPE.STAR].type);
            packet.Write(Glyphs[(byte) GLYPHTYPE.CROSS].type);
            packet.Write(Glyphs[(byte) GLYPHTYPE.MOON].type);
            packet.Write(ps.projectile.damage);
            packet.Write(Minion);
            packet.Write(caster.whoAmI);
            List<GlyphModifier> mods = Modifiers;
            packet.Write(mods.Count);
            for (int j = 0; j < mods.Count; j += 1)
                packet.Write(mods[j].Id);
            packet.Send();
            return ps;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, float scale)
        {
            try
            {
                bool full = true;
                for (int i = 0; i < Glyphs.Length; i += 1)
                    if (Glyphs[i].type == 0)
                        full = false;

                if (Remaining > 0)
                    Remaining -= 1;

                Rectangle bounds = new Rectangle((int) position.X, (int) position.Y, (int) (GFX.SkillSlot.Width * scale), (int) (GFX.SkillSlot.Height * scale));
                PlayerCharacter character = Main.LocalPlayer.GetModPlayer<PlayerCharacter>();

                if (spriteBatch == null || character == null) return;

                if (bounds.Contains(new Point(Main.mouseX, Main.mouseY)))
                {
                    Main.LocalPlayer.mouseInterface = true;
                    if (Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        Main.PlaySound(SoundID.MenuTick);
                        character.CloseGuIs();
                        character.SelectedAbility = this;
                    }
                }

                if (character.SelectedAbility != null)
                {
                    if (character.SelectedAbility.Equals(this))
                    {
                        spriteBatch.Draw(GFX.SelectedSkillSlot, position, Color.White, scale);
                        for (Keys k = Keys.A; k <= Keys.Z; k += 1)
                            if (Main.keyState.IsKeyDown(k) && (Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift)))
                            {
                                for (int i = 0; i < character.Abilities.Length; i += 1)
                                    if (character.Abilities[i] != character.SelectedAbility && character.Abilities[i].Key == k)
                                        character.Abilities[i].Key = Key;

                                Key = k;
                            }
                    }
                    else
                    {
                        spriteBatch.Draw(GFX.SkillSlot, position, Color.White, scale);
                    }
                }
                else
                {
                    spriteBatch.Draw(GFX.SkillSlot, position, Color.White, scale);
                }

                if (full && Remaining == 0 && character.Mana >= ManaCost(character)) spriteBatch.Draw(GFX.GothicLetter[Key], position, Color.White, scale);
                else if (full || character.SpellCraftingGui.GuiActive) spriteBatch.Draw(GFX.GothicLetter[Key], position, new Color(143, 143, 151), scale);
            }
            catch (Exception e)
            {
                ModLoader.GetMod("kRPG").Logger.InfoFormat(e.ToString());
            }
        }

        // ReSharper disable once IdentifierTypo
        public int ManaCost(PlayerCharacter character)
        {
            float multiplier = Glyphs.Aggregate(0.4f, (current, item) => current * ((Glyph) item.modItem).ManaModifier());
            return (int) Math.Round((20 + character.Level) * multiplier);
        }

        public int ProjectileDamage(PlayerCharacter character)
        {
            int constant = Main.hardMode ? character.Level / 3 : 5;
            float multiplier = Main.expertMode ? 1f : 0.65f;
            multiplier = Glyphs.Aggregate(multiplier, (current, item) => current * ((Glyph) item.modItem).DamageModifier());
            return (int) Math.Round(Math.Pow(1.04, Math.Min(130, character.Level)) * 9f * multiplier) + constant;
        }

        public void UseAbility(Player player, Vector2 target)
        {
            bool vanish = Modifiers.Contains(GlyphModifier.Vanish);
            Vector2 oldCenter = player.Center;
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
                            PlayerDeathReason damageSource = PlayerDeathReason.ByOther(13);
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

            UseAction(this, player, vanish ? oldCenter : target);
        }
    }


}