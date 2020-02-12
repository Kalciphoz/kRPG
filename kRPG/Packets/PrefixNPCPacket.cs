using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using kRPG.Enums;
using kRPG.GameObjects.Modifiers;
using kRPG.GameObjects.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Packets
{
    public static class PrefixNPCPacket
    {
        public static void Read(BinaryReader reader)
        {
            if (Main.netMode == 1)
            {
                int npcIndex = reader.ReadInt32();
                int amount = reader.ReadInt32();//4
                if (Main.npc.GetUpperBound(0) >= npcIndex)
                {
                    NPC npc = Main.npc[npcIndex];
                    kNPC kNpc = npc.GetGlobalNPC<kNPC>();
                    for (int i = 0; i < amount; i++)
                    {
                        int modIndex = reader.ReadInt32(); //4
                        NpcModifier modifier = kNpc.ModifierFuncs[modIndex].Invoke(kNpc, npc);
                        modifier.Apply();
                        kNpc.Modifiers.Add(modifier);
                    }

                    kNpc.MakeNotable(npc);
                }
                else
                {
                    kRPG.LogMessage("NPC index does not exist!");
                }
            }
        }
   
        public static void Write(NPC npc, int amount, List<NpcModifier> modifiers)
        {
            if (Main.netMode == 2)
            {
                ModPacket packet = kRPG.Mod.GetPacket();
                packet.Write((byte)Message.PrefixNpc);//1
                packet.Write(npc.whoAmI);//4
                packet.Write(amount);//4
                if (amount > 0)
                {
                    for (int i = 0; i < amount; i++)
                    {
                        int modIndex = 0;
                        for (int ii = 0; ii < kNPC.ModifierDictionary.Length; ii++)
                        {
                            if (kNPC.ModifierDictionary[ii] != modifiers[i].GetType().AssemblyQualifiedName)
                                continue;
                            modIndex = ii;
                            break;
                        }
                        packet.Write(modIndex); //4
                        modifiers[i].Pack(packet);//?
                    }
                }

                packet.Send();
            }
        }
    }
}