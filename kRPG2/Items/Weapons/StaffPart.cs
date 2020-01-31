using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace kRPG2.Items.Weapons
{
    public class StaffPart
    {
        public Vector2 Origin { get; set; }
        public Texture2D Texture { get; set; }

        public Point GetDrawOrigin(Point staffSize, Point staffOrigin, Point combinedSize)
        {
            return this is Staff
                ? new Point(0, combinedSize.Y - Texture.Height)
                : new Point((int) (staffOrigin.X - Origin.X), (int) (combinedSize.Y - staffSize.Y + staffOrigin.Y - Origin.Y));
        }
    }
}