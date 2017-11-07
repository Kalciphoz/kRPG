using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using kRPG.Projectiles;
using kRPG.Items.Weapons;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using kRPG.Items.Glyphs;
using Terraria.ModLoader.IO;
using kRPG.GUI;
using kRPG.Items;
using Terraria.Audio;
using Terraria.ID;

namespace kRPG
{
    public class ProceduralSpell
    {
        private Mod mod;

        public Keys key = Keys.A;

        public int cooldown = 90;
        public int remaining = 0;

        public List<ProceduralSpellProj> circlingProtection = new List<ProceduralSpellProj>();
        public bool minion
        {
            get { return !(glyphs[(byte)GLYPHTYPE.STAR].modItem is Star_Blue); }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Player player)
        {
            try
            {
                bool full = true;
                for (int i = 0; i < glyphs.Length; i += 1)
                    if (glyphs[i].type == 0)
                        full = false;

                if (remaining > 0)
                    remaining -= 1;

                Rectangle bounds = new Rectangle((int)position.X, (int)position.Y, GFX.skillSlot.Width, GFX.skillSlot.Height);
                PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();

                if (spriteBatch == null || character == null) return;

                if (bounds.Contains(new Point(Main.mouseX, Main.mouseY)))
                {
                    Main.LocalPlayer.mouseInterface = true;
                    if (Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        Main.PlaySound(SoundID.MenuTick);
                        character.CloseGUIs();
                        character.selectedAbility = this;
                        character.spellcraftingGUI.guiActive = true;
                    }
                }

                if (character.selectedAbility != null)
                {
                    if (character.selectedAbility.Equals(this))
                    {
                        spriteBatch.Draw(GFX.selectedSkillSlot, position, Color.White);
                        for (Keys k = Keys.A; k <= Keys.Z; k += 1)
                            if (Main.keyState.IsKeyDown(k) && (Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift)))
                            {
                                for (int i = 0; i < character.abilities.Length; i += 1)
                                    if (character.abilities[i] != character.selectedAbility && character.abilities[i].key == k)
                                        character.abilities[i].key = key;

                                key = k;
                            }
                    }
                    else
                        spriteBatch.Draw(GFX.skillSlot, position, Color.White);
                }
                else
                    spriteBatch.Draw(GFX.skillSlot, position, Color.White);
                
                if (full && remaining == 0 && character.mana >= ManaCost(character)) spriteBatch.Draw(GFX.gothicLetter[key], position, Color.White);
                else if (full || character.spellcraftingGUI.guiActive) spriteBatch.Draw(GFX.gothicLetter[key], position, new Color(143, 143, 151));
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
            }
        }

        public Item[] glyphs = new Item[3];
        public int projCount
        {
            get
            {
                return ((Moon)glyphs[(byte)GLYPHTYPE.MOON].modItem).projCount;
            }
        }

        public ProceduralSpell(Mod mod)
        {
            this.mod = mod;
            for (int i = 0; i < glyphs.Length; i += 1)
            {
                glyphs[i] = new Item();
            }
        }

        public Action<ProceduralSpell, Player, Vector2> useAction
        {
            get
            {
                Glyph glyph = (Glyph)glyphs[(byte)GUI.GLYPHTYPE.STAR].modItem;
                return glyph.GetUseAbility();
            }
        }
        public void UseAbility(Player player, Vector2 target)
        {
            bool vanish = ((Items.Glyphs.Star)glyphs[(int)GLYPHTYPE.STAR].modItem).modifiers.Contains(GlyphModifier.vanish);
            Vector2 oldCenter = player.Center;
            if (vanish)
            {
                player.Center = target;
                Main.PlaySound(new LegacySoundStyle(2, 14, Terraria.Audio.SoundType.Sound).WithVolume(0.5f), oldCenter);
                Projectile.NewProjectile(oldCenter - new Vector2(24, 48), Vector2.Zero, mod.ProjectileType<SmokePellets>(), 2, 0f, player.whoAmI);
            }
            useAction(this, player, vanish ? oldCenter : target);
        }

        public Action<ProceduralSpell, Player, Vector2, Vector2, Entity> castAction
        {
            get
            {
                Glyph glyph = (Glyph)glyphs[(byte)GUI.GLYPHTYPE.MOON].modItem;
                return glyph.GetCastAction();
            }
        }
        public void CastSpell(Player player, Vector2 origin, Vector2 target, Entity caster)
        {
            Main.PlaySound(Terraria.ID.SoundID.Item8, caster.position);
            castAction(this, player, origin, target, caster);
        }

        //angle from 0f to 1f
        public ProceduralSpellProj CreateProjectile(Player player, Vector2 velocity, float angle = 0f, Vector2? position = null, Entity caster = null)
        {
            if (caster == null) caster = player;
            Projectile projectile = Main.projectile[Projectile.NewProjectile(position == null ? caster.Center : (Vector2)position, velocity.RotatedBy(API.Tau * angle), mod.ProjectileType<ProceduralSpellProj>(), ProjectileDamage(player.GetModPlayer<PlayerCharacter>()), 3f, player.whoAmI)];
            ProceduralSpellProj ps = (ProceduralSpellProj)projectile.modProjectile;
            ps.origin = projectile.position;
            foreach (Item item in glyphs)
            {
                Glyph glyph = (Glyph)item.modItem;
                if (glyph.GetAIAction() != null)
                    ps.ai.Add(glyph.GetAIAction());
                if (glyph.GetInitAction() != null)
                    ps.init.Add(glyph.GetInitAction());
                if (glyph.GetImpactAction() != null)
                    ps.impact.Add(glyph.GetImpactAction());
                if (glyph.GetKillAction() != null)
                    ps.kill.Add(glyph.GetKillAction());
                foreach (GlyphModifier modifier in glyph.modifiers)
                {
                    if (modifier.impact != null)
                        ps.impact.Add(modifier.impact);
                    if (modifier.draw != null)
                        ps.draw.Add(modifier.draw);
                    if (modifier.init != null)
                        ps.init.Add(modifier.init);
                }
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
            return ps;
        }

        public bool CompleteSkill()
        {
            bool complete = true;
            for (int i = 0; i < glyphs.Length; i += 1)
                if (glyphs[i].type == 0) complete = false;

            return complete;
        }

        public int ProjectileDamage(PlayerCharacter character)
        {
            float multiplier = Main.expertMode ? 0.6f : 0.4f;
            foreach (Item item in glyphs)
                multiplier *= ((Glyph)item.modItem).DamageModifier();
            return (int)Math.Round((character.level + 5f) * multiplier); 
        }

        public int ManaCost(PlayerCharacter character)
        {
            float multiplier = 0.6f;
            foreach (Item item in glyphs)
                multiplier *= ((Glyph)item.modItem).ManaModifier();
            return (int)Math.Round((10 + character.level) * multiplier);
        }
    }

    public class SpellEffect
    {
        private ProceduralSpell ability;
        private Vector2 target;
        private int timeLeft;
        private Action<ProceduralSpell, int> update;

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
