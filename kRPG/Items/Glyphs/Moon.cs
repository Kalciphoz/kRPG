using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace kRPG.Items.Glyphs
{
    public class Moon : Glyph
    {
        public int ProjCount { get; set; } = 5;

        public override ModItem Clone(Item tItem)
        {
            Moon copy = (Moon) base.Clone(tItem);
            copy.ProjCount = ProjCount;
            return copy;
        }

        public override void Load(TagCompound tag)
        {
            base.Load(tag);
            ProjCount = tag.GetInt("projCount");
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(mod, "projCount", ProjCount + " Projectiles"));
        }

        public override void NetRecieve(BinaryReader reader)
        {
            base.NetRecieve(reader);
            ProjCount = reader.ReadInt32();
        }

        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
            writer.Write(ProjCount);
        }

        public override TagCompound Save()
        {
            TagCompound compound = base.Save();
            compound.Add("projCount", ProjCount);
            return compound;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Generic Moon Glyph; Please Ignore");
        }
    }
}