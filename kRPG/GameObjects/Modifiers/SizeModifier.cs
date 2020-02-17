using System;
using System.IO;
using kRPG.GameObjects.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.GameObjects.Modifiers
{
    public class SizeModifier : NpcModifier
    {
        public SizeModifier(kNPC kNpc, NPC npc) : base(kNpc, npc)
        {
            this.npc = npc;
        }

        private float LifeModifier { get; set; }
        private float ScaleModifier { get; set; }


        

        public override void Initialize()
        {
            ScaleModifier = Main.rand.NextFloat(.5f, 2.0f);
            LifeModifier = Main.rand.NextFloat(.5f, 1.5f);
            npc.lifeMax = (int)(npc.lifeMax * LifeModifier);
            npc.life = (int)(npc.life * LifeModifier);
            kRPG.LogMessage($"Initializing Size Modifier, Scale: {ScaleModifier} Life Modifier {LifeModifier}");
        }

        public override void Apply()
        {
            npc.scale *= ScaleModifier;
            if (ScaleModifier > 1.5)
            {
                npc.GivenName = "Giant " + npc.FullName;
            }
            else if (ScaleModifier < 1)
                npc.GivenName = "Small " + npc.FullName;
            else
                npc.GivenName = "Massive " + npc.FullName;
            // npc.GetGlobalNPC<kNPC>().SpeedModifier *= (float)Math.Pow(ScaleModifier, 0.9);
        }

        public new static NpcModifier New(kNPC kNpc, NPC npc)
        {
            return new SizeModifier(kNpc, npc);
        }

        //public new static NpcModifier Random(kNPC kNpc, NPC npc)
        //{
        //    return new SizeModifier(kNpc, npc, .5f + Main.rand.NextFloat(2), .5f + Main.rand.NextFloat(1));
        //}

        public override int Unpack(BinaryReader reader)
        {
            ScaleModifier = reader.ReadSingle();
            LifeModifier = reader.ReadSingle();
            npc.lifeMax = reader.ReadInt32();
            npc.life = reader.ReadInt32();
#if DEBUG
            kRPG.LogMessage("Reading ScaleModifier: " + ScaleModifier.ToString("F"));
            kRPG.LogMessage("Reading LifeModifier: " + LifeModifier.ToString("F"));
#endif
            return 8;
        }

        public override int Pack(ModPacket packet)
        {
            packet.Write(ScaleModifier);
            packet.Write(LifeModifier);
            packet.Write(npc.lifeMax);
            packet.Write(npc.life);
            return 8;
        }
    }
}