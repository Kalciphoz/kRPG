using System;
using Microsoft.Xna.Framework;
using Terraria;

namespace kRPG2
{
    public class SpellEffect
    {
        private  ProceduralSpell Ability { get;  }
        private Vector2 Target { get; set; }
        private int TimeLeft { get; set; }
        private Action<ProceduralSpell, int> update { get;  }

        public SpellEffect(ProceduralSpell ability, Vector2 target, int timeLeft, Action<ProceduralSpell, int> update)
        {
            this.Ability = ability;
            this.Target = target;
            this.TimeLeft = timeLeft;
            this.update = update;
            Main.player[Main.myPlayer].GetModPlayer<PlayerCharacter>().SpellEffects.Add(this);
        }

        public void Update(PlayerCharacter character)
        {
            update(Ability, TimeLeft);
            TimeLeft -= 1;
            if (TimeLeft <= 0)
                character.SpellEffects.Remove(this);
        }
    }
}
