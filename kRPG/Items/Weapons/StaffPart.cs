using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace kRPG.Items.Weapons
{
    public class StaffPart
    {
        public Texture2D texture;
        public Vector2 origin;

        public Point GetDrawOrigin(Point staffSize, Point staffOrigin, Point combinedSize)
        {
            return this is Staff ? new Point(0, combinedSize.Y - texture.Height) : new Point((int)(staffOrigin.X - origin.X), (int)(combinedSize.Y - staffSize.Y + staffOrigin.Y - origin.Y));
        }
    }
}
