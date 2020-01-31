using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace kRPG2
{
    public class InterfaceButton
    {
        private readonly Action<Player, SpriteBatch> hoverAction;

        private  bool HoverActionSet { get;  }
        private  Func<Rectangle> Position { get; }
        private  Action<Player> PressAction { get;  }

        public InterfaceButton(Func<Rectangle> position, Action<Player> pressAction)
        {
            this.Position = position;
            this.PressAction = pressAction;
        }

        public InterfaceButton(Func<Rectangle> position, Action<Player> pressAction, Action<Player, SpriteBatch> hoverAction)
        {
            this.Position = position;
            this.PressAction = pressAction;
            this.hoverAction = hoverAction;
            HoverActionSet = true;
        }

        public virtual void Update(SpriteBatch spriteBatch, Player player)
        {
            if (!Position().Contains(Main.mouseX, Main.mouseY))
                return;
            Main.LocalPlayer.mouseInterface = true;

            if (HoverActionSet)
                hoverAction(player, spriteBatch);

            if (Main.mouseLeft && Main.mouseLeftRelease)
                PressAction(player);
        }
    }
}