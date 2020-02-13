using System.Collections.Generic;
using kRPG.Enums;
using kRPG.GameObjects.Items.Glyphs;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Packets
{
    public static class CreateProjectilePacket
    {
        public static void Write(int playerWhoAmI, int projectileWhoAmI, int starType, int crossType, int moonType, float damage, bool minion, int casterWhoAmI, List<GlyphModifier> modifiers)
        {
            if (Main.netMode == 1)
            {
                ModPacket packet = kRPG.Mod.GetPacket();
                packet.Write((byte) Message.CreateProjectile);
                packet.Write(playerWhoAmI);
                packet.Write(projectileWhoAmI);
                packet.Write(starType);
                packet.Write(crossType);
                packet.Write(moonType);
                packet.Write(damage);
                packet.Write(minion);
                packet.Write(casterWhoAmI);
                List<GlyphModifier> mods = modifiers;
                packet.Write(mods.Count);
                for (int j = 0; j < mods.Count; j += 1)
                    packet.Write(mods[j].Id);
                packet.Send();
            }
        }
    }
}