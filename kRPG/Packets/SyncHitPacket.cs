using System.Collections.Generic;
using System.IO;

using kRPG.Enums;
using kRPG.GameObjects.Players;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Packets
{
    public static class SyncHitPacket
    {
        public static void Read(BinaryReader reader)
        {
            if (Main.netMode == Constants.NetModes.Client)
            {
                PlayerCharacter character = Main.player[reader.ReadInt32()].GetModPlayer<PlayerCharacter>();
                character.AccuracyCounter = reader.ReadSingle();
            }
        }

        public static void Write(int player, float accuracyCounter)
        {
            if (Main.netMode == Constants.NetModes.Server)
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