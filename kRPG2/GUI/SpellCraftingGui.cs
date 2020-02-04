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
using System.Linq;
using kRPG2.Enums;
using kRPG2.Items.Glyphs;
using kRPG2.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Star = kRPG2.Items.Glyphs.Star;

namespace kRPG2.GUI
{
    public class SpellcraftingGUI : BaseGui
    {
        public GlyphSlot[] glyphs = new GlyphSlot[3];
        private readonly Func<Vector2> guiPosition;

        public SpellcraftingGUI(Mod mod)
        {
            guiPosition = () => new Vector2(Main.screenWidth / 2f - 100f * Scale, 192f * Scale);

            glyphs[0] = new GlyphSlot(() => guiPosition() + new Vector2(84f, 36f) * Scale, () => Scale, GLYPHTYPE.STAR);

            glyphs[1] = new GlyphSlot(() => guiPosition() + new Vector2(84f, 70f) * Scale, () => Scale, GLYPHTYPE.CROSS);

            glyphs[2] = new GlyphSlot(() => guiPosition() + new Vector2(84f, 106f) * Scale, () => Scale, GLYPHTYPE.MOON);

            GuiElements.Add(this);
        }

        private float Scale => Math.Min(1f, Main.screenWidth / Constants.MaxScreenWidth + 0.4f);

        public override void OnClose()
        {
            Main.LocalPlayer.GetModPlayer<PlayerCharacter>().SelectedAbility = null;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Player player)
        {
            spriteBatch.Draw(GFX.SpellGui, guiPosition(), null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
            foreach (var slot in glyphs)
                slot.Draw(spriteBatch);

            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Place glyphs in all three slots to create a spell",
                new Vector2(Main.screenWidth / 2f - 176f * Scale, Main.screenHeight / 2f + 200f * Scale), Color.White, Scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Press a key while holding shift to bind it as a hotkey",
                new Vector2(Main.screenWidth / 2f - 176f * Scale, Main.screenHeight / 2f + 224f * Scale), Color.White, Scale);

            var buttonPosition = new Vector2(Main.screenWidth / 2f - 92f * Scale, Main.screenHeight / 2f + 256f * Scale);
            spriteBatch.Draw(GFX.ButtonClose, buttonPosition, Color.White, Scale);

            if (!(Main.mouseX >= buttonPosition.X) || !(Main.mouseY >= buttonPosition.Y) ||
                !(Main.mouseX <= buttonPosition.X + (int) (GFX.ButtonConfirm.Width * Scale)) ||
                !(Main.mouseY <= buttonPosition.Y + (int) (GFX.ButtonConfirm.Height * Scale)))
                return;
            Main.LocalPlayer.mouseInterface = true;
            if (!Main.mouseLeft || !Main.mouseLeftRelease)
                return;
            Main.PlaySound(SoundID.MenuTick);
            CloseGui();
        }

        public override bool PreDraw()
        {
            return Main.LocalPlayer.GetModPlayer<PlayerCharacter>().SelectedAbility != null;
        }
    }

    public class GlyphSlot
    {
        private readonly Func<Vector2> position;
        private readonly Func<float> scale;
        private readonly GLYPHTYPE type;

        public GlyphSlot(Func<Vector2> position, Func<float> scale, GLYPHTYPE type)
        {
            this.type = type;
            this.position = position;
            this.scale = scale;
        }

        private ProceduralSpell Ability => Main.LocalPlayer.GetModPlayer<PlayerCharacter>().SelectedAbility;
        private Rectangle Bounds => new Rectangle((int) position().X, (int) position().Y, (int) (30 * scale()), (int) (30 * scale()));

        private Item Glyph
        {
            get => Ability.Glyphs[(byte) type];
            set => Ability.Glyphs[(byte) type] = value;
        }

        public bool AttemptPlace()
        {
            var character = Main.LocalPlayer.GetModPlayer<PlayerCharacter>();

            if (!CanPlaceItem(Main.mouseItem))
                return false;

            foreach (var minion in character.Minions.Where(minion =>
                minion.Source == character.SelectedAbility && minion.projectile.modProjectile is ProceduralMinion))
            {
                foreach (var psp in minion.CirclingProtection)
                    psp.projectile.Kill();
                minion.CirclingProtection.Clear();
                minion.SmallProt?.projectile.Kill();
                minion.projectile.Kill();
            }

            var prevItem = Glyph;
            Glyph = Main.mouseItem;
            Main.mouseItem = prevItem;
            Main.PlaySound(SoundID.Item4, Main.screenPosition + Bounds.Center());
            return true;
        }

        private bool CanPlaceItem(Item item)
        {
            bool check = false;
            switch (type)
            {
                case GLYPHTYPE.STAR:
                    check = item.modItem is Star;
                    break;
                case GLYPHTYPE.CROSS:
                    check = item.modItem is Cross;
                    break;
                case GLYPHTYPE.MOON:
                    check = item.modItem is Moon;
                    break;
            }

            return check || item.type == 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Bounds.Contains(Main.mouseX, Main.mouseY))
            {
                Main.LocalPlayer.mouseInterface = true;
                Main.HoverItem = Glyph.Clone();
                if (Main.mouseLeft && Main.mouseLeftRelease) AttemptPlace();
            }

            if (Glyph.type == 0) return;
            var texture = Main.itemTexture[Glyph.type];
            spriteBatch.Draw(texture, Bounds.TopLeft() + new Vector2(2f, 2f), Color.White, scale());
        }
    }
}