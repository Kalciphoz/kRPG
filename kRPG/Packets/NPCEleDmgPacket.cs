using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using kRPG.Enums;
using kRPG.GameObjects.NPCs;
using Terraria;

namespace kRPG.Packets
{
    public static class NPCEleDmgPacket
    {
        public static void Read(BinaryReader reader)
        {
            if (Main.netMode == 1)
            {

                NPC npc = Main.npc[reader.ReadInt32()];

                kNPC kn = npc.GetGlobalNPC<kNPC>();

                bool fire = reader.ReadBoolean();
                bool cold = reader.ReadBoolean();
                bool lightning = reader.ReadBoolean();
                bool shadow = reader.ReadBoolean();

                //Dictionary<Element, bool> hasElement = new Dictionary<Element, bool>
                //{
                //    {Element.Fire, (bool) tags[DataTag.Flag]}, {Element.Cold, (bool) tags[DataTag.Flag2]}, {Element.Lightning, (bool) tags[DataTag.Flag3]}, {Element.Shadow, (bool) tags[DataTag.Flag4]}
                //};
                Dictionary<Element, bool> hasElement = new Dictionary<Element, bool> {{Element.Fire, fire}, {Element.Cold, cold}, {Element.Lightning, lightning}, {Element.Shadow, shadow}};
                
                int count = Enum.GetValues(typeof(Element)).Cast<Element>().Count(element => hasElement[element]);

                int portionSize = (int) Math.Round(npc.damage * kNPC.EleDmgModifier / 2.0 / count);

                foreach (Element element in Enum.GetValues(typeof(Element)))
                    if (hasElement[element])
                        kn.ElementalDamage[element] = Math.Max(1, portionSize);

                kn.DealsEleDmg = count > 0;
            }
        }
    }
}