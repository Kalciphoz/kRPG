using System;
using System.Linq;
using kRPG.Items.Glyphs;
using kRPG.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.GUI
{
    public enum GLYPHTYPE : byte { STAR, CROSS, MOON }

    public class SpellcraftingGUI : BaseGUI
    {


        private readonly Func<Vector2> guiPosition;

        public GlyphSlot[] glyphs = new GlyphSlot[3];

        private float Scale => Math.Min(1f, Main.screenWidth / 3840f + 0.4f);

        public SpellcraftingGUI(Mod mod)
        {

            guiPosition = () => new Vector2(Main.screenWidth / 2f - 100f * Scale, 192f * Scale);

            glyphs[0] = new GlyphSlot(() => guiPosition() + new Vector2(84f, 36f) * Scale, () => Scale, GLYPHTYPE.STAR);

            glyphs[1] = new GlyphSlot(() => guiPosition() + new Vector2(84f, 70f) * Scale, () => Scale, GLYPHTYPE.CROSS);

            glyphs[2] = new GlyphSlot(() => guiPosition() + new Vector2(84f, 106f) * Scale, () => Scale, GLYPHTYPE.MOON);

            gui_elements.Add(this);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Player player)
        {
            spriteBatch.Draw(GFX.spellGui, guiPosition(), null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
            foreach (GlyphSlot slot in glyphs)
                slot.Draw(spriteBatch);

            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Place glyphs in all three slots to create a spell", new Vector2(Main.screenWidth / 2f - 176f * Scale, Main.screenHeight / 2f + 200f * Scale), Color.White, Scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Press a key while holding shift to bind it as a hotkey", new Vector2(Main.screenWidth / 2f - 176f * Scale, Main.screenHeight / 2f + 224f * Scale), Color.White, Scale);

            Vector2 buttonPosition = new Vector2(Main.screenWidth / 2f - 92f * Scale, Main.screenHeight / 2f + 256f * Scale);
            spriteBatch.Draw(GFX.button_close, buttonPosition, Color.White, Scale);

            if (!(Main.mouseX >= buttonPosition.X) || !(Main.mouseY >= buttonPosition.Y) ||
                !(Main.mouseX <= buttonPosition.X + (int)(GFX.button_confirm.Width * Scale)) ||
                !(Main.mouseY <= buttonPosition.Y + (int)(GFX.button_confirm.Height * Scale)))
                return;
            Main.LocalPlayer.mouseInterface = true;
            if (!Main.mouseLeft || !Main.mouseLeftRelease)
                return;
            Main.PlaySound(SoundID.MenuTick);
            CloseGUI();
        }

        public override bool PreDraw()
        {
            return Main.LocalPlayer.GetModPlayer<PlayerCharacter>().selectedAbility != null;
        }

        public override void OnClose()
        {
            Main.LocalPlayer.GetModPlayer<PlayerCharacter>().selectedAbility = null;
        }
    }

    public class GlyphSlot
    {
        private readonly GLYPHTYPE type;
        private Item Glyph {
            get => Ability.glyphs[(byte)type];
            set => Ability.glyphs[(byte)type] = value;
        }
        private readonly Func<Vector2> position;
        private readonly Func<float> scale;
        private Rectangle Bounds => new Rectangle((int)position().X, (int)position().Y, (int)(30 * scale()), (int)(30 * scale()));

        private ProceduralSpell Ability => Main.LocalPlayer.GetModPlayer<PlayerCharacter>().selectedAbility;

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

            if (!CanPlaceItem(Main.mouseItem))
                return false;

            foreach (ProceduralMinion minion in character.minions.Where(minion => minion.source == character.selectedAbility && minion.projectile.modProjectile is ProceduralMinion))
            {
                foreach (ProceduralSpellProj psp in minion.circlingProtection)
                    psp.projectile.Kill();
                minion.circlingProtection.Clear();
                minion.smallProt?.projectile.Kill();
                minion.projectile.Kill();
            }
            Item prevItem = Glyph;
            Glyph = Main.mouseItem;
            Main.mouseItem = prevItem;
            Main.PlaySound(SoundID.Item4, Main.screenPosition + Bounds.Center());
            return true;

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
