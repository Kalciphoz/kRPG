using System;
using System.IO;
using System.Runtime.Remoting.Messaging;
using kRPG.GameObjects.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

// ReSharper disable IdentifierTypo

namespace kRPG.GameObjects.Modifiers
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
            npc.GivenName = "Shimmering " + npc.FullName;
        }

        //private float RegenTimer { get; set; }

        public new static NpcModifier New(kNPC kNpc, NPC npc)
        {
            return new LifeRegenModifier(kNpc, npc);
        }

        public override void Update(NPC kNpc)
        {
            /*
             * Side note, we want to regen 5% of there health every 10 seconds, (or in terraria terms, 10 frames.)
             * This function gets called each frame.
             * So we increment the counter and each time it hits 15 we apply the regen.
             */

            //RegenTimer += 1;

            ////Calculate what 5% of their possible max life is, that is what they will regenerate.
            //int amountToRegenerated = (int)(npc.lifeMax * .05f);

            ////If they are dead, leave them dead.
            //if (npc.life <= 0)
            //    return;


            ////If the amount of life we regen is less than there current life, we do nothing.
            //if (npc.life < amountToRegenerated)
            //    return;

            ////ok 10 frames has passed, let's apply regeneration!
            //if (RegenTimer > 10)
            //{
            //    int newHealth = kNpc.life + amountToRegenerated;
            //    if (newHealth > kNpc.lifeMax)
            //        newHealth = kNpc.lifeMax;
            //    npc.life = newHealth;
            //    RegenTimer = 0;
            //}


            //int amount = kNpc.lifeMax / 20;//20
            //if (!(RegenTimer > 60f / amount))
            //    return;
            //kNpc.life = Math.Min(kNpc.life + (int) (RegenTimer / (60f / amount)), kNpc.lifeMax);
            //RegenTimer %= 60f / amount;
        }

        public override int Unpack(BinaryReader reader)
        {
//            RegenTimer = reader.ReadSingle();
//#if DEBUG
//            kRPG.LogMessage("Reading RegenTimer: " + RegenTimer.ToString("F"));
//#endif
//            return 4;
            return 0;
        }

        public override int Pack(ModPacket packet)
        {
            //packet.Write(RegenTimer);
            //return 4;
            return 0;
        }

        public override void Initialize()
        {
            //RegenTimer = 0;
        }

        public override void OnHitByProjectile(NPC oNpc, Projectile projectile, int damage, float knockBack, bool crit)
        {
            base.OnHitByProjectile(oNpc, projectile, damage, knockBack, crit);
        }
    }
}