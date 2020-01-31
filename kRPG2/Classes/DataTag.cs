using System;
using System.IO;

namespace kRPG2.Classes
{
    public class DataTag
    {
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
        public static DataTag Resilience { get; set; } = new DataTag(reader => reader.ReadInt32());
        public static DataTag Shadow { get; set; } = new DataTag(reader => reader.ReadInt32());
        public static DataTag TargetX { get; set; } = new DataTag(reader => reader.ReadSingle());
        public static DataTag TargetY { get; set; } = new DataTag(reader => reader.ReadSingle());
        public static DataTag Wits { get; set; } = new DataTag(reader => reader.ReadInt32());
        public Func<BinaryReader, object> Read { get; set; }

        public DataTag(Func<BinaryReader, object> read)
        {
            Read = read;
        }
    }
}