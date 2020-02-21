using System;
using kRPG.Content.GUI.Base;
using kRPG.Content.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace kRPG.Content.GUI
{
    public class AbilitiesGui : BaseGui
    {
        public AbilitiesGui()
        {
            GuiPosition = new Vector2(Main.screenWidth - GFX.GFX.SkillSlot.Width * 8 * Scale, Main.screenHeight - GFX.GFX.SkillSlot.Height * Scale - 12);
        }

        private Vector2 GuiPosition { get; }

        private float Scale { get; } = Math.Min(1f, Main.screenWidth / 1920f);

        public override void PostDraw(SpriteBatch spriteBatch, Player player)
        {
            try
            {

                PlayerCharacter modPlayer = player.GetModPlayer<PlayerCharacter>();
                for (int i = 0; i < modPlayer.Abilities.Length; i += 1)
                    modPlayer.Abilities[i].Draw(spriteBatch, GuiPosition + new Vector2(i * (GFX.GFX.SkillSlot.Width + 8f) * Scale, 0), Scale);
            }
            catch (Exception e)
            {
                kRPG.LogMessage("AbilitiesGui PostDraw Error: " + e);
            }
            


        }
    }
}