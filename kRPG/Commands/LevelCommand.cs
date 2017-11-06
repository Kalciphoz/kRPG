using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ModLoader;

namespace kRPG.Commands
{
    public class LevelCommand : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Chat; }
        }

        public override string Command
        {
            get { return "level"; }
        }

        public override string Usage
        {
            get { return "/level <level>"; }
        }

        public override string Description
        {
	        get { return "Sets your character level to the chosen value"; }
        }

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            PlayerCharacter character = caller.Player.GetModPlayer<PlayerCharacter>(mod);
            float xp = character.xp / character.ExperienceToLevel();
            character.level = Int32.Parse(args[0]);
            character.xp = (int)(character.ExperienceToLevel() * xp);
        }
    }
}
