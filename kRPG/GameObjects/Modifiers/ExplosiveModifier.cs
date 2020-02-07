using System.IO;
using kRPG.Enums;
using kRPG.GameObjects.Items.Projectiles;
using kRPG.GameObjects.NPCs;
using kRPG.GameObjects.SFX;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace kRPG.GameObjects.Modifiers
{
    public class ExplosiveModifier : NpcModifier
    {
        public ExplosiveModifier(kNPC kNpc, NPC npc, float lifeModifier = 0.5f) : base(kNpc, npc)
        {
            this.npc = npc;
            npc.GivenName = "Explosive " + npc.GivenName;
            Apply();
        }

        private float LifeModifier { get; set; } = 0.5f;

        public override void Apply()
        {
            npc.lifeMax = (int) (npc.lifeMax * LifeModifier);
            npc.life = (int) (npc.life * LifeModifier);
        }

        public new static NpcModifier New(kNPC kNpc, NPC npc)
        {
            return new ExplosiveModifier(kNpc, npc);
        }

        public override void NPCLoot(NPC npc)
        {
            SoundManager.PlaySound(Sounds.LegacySoundStyle_Item14, npc.Center, .5f);
            //Main.PlaySound(new LegacySoundStyle(2, 14).WithVolume(0.5f), npc.Center);
            Projectile proj = Main.projectile[
                Projectile.NewProjectile(npc.Center - new Vector2(16, 32), Vector2.Zero, ModContent.ProjectileType<NpcExplosion>(), npc.damage * 5 / 4, 0f)];
        }

        public new static NpcModifier Random(kNPC kNpc, NPC npc)
        {
            return new ExplosiveModifier(kNpc, npc, Main.rand.NextFloat(0.5f, 0.9f));
        }

        public override void Read(BinaryReader reader)
        {
            LifeModifier = reader.ReadSingle();
        }

        public override void Write(ModPacket packet)
        {
            packet.Write(LifeModifier);
        }
    }
}