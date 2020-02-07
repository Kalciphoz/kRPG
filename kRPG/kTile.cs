using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using kRPG.GUI;

namespace kRPG
{
    public class kTile : GlobalTile
    {
        public override void RightClick(int i, int j, int type)
        {
            if (type == TileID.Anvils || type == TileID.MythrilAnvil)
            {
                Main.PlaySound(SoundID.MenuOpen, new Vector2(i * 16f + 16, j * 16f + 8));
                PlayerCharacter character = Main.LocalPlayer.GetModPlayer<PlayerCharacter>(mod);
                character.CloseGUIs();
                if (character.anvilGUI == null) character.anvilGUI = new AnvilGUI(mod, character);
                character.anvilGUI.guiActive = true;
                character.anvilGUI.position = new Vector2(i * 16f + 16f, j * 16f + 8f);
            }
        }
    }
}
