using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace kRPG
{
    public class InterfaceButton
    {
        private readonly Func<Rectangle> position;
        private readonly Action<Player> pressAction;
        private readonly Action<Player, SpriteBatch> hoverAction;

        private readonly bool hoverActionSet = false;

        public InterfaceButton(Func<Rectangle> position, Action<Player> pressAction)
        {
            this.position = position;
            this.pressAction = pressAction;
        }

        public InterfaceButton(Func<Rectangle> position, Action<Player> pressAction, Action<Player, SpriteBatch> hoverAction)
        {
            this.position = position;
            this.pressAction = pressAction;
            this.hoverAction = hoverAction;
            hoverActionSet = true;
        }

        public virtual void Update(SpriteBatch spriteBatch, Player player)
        {
            if (!position().Contains(Main.mouseX, Main.mouseY))
                return;
            Main.LocalPlayer.mouseInterface = true;

            if (hoverActionSet)
                hoverAction(player, spriteBatch);

            if (Main.mouseLeft && Main.mouseLeftRelease)
                pressAction(player);
        }
    }
}