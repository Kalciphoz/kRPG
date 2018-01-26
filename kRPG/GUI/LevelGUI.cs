using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using System;
using ReLogic;
using ReLogic.Graphics;
using Terraria.ID;

namespace kRPG.GUI
{
    public class LevelGUI : BaseGUI
    {
        private PlayerCharacter character;
        private Mod mod;
        private kRPG krpg;

        private float Scale
        {
            get
            {
                return Math.Min(1f, Main.screenWidth / 3840f + 0.5f);
            }
        }
        private float Width
        {
            get
            {
                return 200f * Scale;
            }
        }
        private Vector2 GuiPosition
        {
            get
            {
                return new Vector2((float)Main.screenWidth / 2f - Width, 50f * Scale + 50f);
            }
        }
        private Dictionary<STAT, Vector2> Position
        {
            get
            {
                return new Dictionary<STAT, Vector2>()
                {
                    { STAT.RESILIENCE, new Vector2(GuiPosition.X + 52f * Scale, GuiPosition.Y - 40f * Scale) },
                    { STAT.QUICKNESS, new Vector2(GuiPosition.X + 172f * Scale, GuiPosition.Y) },
                    { STAT.POTENCY, new Vector2(GuiPosition.X + 292f * Scale, GuiPosition.Y - 40f * Scale) }
                };
            }
        }

        private Dictionary<STAT, StatFlame> statFlame = new Dictionary<STAT, StatFlame>();
        
        public Dictionary<STAT, int> allocated = new Dictionary<STAT, int>()
        {
            { STAT.RESILIENCE, 0 },
            { STAT.QUICKNESS, 0 },
            { STAT.POTENCY, 0 }
        };

        public LevelGUI(PlayerCharacter character, Mod mod) : base()
        {
            this.character = character;
            this.mod = mod;
            krpg = (kRPG)mod;

            statFlame = new Dictionary<STAT, StatFlame>();
            statFlame[STAT.RESILIENCE] = new StatFlame(mod, this, STAT.RESILIENCE, delegate() { return Position[STAT.RESILIENCE]; }, GFX.flames[STAT.RESILIENCE]);
            statFlame[STAT.QUICKNESS] = new StatFlame(mod, this, STAT.QUICKNESS, delegate () { return Position[STAT.QUICKNESS]; }, GFX.flames[STAT.QUICKNESS]);
            statFlame[STAT.POTENCY] = new StatFlame(mod, this, STAT.POTENCY, delegate () { return Position[STAT.POTENCY]; }, GFX.flames[STAT.POTENCY]);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Player player)
        {
            character = player.GetModPlayer<PlayerCharacter>();

            spriteBatch.Draw(GFX.deerskull, GuiPosition, Color.White, Scale);

            int remaining = character.level - character.pointsAllocated - 1;
            foreach (STAT stat in allocated.Keys)
                remaining -= allocated[stat];
            string text = "You have " + (remaining == 0 ? "no" : remaining.ToString()) + (remaining == 1 ? " point " : " points ") + "remaining";
            float width = Main.fontMouseText.MeasureString(text).X * Scale;

            spriteBatch.DrawStringWithShadow(Main.fontMouseText, text, GuiPosition - new Vector2(width / 2f - 200f, 38f * Scale + 38f), Color.White, Scale);

            Vector2 buttonPosition = new Vector2(Main.screenWidth / 2f - 92f * Scale, Main.screenHeight / 2f + 320f * Scale);
            spriteBatch.Draw(GFX.button_cancel, buttonPosition, Color.White, Scale);

            if (Main.mouseX >= buttonPosition.X && Main.mouseY >= buttonPosition.Y && Main.mouseX <= buttonPosition.X + GFX.button_confirm.Width * Scale && Main.mouseY <= buttonPosition.Y + GFX.button_confirm.Height * Scale)
            {
                Main.LocalPlayer.mouseInterface = true;
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    Main.PlaySound(SoundID.MenuTick);
                    CloseGUI();
                    return;
                }
            }

            statFlame[STAT.RESILIENCE].Draw(spriteBatch, player, Scale);
            statFlame[STAT.RESILIENCE].Update(spriteBatch, player);
            statFlame[STAT.QUICKNESS].Draw(spriteBatch, player, Scale);
            statFlame[STAT.QUICKNESS].Update(spriteBatch, player);
            statFlame[STAT.POTENCY].Draw(spriteBatch, player, Scale);
            statFlame[STAT.POTENCY].Update(spriteBatch, player);

            buttonPosition = new Vector2(Main.screenWidth / 2f - 92f * Scale, Main.screenHeight / 2f + 256f * Scale);
            spriteBatch.Draw(GFX.button_confirm, buttonPosition, Color.White, Scale);

            if (Main.mouseX >= buttonPosition.X && Main.mouseY >= buttonPosition.Y && Main.mouseX <= buttonPosition.X + GFX.button_confirm.Width && Main.mouseY <= buttonPosition.Y + GFX.button_confirm.Height)
            {
                Main.LocalPlayer.mouseInterface = true;
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    try
                    {
                        Main.PlaySound(SoundID.MenuTick);
                        foreach (STAT s in allocated.Keys)
                        {
                            character.baseStats[s] += allocated[s];
                        }
                        foreach (STAT stat in Enum.GetValues(typeof(STAT)))
                        {
                            allocated[stat] = 0;
                        }

                        guiActive = false;
                        GFX.sfx_levelUp.Play(0.2f*Main.soundVolume, -0.6f, -0.2f);
                        return;
                    }
                    catch (SystemException e)
                    {
                        Main.NewText(e.ToString());
                    }
                }
            }

            STAT? hoverstat = null;
            foreach (STAT s in statFlame.Keys)
                if (statFlame[s].CheckHover()) hoverstat = s;

            if (hoverstat != null) spriteBatch.Draw(GFX.deerskull_eyes[hoverstat.Value], GuiPosition, Color.White, Scale);
        }
    }
}
