using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace kRPG2.GUI
{
    public class AbilitiesGui : BaseGui
    {

        public AbilitiesGui()
        {
            GuiPosition = new Vector2(Main.screenWidth - GFX.SkillSlot.Width * 8 * Scale, Main.screenHeight - GFX.SkillSlot.Height * Scale - 12);
        }

        private Vector2 GuiPosition { get; }

        private float Scale { get; } = Math.Min(1f, Main.screenWidth / 1920f);

        public override void PostDraw(SpriteBatch spriteBatch, Player player)
        {
            PlayerCharacter modPlayer = player.GetModPlayer<PlayerCharacter>();
            for (int i = 0; i < modPlayer.Abilities.Length; i += 1)
                modPlayer.Abilities[i].Draw(spriteBatch, GuiPosition + new Vector2(i * (GFX.SkillSlot.Width + 8f) * Scale, 0), Scale);
        }
    }
}