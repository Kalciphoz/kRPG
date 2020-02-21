using System.IO;
using kRPG.Content.NPCs;
using Terraria;
using Terraria.ModLoader;

// ReSharper disable IdentifierTypo

namespace kRPG.Content.Modifiers
{
    public class LifeRegenModifier : NpcModifier
    {
        public LifeRegenModifier() { }
        public LifeRegenModifier(kNPC kNpc, NPC npc) : base(kNpc, npc)
        {
            this.npc = npc;
        }

        public override void Apply()
        {
            AddNamePrefix("Shimmering");

        }

        public new static NpcModifier New(kNPC kNpc, NPC npc)
        {
            return new LifeRegenModifier(kNpc, npc);
        }

        public override void Update(NPC kNpc)
        {
           
        }

        public override int Unpack(BinaryReader reader)
        {
            return 0;
        }

        public override int Pack(ModPacket packet)
        {
            return 0;
        }

        public override void Initialize()
        {
            npc.life *= 2;
            npc.lifeMax *= 2;
            npc.lifeRegen = 20;
        }

        public override void OnHitByProjectile(NPC oNpc, Projectile projectile, int damage, float knockBack, bool crit)
        {
            base.OnHitByProjectile(oNpc, projectile, damage, knockBack, crit);
        }
    }
}