using Terraria;
using Terraria.ModLoader;

namespace kRPG.GameObjects.Buffs
{
    public class Physical : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Stunned");
            Description.SetDefault("Halted all movement");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.velocity *= 0.8f;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.velocity *= 0.8f;
            player.noItems = true;
        }
    }
}