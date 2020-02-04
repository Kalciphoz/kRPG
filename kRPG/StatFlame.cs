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
using kRPG.Enums;
using kRPG.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG
{
    public class StatFlame
    {
        private readonly int animationTime = 5;

        public StatFlame(Mod mod, LevelGui levelGui, STAT id, Func<Vector2> position, Texture2D texture)
        {
            Mod = mod;
            LevelGui = levelGui;
            Id = id;
            Position = position;
            Texture = texture;
            Counter = (int) id * 8;
        }

        private int Allocated
        {
            get => LevelGui.allocated[Id];
            set => LevelGui.allocated[Id] = value;
        }

        private int Counter { get; set; }
        private int FrameNumber { get; set; }
        private STAT Id { get; }
        private LevelGui LevelGui { get; }
        private Mod Mod { get; }
        private Func<Vector2> Position { get; }
        private Texture2D Texture { get; }

        public bool CheckHover()
        {
            return Main.mouseX >= Position().X && Main.mouseY >= Position().Y && Main.mouseX <= Position().X + Texture.Width &&
                   Main.mouseY <= Position().Y + 68;
        }

        public void Draw(SpriteBatch spriteBatch, Player player, float scale)
        {
            PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
            if (Counter > 8 * animationTime - 1) Counter = 0;
            FrameNumber = (int) Math.Floor(Counter / (double) animationTime);
            spriteBatch.Draw(character.Rituals[RITUAL.DEMON_PACT] && Id == STAT.RESILIENCE ? GFX.FlamesConverted : Texture, Position(),
                new Rectangle(0, FrameNumber * 68, 56, 68), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            string text = (Allocated + character.BaseStats[Id]).ToString();
            float width = Main.fontItemStack.MeasureString(text).X;
            spriteBatch.DrawStringWithShadow(Main.fontItemStack, text, Position() + new Vector2(28f - width / 2f, 36f) * scale,
                Allocated > 0 ? Color.Lime : Color.White, scale);
            Counter++;
        }

        public void Update(SpriteBatch spriteBatch, Player player)
        {
            PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();

            if (!CheckHover())
                return;
            switch (Id)
            {
                case STAT.RESILIENCE:
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Resilience", new Vector2(Main.screenWidth / 2f - 96f, Main.screenHeight / 2f + 128f),
                        Color.Red);
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText,
                        character.Rituals[RITUAL.DEMON_PACT]
                            ? "Converted into Potency by Demon Pact"
                            : "Increases your defence, life regeneration, and maximum life",
                        new Vector2(Main.screenWidth / 2f - 128f, Main.screenHeight / 2f + 152f), Color.White);
                    break;
                case STAT.QUICKNESS:
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Quickness", new Vector2(Main.screenWidth / 2f - 96f, Main.screenHeight / 2f + 128f),
                        Color.Lime);
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Increases your speed, evasion, and crit chance",
                        new Vector2(Main.screenWidth / 2f - 128f, Main.screenHeight / 2f + 152f), Color.White);
                    break;
                case STAT.POTENCY:
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Potency", new Vector2(Main.screenWidth / 2f - 96f, Main.screenHeight / 2f + 128f),
                        Color.Blue);
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Increases your damage, leech, and crit multiplier",
                        new Vector2(Main.screenWidth / 2f - 128f, Main.screenHeight / 2f + 152f), Color.White);
                    break;
            }

            if (Allocated == 0)
                spriteBatch.DrawStringWithShadow(Main.fontMouseText, "<Click to allocate>",
                    new Vector2(Main.screenWidth / 2f - 128f, Main.screenHeight / 2f + 176f), Color.White);
            else
                spriteBatch.DrawStringWithShadow(Main.fontMouseText, "<Allocated " + Allocated + ">",
                    new Vector2(Main.screenWidth / 2f - 128f, Main.screenHeight / 2f + 176f), Color.White);

            int total = LevelGui.allocated.Keys.Sum(stat => LevelGui.allocated[stat]);
            if (Main.mouseLeft && Main.mouseLeftRelease && total + character.PointsAllocated < character.Level - 1)
            {
                Main.PlaySound(SoundID.MenuTick);
                Allocated += 1;
            }

            if (Main.mouseRight && Main.mouseRightRelease && Allocated > 0)
            {
                Main.PlaySound(SoundID.MenuTick);
                Allocated -= 1;
            }
        }
    }
}