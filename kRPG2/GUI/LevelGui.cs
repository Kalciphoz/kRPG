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
using System.Linq;
using kRPG2.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG2.GUI
{
    public class LevelGui : BaseGui
    {
        public Dictionary<STAT, int> allocated = new Dictionary<STAT, int> {{STAT.RESILIENCE, 0}, {STAT.QUICKNESS, 0}, {STAT.POTENCY, 0}};
        private PlayerCharacter character;

        private readonly Dictionary<STAT, StatFlame> statFlame;

        public LevelGui(PlayerCharacter character, Mod mod)
        {
            this.character = character;

            statFlame = new Dictionary<STAT, StatFlame>
            {
                [STAT.RESILIENCE] = new StatFlame(mod, this, STAT.RESILIENCE, () => Position[STAT.RESILIENCE], GFX.Flames[STAT.RESILIENCE]),
                [STAT.QUICKNESS] = new StatFlame(mod, this, STAT.QUICKNESS, () => Position[STAT.QUICKNESS], GFX.Flames[STAT.QUICKNESS]),
                [STAT.POTENCY] = new StatFlame(mod, this, STAT.POTENCY, () => Position[STAT.POTENCY], GFX.Flames[STAT.POTENCY])
            };
        }

        private Vector2 GuiPosition => new Vector2(Main.screenWidth / 2f - Width, 50f * Scale + 50f);

        private Dictionary<STAT, Vector2> Position =>
            new Dictionary<STAT, Vector2>
            {
                {STAT.RESILIENCE, new Vector2(GuiPosition.X + 52f * Scale, GuiPosition.Y - 40f * Scale)},
                {STAT.QUICKNESS, new Vector2(GuiPosition.X + 172f * Scale, GuiPosition.Y)},
                {STAT.POTENCY, new Vector2(GuiPosition.X + 292f * Scale, GuiPosition.Y - 40f * Scale)}
            };

        private float Scale => Math.Min(1f, Main.screenWidth / Constants.MaxScreenWidth + 0.5f);

        private float Width => 200f * Scale;

        public override void PostDraw(SpriteBatch spriteBatch, Player player)
        {
            character = player.GetModPlayer<PlayerCharacter>();

            spriteBatch.Draw(GFX.DeerSkull, GuiPosition, Color.White, Scale);

            int remaining = character.Level - character.PointsAllocated - 1;
            remaining = allocated.Keys.Aggregate(remaining, (current, stat) => current - allocated[stat]);
            string text = "You have " + (remaining == 0 ? "no" : remaining.ToString()) + (remaining == 1 ? " point " : " points ") + "remaining";
            float width = Main.fontMouseText.MeasureString(text).X * Scale;

            spriteBatch.DrawStringWithShadow(Main.fontMouseText, text, GuiPosition - new Vector2(width / 2f - 200f, 38f * Scale + 38f), Color.White, Scale);

            var buttonPosition = new Vector2(Main.screenWidth / 2f - 92f * Scale, Main.screenHeight / 2f + 320f * Scale);
            spriteBatch.Draw(GFX.ButtonCancel, buttonPosition, Color.White, Scale);

            if (Main.mouseX >= buttonPosition.X && Main.mouseY >= buttonPosition.Y && Main.mouseX <= buttonPosition.X + GFX.ButtonConfirm.Width * Scale &&
                Main.mouseY <= buttonPosition.Y + GFX.ButtonConfirm.Height * Scale)
            {
                Main.LocalPlayer.mouseInterface = true;
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    Main.PlaySound(SoundID.MenuTick);
                    CloseGui();
                    return;
                }
            }

            statFlame[STAT.RESILIENCE].Draw(spriteBatch, player, Scale);
            statFlame[STAT.RESILIENCE].Update(spriteBatch, player);
            statFlame[STAT.QUICKNESS].Draw(spriteBatch, player, Scale);
            statFlame[STAT.QUICKNESS].Update(spriteBatch, player);
            statFlame[STAT.POTENCY].Draw(spriteBatch, player, Scale);
            statFlame[STAT.POTENCY].Update(spriteBatch, player);

            buttonPosition = new Vector2(Main.screenWidth / 2f - 92f * Scale, Main.screenHeight / 2f + 256f * Scale);
            spriteBatch.Draw(GFX.ButtonConfirm, buttonPosition, Color.White, Scale);

            if (Main.mouseX >= buttonPosition.X && Main.mouseY >= buttonPosition.Y && Main.mouseX <= buttonPosition.X + GFX.ButtonConfirm.Width &&
                Main.mouseY <= buttonPosition.Y + GFX.ButtonConfirm.Height)
            {
                Main.LocalPlayer.mouseInterface = true;
                if (Main.mouseLeft && Main.mouseLeftRelease)
                    try
                    {
                        Main.PlaySound(SoundID.MenuTick);
                        foreach (var s in allocated.Keys)
                            character.BaseStats[s] += allocated[s];
                        foreach (STAT stat in Enum.GetValues(typeof(STAT)))
                            allocated[stat] = 0;

                        GuiActive = false;
                        GFX.SfxLevelUp.Play(0.2f * Main.soundVolume, -0.6f, -0.2f);
                        return;
                    }
                    catch (SystemException e)
                    {
                        Main.NewText(e.ToString());
                    }
            }

            STAT? hoverStat = null;
            foreach (var s in statFlame.Keys.Where(s => statFlame[s].CheckHover()))
                hoverStat = s;

            if (hoverStat != null) spriteBatch.Draw(GFX.DeerSkullEyes[hoverStat.Value], GuiPosition, Color.White, Scale);
        }
    }
}