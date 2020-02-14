using System;
using System.IO;
using kRPG.GameObjects.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.GameObjects.Modifiers
{
    public class DamageModifier : NpcModifier
    {
        public DamageModifier(kNPC kNpc, NPC npc, float dmgModifier = 1.2f) : base(kNpc, npc)
        {
            this.npc = npc;
            npc.GivenName = "Brutal " + npc.GivenName;
            DmgModifier = dmgModifier;
            if (Main.netMode == Constants.NetModes.Client)
                return;
            //Apply(); Virtual calls in constructors is a no-no
            npc.damage = (int)Math.Round(npc.damage * DmgModifier);
            npc.defense = 1;

        }

        private float DmgModifier { get; set; }

        public override void Apply()
        {
            npc.damage = (int) Math.Round(npc.damage * DmgModifier);
            npc.defense = 1;
        }

        public new static NpcModifier New(kNPC kNpc, NPC npc)
        {
            return new DamageModifier(kNpc, npc);
        }

        //public new static NpcModifier Random(kNPC kNpc, NPC npc)
        //{
        //    return new DamageModifier(kNpc, npc, 1f + Main.rand.NextFloat(1));
        //}

        public override int Unpack(BinaryReader reader)
        {
           
            DmgModifier = reader.ReadSingle();
            kRPG.LogMessage("Reading DamageModifier: " + DmgModifier.ToString("F"));
            return 4;
        }

        public override int Pack(ModPacket packet)
        {
            packet.Write(DmgModifier);
            return 4;
        }
    }
}