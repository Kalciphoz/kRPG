using System.Collections.Generic;
using kRPG.Enums;

namespace kRPG.GameObjects.Items.Glyphs
{
    public class Cross : Glyph
    {
        public virtual Dictionary<Element, float> EleDmg { get; set; } =
            new Dictionary<Element, float> {{Element.Fire, 0}, {Element.Cold, 0}, {Element.Lightning, 0}, {Element.Shadow, 0}};

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Generic Cross Glyph; Please Ignore");
        }
    }
}