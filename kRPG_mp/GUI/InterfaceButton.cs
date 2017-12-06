using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;

namespace kRPG_mp.GUI
{
    public class InterfaceButton
    {
        private Func<Rectangle> position;
        private Action<Player> pressAction;
        private Action<SpriteBatch> hoverAction;

        private bool hoverActionSet = false;

        public InterfaceButton(Func<Rectangle> position, Action<Player> pressAction)
        {
            this.position = position;
            this.pressAction = pressAction;
        }

        public InterfaceButton(Func<Rectangle> position, Action<Player> pressAction, Action<SpriteBatch> hoverAction)
        {
            this.position = position;
            this.pressAction = pressAction;
            this.hoverAction = hoverAction;
            hoverActionSet = true;
        }

        public virtual void Update(SpriteBatch spriteBatch, Player player)
        {
            if (position().Contains(Main.mouseX, Main.mouseY))
            {
                Main.LocalPlayer.mouseInterface = true;

                if (hoverActionSet)
                    hoverAction(spriteBatch);

                if (Main.mouseLeft && Main.mouseLeftRelease)
                    pressAction(player);
            }
        }
    }
}
