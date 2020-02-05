using kRPG.Enums;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Buffs
{
    /// <summary>
    ///     Fire Debuff
    /// </summary>
    public class Fire : ModBuff
    {
        /// <summary>
        ///     Default Values
        /// </summary>
        public override void SetDefaults()
        {
            //Name shown when in effect
            DisplayName.SetDefault("Burning");
            //Description of effect
            Description.SetDefault("Taking fire damage over time");
            //This buff is a debuff
            Main.debuff[Type] = true;
            //Don't save the effect on the player when they leave the game.
            Main.buffNoSave[Type] = true;
        }

        /// <summary>
        ///     Update NPC
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="buffIndex"></param>
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<kNPC>().HasAilment[Element.Fire] = true;
        }

        /// <summary>
        ///     Update Player
        /// </summary>
        /// <param name="player"></param>
        /// <param name="buffIndex"></param>
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<PlayerCharacter>().HasAilment[Element.Fire] = true;
        }
    }
}