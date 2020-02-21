using System;
using System.Linq;
using kRPG.Content.Players;
using Terraria;
using Terraria.ID;

namespace kRPG.Content.Currency
{
    public static class Coins
    {

        public const byte Copper = 0;
        public const byte Silver = 1;
        public const byte Gold = 2;
        public const byte Platinum = 3;


        public static int CoinStackValue(Item i)
        {
            int coins = 0;
            switch (i.type)
            {
                case ItemID.CopperCoin:
                    coins += i.stack;
                    break;
                case ItemID.SilverCoin:
                    coins += i.stack * 100;
                    break;
                case ItemID.GoldCoin:
                    coins += i.stack * 10000;
                    break;
                case ItemID.PlatinumCoin:
                    coins += i.stack * 1000000;
                    break;
            }

            return coins;
        }


        public static int[] CountCoins(Item i)
        {
            int[] coins = new int[4];
            switch (i.type)
            {
                case ItemID.CopperCoin:
                    coins[Coins.Copper] += i.stack;
                    break;
                case ItemID.SilverCoin:
                    coins[Coins.Silver] += i.stack * 100;
                    break;
                case ItemID.GoldCoin:
                    coins[Coins.Gold] += i.stack * 10000;
                    break;
                case ItemID.PlatinumCoin:
                    coins[Coins.Platinum] += i.stack * 1000000;
                    break;
            }

            return coins;
        }

        public static int Wealth(this Player player)
        {
            PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();

            int coins = player.inventory.Sum(CoinStackValue);

            for (int i = 0; i < character.Inventories.Length; i += 1)
                if (character.ActiveInvPage != i)
                    coins += character.Inventories[i].Sum(CoinStackValue);

            coins += player.bank.item.Sum(CoinStackValue);

            coins += player.bank2.item.Sum(CoinStackValue);

            coins += player.bank3.item.Sum(CoinStackValue);

            return coins;
        }

        public static string MoneyToString(int amount)
        {
            string output = "";
            int[] coins = Coins.SeparateCoinTypes(amount);

            if (coins[Coins.Platinum] > 0)
                output += coins[Coins.Platinum] + " platinum ";
            if (coins[Coins.Gold] > 0)
                output += coins[Coins.Gold] + " gold ";
            if (coins[Coins.Silver] > 0)
                output += coins[Coins.Silver] + " silver ";
            if (coins[Coins.Copper] > 0)
                output += coins[Coins.Copper] + " copper ";

            return output;
        }

        public static int[] SeparateCoinTypes(int amount)
        {
            //splitting the cost into individual coin types
            int[] output = new int[4];
            output[Copper] = Math.Max(0, amount % 100);
            output[Silver] = Math.Max(0, (amount % 10000 - output[Copper]) / 100);
            output[Gold] = Math.Max(0, (amount % 1000000 - output[Copper] - output[Silver]) / 10000);
            output[Platinum] = Math.Max(0, (amount - output[Copper] - output[Silver] - output[Gold]) / 1000000);

            return output;
        }

        public static void RemoveCoins(this Player player, int amount)
        {
            int[] coinType = { 71, 72, 73, 74 };

            //splitting the cost into individual coin types
            int[] cost = SeparateCoinTypes(amount);

            int[] coins = new int[4];
            for (int i = 0; i < coins.Length; i++)
                coins[i] = 0;

            coins = SeparateCoinTypes(player.Wealth());

            PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
            //foreach (Item[] inventory in character.inventories)
            //    foreach (Item i in inventory)
            //        coins = SumCoins(coins, CountCoins(i));

            //foreach (Item i in player.bank.item)
            //    coins = SumCoins(coins, CountCoins(i));

            //foreach (Item i in player.bank2.item)
            //    coins = SumCoins(coins, CountCoins(i));

            //foreach (Item i in player.bank3.item)
            //    coins = SumCoins(coins, CountCoins(i));

            for (int i = 0; i < 4; i++)
                if (coins[i] >= cost[i])
                {
                    foreach (Item item in player.inventory)
                        item.stack = RemoveCoins(item, coinType[i], ref cost[i]);

                    for (int j = 0; j < character.Inventories.Length; j += 1)
                        if (character.ActiveInvPage != j)
                            foreach (Item item in character.Inventories[j])
                                item.stack = RemoveCoins(item, coinType[i], ref cost[i]);

                    foreach (Item item in player.bank.item)
                        item.stack = RemoveCoins(item, coinType[i], ref cost[i]);

                    foreach (Item item in player.bank2.item)
                        item.stack = RemoveCoins(item, coinType[i], ref cost[i]);

                    foreach (Item item in player.bank3.item)
                        item.stack = RemoveCoins(item, coinType[i], ref cost[i]);
                }

                else
                {
                    cost[i + 1] += 1;
                    cost[i] -= 100;
                    Item.NewItem((int)player.position.X, (int)player.position.Y, 0, 0, coinType[i], -cost[i], true, 0, true);
                }
        }

        public static int RemoveCoins(Item item, int coinType, ref int amount)
        {
            int stackSize = item.stack;
            if (item.type != coinType)
                return stackSize;
            if (stackSize >= amount)
            {
                stackSize -= amount;
                amount = 0;
            }
            else
            {
                amount -= item.stack;
                stackSize = 0;
            }

            return stackSize;
        }
    }
}
