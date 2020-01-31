using System;
using System.Collections.Generic;
using System.Linq;
using kRPG2.Enums;
using kRPG2.Items.Glyphs;
using Microsoft.Xna.Framework;
using Terraria;

namespace kRPG2.Projectiles
{
    public class ProceduralMinion : ProceduralProjectile
    {
        public bool Attack { get; set; }
        public List<ProceduralSpellProj> CirclingProtection { get; set; } = new List<ProceduralSpellProj>();

        public int Cooldown { get; set; }
        protected float Distance { get; set; }
        public List<Action<ProceduralMinion>> GlyphModifiers { get; set; } = new List<Action<ProceduralMinion>>();

        // ReSharper disable once IdentifierTypo
        public ProceduralSpellProj SmallProt { get; set; } = null;
        public ProceduralSpell Source { get; set; }
        protected NPC Target { get; set; }

        public override void AI()
        {
            if (Main.netMode == 2) return;
            bool self = Source.Glyphs[(byte) GLYPHTYPE.MOON].modItem is Moon_Green;
            if ((!self || CirclingProtection.Count(spell => spell.projectile.active) <= Source.ProjCount - 3) && Cooldown <= 0)
            {
                if (!self)
                {
                    GetTarget();
                    if (Distance <= 480f && Attack)
                        if (this is ProceduralMinion)
                            Source.CastSpell(Main.player[projectile.owner], projectile.Center, Target.Center, projectile);
                }
                else if (this is ProceduralMinion)
                {
                    Source.CastSpell(Main.player[projectile.owner], projectile.Center, projectile.Center, projectile);
                }

                Cooldown = Source.Cooldown * 2;
            }
            else
            {
                Cooldown -= 1;
            }
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public NPC GetTarget()
        {
            Attack = false;
            Target = Main.npc.First();
            var player = Main.player[projectile.owner];
            Distance = Vector2.Distance(projectile.Center, Target.Center);
            foreach (var npc in Main.npc)
            {
                float f = Vector2.Distance(projectile.Center, npc.Center);
                if (!(f < Distance) || !npc.active || npc.life <= 0 || npc.friendly || npc.damage <= 0)
                    continue;
                Target = npc;
                Distance = f;
                Attack = true;
            }

            if (!player.HasMinionAttackTargetNPC)
                return Target;
            Target = Main.npc[player.MinionAttackTargetNPC];
            Attack = true;

            return Target;
        }

        public override void Kill(int timeLeft)
        {
            foreach (var spell in CirclingProtection)
                spell.projectile.Kill();
            CirclingProtection.Clear();
            SmallProt?.projectile.Kill();
        }

        public override void PostAI()
        {
            foreach (var modifier in GlyphModifiers)
                modifier(this);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Procedurally Generated Minion; Please Ignore");
        }
    }
}