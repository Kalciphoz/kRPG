using System.Collections.Generic;
using System.IO;

using kRPG.Enums;
using kRPG.GameObjects.Players;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Packets
{
    public static class SyncCritHitPacket
    {
        public static void Read( BinaryReader reader)
        {
            if (Main.netMode == 1)
            {
                PlayerCharacter character = Main.player[reader.ReadInt32()].GetModPlayer<PlayerCharacter>();
                character.CritAccuracyCounter = (float) reader.ReadSingle();
            }
        }

        public static void Write(int player, float critAccuracyCounter)
        {
            if (Main.netMode == 2)
            {
                ModPacket packet = kRPG.Mod.GetPacket();
                packet.Write((byte) Message.SyncCritHit);
                packet.Write(player);
                packet.Write(critAccuracyCounter);
                packet.Send();
            }
        }
    }
}