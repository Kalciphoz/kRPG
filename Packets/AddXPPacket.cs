using System.Collections.Generic;
using System.IO;
using kRPG.Content.Players;
using kRPG.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.Packets
{
    public static class AddXPPacket
    {
        public static void Read(BinaryReader reader)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                //Player player = Main.player[Main.myPlayer];
                //if (Vector2.Distance(player.Center, Main.npc[(int)tags[DataTag.npcId]].Center) > 1024)
                //    break;
                PlayerCharacter character = Main.LocalPlayer.GetModPlayer<PlayerCharacter>();
                //character.AddXp((int) tags[DataTag.Amount]);
                character.AddXp((int)reader.ReadInt32());
            }
        }

        public static bool Write(int scaled, int target)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = kRPG.Mod.GetPacket();
                packet.Write((byte)Message.AddXp);
                packet.Write(scaled);
                //todo this seems extra....
               // packet.Write(target);
                packet.Send();
                return true;
            }

            return false;
        }
    }
}