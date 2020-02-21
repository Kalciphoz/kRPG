using System.Collections.Generic;
using System.IO;
using kRPG.Content.Items.Weapons.Ranged;
using kRPG.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.Packets
{
    public static class BowInitPacket
    {
        public static void Read(BinaryReader reader)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                int itemId = reader.ReadInt32();
                RangedWeapon bow = (RangedWeapon) Main.item[itemId].modItem;
                bow.dps = reader.ReadSingle();
                bow.enemyDef = reader.ReadInt32();
                bow.Initialize();
            }
        }

        public static void Write(int whoAmI, float dps, int enemyDef)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = kRPG.Mod.GetPacket();
                packet.Write((byte) Message.BowInit);
                packet.Write(whoAmI);
                packet.Write(dps);
                packet.Write(enemyDef);
                packet.Send();
            }
        }
    }
}