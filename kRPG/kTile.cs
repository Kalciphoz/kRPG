using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG
{
    public class kTile : GlobalTile
    {
        public override void RightClick(int i, int j, int type)
        {
            if (Main.netMode == 2)
                return;
            if (type != TileID.Anvils && type != TileID.MythrilAnvil)
                return;
            var character = Main.player[Main.myPlayer].GetModPlayer<PlayerCharacter>();
            character.CloseGUIs();
            character.anvilGUI.guiActive = true;
            character.anvilGUI.position = new Vector2(i * 16f + 16f, j * 16f + 8f);
        }
    }
}