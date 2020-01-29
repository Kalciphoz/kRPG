using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ModLoader;

namespace kRPG.Commands
{
    public class RitualsCommand : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Chat; }
        }

        public override string Command
        {
            get { return "toggleritual"; }
        }

        public override string Usage
        {
            get { return "/toggleritual <name>"; }
        }

        public override string Description
        {
	        get { return "Switches a ritual on or off"; }
        }

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            try
            {
                RITUAL ritual = kRPG.ritualByName[args[0]];
                PlayerCharacter character = caller.Player.GetModPlayer<PlayerCharacter>();
                character.rituals[ritual] = !character.rituals[ritual];
            }
            catch (KeyNotFoundException)
            {
                throw new UsageException("Ritual not found: " + args);
            }
        }
    }
}
