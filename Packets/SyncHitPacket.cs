using System.Collections.Generic;
using System.IO;
using kRPG.Content.Players;
using kRPG.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.Packets
{
    public static class SyncHitPacket
    {
        public static void Read(BinaryReader reader)
        {
            if (Main.netMode ==NetmodeID.MultiplayerClient)
            {
                PlayerCharacter character = Main.player[reader.ReadInt32()].GetModPlayer<PlayerCharacter>();
                character.AccuracyCounter = reader.ReadSingle();
            }
        }

        public static void Write(int player, float accuracyCounter)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = kRPG.Mod.GetPacket();
                packet.Write((byte) Message.SyncHit);
                packet.Write(player);
                packet.Write(accuracyCounter);
                packet.Send();
            }
        }
    }
}