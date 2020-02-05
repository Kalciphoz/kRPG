using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace kRPG
{
    public class Trail
    {
        public Trail(Vector2 position, int timeleft, Action<SpriteBatch, Player, Vector2, Vector2[], float> draw)
        {
            Position = position;
            Timeleft = timeleft;
            this.draw = draw;
            Scale = 1f;
        }

        public Vector2[] Displacement { get; set; }

        private Action<SpriteBatch, Player, Vector2, Vector2[], float> draw { get; }

        private Vector2 Position { get; }
        public float Scale { get; set; } = 1f;
        private int Timeleft { get; set; }

        public void Draw(SpriteBatch spritebatch, Player player)
        {
            Timeleft -= 1;
            for (int i = 0; i < Displacement.Length; i += 1)
                Displacement[i] += new Vector2(0.6f, 0f).RotatedBy(Displacement[i].ToRotation());
            draw(spritebatch, player, Position, Displacement, Scale);
            Scale -= 0.01f;
            if (Timeleft <= 0)
                player.GetModPlayer<PlayerCharacter>().Trails.Remove(this);
        }
    }
}