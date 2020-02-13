using System.Collections.Generic;
using System.Globalization;
using System.IO;

using kRPG.Enums;
using kRPG.GameObjects.Items.Weapons.Melee;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Packets
{
    public static class SwordInitPacket
    {
        public static void Read( BinaryReader reader)
        {
            if (Main.netMode == 1)
            {
                ProceduralSword sword = (ProceduralSword) Main.item[reader.ReadInt32()].modItem;
                sword.Blade = SwordBlade.Blades[reader.ReadInt32()];
                sword.Hilt = SwordHilt.Hilts[reader.ReadInt32()];
                sword.Accent = SwordAccent.Accents[reader.ReadInt32()];
                sword.Dps = reader.ReadSingle();
                Main.NewText(sword.Dps.ToString(CultureInfo.InvariantCulture));
                sword.EnemyDef = reader.ReadInt32();
                sword.Initialize();
            }
        }

        public static void Write(int id, int bladeType, int hiltType, int accentType, float dps, int enemyDef)
        {
            if (Main.netMode == 2)
            {
                ModPacket packet = kRPG.Mod.GetPacket();
                packet.Write((byte) Message.SwordInit);
                packet.Write(id);
                packet.Write(bladeType);
                packet.Write(hiltType);
                packet.Write(accentType);
                packet.Write(dps);
                packet.Write(enemyDef);
                packet.Send();
            }
        }
    }
}