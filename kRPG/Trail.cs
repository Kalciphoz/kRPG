using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace kRPG
{
    public class Trail
    {
        private Vector2 position;
        private Action<SpriteBatch, Player, Vector2, Vector2[], float> draw;
        private int timeleft;
        public Vector2[] displacement;
        public float scale = 1f;

        public Trail(Vector2 position, int timeleft, Action<SpriteBatch, Player, Vector2, Vector2[], float> draw)
        {
            this.position = position;
            this.timeleft = timeleft;
            this.draw = draw;
            scale = 1f;
        }

        public void Draw(SpriteBatch spritebatch, Player player)
        {
            timeleft -= 1;
            for (int i = 0; i < displacement.Length; i += 1)
                displacement[i] += new Vector2(0.6f, 0f).RotatedBy(displacement[i].ToRotation());
            draw(spritebatch, player, position, displacement, scale);
            scale -= 0.01f;
            if (timeleft <= 0)
                player.GetModPlayer<PlayerCharacter>().trails.Remove(this);
        }
    }
}
