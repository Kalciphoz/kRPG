using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ModLoader;
using System;
using ReLogic;
using ReLogic.Graphics;
using System.Reflection;
using Terraria.ID;
using Terraria.GameInput;
using Terraria.UI;
using Terraria.Localization;

namespace kRPG.GUI
{
    public class StatusBar : BaseGUI
    {
        private PlayerCharacter character;
        private Mod mod;
        private kRPG krpg;

        private Vector2 GuiPosition
        {
            get
            {
                return new Vector2(4f, 6f) * Scale;
            }
        }
        private float Scale
        {
            get
            {
                return Math.Min(1f, Main.screenWidth / 3840f + 0.4f);
            }
        }
        private Vector2 buffposition
        {
            get
            {
                return new Vector2(Main.playerInventory ? 560f : 24f, Main.playerInventory ? 16f : 80f);
            }
        }

        private Vector2 bar_life_origin;
        private const int bar_life_length = 302;
        private const int bar_life_thickness = 28;

        private Vector2 bar_mana_origin;
        private const int bar_mana_length = 286;
        private const int bar_mana_thickness = 18;

        private Vector2 bar_xp_origin;
        private const int bar_xp_length = 138;
        private const int bar_xp_thickness = 6;

        private Vector2 bubbles_origin;
        private const int bubbles_length = 132;
        private const int bubbles_thickness = 22;

        private Vector2 points_origin
        {
            get
            {
                return GuiPosition + new Vector2(242f, 112f) * Scale;
            }
        }

        public StatusBar(PlayerCharacter character, Mod mod) : base()
        {
            this.character = character;
            this.mod = mod;
            krpg = (kRPG)mod;

            bar_life_origin = new Vector2(278f, 50f);
            bar_mana_origin = new Vector2(254f, 92f);
            bar_xp_origin = new Vector2(370f, 122f);
            bubbles_origin = new Vector2(284, 134);

            AddButton(delegate () { return new Rectangle((int)(points_origin.X), (int)(points_origin.Y), (int)(GFX.unspentPoints.Width * Scale), (int)(GFX.unspentPoints.Height * Scale)); }, delegate (Player player)
            {
                character.CloseGUIs();
                Main.PlaySound(SoundID.MenuTick);
                character.levelGUI.guiActive = player.GetModPlayer<PlayerCharacter>(mod).UnspentPoints() && !Main.playerInventory;
            }, delegate (Player player, SpriteBatch spriteBatch)
            {
                Main.LocalPlayer.mouseInterface = true;
                string s = Main.player[Main.myPlayer].GetModPlayer<PlayerCharacter>(mod).UnspentPoints() ? "Click here to allocate stat points" : "You have no unspent stat points";
                Main.instance.MouseText(s);
            });
        }

        public override void PostDraw(SpriteBatch spriteBatch, Player player)
        {
            if (!Main.playerInventory && !Main.player[Main.myPlayer].ghost)
            {
                this.character = player.GetModPlayer<PlayerCharacter>();

                DrawHotbar(spriteBatch);

                spriteBatch.Draw(GFX.statusBars_BG, GuiPosition, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);

                int currentLifeLength = (int)Math.Round((decimal)player.statLife / (decimal)player.statLifeMax2 * bar_life_length);
                spriteBatch.Draw(GFX.statusBars, GuiPosition + bar_life_origin * Scale, new Rectangle((int)(bar_life_origin.X + bar_life_length - currentLifeLength), (int)bar_life_origin.Y, currentLifeLength, bar_life_thickness), Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
                int currentManaLength = (int)Math.Round((decimal)character.mana / (decimal)player.statManaMax2 * bar_mana_length);
                spriteBatch.Draw(GFX.statusBars, GuiPosition + bar_mana_origin * Scale, new Rectangle((int)(bar_mana_origin.X + bar_mana_length - currentManaLength), (int)bar_mana_origin.Y, currentManaLength, bar_mana_thickness), Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
                int currentXPLength = (int)Math.Round((decimal)bar_xp_length * (decimal)character.xp / (decimal)character.ExperienceToLevel());
                spriteBatch.Draw(GFX.statusBars, GuiPosition + bar_xp_origin * Scale, new Rectangle((int)bar_xp_origin.X, (int)bar_xp_origin.Y, currentXPLength, bar_xp_thickness), Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);

                spriteBatch.Draw(GFX.characterFrame, GuiPosition, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
                spriteBatch.DrawStringWithShadow(Main.fontMouseText, player.statLife.ToString() + " / " + player.statLifeMax2.ToString(), GuiPosition + new Vector2(bar_life_origin.X * Scale + 24f * Scale, (bar_life_origin.Y + 4f) * Scale), Color.White, Scale);
                spriteBatch.DrawStringWithShadow(Main.fontMouseText, character.mana.ToString() + " / " + player.statManaMax2.ToString(), GuiPosition + new Vector2(bar_mana_origin.X * Scale + 24f * Scale, bar_mana_origin.Y * Scale), Color.White, 0.8f * Scale);

                DrawNumerals(spriteBatch, character.level, Scale);

                if (character.UnspentPoints())
                    spriteBatch.Draw(GFX.unspentPoints, points_origin, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);

                if (player.lavaTime < player.lavaMax)
                {
                    int currentBubbles = (int)Math.Round((decimal)bubbles_length * player.lavaTime / player.lavaMax);
                    spriteBatch.Draw(GFX.bubbles_lava, GuiPosition + bubbles_origin * Scale, new Rectangle(0, 0, currentBubbles, bubbles_thickness), Color.White, Scale);
                }
                if (player.breath < player.breathMax)
                {
                    int currentBubbles = (int)Math.Round((decimal)bubbles_length * player.breath / player.breathMax);
                    spriteBatch.Draw(GFX.bubbles, GuiPosition + bubbles_origin * Scale, new Rectangle(0, 0, currentBubbles, bubbles_thickness), Color.White, Scale);
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
                Main.buffString = "";
                Main.bannerMouseOver = false;
                if (!Main.recBigList) { Main.recStart = 0; }
                if (!Main.ingameOptionsWindow && !Main.playerInventory && !Main.inFancyUI)
                {
                    DrawBuffs();
                }
            }
        }

        private static MethodInfo DrawBuffIcon = typeof(Main).GetMethod("DrawBuffIcon", BindingFlags.NonPublic | BindingFlags.Static);

        public static void DrawBuffs()
        {
            int num = -1;
            int num2 = 11;
            for (int i = 0; i < 22; i++)
            {
                if (Main.player[Main.myPlayer].buffType[i] > 0)
                {
                    int b = Main.player[Main.myPlayer].buffType[i];
                    int x = 320 + i * 38;
                    int num3 = 8;
                    if (i >= num2)
                    {
                        x = 32 + (i - num2) * 38;
                        num3 += 50;
                    }
                    num = (int)DrawBuffIcon.Invoke(null, new object[] { num, i, b, x, num3 }); // Main.DrawBuffIcon(num, i, b, x, num3);
                }
                else
                {
                    Main.buffAlpha[i] = 0.4f;
                }
            }
            if (num >= 0)
            {
                int num4 = Main.player[Main.myPlayer].buffType[num];
                if (num4 > 0)
                {
                    Main.buffString = Lang.GetBuffDescription(num4);
                    int itemRarity = 0;
                    if (num4 == 26 && Main.expertMode)
                    {
                        Main.buffString = Language.GetTextValue("BuffDescription.WellFed_Expert");
                    }
                    if (num4 == 147)
                    {
                        Main.bannerMouseOver = true;
                    }
                    if (num4 == 94)
                    {
                        int num5 = (int)(Main.player[Main.myPlayer].manaSickReduction * 100f) + 1;
                        Main.buffString = Main.buffString + num5 + "%";
                    }
                    if (Main.meleeBuff[num4])
                    {
                        itemRarity = -10;
                    }
                    BuffLoader.ModifyBuffTip(num4, ref Main.buffString, ref itemRarity);
                    Main.instance.MouseTextHackZoom(Lang.GetBuffName(num4), itemRarity, 0);
                }
            }
        }

        public static void DrawNumerals(SpriteBatch spriteBatch, int level, float scale)
        {
            Vector2 origin = Main.playerInventory ? new Vector2(132f, 60f) * scale : new Vector2(190f, 58f) * scale;
            if (level < 10)
            {
                spriteBatch.Draw(GFX.gothicNumeral[level], new Vector2(origin.X - 16f * scale, origin.Y), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            else if (level < 100)
            {
                spriteBatch.Draw(GFX.gothicNumeral[level / 10], new Vector2(origin.X - 34f * scale, origin.Y), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(GFX.gothicNumeral[level % 10], new Vector2(origin.X + 2f * scale, origin.Y), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            else if (level < 1000)
            {
                spriteBatch.Draw(GFX.gothicNumeral[level / 100], new Vector2(origin.X - 52f * scale, origin.Y), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(GFX.gothicNumeral[level % 100 / 10], new Vector2(origin.X - 16f, origin.Y), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(GFX.gothicNumeral[level % 10], new Vector2(origin.X + 20f * scale, origin.Y), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
        }

        private void DrawHotbar(SpriteBatch spirteBatch)
        {
            string text = "";
            if (Main.player[Main.myPlayer].inventory[Main.player[Main.myPlayer].selectedItem].Name != null && Main.player[Main.myPlayer].inventory[Main.player[Main.myPlayer].selectedItem].Name != "")
            {
                text = Main.player[Main.myPlayer].inventory[Main.player[Main.myPlayer].selectedItem].AffixName();
            }
            Vector2 vector = Main.fontMouseText.MeasureString(text) / 2;
            Main.spriteBatch.DrawStringWithShadow(Main.fontMouseText, text, new Vector2(Main.screenWidth - 240 - vector.X - 16f, 0f), new Color((int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor), 1f);
            int pos_X = Main.screenWidth - 480;
            for (int i = 0; i < 10; i++)
            {
                if (i == Main.player[Main.myPlayer].selectedItem)
                {
                    if (Main.hotbarScale[i] < 1f)
                    {
                        Main.hotbarScale[i] += 0.05f;
                    }
                }
                else if ((double)Main.hotbarScale[i] > 0.75)
                {
                    Main.hotbarScale[i] -= 0.05f;
                }
                float num2 = Main.hotbarScale[i];
                int num3 = (int)(20f + 22f * (1f - num2));
                int a = (int)(75f + 150f * num2);
                Color lightColor = new Color(255, 255, 255, a);
                if (!Main.player[Main.myPlayer].hbLocked && !PlayerInput.IgnoreMouseInterface && Main.mouseX >= pos_X && (float)Main.mouseX <= (float)pos_X + (float)Main.inventoryBackTexture.Width * Main.hotbarScale[i] && Main.mouseY >= num3 && (float)Main.mouseY <= (float)num3 + (float)Main.inventoryBackTexture.Height * Main.hotbarScale[i] && !Main.player[Main.myPlayer].channel)
                {
                    Main.player[Main.myPlayer].mouseInterface = true;
                    Main.player[Main.myPlayer].showItemIcon = false;
                    if (Main.mouseLeft && !Main.player[Main.myPlayer].hbLocked && !Main.blockMouse)
                    {
                        Main.player[Main.myPlayer].changeItem = i;
                    }
                    Main.hoverItemName = Main.player[Main.myPlayer].inventory[i].AffixName();
                    if (Main.player[Main.myPlayer].inventory[i].stack > 1)
                    {
                        object obj = Main.hoverItemName;
                        Main.hoverItemName = string.Concat(new object[]
                        {
                    obj,
                    " (",
                    Main.player[Main.myPlayer].inventory[i].stack,
                    ")"
                        });
                    }
                    Main.rare = Main.player[Main.myPlayer].inventory[i].rare;
                }
                float num4 = Main.inventoryScale;
                Main.inventoryScale = num2;
                ItemSlot.Draw(Main.spriteBatch, Main.player[Main.myPlayer].inventory, 13, i, new Vector2((float)pos_X, (float)num3), Color.White);
                Main.inventoryScale = num4;
                pos_X += (int)((float)Main.inventoryBackTexture.Width * Main.hotbarScale[i]) + 4;
            }
            int selectedItem = Main.player[Main.myPlayer].selectedItem;
            if (selectedItem >= 10 && (selectedItem != 58 || Main.mouseItem.type > 0))
            {
                float num5 = 1f;
                int num6 = (int)(20f + 22f * (1f - num5));
                int a2 = (int)(75f + 150f * num5);
                Color lightColor2 = new Color(255, 255, 255, a2);
                float num7 = Main.inventoryScale;
                Main.inventoryScale = num5;
                ItemSlot.Draw(Main.spriteBatch, Main.player[Main.myPlayer].inventory, 13, selectedItem, new Vector2((float)pos_X, (float)num6), Color.White);
                Main.inventoryScale = num7;
            }
        }
    }
}
