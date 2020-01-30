using System;
using kRPG.Items;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Commands
{
    public class UpgradeCommand : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Chat; }
        }

        public override string Command
        {
            get { return "upgrade"; }
        }

        public override string Usage
        {
            get { return "/upgrade <upgradelevel>"; }
        }

        public override string Description
        {
	        get { return "Upgrades the mouse item to the specified level"; }
        }

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            byte upgrade = Byte.Parse(args[0]);
            mod.GetGlobalItem<kItem>().SetUpgradeLevel(Main.mouseItem, upgrade);
        }
    }
}
