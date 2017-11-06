using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace kRPG
{
    public class kTile : GlobalTile
    {
        public override void RightClick(int i, int j, int type)
        {
            if (Main.netMode != 2)
                if (type == TileID.Anvils || type == TileID.MythrilAnvil)
                {
                    /*if (Main.MouseWorld.X >= i * 16 && Main.MouseWorld.X <= (i + 2) * 16 && Main.MouseWorld.Y >= j * 16 && Main.MouseWorld.Y <= (j + 1) * 16)
                    {
                        Player player = Main.player[Main.myPlayer];
                        if (Vector2.Distance(player.position, new Vector2(i * 16f + 16f, j * 16f + 8f)) <= 80f)
                    }*/

                    PlayerCharacter character = Main.player[Main.myPlayer].GetModPlayer<PlayerCharacter>(mod);
                    character.CloseGUIs();
                    character.anvilGUI.guiActive = true;
                    character.anvilGUI.position = new Vector2(i * 16f + 16f, j * 16f + 8f);
                }
        }
    }
}
