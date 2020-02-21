using kRPG.Content.Players;
using Terraria.ModLoader;

namespace kRPG.Commands
{
    public class LevelCommand : ModCommand
    {
        public override string Command => "level";

        public override string Description => "Sets your character level to the chosen value";
        public override CommandType Type => CommandType.Chat;

        public override string Usage => "/level <level>";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            PlayerCharacter character = caller.Player.GetModPlayer<PlayerCharacter>();
            float xp = character.Experience / (float) character.ExperienceToLevel();
            character.Level = int.Parse(args[0]);
            character.Experience = (int) (character.ExperienceToLevel() * xp);
        }
    }
}