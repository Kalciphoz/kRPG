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
        public SizeModifier() { }
        public SizeModifier(kNPC kNpc, NPC npc) : base(kNpc, npc)
        {
            this.npc = npc;
        }

        //   private float LifeModifier { get; set; }
        public float ScaleModifier { get; private set; }


        

        public override void Initialize()
        {
            ScaleModifier = Main.rand.NextFloat(.75f, 2.0f);
            npc.life = npc.lifeMax = (int)(npc.life * (ScaleModifier));
            npc.GetGlobalNPC<kNPC>().SpeedModifier *= (float)Math.Pow(ScaleModifier, 0.9);
            
            
            kRPG.LogMessage($"Initializing Size Modifier, Scale: {ScaleModifier} ");
        }

        public override void Apply()
        {
            npc.scale *= ScaleModifier;
            if (ScaleModifier > 1.5)
                AddNamePrefix("Giant");
            else if (ScaleModifier < 1)
                AddNamePrefix("Small");
            else
                AddNamePrefix("Massive");
        }

        public new static NpcModifier New(kNPC kNpc, NPC npc)
        {
            return new SizeModifier(kNpc, npc);
        }

        public override int Unpack(BinaryReader reader)
        {
            ScaleModifier = reader.ReadSingle();
#if DEBUG
            kRPG.LogMessage("Reading ScaleModifier: " + ScaleModifier.ToString("F"));
#endif
            return 8;
        }

        public override int Pack(ModPacket packet)
        {
            packet.Write(ScaleModifier);
            return 8;
        }
    }
}