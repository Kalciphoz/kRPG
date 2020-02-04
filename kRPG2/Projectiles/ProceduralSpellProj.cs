//  Fairfield Tek L.L.C.
//  Copyright (c) 2016, Fairfield Tek L.L.C.
// 
// 
// THIS SOFTWARE IS PROVIDED BY FairfieldTek LLC ''AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL FAIRFIELDTEK LLC BE LIABLE FOR ANY DIRECT, INDIRECT,
// INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
// OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
// DAMAGE.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using kRPG2.Enums;
using kRPG2.Items.Glyphs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG2.Projectiles
{
    public class ProceduralSpellProj : ProceduralProjectile
    {
        public static Action<ProceduralSpellProj> aiRotateToVelocity = delegate(ProceduralSpellProj spell)
        {
            spell.projectile.rotation = (float) Math.Atan2(spell.projectile.velocity.Y, spell.projectile.velocity.X) + (float) API.Tau / 8f;
        };

        public static Action<ProceduralSpellProj> aiWhirlcast = delegate(ProceduralSpellProj spell)
        {
            var owner = Main.player[spell.projectile.owner];
        };

        private readonly Queue<Vector2> oldPositions = new Queue<Vector2>();
        private readonly Queue<float> oldRotations = new Queue<float>();

        public List<Action<ProceduralSpellProj>> ai { get; set; } = new List<Action<ProceduralSpellProj>>(); // <=== Look here:
        public float Alpha { get; set; }
        public Vector2 BasePosition { get; set; } = Vector2.Zero;
        public Vector2 BaseVelocity { get; set; } = Vector2.Zero;
        public bool Bounce { get; set; } = false;
        public Entity Caster { get; set; }

        public float DisplacementAngle { get; set; } = 0f;
        public Vector2 DisplacementVelocity { get; set; } = Vector2.Zero;

        public List<Action<ProceduralSpellProj, SpriteBatch, Color>> draw { get; set; } = new List<Action<ProceduralSpellProj, SpriteBatch, Color>>();
        public bool DrawTrail { get; set; } = false;
        public bool Homing { get; set; } = false;

        public List<Action<ProceduralSpellProj, NPC, int>> Impacts { get; set; } = new List<Action<ProceduralSpellProj, NPC, int>>();

        public List<Action<ProceduralSpellProj>> Inits { get; set; } = new List<Action<ProceduralSpellProj>>();

        public List<Action<ProceduralSpellProj>> Kills { get; set; } = new List<Action<ProceduralSpellProj>>();

        public bool Lighted { get; set; } = false;
        public bool Minion => Caster is Projectile;
        public Vector2 Origin { get; set; } = Vector2.Zero;
        public Item SelectedItem { get; set; }
        public ProceduralSpell Source { get; set; }

        public override void AI()
        {
            if (Alpha < 1f) Alpha += 0.02f;
            if (Caster == null) projectile.Kill();
            else if (!Caster.active) projectile.Kill();
            foreach (var action in ai.Where(action => action != null))
                action(this);
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override ModProjectile Clone()
        {
            var copy = (ProceduralSpellProj) MemberwiseClone();
            copy.ai = new List<Action<ProceduralSpellProj>>();
            foreach (var action in ai) copy.ai.Add(action);
            copy.Impacts = new List<Action<ProceduralSpellProj, NPC, int>>();
            foreach (var action in Impacts) copy.Impacts.Add(action);
            copy.Inits = new List<Action<ProceduralSpellProj>>();
            foreach (var action in Inits) copy.Inits.Add(action);
            copy.Kills = new List<Action<ProceduralSpellProj>>();
            foreach (var action in Kills) copy.Kills.Add(action);
            return copy;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale)
        {
            foreach (var action in draw.Where(action => action != null))
                action(this, spriteBatch, color);
            if (LocalTexture == null)
            {
                Initialize();
                return;
            }

            if (DrawTrail)
            {
                for (int i = 0; i < oldPositions.Count; i += 1)
                    spriteBatch.Draw(LocalTexture, oldPositions.ElementAt(i) - Main.screenPosition + LocalTexture.Bounds.Center(), null,
                        (Lighted ? Color.White : color) * Alpha * (0.04f + 0.09f * i), oldRotations.ElementAt(i), LocalTexture.Bounds.Center(), scale,
                        projectile.spriteDirection >= 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

                oldPositions.Enqueue(position + Main.screenPosition);
                oldRotations.Enqueue(rotation);
                if (oldPositions.Count > 5)
                {
                    oldPositions.Dequeue();
                    oldRotations.Dequeue();
                }
            }

            spriteBatch.Draw(LocalTexture, position + LocalTexture.Bounds.Center(), null, (Lighted ? Color.White : color) * Alpha, rotation,
                LocalTexture.Bounds.Center(), scale, projectile.spriteDirection >= 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        }

        public override void Initialize()
        {
            foreach (var action in Inits.Where(action => action != null))
                action(this);
            if (!Minion)
                return;
            projectile.melee = false;
            projectile.ranged = false;
            projectile.magic = false;
        }

        public override void Kill(int timeLeft)
        {
            foreach (var action in Kills.Where(action => action != null))
                action(this);
            if (timeLeft > 0)
                Main.PlaySound(SoundID.Dig, projectile.position);
        }

        // ReSharper disable once IdentifierTypo
        public override void OnHitNPC(NPC target, int damage, float knockBack, bool crit)
        {
            foreach (var action in Impacts.Where(action => action != null))
                action(this, target, damage);
        }

        public override bool OnTileCollide(Vector2 velocity)
        {
            if (!Bounce)
                return true;
            Main.PlaySound(SoundID.Item10, projectile.position);

            if (Math.Abs(projectile.velocity.Y - velocity.Y) > .01)
                projectile.velocity.Y = -velocity.Y;

            if (Math.Abs(projectile.velocity.X - velocity.X) > .01)
                projectile.velocity.X = -velocity.X;

            return false;
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.owner = reader.ReadInt32();
            int starType = reader.ReadInt32();
            int crossType = reader.ReadInt32();
            int moonType = reader.ReadInt32();
            projectile.damage = reader.ReadInt32();
            bool minionCaster = reader.ReadBoolean();
            Caster = minionCaster ? Main.projectile[reader.ReadInt32()] : (Entity) Main.player[reader.ReadInt32()];
            var modifiers = new List<GlyphModifier>();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i += 1)
                modifiers.Add(GlyphModifier.Modifiers[reader.ReadInt32()]);
            if (Source == null)
            {
                Source = new ProceduralSpell(mod);
                Source.Glyphs[(byte) GLYPHTYPE.STAR].SetDefaults(starType, true);
                Source.Glyphs[(byte) GLYPHTYPE.CROSS].SetDefaults(crossType, true);
                Source.Glyphs[(byte) GLYPHTYPE.MOON].SetDefaults(moonType, true);
                Source.ModifierOverride = modifiers;
            }

            foreach (var item in Source.Glyphs)
            {
                var glyph = (Glyph) item.modItem;
                if (glyph.GetAiAction() != null)
                    ai.Add(glyph.GetAiAction());
                if (glyph.GetInitAction() != null)
                    Inits.Add(glyph.GetInitAction());
                if (glyph.GetImpactAction() != null)
                    Impacts.Add(glyph.GetImpactAction());
                if (glyph.GetKillAction() != null)
                    Kills.Add(glyph.GetKillAction());
            }

            foreach (var modifier in modifiers)
            {
                if (modifier.Impact != null)
                    Impacts.Add(modifier.Impact);
                if (modifier.Draw != null)
                    draw.Add(modifier.Draw);
                if (modifier.Init != null)
                    Inits.Add(modifier.Init);
            }

            Initialize();
        }

        public Vector2 RelativePos(Vector2 referencePoint)
        {
            return projectile.Center - referencePoint;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            if (Source == null) return;
            writer.Write(projectile.owner);
            writer.Write(Source.Glyphs[(byte) GLYPHTYPE.STAR].type);
            writer.Write(Source.Glyphs[(byte) GLYPHTYPE.CROSS].type);
            writer.Write(Source.Glyphs[(byte) GLYPHTYPE.MOON].type);
            writer.Write(projectile.damage);
            writer.Write(Minion);
            writer.Write(Caster.whoAmI);
            var modifiers = Source.Modifiers;
            writer.Write(modifiers.Count);
            for (int j = 0; j < modifiers.Count; j += 1)
                writer.Write(modifiers[j].Id);
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.scale = 1f;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 210;
            projectile.tileCollide = false;
            projectile.knockBack = 4f;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Procedurally Generated Spell-Projectile; Please Ignore");
        }
    }
}