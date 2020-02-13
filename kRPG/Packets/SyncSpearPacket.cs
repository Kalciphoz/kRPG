using System.Collections.Generic;
using System.IO;

using kRPG.Enums;
using kRPG.GameObjects.Items.Weapons.Melee;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Packets
{
    public static class SyncSpearPacket
    {
        public static void Read( BinaryReader reader)
        {
            if (Main.netMode == 1)
            {
                ProceduralSpear spear = (ProceduralSpear) Main.projectile[reader.ReadInt32()].modProjectile;
                spear.Blade = SwordBlade.Blades[reader.ReadInt32()];
                spear.Hilt = SwordHilt.Hilts[reader.ReadInt32()];
                spear.Accent = SwordAccent.Accents[reader.ReadInt32()];
                spear.Initialize();
            }
        }

        public static void Write(int bladeType, int hiltType, int accentType)
        {
            if (Main.netMode == 2)
            {
                ModPacket packet = kRPG.Mod.GetPacket();
                packet.Write((byte) Message.SyncSpear);
                packet.Write(bladeType);
                packet.Write(hiltType);
                packet.Write(accentType);
                packet.Send();
            }
        }
    }
}