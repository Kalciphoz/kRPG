using System;
using Terraria.ModLoader;

namespace kRPG.Commands
{
    public class LevelCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "level";

        public override string Usage => "/level <level>";

        public override string Description => "Sets your character level to the chosen value";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            var character = caller.Player.GetModPlayer<PlayerCharacter>();
            float xp = (float) character.xp / (float) character.ExperienceToLevel();
            character.level = int.Parse(args[0]);
            character.xp = (int) (character.ExperienceToLevel() * xp);
        }
    }
}