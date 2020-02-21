﻿using System;
using System.IO;
using kRPG.Content.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Content.Modifiers
{
    public class SpeedModifier : NpcModifier
    {
        public SpeedModifier() { }
        public SpeedModifier(kNPC kNpc, NPC npc) : base(kNpc, npc)
        {
            this.npc = npc;
            Knpc = kNpc;
            
            
        }

        private kNPC Knpc { get; }
        private float SpeedModifierAdj { get; set; }

       

        public override void Initialize()
        {
            SpeedModifierAdj = 1f + Main.rand.NextFloat(.8f);
            kRPG.LogMessage("Initializing Speed Modifer: " + SpeedModifierAdj);
            
            npc.netUpdate = true;
        }

        public override void Apply()
        {
            AddNamePrefix("Swift");
            npc.GetGlobalNPC<kNPC>().SpeedModifier *= (float)Math.Pow(SpeedModifierAdj, 0.9);

            
        }

        public new static NpcModifier New(kNPC kNpc, NPC npc)
        {
            return new SpeedModifier(kNpc, npc);
        }

        //public new static NpcModifier Random(kNPC kNpc, NPC npc)
        //{
        //    return new SpeedModifier(kNpc, npc, 1f + Main.rand.NextFloat(2));
        //}

        public override int Unpack(BinaryReader reader)
        {
            SpeedModifierAdj = reader.ReadSingle();
#if DEBUG
            kRPG.LogMessage("Reading SpeedModifierAdj: " + SpeedModifierAdj.ToString("F"));
#endif
            return 4;
        }

        public override int Pack(ModPacket packet)
        {
            packet.Write(SpeedModifierAdj);
            return 4;
        }
    }
}