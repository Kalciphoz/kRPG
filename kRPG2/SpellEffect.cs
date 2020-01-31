using System;
using Microsoft.Xna.Framework;
using Terraria;

namespace kRPG2
{
    public class SpellEffect
    {
        public SpellEffect(ProceduralSpell ability, Vector2 target, int timeLeft, Action<ProceduralSpell, int> update)
        {
            Ability = ability;
            Target = target;
            TimeLeft = timeLeft;
            this.update = update;
            Main.player[Main.myPlayer].GetModPlayer<PlayerCharacter>().SpellEffects.Add(this);
        }

        private ProceduralSpell Ability { get; }
        private Vector2 Target { get; }
        private int TimeLeft { get; set; }
        private Action<ProceduralSpell, int> update { get; }

        public void Update(PlayerCharacter character)
        {
            update(Ability, TimeLeft);
            TimeLeft -= 1;
            if (TimeLeft <= 0)
                character.SpellEffects.Remove(this);
        }
    }
}