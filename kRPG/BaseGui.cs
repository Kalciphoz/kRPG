using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace kRPG
{
    public class BaseGui
    {
        public static List<BaseGui> guiElements = new List<BaseGui>();

        public List<InterfaceButton> buttons = new List<InterfaceButton>();

        public bool guiActive;

        public BaseGui()
        {
            guiElements.Add(this);
        }

        public virtual bool RemoveOnClose => false;

        public InterfaceButton AddButton(Func<Rectangle> position, Action<Player> pressAction)
        {
            InterfaceButton button = new InterfaceButton(position, pressAction);
            buttons.Add(button);
            return button;
        }

        public InterfaceButton AddButton(Func<Rectangle> position, Action<Player> pressAction, Action<Player, SpriteBatch> hoverAction)
        {
            InterfaceButton button = new InterfaceButton(position, pressAction, hoverAction);
            buttons.Add(button);
            return button;
        }

        public void CloseGui()
        {
            OnClose();
            guiActive = false;
            if (RemoveOnClose) guiElements.Remove(this);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Player player)
        {
            PostDraw(spriteBatch, player);

            foreach (InterfaceButton button in buttons)
                button.Update(spriteBatch, player);
        }

        public virtual void OnClose()
        {
        }

        public virtual void PostDraw(SpriteBatch spriteBatch, Player player)
        {
        }

        public virtual bool PreDraw()
        {
            return guiActive;
        }

        public void RemoveButton(InterfaceButton button)
        {
            buttons.Remove(button);
        }

        //public virtual void PostDraw(SpriteBatch spriteBatch, Player player) {}
    }
}