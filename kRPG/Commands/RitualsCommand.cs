using System.Collections.Generic;
using Terraria.ModLoader;

namespace kRPG.Commands
{
    public class RitualsCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "toggleritual";

        public override string Usage => "/toggleritual <name>";

        public override string Description => "Switches a ritual on or off";

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
