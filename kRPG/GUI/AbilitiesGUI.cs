using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace kRPG.GUI
{
    public class AbilitiesGUI : BaseGui
    {

        public AbilitiesGUI()
        {
            GuiPosition = new Vector2(Main.screenWidth - GFX.skillSlot.Width * 8 * Scale, Main.screenHeight - GFX.skillSlot.Height * Scale - 12);
        }

        private Vector2 GuiPosition { get; }

        private float Scale { get; } = Math.Min(1f, Main.screenWidth / 1920f);

        public override void PostDraw(SpriteBatch spriteBatch, Player player)
        {
            PlayerCharacter modPlayer = player.GetModPlayer<PlayerCharacter>();
            for (int i = 0; i < modPlayer.abilities.Length; i += 1)
                modPlayer.abilities[i].Draw(spriteBatch, GuiPosition + new Vector2(i * (GFX.skillSlot.Width + 8f) * Scale, 0), Scale);
        }
    }
}