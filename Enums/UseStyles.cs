using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kRPG.Enums
{
    public enum UseStyles:int
    {
        None = 0,
        /// <summary>
        /// Used for: 80% of every usable item including, Swords, Acorns, Timers, etc
        /// </summary>
        GeneralSwingingThrowing = 1,
        /// <summary>
        /// Used for: All potions and healing items.
        /// </summary>
        EatingUsing = 2,
        /// <summary>
        /// Used for: Shortswords
        /// </summary>
        Stabbing = 3,
        /// <summary>
        /// Used for: Life Crystals, Mana Crystals, Summoning Items
        /// </summary>
        HoldingUp = 4,
        /// <summary>
        /// Used for: Guns, Spellbooks, Drills, Chainsaws, Flails, Spears
        /// </summary>
        HoldingOut = 5
    }
}
