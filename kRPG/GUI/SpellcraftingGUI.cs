using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using kRPG.Items.Glyphs;
using Terraria.UI;
using ReLogic;
using ReLogic.Graphics;
using kRPG.Projectiles;
using Terraria.ID;

namespace kRPG.GUI
{
    public enum GLYPHTYPE : byte { STAR, CROSS, MOON }

    public class SpellcraftingGUI : BaseGUI
    {
        private Mod mod;
        private kRPG krpg;

        private Func<Vector2> guiposition;

        public GlyphSlot[] glyphs = new GlyphSlot[3];

        private float Scale
        {
            get
            {
                return Math.Min(1f, Main.screenWidth / 3840f + 0.4f);
            }
        }

        public SpellcraftingGUI(Mod mod)
        {
            this.mod = mod;
            krpg = (kRPG)mod;
            guiposition = delegate () { return new Vector2((float)Main.screenWidth / 2f - 100f * Scale, 192f * Scale); };
            glyphs[0] = new GlyphSlot(delegate () { return guiposition() + new Vector2(84f, 36f) * Scale; }, delegate () { return Scale; }, GLYPHTYPE.STAR);
            glyphs[1] = new GlyphSlot(delegate () { return guiposition() + new Vector2(84f, 70f) * Scale; }, delegate () { return Scale; }, GLYPHTYPE.CROSS);
            glyphs[2] = new GlyphSlot(delegate () { return guiposition() + new Vector2(84f, 106f) * Scale; }, delegate () { return Scale; }, GLYPHTYPE.MOON);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Player player)
        {
            spriteBatch.Draw(GFX.spellGui, guiposition(), null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
            foreach (GlyphSlot slot in glyphs)
                slot.Draw(spriteBatch);

            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Place glyphs in all three slots to create a spell", new Vector2(Main.screenWidth / 2f - 176f * Scale, Main.screenHeight / 2f + 200f * Scale), Color.White, Scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Press a key while holding shift to bind it as a hotkey", new Vector2(Main.screenWidth / 2f - 176f * Scale, Main.screenHeight / 2f + 224f * Scale), Color.White, Scale);

            Vector2 buttonPosition = new Vector2(Main.screenWidth / 2f - 92f * Scale, Main.screenHeight / 2f + 256f * Scale);
            spriteBatch.Draw(GFX.button_close, buttonPosition, Color.White, Scale);

            if (Main.mouseX >= buttonPosition.X && Main.mouseY >= buttonPosition.Y && Main.mouseX <= buttonPosition.X + (int)(GFX.button_confirm.Width * Scale) && Main.mouseY <= buttonPosition.Y + (int)(GFX.button_confirm.Height * Scale))
            {
                Main.LocalPlayer.mouseInterface = true;
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    Main.PlaySound(SoundID.MenuTick);
                    CloseGUI();
                }
            }
        }

        public override void OnClose()
        {
            Main.LocalPlayer.GetModPlayer<PlayerCharacter>().selectedAbility = null;
        }
    }

    public class GlyphSlot
    {
        private GLYPHTYPE type;
        private Item Glyph
        {
            get
            {
                return Ability.glyphs[(byte)type];
            }
            set
            {
                Ability.glyphs[(byte)type] = value;
            }
        }
        private Func<Vector2> position;
        private Func<float> scale;
        private Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)position().X, (int)position().Y, (int)(30 * scale()), (int)(30 * scale()));
            }
        }
        private ProceduralSpell Ability
        {
            get
            {
                return Main.LocalPlayer.GetModPlayer<PlayerCharacter>().selectedAbility;
            }
        }

        public GlyphSlot(Func<Vector2> position, Func<float> scale, GLYPHTYPE type)
        {
            this.type = type;
            this.position = position;
            this.scale = scale;
        }

        private bool CanPlaceItem(Item item)
        {
            bool check = false;
            switch (type)
            {
                case GLYPHTYPE.STAR:
                    check = item.modItem is Items.Glyphs.Star;
                    break;
                case GLYPHTYPE.CROSS:
                    check = item.modItem is Cross;
                    break;
                case GLYPHTYPE.MOON:
                    check = item.modItem is Moon;
                    break;
            }
            return check || item.type == 0;
        }

        public bool AttemptPlace()
        {
            PlayerCharacter character = Main.LocalPlayer.GetModPlayer<PlayerCharacter>();

            if (CanPlaceItem(Main.mouseItem))
            {
                foreach (ProceduralMinion minion in character.minions.Where(minion => minion.source == character.selectedAbility && minion.projectile.modProjectile is ProceduralMinion))
                {
                    foreach (ProceduralSpellProj psp in minion.circlingProtection)
                        psp.projectile.Kill();
                    minion.circlingProtection.Clear();
                    if (minion.smallProt != null) minion.smallProt.projectile.Kill();
                    minion.projectile.Kill();
                }
                Item prevItem = Glyph;
                Glyph = Main.mouseItem;
                Main.mouseItem = prevItem;
                Main.PlaySound(Terraria.ID.SoundID.Item4, Main.screenPosition + Bounds.Center());
                return true;
            }
            else
                return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Bounds.Contains(Main.mouseX, Main.mouseY))
            {
                Main.LocalPlayer.mouseInterface = true;
                Main.HoverItem = Glyph.Clone();
                if (Main.mouseLeft && Main.mouseLeftRelease) AttemptPlace();
            }
            if (Glyph.type == 0) return;
            Texture2D texture = Main.itemTexture[Glyph.type];
            spriteBatch.Draw(texture, Bounds.TopLeft() + new Vector2(2f, 2f), Color.White, scale());
        }
    }
}
