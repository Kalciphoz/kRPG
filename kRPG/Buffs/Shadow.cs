using Terraria;
using Terraria.ModLoader;

namespace kRPG.Buffs
{
    public class Shadow : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Doomed");
            Description.SetDefault("Deal less damage");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<kNPC>().hasAilment[ELEMENT.SHADOW] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<PlayerCharacter>().hasAilment[ELEMENT.SHADOW] = true;
        }
    }
}