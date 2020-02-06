using System;
using System.Linq;
using kRPG.Enums;
using kRPG.GameObjects.GUI;
using kRPG.GameObjects.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.GameObjects.Stats
{
    public class StatFlame
    {
        private readonly int animationTime = 5;

        public StatFlame(Mod mod, LevelGui levelGui, PlayerStats id, Func<Vector2> position, Texture2D texture)
        {
            Mod = mod;
            LevelGui = levelGui;
            Id = id;
            Position = position;
            Texture = texture;
            Counter = (int) id * 8;
        }

        private int Allocated
        {
            get => LevelGui.allocated[Id];
            set => LevelGui.allocated[Id] = value;
        }

        private int Counter { get; set; }
        private int FrameNumber { get; set; }
        private PlayerStats Id { get; }
        private LevelGui LevelGui { get; }
        private Mod Mod { get; }
        private Func<Vector2> Position { get; }
        private Texture2D Texture { get; }

        public bool CheckHover()
        {
            return Main.mouseX >= Position().X && Main.mouseY >= Position().Y && Main.mouseX <= Position().X + Texture.Width &&
                   Main.mouseY <= Position().Y + 68;
        }

        public void Draw(SpriteBatch spriteBatch, Terraria.Player player, float scale)
        {
            PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
            if (Counter > 8 * animationTime - 1) Counter = 0;
            FrameNumber = (int) Math.Floor(Counter / (double) animationTime);
            spriteBatch.Draw(character.Rituals[Ritual.DemonPact] && Id == PlayerStats.Resilience ? GFX.GFX.FlamesConverted : Texture, Position(),
                new Rectangle(0, FrameNumber * 68, 56, 68), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            string text = (Allocated + character.BaseStats[Id]).ToString();
            float width = Main.fontItemStack.MeasureString(text).X;
            spriteBatch.DrawStringWithShadow(Main.fontItemStack, text, Position() + new Vector2(28f - width / 2f, 36f) * scale,
                Allocated > 0 ? Color.Lime : Color.White, scale);
            Counter++;
        }

        public void Update(SpriteBatch spriteBatch, Terraria.Player player)
        {
            PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();

            if (!CheckHover())
                return;
            switch (Id)
            {
                case PlayerStats.Resilience:
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Resilience", new Vector2(Main.screenWidth / 2f - 96f, Main.screenHeight / 2f + 128f),
                        Color.Red);
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText,
                        character.Rituals[Ritual.DemonPact]
                            ? "Converted into Potency by Demon Pact"
                            : "Increases your defence, life regeneration, and maximum life",
                        new Vector2(Main.screenWidth / 2f - 128f, Main.screenHeight / 2f + 152f), Color.White);
                    break;
                case PlayerStats.Quickness:
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Quickness", new Vector2(Main.screenWidth / 2f - 96f, Main.screenHeight / 2f + 128f),
                        Color.Lime);
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Increases your speed, evasion, and crit chance",
                        new Vector2(Main.screenWidth / 2f - 128f, Main.screenHeight / 2f + 152f), Color.White);
                    break;
                case PlayerStats.Potency:
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Potency", new Vector2(Main.screenWidth / 2f - 96f, Main.screenHeight / 2f + 128f),
                        Color.Blue);
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Increases your damage, leech, and crit multiplier",
                        new Vector2(Main.screenWidth / 2f - 128f, Main.screenHeight / 2f + 152f), Color.White);
                    break;
            }

            if (Allocated == 0)
                spriteBatch.DrawStringWithShadow(Main.fontMouseText, "<Click to allocate>",
                    new Vector2(Main.screenWidth / 2f - 128f, Main.screenHeight / 2f + 176f), Color.White);
            else
                spriteBatch.DrawStringWithShadow(Main.fontMouseText, "<Allocated " + Allocated + ">",
                    new Vector2(Main.screenWidth / 2f - 128f, Main.screenHeight / 2f + 176f), Color.White);

            int total = LevelGui.allocated.Keys.Sum(stat => LevelGui.allocated[stat]);
            if (Main.mouseLeft && Main.mouseLeftRelease && total + character.PointsAllocated < character.Level - 1)
            {
                Main.PlaySound(SoundID.MenuTick);
                Allocated += 1;
            }

            if (Main.mouseRight && Main.mouseRightRelease && Allocated > 0)
            {
                Main.PlaySound(SoundID.MenuTick);
                Allocated -= 1;
            }
        }
    }
}