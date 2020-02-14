using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kRPG.Enums;
using kRPG.GameObjects.Players;
using kRPG.GameObjects.SFX;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.GameObjects.Recipes
{
    public static class RecipeHelper
    {
        public static void ApiCreate(this Terraria.Recipe recipe)
        {
            for (int requirementIndex = 0; requirementIndex < Terraria.Recipe.maxRequirements; requirementIndex++)
            {
                Item requiredItem = recipe.requiredItem[requirementIndex];
                if (requiredItem.type == ItemID.None)
                    break;
                int requiredAmount = requiredItem.stack;
                if (recipe is ModRecipe modRecipe)
                    requiredAmount = modRecipe.ConsumeItem(requiredItem.type, requiredItem.stack);
                if (recipe.alchemy && Main.player[Main.myPlayer].alchemyTable)
                {
                    if (requiredAmount > 1)
                    {
                        int num2 = 0;
                        for (int j = 0; j < requiredAmount; j++)
                            if (Main.rand.Next(3) == 0)
                                num2++;
                        requiredAmount -= num2;
                    }
                    else if (Main.rand.Next(3) == 0)
                    {
                        requiredAmount = 0;
                    }
                }

                if (requiredAmount <= 0)
                    continue;

                Item[] array = Main.player[Main.myPlayer].inventory;
                InvLogic(recipe, array, requiredItem, requiredAmount);
                PlayerCharacter character = Main.LocalPlayer.GetModPlayer<PlayerCharacter>();
                for (int j = 0; j < character.Inventories.Length; j += 1)
                    if (character.ActiveInvPage != j)
                    {
                        array = character.Inventories[j];
                        InvLogic(recipe, array, requiredItem, requiredAmount);
                    }

                if (Main.player[Main.myPlayer].chest == -1)
                    continue;
                if (Main.player[Main.myPlayer].chest > -1)
                    array = Main.chest[Main.player[Main.myPlayer].chest].item;
                else
                    switch (Main.player[Main.myPlayer].chest)
                    {
                        case -2:
                            array = Main.player[Main.myPlayer].bank.item;
                            break;
                        case -3:
                            array = Main.player[Main.myPlayer].bank2.item;
                            break;
                        case -4:
                            array = Main.player[Main.myPlayer].bank3.item;
                            break;
                    }

                for (int l = 0; l < 40; l++)
                {
                    Item item3 = array[l];
                    if (requiredAmount <= 0)
                        break;

                    if (!item3.IsTheSameAs(requiredItem) && !recipe.useWood(item3.type, requiredItem.type) &&
                        !recipe.useSand(item3.type, requiredItem.type) && !recipe.useIronBar(item3.type, requiredItem.type) &&
                        !recipe.usePressurePlate(item3.type, requiredItem.type) && !recipe.useFragment(item3.type, requiredItem.type) &&
                        !recipe.AcceptedByItemGroups(item3.type, requiredItem.type))
                        continue;
                    if (item3.stack > requiredAmount)
                    {
                        item3.stack -= requiredAmount;
                        if (Main.netMode == 1 && Main.player[Main.myPlayer].chest >= 0)
                            NetMessage.SendData(32, -1, -1, null, Main.player[Main.myPlayer].chest, l);
                        requiredAmount = 0;
                    }
                    else
                    {
                        requiredAmount -= item3.stack;
                        array[l] = new Item();
                        if (Main.netMode == 1 && Main.player[Main.myPlayer].chest >= 0)
                            NetMessage.SendData(32, -1, -1, null, Main.player[Main.myPlayer].chest, l);
                    }
                }

            }

            AchievementsHelper.NotifyItemCraft(recipe);
            AchievementsHelper.NotifyItemPickup(Main.player[Main.myPlayer], recipe.createItem);
            FindRecipes();
        }
        public static void FindRecipes()
        {
            if (Main.netMode == 2) return;
            int num = Main.availableRecipe[Main.focusRecipe];
            float num2 = Main.availableRecipeY[Main.focusRecipe];
            for (int i = 0; i < Terraria.Recipe.maxRecipes; i++)
                Main.availableRecipe[i] = 0;
            Main.numAvailableRecipes = 0;
            bool flag = Main.guideItem.type > 0 && Main.guideItem.stack > 0 && Main.guideItem.Name != "";
            if (flag)
            {
                for (int j = 0; j < Terraria.Recipe.maxRecipes; j++)
                {
                    if (Main.recipe[j].createItem.type == ItemID.None)
                        break;
                    int num3 = 0;
                    while (num3 < Terraria.Recipe.maxRequirements && Main.recipe[j].requiredItem[num3].type != 0)
                    {
                        if (Main.guideItem.IsTheSameAs(Main.recipe[j].requiredItem[num3]) ||
                            Main.recipe[j].useWood(Main.guideItem.type, Main.recipe[j].requiredItem[num3].type) ||
                            Main.recipe[j].useSand(Main.guideItem.type, Main.recipe[j].requiredItem[num3].type) ||
                            Main.recipe[j].useIronBar(Main.guideItem.type, Main.recipe[j].requiredItem[num3].type) ||
                            Main.recipe[j].useFragment(Main.guideItem.type, Main.recipe[j].requiredItem[num3].type) ||
                            Main.recipe[j].AcceptedByItemGroups(Main.guideItem.type, Main.recipe[j].requiredItem[num3].type) ||
                            Main.recipe[j].usePressurePlate(Main.guideItem.type, Main.recipe[j].requiredItem[num3].type))
                        {
                            Main.availableRecipe[Main.numAvailableRecipes] = j;
                            Main.numAvailableRecipes++;
                            break;
                        }

                        num3++;
                    }
                }
            }
            else
            {
                Dictionary<int, int> dictionary = new Dictionary<int, int>();
                Item item;
                Item[] inv = Main.player[Main.myPlayer].inventory;
                foreach (Item t in inv)
                {
                    item = t;
                    if (item.stack <= 0)
                        continue;
                    if (dictionary.ContainsKey(item.netID))
                    {
                        Dictionary<int, int> dictionary2;
                        int netId;
                        (dictionary2 = dictionary)[netId = item.netID] = dictionary2[netId] + item.stack;
                    }
                    else
                    {
                        dictionary[item.netID] = item.stack;
                    }
                }

                if (Main.player[Main.myPlayer].active)
                {
                    PlayerCharacter character = Main.player[Main.myPlayer].GetModPlayer<PlayerCharacter>();
                    for (int inventoryPage = 0; inventoryPage < character.Inventories.Length; inventoryPage += 1)
                        if (inventoryPage != character.ActiveInvPage)
                            foreach (Item inventorySlot in character.Inventories[inventoryPage])
                                if (dictionary.ContainsKey(inventorySlot.netID))
                                {
                                    Dictionary<int, int> dictionary2;
                                    int netId;
                                    (dictionary2 = dictionary)[netId = inventorySlot.netID] = dictionary2[netId] + inventorySlot.stack;
                                }
                                else
                                {
                                    dictionary[inventorySlot.netID] = inventorySlot.stack;
                                }
                }

                Item[] array = new Item[0];
                if (Main.player[Main.myPlayer].chest != -1)
                {
                    if (Main.player[Main.myPlayer].chest > -1)
                        array = Main.chest[Main.player[Main.myPlayer].chest].item;
                    else
                        switch (Main.player[Main.myPlayer].chest)
                        {
                            //Inventory Bank Main I
                            case -2:
                                array = Main.player[Main.myPlayer].bank.item;
                                break;
                            //Inventory Bank Page II
                            case -3:
                                array = Main.player[Main.myPlayer].bank2.item;
                                break;
                            //Inventory Bank Page III
                            case -4:
                                array = Main.player[Main.myPlayer].bank3.item;
                                break;
                        }

                    for (int inventorySlot = 0; inventorySlot < 40; inventorySlot++)
                    {
                        item = array[inventorySlot];
                        if (item.stack <= 0)
                            continue;
                        if (dictionary.ContainsKey(item.netID))
                        {
                            Dictionary<int, int> dictionary3;
                            int netId2;
                            //todo, Ok, no idea what this code is doing, will need to figure it out
                            (dictionary3 = dictionary)[netId2 = item.netID] = dictionary3[netId2] + item.stack;
                        }
                        else
                        {
                            dictionary[item.netID] = item.stack;
                        }
                    }
                }

                int num4 = 0;
                while (num4 < Terraria.Recipe.maxRecipes && Main.recipe[num4].createItem.type != ItemID.None)
                {
                    bool flag2 = true;
                    if (flag2)
                    {
                        int num5 = 0;
                        while (num5 < Terraria.Recipe.maxRequirements && Main.recipe[num4].requiredTile[num5] != -1)
                        {
                            if (!Main.player[Main.myPlayer].adjTile[Main.recipe[num4].requiredTile[num5]])
                            {
                                flag2 = false;
                                break;
                            }

                            num5++;
                        }
                    }

                    if (flag2)
                        for (int m = 0; m < Terraria.Recipe.maxRequirements; m++)
                        {
                            item = Main.recipe[num4].requiredItem[m];
                            if (item.type == ItemID.None)
                                break;
                            int num6 = item.stack;
                            bool flag3 = false;
                            foreach (int current in dictionary.Keys.Where(current =>
                                Main.recipe[num4].useWood(current, item.type) || Main.recipe[num4].useSand(current, item.type) ||
                                Main.recipe[num4].useIronBar(current, item.type) || Main.recipe[num4].useFragment(current, item.type) ||
                                Main.recipe[num4].AcceptedByItemGroups(current, item.type) || Main.recipe[num4].usePressurePlate(current, item.type)))
                            {
                                num6 -= dictionary[current];
                                flag3 = true;
                            }

                            if (!flag3 && dictionary.ContainsKey(item.netID))
                                num6 -= dictionary[item.netID];

                            if (num6 <= 0)
                                continue;
                            flag2 = false;
                            break;
                        }

                    if (flag2)
                    {
                        bool flag4 = !Main.recipe[num4].needWater || Main.player[Main.myPlayer].adjWater || Main.player[Main.myPlayer].adjTile[172];
                        bool flag5 = !Main.recipe[num4].needHoney || Main.recipe[num4].needHoney == Main.player[Main.myPlayer].adjHoney;
                        bool flag6 = !Main.recipe[num4].needLava || Main.recipe[num4].needLava == Main.player[Main.myPlayer].adjLava;
                        bool flag7 = !Main.recipe[num4].needSnowBiome || Main.player[Main.myPlayer].ZoneSnow;
                        if (!flag4 || !flag5 || !flag6 || !flag7)
                            flag2 = false;
                    }

                    if (flag2 && RecipeHooks.RecipeAvailable(Main.recipe[num4]))
                    {
                        Main.availableRecipe[Main.numAvailableRecipes] = num4;
                        Main.numAvailableRecipes++;
                    }

                    num4++;
                }
            }

            for (int n = 0; n < Main.numAvailableRecipes; n++)
            {
                if (num != Main.availableRecipe[n])
                    continue;
                Main.focusRecipe = n;
                break;
            }

            if (Main.focusRecipe >= Main.numAvailableRecipes)
                Main.focusRecipe = Main.numAvailableRecipes - 1;
            if (Main.focusRecipe < 0)
                Main.focusRecipe = 0;
            float num7 = Main.availableRecipeY[Main.focusRecipe] - num2;
            for (int num8 = 0; num8 < Terraria.Recipe.maxRecipes; num8++)
                Main.availableRecipeY[num8] -= num7;
        }
        public static void CraftItem(this Terraria.Recipe r)
        {
            int stack = Main.mouseItem.stack;
            Main.mouseItem = r.createItem.Clone();
            Main.mouseItem.stack += stack;
            if (stack <= 0)
                Main.mouseItem.Prefix(-1);
            Main.mouseItem.position.X = Main.player[Main.myPlayer].position.X + Main.player[Main.myPlayer].width / 2f - Main.mouseItem.width / 2f;
            Main.mouseItem.position.Y = Main.player[Main.myPlayer].position.Y + Main.player[Main.myPlayer].height / 2f - Main.mouseItem.height / 2f;
            ItemText.NewText(Main.mouseItem, r.createItem.stack);
            r.ApiCreate();
            if (Main.mouseItem.type <= ItemID.None && r.createItem.type <= 0)
                return;
            RecipeHooks.OnCraft(Main.mouseItem, r);
            ItemLoader.OnCraft(Main.mouseItem, r);
            //Main.PlaySound(7);
            SoundManager.PlaySound(Sounds.Grass);
        }

        public static void InvLogic(this Recipe recipe, Item[] array, Item requiredItem, int requiredAmount)
        {
            for (int k = 0; k < array.Length; k++)
            {
                Item item2 = array[k];
                if (requiredAmount <= 0)
                    break;

                if (!item2.IsTheSameAs(requiredItem) && !recipe.useWood(item2.type, requiredItem.type) && !recipe.useSand(item2.type, requiredItem.type) &&
                    !recipe.useFragment(item2.type, requiredItem.type) && !recipe.useIronBar(item2.type, requiredItem.type) &&
                    !recipe.usePressurePlate(item2.type, requiredItem.type) && !recipe.AcceptedByItemGroups(item2.type, requiredItem.type))
                    continue;
                if (item2.stack > requiredAmount)
                {
                    item2.stack -= requiredAmount;
                    requiredAmount = 0;
                }
                else
                {
                    requiredAmount -= item2.stack;
                    array[k] = new Item();
                }
            }
        }
    }
}
