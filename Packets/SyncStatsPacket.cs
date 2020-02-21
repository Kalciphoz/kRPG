using System.Collections.Generic;
using System.IO;
using kRPG.Content.Players;
using kRPG.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.Packets
{
    public static class SyncStatsPacket
    {
        public static void Read( BinaryReader reader)
        {
            if (Main.netMode == NetmodeID.Server)
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
            if (Main.netMode == NetmodeID.MultiplayerClient)
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