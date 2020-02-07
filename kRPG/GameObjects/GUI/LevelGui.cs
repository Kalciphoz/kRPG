using System;
using System.Collections.Generic;
using System.Linq;
using kRPG.Enums;
using kRPG.GameObjects.GUI.Base;
using kRPG.GameObjects.Players;
using kRPG.GameObjects.Stats;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.GameObjects.GUI
{
    public class LevelGui : BaseGui
    {
        public Dictionary<PlayerStats, int> allocated = new Dictionary<PlayerStats, int> {{PlayerStats.Resilience, 0}, {PlayerStats.Quickness, 0}, {PlayerStats.Potency, 0}};
        private PlayerCharacter character;

        private readonly Dictionary<PlayerStats, StatFlame> statFlame;

        public LevelGui(PlayerCharacter character, Mod mod)
        {
            this.character = character;

            statFlame = new Dictionary<PlayerStats, StatFlame>
            {
                [PlayerStats.Resilience] = new StatFlame(mod, this, PlayerStats.Resilience, () => Position[PlayerStats.Resilience], GFX.GFX.Flames[PlayerStats.Resilience]),
                [PlayerStats.Quickness] = new StatFlame(mod, this, PlayerStats.Quickness, () => Position[PlayerStats.Quickness], GFX.GFX.Flames[PlayerStats.Quickness]),
                [PlayerStats.Potency] = new StatFlame(mod, this, PlayerStats.Potency, () => Position[PlayerStats.Potency], GFX.GFX.Flames[PlayerStats.Potency])
            };
        }

        private Vector2 GuiPosition => new Vector2(Main.screenWidth / 2f - Width, 50f * Scale + 50f);

        private Dictionary<PlayerStats, Vector2> Position =>
            new Dictionary<PlayerStats, Vector2>
            {
                {PlayerStats.Resilience, new Vector2(GuiPosition.X + 52f * Scale, GuiPosition.Y - 40f * Scale)},
                {PlayerStats.Quickness, new Vector2(GuiPosition.X + 172f * Scale, GuiPosition.Y)},
                {PlayerStats.Potency, new Vector2(GuiPosition.X + 292f * Scale, GuiPosition.Y - 40f * Scale)}
            };

        private float Scale => Math.Min(1f, Main.screenWidth / Constants.MaxScreenWidth + 0.5f);

        private float Width => 200f * Scale;

        public override void PostDraw(SpriteBatch spriteBatch, Player player)
        {
            character = player.GetModPlayer<PlayerCharacter>();

            spriteBatch.Draw(GFX.GFX.DeerSkull, GuiPosition, Color.White, Scale);

            int remaining = character.Level - character.PointsAllocated - 1;
            remaining = allocated.Keys.Aggregate(remaining, (current, stat) => current - allocated[stat]);
            string text = "You have " + (remaining == 0 ? "no" : remaining.ToString()) + (remaining == 1 ? " point " : " points ") + "remaining";
            float width = Main.fontMouseText.MeasureString(text).X * Scale;

            spriteBatch.DrawStringWithShadow(Main.fontMouseText, text, GuiPosition - new Vector2(width / 2f - 200f, 38f * Scale + 38f), Color.White, Scale);

            Vector2 buttonPosition = new Vector2(Main.screenWidth / 2f - 92f * Scale, Main.screenHeight / 2f + 320f * Scale);
            spriteBatch.Draw(GFX.GFX.ButtonCancel, buttonPosition, Color.White, Scale);

            if (Main.mouseX >= buttonPosition.X && Main.mouseY >= buttonPosition.Y && Main.mouseX <= buttonPosition.X + GFX.GFX.ButtonConfirm.Width * Scale &&
                Main.mouseY <= buttonPosition.Y + GFX.GFX.ButtonConfirm.Height * Scale)
            {
                Main.LocalPlayer.mouseInterface = true;
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    Main.PlaySound(SoundID.MenuTick);
                    CloseGui();
                    return;
                }
            }

            statFlame[PlayerStats.Resilience].Draw(spriteBatch, player, Scale);
            statFlame[PlayerStats.Resilience].Update(spriteBatch, player);
            statFlame[PlayerStats.Quickness].Draw(spriteBatch, player, Scale);
            statFlame[PlayerStats.Quickness].Update(spriteBatch, player);
            statFlame[PlayerStats.Potency].Draw(spriteBatch, player, Scale);
            statFlame[PlayerStats.Potency].Update(spriteBatch, player);

            buttonPosition = new Vector2(Main.screenWidth / 2f - 92f * Scale, Main.screenHeight / 2f + 256f * Scale);
            spriteBatch.Draw(GFX.GFX.ButtonConfirm, buttonPosition, Color.White, Scale);

            if (Main.mouseX >= buttonPosition.X && Main.mouseY >= buttonPosition.Y && Main.mouseX <= buttonPosition.X + GFX.GFX.ButtonConfirm.Width &&
                Main.mouseY <= buttonPosition.Y + GFX.GFX.ButtonConfirm.Height)
            {
                Main.LocalPlayer.mouseInterface = true;
                if (Main.mouseLeft && Main.mouseLeftRelease)
                    try
                    {
                        Main.PlaySound(SoundID.MenuTick);
                        foreach (PlayerStats s in allocated.Keys)
                            character.BaseStats[s] += allocated[s];
                        foreach (PlayerStats stat in Enum.GetValues(typeof(PlayerStats)))
                            allocated[stat] = 0;

                        GuiActive = false;
                        GFX.GFX.SfxLevelUp.Play(0.2f * Main.soundVolume, -0.6f, -0.2f);
                        return;
                    }
                    catch (SystemException e)
                    {
                        Main.NewText(e.ToString());
                    }
            }

            PlayerStats? hoverStat = null;
            foreach (PlayerStats s in statFlame.Keys.Where(s => statFlame[s].CheckHover()))
                hoverStat = s;

            if (hoverStat != null) spriteBatch.Draw(GFX.GFX.DeerSkullEyes[hoverStat.Value], GuiPosition, Color.White, Scale);
        }
    }
}