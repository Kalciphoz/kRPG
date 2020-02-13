using System;
using kRPG.GameObjects.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace kRPG.GameObjects.NPCs
{
    public class Trail
    {
        public Trail(Vector2 position, int timeLeft, Action<SpriteBatch, Player, Vector2, Vector2[], float> draw)
        {
            Position = position;
            TimeLeft = timeLeft;
            this.draw = draw;
            Scale = 1f;
        }

        public Vector2[] Displacement { get; set; }

        private Action<SpriteBatch, Player, Vector2, Vector2[], float> draw { get; }

        private Vector2 Position { get; }
        public float Scale { get; set; } = 1f;
        private int TimeLeft { get; set; }

        public void Draw(SpriteBatch spriteBatch, Player player)
        {
            TimeLeft -= 1;
            for (int i = 0; i < Displacement.Length; i += 1)
                Displacement[i] += new Vector2(0.6f, 0f).RotatedBy(Displacement[i].ToRotation());
            draw(spriteBatch, player, Position, Displacement, Scale);
            Scale -= 0.01f;
            if (TimeLeft <= 0)
                player.GetModPlayer<PlayerCharacter>().Trails.Remove(this);
        }
    }
}