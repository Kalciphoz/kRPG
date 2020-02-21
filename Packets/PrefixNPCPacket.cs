﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using kRPG.Content.Modifiers;
using kRPG.Content.NPCs;
using kRPG.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.Packets
{
    public static class PrefixNPCPacket
    {





        public static void Read(BinaryReader reader)
        {

            try
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    int refNum = reader.ReadInt32();
                    int npcIndex = reader.ReadInt32();
                    int amount = reader.ReadInt32();

                    // kRPG.LogMessage($"Ref Num: {refNum} WhoAmI: {npcIndex} Amount: {amount}");

                    if (Main.npc[npcIndex].netID == 0)
                    {
                        //So we have a problem...
                        //The mob is on the server but not on the client.
                        //We need to clear out the buffer to prevent an error...
                        //The thing that bothers me is that in practice this shouldn't happen.

                        kRPG.LogMessage("WARNING: NPC does not exist.");

                        string mods = "";
                        //Well... this should clear out the network stack.
                        for (int i = 0; i < amount; i++)
                        {
                            int modIndex = reader.ReadInt32();
                            mods += " " + modIndex;
                            Type t = Type.GetType(ModiferFunctions.Instance.Modifiers[modIndex].ClassName);
                                
                            NpcModifier modifier = (NpcModifier)Activator.CreateInstance(t);
                            modifier.Unpack(reader);
                        }
                        kRPG.LogMessage($"RefNum: {refNum} NpcIndex: {npcIndex} Amount: {amount} Mods: {mods}");


                    }
                    else
                    {
                        NPC npc = Main.npc[npcIndex];

                        kNPC kNpc = npc.GetGlobalNPC<kNPC>();
                        kNpc.Modifiers.Clear();

                        for (int i = 0; i < amount; i++)
                        {
                            int modIndex = reader.ReadInt32();
                            NpcModifier modifier;

                            if (kNpc.Modifiers.ContainsKey(modIndex))
                            {
                                modifier = kNpc.Modifiers[modIndex];
                            }
                            else
                            {
                                modifier = ModiferFunctions.Instance.Modifiers[modIndex].Function.Invoke(kNpc, npc);
                            }

                            modifier.Unpack(reader);
                            modifier.Apply();
                            kNpc.Modifiers.Add(modIndex, modifier);
                        }

                        //Hrm, not sure why this is here.....
                        //kNpc.MakeNotable(npc);
                    }

                }
            }
            catch (Exception err)
            {
                kRPG.LogMessage("Error: " + err);
            }
        }

        public static void Write(NPC npc, Dictionary<int, NpcModifier> modifiers)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                int bytes = 0;
                ModPacket packet = kRPG.Mod.GetPacket();
                packet.Write((byte)Message.PrefixNpc);//1
                bytes += 1;



                int refNum = new Random().Next(0, int.MaxValue);
                packet.Write(refNum);
                bytes += 4;

                packet.Write(npc.whoAmI);
                bytes += 4;

                packet.Write(modifiers.Count);
                bytes += 4;

                string ModIds = "";

                if (modifiers.Count > 0)
                {
                    foreach (KeyValuePair<int, NpcModifier> modifier in modifiers)
                    {
                        //int modIndex = 0;
                        //for (int ii = 0; ii < kNPC.modifierDictionary.Length; ii++)
                        //{
                        //    if (kNPC.modifierDictionary[ii] != modifier.GetType().AssemblyQualifiedName)
                        //        continue;
                        //    ModIds += " " + ii;
                        //    modIndex = ii;
                        //    break;
                        //}
                        packet.Write(modifier.Key);
                        bytes += 4;
                        bytes += modifier.Value.Pack(packet);
                    }
                }
#if DEBUG
                kRPG.LogMessage($"RefId: {refNum} WhoAmI: {npc.whoAmI}  Packet Size: {bytes} Mods: {ModIds}");
#endif
                packet.Send();
            }
        }
    }
}