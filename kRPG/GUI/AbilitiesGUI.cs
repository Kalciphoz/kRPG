using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.GUI
{
    public class AbilitiesGUI : BaseGUI
    {
        private PlayerCharacter character;
        private Mod mod;
        private kRPG krpg;

        private Vector2 GuiPosition => new Vector2(Main.screenWidth - GFX.skillSlot.Width * 8 * scale, Main.screenHeight - GFX.skillSlot.Height * scale - 12);

        private float scale => Math.Min(1f, Main.screenWidth / 1920f);

        public AbilitiesGUI(PlayerCharacter character, Mod mod) : base()
        {
            this.character = character;
            this.mod = mod;
            krpg = (kRPG) mod;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Player player)
        {
            var modPlayer = player.GetModPlayer<PlayerCharacter>();
            for (int i = 0; i < modPlayer.abilities.Length; i += 1)
                modPlayer.abilities[i].Draw(spriteBatch, GuiPosition + new Vector2(i * (GFX.skillSlot.Width + 8f) * scale, 0), scale);
        }
    }
}