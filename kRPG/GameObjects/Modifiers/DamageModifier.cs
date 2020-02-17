using System;
using System.IO;
using kRPG.GameObjects.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.GameObjects.Modifiers
{
    public class DamageModifier : NpcModifier
    {
        public DamageModifier() { }
        public DamageModifier(kNPC kNpc, NPC npc) : base(kNpc, npc)
        {
            this.npc = npc;
            
        }

        private float DmgModifier { get; set; }

        public override void Initialize()
        {
            DmgModifier = Main.rand.Next(0, 200) / 100.0f;
            kRPG.LogMessage("Initializing Damage Modifier: " + DmgModifier);
        }

        public override void Apply()
        {
            npc.GivenName = "Brutal " + npc.FullName;
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
#if DEBUG
            kRPG.LogMessage("Reading DamageModifier: " + DmgModifier.ToString("F"));
#endif
            return 4;
        }

        public override int Pack(ModPacket packet)
        {
            packet.Write(DmgModifier);
            return 4;
        }
    }
}