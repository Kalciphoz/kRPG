using System.Collections.Generic;
using System.IO;

using kRPG.Enums;
using kRPG.GameObjects.Players;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Packets
{
    public static class SyncStatsPacket
    {
        public static void Read( BinaryReader reader)
        {
            if (Main.netMode == 2)
            {
                PlayerCharacter character = Main.player[reader.ReadInt32()].GetModPlayer<PlayerCharacter>();
                character.Level = reader.ReadInt32();
                character.BaseStats[PlayerStats.Resilience] = reader.ReadInt32();
                character.BaseStats[PlayerStats.Quickness] = reader.ReadInt32();
                character.BaseStats[PlayerStats.Potency] = reader.ReadInt32();
                character.BaseStats[PlayerStats.Wits] = reader.ReadInt32();
            }
        }

        public static void Write(int whoAmI, int level, int resilience, int quickness, int potency, int wits)
        {
            if (Main.netMode == 1)
            {
                ModPacket packet = kRPG.Mod.GetPacket();
                packet.Write((byte) Message.SyncStats);
                packet.Write(whoAmI);
                packet.Write(level);
                packet.Write(resilience);
                packet.Write(quickness);
                packet.Write(potency);
                packet.Write(wits);
                packet.Send();
            }
        }
    }
}