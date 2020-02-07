using System.IO;
using kRPG.Enums;
using kRPG.GameObjects.Items.Projectiles;
using kRPG.GameObjects.NPCs;
using kRPG.GameObjects.SFX;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.GameObjects.Modifiers
{
    public class ExplosiveModifier : NpcModifier
    {
        public ExplosiveModifier(kNPC kNpc, NPC npc, float lifeModifier = 0.5f) : base(kNpc, npc)
        {
            this.npc = npc;
            npc.GivenName = "Explosive " + npc.GivenName;
            //Apply(); Virtual Call in constructor is bad.
            npc.lifeMax = (int)(npc.lifeMax * LifeModifier);
            npc.life = (int)(npc.life * LifeModifier);
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

        public override void NpcLoot(NPC oNpc)
        {
            SoundManager.PlaySound(Sounds.LegacySoundStyle_Item14, oNpc.Center, .5f);
            //Main.PlaySound(new LegacySoundStyle(2, 14).WithVolume(0.5f), oNpc.Center);
            Projectile proj = Main.projectile[Projectile.NewProjectile(oNpc.Center - new Vector2(16, 32), Vector2.Zero, ModContent.ProjectileType<NpcExplosion>(), oNpc.damage * 5 / 4, 0f)];
        }

        //public new static NpcModifier Random(kNPC kNpc, NPC npc)
        //{
        //    return new ExplosiveModifier(kNpc, npc, Main.rand.NextFloat(0.5f, 0.9f));
        //}

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