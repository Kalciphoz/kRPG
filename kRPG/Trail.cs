// Kalciphoz's RPG Mod
//  Copyright (c) 2016, Kalciphoz's RPG Mod
// 
// 
// THIS SOFTWARE IS PROVIDED BY Kalciphoz's ''AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES,
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace kRPG
{
    public class Trail
    {
        public Trail(Vector2 position, int timeleft, Action<SpriteBatch, Player, Vector2, Vector2[], float> draw)
        {
            Position = position;
            Timeleft = timeleft;
            this.draw = draw;
            Scale = 1f;
        }

        public Vector2[] Displacement { get; set; }

        private Action<SpriteBatch, Player, Vector2, Vector2[], float> draw { get; }

        private Vector2 Position { get; }
        public float Scale { get; set; } = 1f;
        private int Timeleft { get; set; }

        public void Draw(SpriteBatch spritebatch, Player player)
        {
            Timeleft -= 1;
            for (int i = 0; i < Displacement.Length; i += 1)
                Displacement[i] += new Vector2(0.6f, 0f).RotatedBy(Displacement[i].ToRotation());
            draw(spritebatch, player, Position, Displacement, Scale);
            Scale -= 0.01f;
            if (Timeleft <= 0)
                player.GetModPlayer<PlayerCharacter>().Trails.Remove(this);
        }
    }
}