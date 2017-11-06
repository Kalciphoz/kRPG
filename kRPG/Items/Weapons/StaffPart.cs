using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kRPG.Items.Weapons
{
    public class StaffPart
    {
        public Texture2D texture;
        public Vector2 origin;

        public Point GetDrawOrigin(Point staffSize, Point staffOrigin, Point combinedSize)
        {
            if (this is Staff)
                return new Point(0, combinedSize.Y - texture.Height);
            else
                return new Point((int)(staffOrigin.X - origin.X), (int)(combinedSize.Y - staffSize.Y + staffOrigin.Y - origin.Y));
        }
    }
}
