using System.IO;
using kRPG.Content.Items.Projectiles;
using kRPG.Content.NPCs;
using kRPG.Content.SFX;
using kRPG.Enums;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.Content.Modifiers
{
    public class ExplosiveModifier : NpcModifier
    {
        private kNPC kNpc { get; set; }

        public ExplosiveModifier() { }


        public ExplosiveModifier(kNPC kNpc, NPC npc) : base(kNpc, npc)
        {
            this.npc = npc;
            this.kNpc = kNpc;

        }

        private float LifeModifier { get; set; } = 0.5f;

        public override void Initialize()
        {
            LifeModifier = Main.rand.NextFloat(0.5f, 0.9f);
            npc.lifeMax = (int)(npc.lifeMax * LifeModifier);
            npc.life = (int)(npc.life * LifeModifier);
            kRPG.LogMessage("Initializing Explosive Modifier: " + LifeModifier);
        }

        public override void Apply()
        {
            AddNamePrefix("Explosive");
            

        }

        public new static NpcModifier New(kNPC kNpc, NPC npc)
        {
            return new ExplosiveModifier(kNpc, npc);
        }

        public override void NpcLoot(NPC oNpc)
        {
            SoundManager.PlaySound(Sounds.LegacySoundStyle_Item14, oNpc.Center, .5f);
            //Projectile proj = Main.projectile[Projectile.NewProjectile(oNpc.Center - new Vector2(16, 32), Vector2.Zero, ModContent.ProjectileType<NpcExplosion>(), oNpc.damage * 5 / 4, 0f)];
            if (Main.netMode == NetmodeID.Server || Main.netMode == NetmodeID.SinglePlayer)
            {
                Projectile proj = Main.projectile[Projectile.NewProjectile(oNpc.Center - new Vector2(16, 32), Vector2.Zero, ModContent.ProjectileType<NpcExplosion>(), oNpc.damage, 0f)];
            }
        }

        public override int Unpack(BinaryReader reader)
        {
            LifeModifier = reader.ReadSingle();
#if DEBUG
            kRPG.LogMessage("Reading LifeModifier: " + LifeModifier.ToString("F"));
#endif
            return 4;
        }

        public override int Pack(ModPacket packet)
        {
            packet.Write(LifeModifier);
            return 4;
        }
    }
}