using System;
using System.Diagnostics;
using System.Linq;
using kRPG.Enums;
using kRPG.GameObjects.GUI.Base;
using kRPG.GameObjects.Items.Glyphs;
using kRPG.GameObjects.Items.Projectiles;
using kRPG.GameObjects.Players;
using kRPG.GameObjects.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Star = kRPG.GameObjects.Items.Glyphs.Star;

namespace kRPG.GameObjects.GUI
{
    public class SpellcraftingGUI : BaseGui
    {
        public GlyphSlot[] glyphs = new GlyphSlot[3];

        private readonly Func<Vector2> guiPosition;

        public SpellcraftingGUI(Mod mod)
        {
            guiPosition = () => new Vector2(Main.screenWidth / 2f - 100f * Scale, 192f * Scale);

            glyphs[0] = new GlyphSlot(() => guiPosition() + new Vector2(84f, 36f) * Scale, () => Scale, GlyphType.Star);

            glyphs[1] = new GlyphSlot(() => guiPosition() + new Vector2(84f, 70f) * Scale, () => Scale, GlyphType.Cross);

            glyphs[2] = new GlyphSlot(() => guiPosition() + new Vector2(84f, 106f) * Scale, () => Scale, GlyphType.Moon);

            //not needed, the base class does this.
            // GuiElements.Add(this);
        }

        private float Scale => Math.Min(1f, Main.screenWidth / Constants.MaxScreenWidth + 0.4f);

        public override void OnClose()
        {
            Main.LocalPlayer.GetModPlayer<PlayerCharacter>().SelectedAbility = null;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Player player)
        {
            spriteBatch.Draw(GFX.GFX.SpellGui, guiPosition(), null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);

            foreach (GlyphSlot slot in glyphs)
                slot.Draw(spriteBatch);

            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Place glyphs in all three slots to create a spell",
                new Vector2(Main.screenWidth / 2f - 176f * Scale, Main.screenHeight / 2f + 200f * Scale), Color.White, Scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Press a key while holding shift to bind it as a hotkey",
                new Vector2(Main.screenWidth / 2f - 176f * Scale, Main.screenHeight / 2f + 224f * Scale), Color.White, Scale);

            Vector2 buttonPosition = new Vector2(Main.screenWidth / 2f - 92f * Scale, Main.screenHeight / 2f + 256f * Scale);
            spriteBatch.Draw(GFX.GFX.ButtonClose, buttonPosition, Color.White, Scale);

            if (!(Main.mouseX >= buttonPosition.X) || !(Main.mouseY >= buttonPosition.Y) ||
                !(Main.mouseX <= buttonPosition.X + (int) (GFX.GFX.ButtonConfirm.Width * Scale)) ||
                !(Main.mouseY <= buttonPosition.Y + (int) (GFX.GFX.ButtonConfirm.Height * Scale)))
                return;

            Main.LocalPlayer.mouseInterface = true;

            if (!Main.mouseLeft || !Main.mouseLeftRelease)
                return;

            Main.PlaySound(SoundID.MenuTick);

            CloseGui();
        }

        public override bool PreDraw()
        {
            return Main.LocalPlayer.GetModPlayer<PlayerCharacter>().SelectedAbility != null;
        }
    }

    public class GlyphSlot
    {
        private readonly Func<Vector2> position;
        private readonly Func<float> scale;
        private readonly GlyphType type;

        public GlyphSlot(Func<Vector2> position, Func<float> scale, GlyphType type)
        {
            this.type = type;
            this.position = position;
            this.scale = scale;
        }

        private ProceduralSpell Ability => Main.LocalPlayer.GetModPlayer<PlayerCharacter>().SelectedAbility;
        private Rectangle Bounds => new Rectangle((int) position().X, (int) position().Y, (int) (30 * scale()), (int) (30 * scale()));

        private Item Glyph
        {
            get => Ability.Glyphs[(byte) type];
            set => Ability.Glyphs[(byte) type] = value;
        }

        public bool AttemptPlace()
        {
            PlayerCharacter character = Main.LocalPlayer.GetModPlayer<PlayerCharacter>();

            if (!CanPlaceItem(Main.mouseItem))
                return false;

            foreach (ProceduralMinion minion in character.Minions.Where(minion =>
                minion.Source == character.SelectedAbility && minion.projectile.modProjectile is ProceduralMinion))
            {
                foreach (ProceduralSpellProj psp in minion.CirclingProtection)
                    psp.projectile.Kill();
                minion.CirclingProtection.Clear();
                minion.SmallProt?.projectile.Kill();
                minion.projectile.Kill();
            }

            Item prevItem = Glyph;
            Glyph = Main.mouseItem;
            Main.mouseItem = prevItem;
            Main.PlaySound(SoundID.Item4, Main.screenPosition + Bounds.Center());

            return true;
        }

        private bool CanPlaceItem(Item item)
        {
            bool check = false;
            switch (type)
            {
                case GlyphType.Star:
                    check = item.modItem is Star;
                    break;
                case GlyphType.Cross:
                    check = item.modItem is Cross;
                    break;
                case GlyphType.Moon:
                    check = item.modItem is Moon;
                    break;
            }

            return check || item.type == 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Bounds.Contains(Main.mouseX, Main.mouseY))
            {
                Main.LocalPlayer.mouseInterface = true;
                Main.HoverItem = Glyph.Clone();
                if (Main.mouseLeft && Main.mouseLeftRelease)
                    AttemptPlace();
            }

            if (Glyph.type == 0)
                return;

            Texture2D texture = Main.itemTexture[Glyph.type];
            spriteBatch.Draw(texture, Bounds.TopLeft() + new Vector2(2f, 2f), Color.White, scale());
        }
    }
}