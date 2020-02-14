using System.Collections.Generic;
using System.IO;

using kRPG.Enums;
using kRPG.GameObjects.Items.Weapons.Melee;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Packets
{
    public static class StaffInitPacket
    {
        public static void Read( BinaryReader reader)
        {
            if (Main.netMode == Constants.NetModes.Client)
            {
                ProceduralStaff staff = (ProceduralStaff) Main.item[reader.ReadInt32()].modItem;

                staff.Staff = Staff.Staffs[reader.ReadInt32()];

                staff.Gem = StaffGem.Gems[reader.ReadInt32()];
                staff.Ornament = StaffOrnament.Ornament[reader.ReadInt32()];
                staff.Dps = reader.ReadSingle();
                staff.EnemyDef = reader.ReadInt32();
                staff.Initialize();
            }
        }

        public static void Write(int id, int staffType, int gemType, int ornamentType, float dps, int enemyDef)
        {
            if (Main.netMode == Constants.NetModes.Server)
            {
                ModPacket packet = kRPG.Mod.GetPacket();
                packet.Write((byte) Message.StaffInit);
                packet.Write(id);
                packet.Write(staffType);
                packet.Write(gemType);
                packet.Write(ornamentType);
                packet.Write(dps);
                packet.Write(enemyDef);
                packet.Send();
            }
        }
    }
}