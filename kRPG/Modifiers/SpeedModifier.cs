using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Modifiers
{
    public class SpeedModifier : NPCModifier
    {
        private float speedModifier = 1f;
        private kNPC kn;
        
        public SpeedModifier(kNPC kNPC, NPC npc, float speedModifier = 1.8f) : base(kNPC, npc)
        {
            this.npc = npc;
            kn = kNPC;
            npc.GivenName = "Swift " + npc.FullName;
            this.speedModifier = speedModifier;
        }

        public override void Apply()
        {
            kn.speedModifier = speedModifier;
        }

        public override void Write(ModPacket packet)
        {
            packet.Write(speedModifier);
        }

        public override void Read(BinaryReader reader)
        {
            speedModifier = reader.ReadSingle();
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            int i = Math.Abs(((int)(Main.time * 15) % 255) - 127);
            drawColor = new Color(127 + i, 255, 127 + i);
        }

        public new static NPCModifier Random(kNPC kNPC, NPC npc)
        {
            return new SpeedModifier(kNPC, npc, 1f+ Main.rand.NextFloat(2));
        }

        public new static NPCModifier New(kNPC kNPC, NPC npc)
        {
            return new SpeedModifier(kNPC, npc);
        }
    }
}