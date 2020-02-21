using System.Reflection;
using kRPG.Content.Items.Procedural;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;

namespace kRPG.Content.GFX
{
    public static class SpriteBatchHelper
    {
        private static readonly FieldInfo InventoryGlowHue = typeof(ItemSlot).GetField("inventoryGlowHue", BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly FieldInfo InventoryGlowTime = typeof(ItemSlot).GetField("inventoryGlowTime", BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly FieldInfo InventoryGlowTimeChest = typeof(ItemSlot).GetField("inventoryGlowTimeChest", BindingFlags.NonPublic | BindingFlags.Static);

        public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Color color, float scale)
        {
            spriteBatch.Draw(texture, position, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float scale)
        {
            spriteBatch.Draw(texture, position, sourceRectangle, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public static void DrawStringWithShadow(this SpriteBatch spriteBatch, DynamicSpriteFont font, string text, Vector2 position, Color color, float scale = 1f)
        {
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, text, position, color, 0f, Vector2.Zero, Vector2.One * scale);
        }

        public static void ItemSlotDrawExtension(this SpriteBatch spriteBatch, Item[] inv, int context, int slot, Vector2 position, Color overrideColor, Color lightColor = default, bool drawSelected = true)
        {
            Player player = Main.player[Main.myPlayer];
            Item item = inv[slot];
            float inventoryScale = Main.inventoryScale;
            Color color = Color.White;
            if (lightColor != Color.Transparent)
                color = lightColor;
            int num = -1;
            bool flag = false;
            int num2 = 0;
            if (PlayerInput.UsingGamepadUI)
            {
                switch (context)
                {
                    case 0:
                    case 1:
                    case 2:
                        num = slot;
                        break;
                    case 3:
                    case 4:
                        num = 400 + slot;
                        break;
                    case 5:
                        num = 303;
                        break;
                    case 6:
                        num = 300;
                        break;
                    case 7:
                        num = 1500;
                        break;
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                        num = 100 + slot;
                        break;
                    case 12:
                        if (inv == player.dye)
                            num = 120 + slot;
                        if (inv == player.miscDyes)
                            num = 185 + slot;
                        break;
                    case 15:
                        num = 2700 + slot;
                        break;
                    case 16:
                        num = 184;
                        break;
                    case 17:
                        num = 183;
                        break;
                    case 18:
                        num = 182;
                        break;
                    case 19:
                        num = 180;
                        break;
                    case 20:
                        num = 181;
                        break;
                    case 22:
                        if (UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeBig != -1)
                            num = 700 + UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeBig;
                        if (UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeSmall != -1)
                            num = 1500 + UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeSmall + 1;
                        break;
                }

                flag = UILinkPointNavigator.CurrentPoint == num;
                if (context == 0)
                {
                    int drawMode = player.DpadRadial.GetDrawMode(slot);
                    num2 = drawMode;
                    if (num2 > 0 && !PlayerInput.CurrentProfile.UsingDpadHotbar())
                        num2 = 0;
                }
            }

            Texture2D texture2D = Main.inventoryBackTexture;
            Color color2 = Main.inventoryBack;
            bool flag2 = false;
            if (item.type > ItemID.None && item.stack > 0 && item.favorited && context != 13 && context != 21 && context != 22 && context != 14)
            {
                texture2D = Main.inventoryBack10Texture;
            }
            else if (item.type > ItemID.None && item.stack > 0 && ItemSlot.Options.HighlightNewItems && item.newAndShiny && context != 13 && context != 21 &&
                     context != 14 && context != 22)
            {
                texture2D = Main.inventoryBack15Texture;
                float num3 = Main.mouseTextColor / 255f;
                num3 = num3 * 0.2f + 0.8f;
                color2.MultiplyRGBA(new Color(num3, num3, num3));
            }
            else if (PlayerInput.UsingGamepadUI && item.type > ItemID.None && item.stack > 0 && num2 != 0 && context != 13 && context != 21 && context != 22)
            {
                texture2D = Main.inventoryBack15Texture;
                float num4 = Main.mouseTextColor / 255f;
                num4 = num4 * 0.2f + 0.8f;
                color2.MultiplyRGBA(num2 == 1 ? new Color(num4, num4 / 2f, num4 / 2f) : new Color(num4 / 2f, num4, num4 / 2f));
            }
            else
            {
                switch (context)
                {
                    case 0 when slot < 10:
                        texture2D = Main.inventoryBack9Texture;
                        break;
                    case 10:
                    case 8:
                    case 16:
                    case 17:
                    case 19:
                    case 18:
                    case 20:
                        texture2D = Main.inventoryBack3Texture;
                        break;
                    case 11:
                    case 9:
                        texture2D = Main.inventoryBack8Texture;
                        break;
                    case 12:
                        texture2D = Main.inventoryBack12Texture;
                        break;
                    case 3:
                        texture2D = Main.inventoryBack5Texture;
                        break;
                    case 4:
                        texture2D = Main.inventoryBack2Texture;
                        break;
                    case 7:
                    case 5:
                        texture2D = Main.inventoryBack4Texture;
                        break;
                    case 6:
                        texture2D = Main.inventoryBack7Texture;
                        break;
                    case 13:
                        {
                            byte b = 200;
                            if (slot == Main.player[Main.myPlayer].selectedItem)
                            {
                                texture2D = Main.inventoryBack14Texture;
                                b = 255;
                            }

                            new Color(b, b, b, b);
                            break;
                        }
                    case 14:
                    case 21:
                        flag2 = true;
                        break;
                    case 15:
                        texture2D = Main.inventoryBack6Texture;
                        break;
                    case 22:
                        texture2D = Main.inventoryBack4Texture;
                        break;
                }
            }

            if (context == 0 && ((int[])InventoryGlowTime.GetValue(null))[slot] > 0 && !inv[slot].favorited)
            {
                float scale = Main.invAlpha / 255f;
                Color value = new Color(63, 65, 151, 255) * scale;
                Color value2 = Main.hslToRgb(((float[])InventoryGlowHue.GetValue(null))[slot], 1f, 0.5f) * scale;
                float num5 = ((int[])InventoryGlowTime.GetValue(null))[slot] / 300f;
                num5 *= num5;
                Color.Lerp(value, value2, num5 / 2f);
                texture2D = Main.inventoryBack13Texture;
            }

            if ((context == 4 || context == 3) && ((int[])InventoryGlowTimeChest.GetValue(null))[slot] > 0 && !inv[slot].favorited)
            {
                float scale2 = Main.invAlpha / 255f;
                Color value3 = new Color(130, 62, 102, 255) * scale2;
                if (context == 3)
                    value3 = new Color(104, 52, 52, 255) * scale2;
                Color value4 = Main.hslToRgb(((float[])InventoryGlowHue.GetValue(null))[slot], 1f, 0.5f) * scale2;
                float num6 = ((int[])InventoryGlowTimeChest.GetValue(null))[slot] / 300f;
                num6 *= num6;
                Color.Lerp(value3, value4, num6 / 2f);
                texture2D = Main.inventoryBack13Texture;
            }

            if (flag)
            {
                texture2D = Main.inventoryBack14Texture;
                color2 = Color.White;
            }

            if (!flag2)
                spriteBatch.Draw(slot == Main.player[Main.myPlayer].selectedItem && drawSelected ? Main.inventoryBack14Texture : texture2D, position, null,
                    overrideColor, 0f, default, inventoryScale, SpriteEffects.None, 0f);
            int num7 = -1;
            switch (context)
            {
                case 8:
                    switch (slot)
                    {
                        case 0:
                            num7 = 0;
                            break;
                        case 1:
                            num7 = 6;
                            break;
                        case 2:
                            num7 = 12;
                            break;
                    }

                    break;
                case 9:
                    switch (slot)
                    {
                        case 10:
                            num7 = 3;
                            break;
                        case 11:
                            num7 = 9;
                            break;
                        case 12:
                            num7 = 15;
                            break;
                    }

                    break;
                case 10:
                    num7 = 11;
                    break;
                case 11:
                    num7 = 2;
                    break;
                case 12:
                    num7 = 1;
                    break;
                case 16:
                    num7 = 4;
                    break;
                case 17:
                    num7 = 13;
                    break;
                case 18:
                    num7 = 7;
                    break;
                case 19:
                    num7 = 10;
                    break;
                case 20:
                    num7 = 17;
                    break;
            }

            if ((item.type <= ItemID.None || item.stack <= 0) && num7 != -1)
            {
                Texture2D texture2D2 = Main.extraTexture[54];
                Rectangle rectangle = texture2D2.Frame(3, 6, num7 % 3, num7 / 3);
                rectangle.Width -= 2;
                rectangle.Height -= 2;
                spriteBatch.Draw(texture2D2, position + texture2D.Size() / 2f * inventoryScale, rectangle, Color.White * 0.35f, 0f, rectangle.Size() / 2f,
                    inventoryScale, SpriteEffects.None, 0f);
            }

            Vector2 vector = texture2D.Size() * inventoryScale;
            if (item.type > ItemID.None && item.stack > 0)
            {
                Texture2D texture2D3 = item.modItem is ProceduralItem ? ((ProceduralItem)item.modItem).LocalTexture : Main.itemTexture[item.type];
                Rectangle rectangle2;
                rectangle2 = Main.itemAnimations[item.type] != null ? Main.itemAnimations[item.type].GetFrame(texture2D3) : texture2D3.Frame();
                Color newColor = color;
                float num8 = 1f;
                ItemSlot.GetItemLight(ref newColor, ref num8, item);
                float num9 = 1f;
                if (rectangle2.Width > 32 || rectangle2.Height > 32)
                {
                    if (rectangle2.Width > rectangle2.Height)
                        num9 = 32f / rectangle2.Width;
                    else
                        num9 = 32f / rectangle2.Height;
                }

                num9 *= inventoryScale;
                Vector2 position2 = position + vector / 2f - rectangle2.Size() * num9 / 2f;
                Vector2 origin = rectangle2.Size() * (num8 / 2f - 0.5f);
                if (ItemLoader.PreDrawInInventory(item, spriteBatch, position2, rectangle2, item.GetAlpha(newColor), item.GetColor(color), origin, num9 * num8))
                {
                    spriteBatch.Draw(texture2D3, position2, rectangle2, item.GetAlpha(newColor), 0f, origin, num9 * num8, SpriteEffects.None, 0f);
                    if (item.color != Color.Transparent)
                        spriteBatch.Draw(texture2D3, position2, rectangle2, item.GetColor(color), 0f, origin, num9 * num8, SpriteEffects.None, 0f);
                }

                ItemLoader.PostDrawInInventory(item, spriteBatch, position2, rectangle2, item.GetAlpha(newColor), item.GetColor(color), origin, num9 * num8);
                if (ItemID.Sets.TrapSigned[item.type])
                    spriteBatch.Draw(Main.wireTexture, position + new Vector2(40f, 40f) * inventoryScale, new Rectangle(4, 58, 8, 8), color, 0f,
                        new Vector2(4f), 1f, SpriteEffects.None, 0f);
                if (item.stack > 1)
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, item.stack.ToString(),
                        position + new Vector2(10f, 26f) * inventoryScale, color, 0f, Vector2.Zero, new Vector2(inventoryScale), -1f, inventoryScale);
                int num10 = -1;
                if (context == 13)
                {
                    if (item.DD2Summon)
                    {
                        for (int i = 0; i < 58; i++)
                            if (inv[i].type == 3822)
                                num10 += inv[i].stack;
                        if (num10 >= 0)
                            num10++;
                    }

                    if (item.useAmmo > 0)
                    {
                        int useAmmo = item.useAmmo;
                        num10 = 0;
                        for (int j = 0; j < 58; j++)
                            if (inv[j].ammo == useAmmo)
                                num10 += inv[j].stack;
                    }

                    if (item.fishingPole > 0)
                    {
                        num10 = 0;
                        for (int k = 0; k < 58; k++)
                            if (inv[k].bait > 0)
                                num10 += inv[k].stack;
                    }

                    if (item.tileWand > 0)
                    {
                        int tileWand = item.tileWand;
                        num10 = 0;
                        for (int l = 0; l < 58; l++)
                            if (inv[l].type == tileWand)
                                num10 += inv[l].stack;
                    }

                    if (item.type == ItemID.Wrench || item.type == ItemID.GreenWrench || item.type == ItemID.BlueWrench || item.type == ItemID.YellowWrench || item.type == ItemID.MulticolorWrench || item.type == ItemID.WireKite)
                    {
                        num10 = 0;
                        for (int m = 0; m < 58; m++)
                            if (inv[m].type == ItemID.Wire)
                                num10 += inv[m].stack;
                    }
                }

                if (num10 != -1)
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, num10.ToString(),
                        position + new Vector2(8f, 30f) * inventoryScale, color, 0f, Vector2.Zero, new Vector2(inventoryScale * 0.8f), -1f, inventoryScale);
                if (context == 13)
                {
                    string text = string.Concat(slot + 1);
                    if (text == "10")
                        text = "0";
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, text, position + new Vector2(8f, 4f) * inventoryScale, color,
                        0f, Vector2.Zero, new Vector2(inventoryScale), -1f, inventoryScale);
                }

                if (context == 13 && item.potion)
                {
                    Vector2 position3 = position + texture2D.Size() * inventoryScale / 2f - Main.cdTexture.Size() * inventoryScale / 2f;
                    Color color3 = item.GetAlpha(color) * (player.potionDelay / (float)player.potionDelayTime);
                    spriteBatch.Draw(Main.cdTexture, position3, null, color3, 0f, default, num9, SpriteEffects.None, 0f);
                }

                if ((context == 10 || context == 18) && item.expertOnly && !Main.expertMode)
                {
                    Vector2 position4 = position + texture2D.Size() * inventoryScale / 2f - Main.cdTexture.Size() * inventoryScale / 2f;
                    Color white = Color.White;
                    spriteBatch.Draw(Main.cdTexture, position4, null, white, 0f, default, num9, SpriteEffects.None, 0f);
                }
            }
            else if (context == 6)
            {
                Texture2D trashTexture = Main.trashTexture;
                Vector2 position5 = position + texture2D.Size() * inventoryScale / 2f - trashTexture.Size() * inventoryScale / 2f;
                spriteBatch.Draw(trashTexture, position5, null, new Color(100, 100, 100, 100), 0f, default, inventoryScale, SpriteEffects.None, 0f);
            }

            if (context == 0 && slot < 10)
            {
                string text2 = string.Concat(slot + 1);
                if (text2 == "10")
                    text2 = "0";
                Color inventoryBack = Main.inventoryBack;
                int num12 = 0;
                if (Main.player[Main.myPlayer].selectedItem == slot)
                {
                    num12 -= 3;
                    inventoryBack.R = 255;
                    inventoryBack.B = 0;
                    inventoryBack.G = 210;
                    inventoryBack.A = 100;
                }

                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, text2, position + new Vector2(6f, 4 + num12) * inventoryScale,
                    inventoryBack, 0f, Vector2.Zero, new Vector2(inventoryScale), -1f, inventoryScale);
            }

            if (num != -1)
                UILinkPointNavigator.SetPosition(num, position + vector * 0.75f);
        }

    }
}
