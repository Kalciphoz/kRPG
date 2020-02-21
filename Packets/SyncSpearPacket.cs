//using System;
//using System.Collections.Generic;
//using System.IO;

//using kRPG.Enums;
//using kRPG.GameObjects.Items.Weapons.Melee;
//using Terraria;
//using Terraria.ModLoader;

//namespace kRPG.Packets
//{
//    public static class SyncSpearPacket
//    {
//        /// <summary>
//        /// Called from server
//        /// </summary>
//        /// <param name="reader"></param>
//        public static void Read(BinaryReader reader)
//        {

//            //if (Main.netMode == NetmodeID.MultiplayerClient)
//            {
//                var projectileIdx = reader.ReadInt32();
//                int blade = reader.ReadInt32();
//                int hilt = reader.ReadInt32();
//                int accent = reader.ReadInt32();

//                kRPG.LogMessage($"SyncSpearPacket: Read: ID: {projectileIdx} Blade: {blade} Hilt: {hilt} Accent: {accent}");

//                try
//                {
//                    var oSpear = (ProceduralSpear) Main.projectile[projectileIdx].modProjectile;
//                    oSpear.Blade = SwordBlade.Blades[blade];
//                    oSpear.Hilt = SwordHilt.Hilts[hilt];
//                    oSpear.Accent = SwordAccent.Accents[accent];
//                    oSpear.Initialize();
//                }
//                catch (Exception e)
//                {
//                    kRPG.LogMessage("--------------------------------->Cannot find projectile: " + projectileIdx);
//                }
//            }
//        }

//        //Called From Client
//        public static void Write(int idx, int bladeType, int hiltType, int accentType)
//        {
//            //if (Main.netMode == NetmodeID.Server)
//            {
//                ModPacket packet = kRPG.Mod.GetPacket();
//                packet.Write((byte)Message.SyncSpear);
//                packet.Write(idx);
//                packet.Write(bladeType);
//                packet.Write(hiltType);
//                packet.Write(accentType);
//                packet.Send();
//            }
//        }
//    }
//}