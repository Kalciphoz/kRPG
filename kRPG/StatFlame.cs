using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using kRPG.GUI;
using ReLogic;
using ReLogic.Graphics;
using Terraria.ID;

namespace kRPG
{
    public class StatFlame
    {
        private Mod mod;
        private STAT id;
        private Func<Vector2> position;
        private Texture2D texture;
        private LevelGUI levelGUI;

        private int counter;
        private int frameNumber = 0;
        private int animationTime = 5;

        private int allocated
        {
            get
            {
                return levelGUI.allocated[id];
            }
            set
            {
                levelGUI.allocated[id] = value;
            }
        }

        public StatFlame(Mod mod, LevelGUI levelGUI, STAT id, Func<Vector2> position, Texture2D texture)
        {
            this.mod = mod;
            this.levelGUI = levelGUI;
            this.id = id;
            this.position = position;
            this.texture = texture;
            this.counter = (int)id * 8;
        }

        public void Draw(SpriteBatch spriteBatch, Player player, float scale)
        {
            PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
            if (counter > (8 * animationTime - 1)) counter = 0;
            frameNumber = (int)(Math.Floor((double)counter / (double)animationTime));
            spriteBatch.Draw(character.rituals[RITUAL.DEMON_PACT] && id == STAT.RESILIENCE ? GFX.flames_converted : texture, position(), new Rectangle(0, frameNumber * 68, 56, 68), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            string text = (allocated + character.baseStats[id]).ToString();
            float width = Main.fontItemStack.MeasureString(text).X;
            spriteBatch.DrawStringWithShadow(Main.fontItemStack, text, position() + new Vector2(28f - width / 2f, 36f) * scale, allocated > 0 ? Color.Lime : Color.White, scale);
            counter++;
        }

        public void Update(SpriteBatch spriteBatch, Player player)
        {
            PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();

            if (CheckHover())
            {
                if (id == STAT.RESILIENCE)
                {
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Resilience", new Vector2(Main.screenWidth / 2f - 96f, Main.screenHeight / 2f + 128f), Color.Red);
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, character.rituals[RITUAL.DEMON_PACT] ? "Converted into Potency by Demon Pact" : "Increases your defence, life regeneration, and maximum life", new Vector2(Main.screenWidth / 2f - 128f, Main.screenHeight / 2f + 152f), Color.White);
                }

                else if (id == STAT.QUICKNESS)
                {
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Quickness", new Vector2(Main.screenWidth / 2f - 96f, Main.screenHeight / 2f + 128f), Color.Lime);
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Increases your speed, evasion, and crit chance", new Vector2(Main.screenWidth / 2f - 128f, Main.screenHeight / 2f + 152f), Color.White);
                }

                else if (id == STAT.POTENCY)
                {
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Potency", new Vector2(Main.screenWidth / 2f - 96f, Main.screenHeight / 2f + 128f), Color.Blue);
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Increases your damage, leech, and crit multiplier", new Vector2(Main.screenWidth / 2f - 128f, Main.screenHeight / 2f + 152f), Color.White);
                }

                if (allocated == 0)
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "<Click to allocate>", new Vector2(Main.screenWidth / 2f - 128f, Main.screenHeight / 2f + 176f), Color.White);
                else
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "<Allocated " + allocated.ToString() + ">", new Vector2(Main.screenWidth / 2f - 128f, Main.screenHeight / 2f + 176f), Color.White);

                int total = 0;
                foreach (STAT stat in levelGUI.allocated.Keys)
                    total += levelGUI.allocated[stat];
                if (Main.mouseLeft && Main.mouseLeftRelease && total + character.pointsAllocated < character.level - 1)
                {
                    Main.PlaySound(SoundID.MenuTick);
                    allocated += 1;
                }
                if (Main.mouseRight && Main.mouseRightRelease && allocated > 0)
                {
                    Main.PlaySound(SoundID.MenuTick);
                    allocated -= 1;
                }
            }
        }

        public bool CheckHover()
        {
            return Main.mouseX >= position().X && Main.mouseY >= position().Y && Main.mouseX <= position().X + texture.Width && Main.mouseY <= position().Y + 68;
        }
    }
}
