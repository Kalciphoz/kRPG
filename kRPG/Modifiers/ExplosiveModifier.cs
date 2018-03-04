using System.IO;
using kRPG.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace kRPG.Modifiers
{
    public class ExplosiveModifier : NPCModifier
    {
        private float lifeModifier = 0.5f;
        private kNPC kNPC;
        
        public ExplosiveModifier(kNPC kNPC, NPC npc, float lifeModifier = 0.5f) : base(kNPC, npc)
        {
            this.npc = npc;
            npc.GivenName = "Explosive " + npc.FullName;
            this.kNPC = kNPC;
            Apply();
        }

        public override void Apply()
        {
            npc.lifeMax = (int)(npc.lifeMax * lifeModifier);
            npc.life = (int)(npc.life * lifeModifier);
        }

        public override void NPCLoot(NPC npc)
        {
            Main.PlaySound(new LegacySoundStyle(2, 14, Terraria.Audio.SoundType.Sound).WithVolume(0.5f), npc.Center);
            Projectile proj = Main.projectile[Projectile.NewProjectile(npc.Center - new Vector2(16, 32), Vector2.Zero, kNPC.mod.ProjectileType<NPC_Explosion>(), npc.damage * 5 / 4, 0f)];
        }

        public override void Write(ModPacket packet)
        {
            packet.Write(lifeModifier);
        }

        public override void Read(BinaryReader reader)
        {
            lifeModifier = reader.ReadSingle();
        }

        public new static NPCModifier Random(kNPC kNPC, NPC npc)
        {
            return new ExplosiveModifier(kNPC, npc, Main.rand.NextFloat(0.5f, 0.9f));
        }

        public new static NPCModifier New(kNPC kNPC, NPC npc)
        {
            return new ExplosiveModifier(kNPC, npc);
        }
    }
}