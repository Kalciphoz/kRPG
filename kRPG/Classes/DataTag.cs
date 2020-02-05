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
using System.IO;

namespace kRPG.Classes
{
    public class DataTag
    {
        public DataTag(Func<BinaryReader, object> read)
        {
            Read = read;
        }

        public static DataTag Amount { get; set; } = new DataTag(reader => reader.ReadInt32());
        public static DataTag AmountSingle { get; set; } = new DataTag(reader => reader.ReadSingle());
        public static DataTag Cold { get; set; } = new DataTag(reader => reader.ReadInt32());
        public static DataTag Damage { get; set; } = new DataTag(reader => reader.ReadInt32());
        public static DataTag EntityId { get; set; } = new DataTag(reader => reader.ReadInt32());
        public static DataTag Fire { get; set; } = new DataTag(reader => reader.ReadInt32());
        public static DataTag Flag { get; set; } = new DataTag(reader => reader.ReadBoolean());
        public static DataTag Flag2 { get; set; } = new DataTag(reader => reader.ReadBoolean());
        public static DataTag Flag3 { get; set; } = new DataTag(reader => reader.ReadBoolean());
        public static DataTag Flag4 { get; set; } = new DataTag(reader => reader.ReadBoolean());
        public static DataTag GlyphCross { get; set; } = new DataTag(reader => reader.ReadInt32());
        public static DataTag GlyphMoon { get; set; } = new DataTag(reader => reader.ReadInt32());
        public static DataTag GlyphStar { get; set; } = new DataTag(reader => reader.ReadInt32());
        public static DataTag ItemDef { get; set; } = new DataTag(reader => reader.ReadInt32());
        public static DataTag ItemDps { get; set; } = new DataTag(reader => reader.ReadSingle());
        public static DataTag ItemId { get; set; } = new DataTag(reader => reader.ReadInt32());
        public static DataTag Lightning { get; set; } = new DataTag(reader => reader.ReadInt32());
        public static DataTag ModifierCount { get; set; } = new DataTag(reader => reader.ReadInt32());
        public static DataTag NpcId { get; set; } = new DataTag(reader => reader.ReadInt32());
        public static DataTag PartPrimary { get; set; } = new DataTag(reader => reader.ReadInt32());
        public static DataTag PartSecondary { get; set; } = new DataTag(reader => reader.ReadInt32());
        public static DataTag PartTertiary { get; set; } = new DataTag(reader => reader.ReadInt32());
        public static DataTag PlayerId { get; set; } = new DataTag(reader => reader.ReadInt32());
        public static DataTag Potency { get; set; } = new DataTag(reader => reader.ReadInt32());
        public static DataTag ProjCount { get; set; } = new DataTag(reader => reader.ReadInt32());
        public static DataTag ProjId { get; set; } = new DataTag(reader => reader.ReadInt32());
        public static DataTag Quickness { get; set; } = new DataTag(reader => reader.ReadInt32());
        public Func<BinaryReader, object> Read { get; set; }
        public static DataTag Resilience { get; set; } = new DataTag(reader => reader.ReadInt32());
        public static DataTag Shadow { get; set; } = new DataTag(reader => reader.ReadInt32());
        public static DataTag TargetX { get; set; } = new DataTag(reader => reader.ReadSingle());
        public static DataTag TargetY { get; set; } = new DataTag(reader => reader.ReadSingle());
        public static DataTag Wits { get; set; } = new DataTag(reader => reader.ReadInt32());
    }
}