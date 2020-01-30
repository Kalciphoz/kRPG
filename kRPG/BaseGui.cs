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

        public bool guiActive = false;

        public BaseGui()
        {
            guiElements.Add(this);
            return;
        }

        public virtual bool PreDraw()
        {
            return guiActive;
        }

        public virtual void Draw(SpriteBatch spriteBatch, Player player)
        {
            PostDraw(spriteBatch, player);

            foreach (var button in buttons)
                button.Update(spriteBatch, player);
        }

        public virtual void PostDraw(SpriteBatch spriteBatch, Player player)
        {
        }

        public InterfaceButton AddButton(Func<Rectangle> position, Action<Player> pressAction)
        {
            var button = new InterfaceButton(position, pressAction);
            buttons.Add(button);
            return button;
        }

        public InterfaceButton AddButton(Func<Rectangle> position, Action<Player> pressAction, Action<Player, SpriteBatch> hoverAction)
        {
            var button = new InterfaceButton(position, pressAction, hoverAction);
            buttons.Add(button);
            return button;
        }

        public void RemoveButton(InterfaceButton button)
        {
            buttons.Remove(button);
        }

        public virtual bool RemoveOnClose => false;

        public void CloseGui()
        {
            OnClose();
            guiActive = false;
            if (RemoveOnClose) guiElements.Remove(this);
        }

        public virtual void OnClose()
        {
        }

        //public virtual void PostDraw(SpriteBatch spriteBatch, Player player) {}
    }
}