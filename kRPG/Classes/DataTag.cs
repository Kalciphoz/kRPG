using System;
using System.IO;

namespace kRPG.Classes
{
    public class DataTag
    {
        public static DataTag amount = new DataTag(reader => reader.ReadInt32());
        public static DataTag amount_single = new DataTag(reader => reader.ReadSingle());
        public static DataTag cold = new DataTag(reader => reader.ReadInt32());
        public static DataTag damage = new DataTag(reader => reader.ReadInt32());
        public static DataTag entityId = new DataTag(reader => reader.ReadInt32());
        public static DataTag fire = new DataTag(reader => reader.ReadInt32());
        public static DataTag flag = new DataTag(reader => reader.ReadBoolean());
        public static DataTag flag2 = new DataTag(reader => reader.ReadBoolean());
        public static DataTag flag3 = new DataTag(reader => reader.ReadBoolean());
        public static DataTag flag4 = new DataTag(reader => reader.ReadBoolean());
        public static DataTag glyph_cross = new DataTag(reader => reader.ReadInt32());
        public static DataTag glyph_moon = new DataTag(reader => reader.ReadInt32());
        public static DataTag glyph_star = new DataTag(reader => reader.ReadInt32());
        public static DataTag itemDef = new DataTag(reader => reader.ReadInt32());
        public static DataTag itemDps = new DataTag(reader => reader.ReadSingle());
        public static DataTag itemId = new DataTag(reader => reader.ReadInt32());
        public static DataTag lightning = new DataTag(reader => reader.ReadInt32());
        public static DataTag modifierCount = new DataTag(reader => reader.ReadInt32());
        public static DataTag npcId = new DataTag(reader => reader.ReadInt32());
        public static DataTag partPrimary = new DataTag(reader => reader.ReadInt32());
        public static DataTag partSecondary = new DataTag(reader => reader.ReadInt32());
        public static DataTag partTertiary = new DataTag(reader => reader.ReadInt32());
        public static DataTag playerId = new DataTag(reader => reader.ReadInt32());
        public static DataTag potency = new DataTag(reader => reader.ReadInt32());
        public static DataTag projCount = new DataTag(reader => reader.ReadInt32());
        public static DataTag projId = new DataTag(reader => reader.ReadInt32());
        public static DataTag quickness = new DataTag(reader => reader.ReadInt32());
        public static DataTag resilience = new DataTag(reader => reader.ReadInt32());
        public static DataTag shadow = new DataTag(reader => reader.ReadInt32());
        public static DataTag targetX = new DataTag(reader => reader.ReadSingle());
        public static DataTag targetY = new DataTag(reader => reader.ReadSingle());
        public static DataTag wits = new DataTag(reader => reader.ReadInt32());
        public Func<BinaryReader, object> read;

        public DataTag(Func<BinaryReader, object> read)
        {
            this.read = read;
        }
    }
}