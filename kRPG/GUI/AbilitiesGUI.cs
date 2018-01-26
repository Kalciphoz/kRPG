using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace kRPG.GUI
{
    public class AbilitiesGUI : BaseGUI
    {
        private PlayerCharacter character;
        private Mod mod;
        private kRPG krpg;

        private Vector2 GuiPosition
        {
            get
            {
                return new Vector2(Main.screenWidth - GFX.skillSlot.Width * 8 * scale, Main.screenHeight - GFX.skillSlot.Height * scale - 12);
            }
        }

        private float scale
        {
            get
            {
                return Math.Min(1f, Main.screenWidth / 1920f);
            }
        }

        public AbilitiesGUI(PlayerCharacter character, Mod mod) : base()
        {
            this.character = character;
            this.mod = mod;
            this.krpg = (kRPG)mod;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Player player)
        {
            PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
            for (int i = 0; i < character.abilities.Length; i += 1)
            {
                character.abilities[i].Draw(spriteBatch, GuiPosition + new Vector2(i * (GFX.skillSlot.Width + 8f) * scale, 0), scale);
            }
        }
    }
}
