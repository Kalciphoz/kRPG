using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace kRPG.Items.Glyphs
{
    public class Moon : Glyph
    {
        public int projCount = 5;

        public override ModItem Clone(Item tItem)
        {
            var copy = (Moon) base.Clone(tItem);
            copy.projCount = projCount;
            return copy;
        }

        public override void Load(TagCompound tag)
        {
            base.Load(tag);
            projCount = tag.GetInt("projCount");
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(mod, "projCount", projCount + " Projectiles"));
        }

        public override void NetRecieve(BinaryReader reader)
        {
            base.NetRecieve(reader);
            projCount = reader.ReadInt32();
        }

        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
            writer.Write(projCount);
        }

        public override TagCompound Save()
        {
            var compound = base.Save();
            compound.Add("projCount", projCount);
            return compound;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Generic Moon Glyph; Please Ignore");
        }
    }
}