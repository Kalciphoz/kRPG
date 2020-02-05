using System.Collections.Generic;
using kRPG.Enums;

namespace kRPG.Items.Glyphs
{
    public class Cross : Glyph
    {
        public virtual Dictionary<ELEMENT, float> EleDmg { get; set; } =
            new Dictionary<ELEMENT, float> {{ELEMENT.FIRE, 0}, {ELEMENT.COLD, 0}, {ELEMENT.LIGHTNING, 0}, {ELEMENT.SHADOW, 0}};

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Generic Cross Glyph; Please Ignore");
        }
    }
}