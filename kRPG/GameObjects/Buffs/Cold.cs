using kRPG.Enums;
using kRPG.GameObjects.NPCs;
using kRPG.GameObjects.Players;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.GameObjects.Buffs
{
    /// <summary>
    ///     The cold buff
    /// </summary>
    public class Cold : ModBuff
    {
        /// <summary>
        ///     Default Values
        /// </summary>
        public override void SetDefaults()
        {
            //Name shown when in effect
            DisplayName.SetDefault("Chilled");
            //Description of effect
            Description.SetDefault("Slowed movement and increased chance to receive critical hits");
            //This buff is a debuff
            Main.debuff[Type] = true;
            //Don't save the effect on the player when they leave the game.
            Main.buffNoSave[Type] = true;
        }

        /// <summary>
        ///     Allows you to make this buff give certain effects to the given NPC. If you remove the buff from the NPC, make sure
        ///     to decrement the buffIndex parameter by 1.
        /// </summary>
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<kNPC>().HasAilment[Element.Cold] = true;
            if (npc.velocity.Length() > 0.2f && !npc.boss)
            {
                npc.velocity.Normalize();
                npc.velocity *= 0.2f;
            }
            else if (npc.velocity.Length() > 6f)
            {
                npc.velocity.Normalize();
                npc.velocity *= 6f;
            }
        }

        /// <summary>
        ///     Allows you to make this buff give certain effects to the given player. If you remove the buff from the player, make
        ///     sure the decrement the buffIndex parameter by 1.
        /// </summary>
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<PlayerCharacter>().HasAilment[Element.Cold] = true;
            if (player.velocity.X > player.maxRunSpeed * .6)
                player.velocity.X = player.maxRunSpeed * .6f;
        }
    }
}