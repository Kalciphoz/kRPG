using System;
using System.Collections.Generic;
using System.IO;
using kRPG.Enums;
using kRPG.GameObjects.Buffs;
using kRPG.GameObjects.Items.Crowns;
using kRPG.GameObjects.Items.Procedural;
using kRPG.GameObjects.Items.Weapons.Melee;
using kRPG.GameObjects.Items.Weapons.Ranged;
using kRPG.GameObjects.Players;
using kRPG.GameObjects.SFX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace kRPG.GameObjects.Items
{
    public class kItem : GlobalItem
    {
        public kItem()
        {
            UpgradeLevel = 255;
            KPrefix = 0;

            ClearPrefixes();
        }

        public int BonusAccuracy { get; set; }
        public int BonusAllres { get; set; }
        public int BonusCrit { get; set; }

        public int BonusDef { get; set; }
        public int BonusEva { get; set; }
        public float BonusLeech { get; set; }
        public int BonusLife { get; set; }
        public int BonusMana { get; set; }
        public float BonusMult { get; set; }
        public float BonusRegen { get; set; }

        public Dictionary<Element, int> ElementalDamage { get; set; } = new Dictionary<Element, int>
        {
            {Element.Fire, 0}, {Element.Cold, 0}, {Element.Lightning, 0}, {Element.Shadow, 0}
        };

        public static Dictionary<Element, string> ElementNames { get; set; } = new Dictionary<Element, string>
        {
            {Element.Fire, "255063000 fire "},
            {Element.Cold, "063127255 cold "},
            {Element.Lightning, "255239000 lightning "},
            {Element.Shadow, "095000191 shadow "}
        };

        public bool Enhanced => KPrefix != 0;

        public override bool InstancePerEntity => true;

        public byte KPrefix { get; set; }

        private List<string> PrefixTooltips { get; set; } = new List<string>();

        public Dictionary<Element, int> ResBonus { get; set; } = new Dictionary<Element, int>
        {
            {Element.Fire, 0}, {Element.Cold, 0}, {Element.Lightning, 0}, {Element.Shadow, 0}
        };

        public Dictionary<PlayerStats, int> StatBonus { get; set; } =
            new Dictionary<PlayerStats, int> { { PlayerStats.Resilience, 0 }, { PlayerStats.Quickness, 0 }, { PlayerStats.Potency, 0 }, { PlayerStats.Wits, 0 } };

        //public static SwordAccent ReforgeAccent { get; set; }
        //public static SwordBlade Reforgeblade { get; set; }
        //public static int reforgedef { get; set; }
        //public static float reforgedps { get; set; }
        //public static StaffGem reforgegem { get; set; }
        //public static SwordHilt reforgehilt { get; set; }
        //private static byte reforgelevel { get; set; } = 0;
        //public static string reforgename { get; set; }
        //public static StaffOrnament reforgeornament { get; set; }
        //public static Staff reforgestaff { get; set; }

        public static Dictionary<PlayerStats, string> StatNames { get; set; } = new Dictionary<PlayerStats, string>
        {
            {PlayerStats.Resilience, "223000000 resilience"},
            {PlayerStats.Quickness, "000191031 quickness"},
            {PlayerStats.Potency, "027063255 potency"},
            {PlayerStats.Wits, "239223031 wits"}
        };

        public byte UpgradeLevel { get; set; }

        public kItem ApplyStats(Item item, bool clean = false)
        {
            if (item.maxStack > 1) return this;
            bool fav = item.favorited;

            if (!clean)
                try
                {
                    switch (item.modItem)
                    {
                        case ProceduralSword oSword:
                            oSword.ResetStats();
                            break;
                        case ProceduralStaff oStaff:
                            oStaff.ResetStats();
                            break;
                        case RangedWeapon oWeapon:
                            oWeapon.SetStats();
                            break;
                        default:
                            //item.SetItemDefaults(item.type);
                            break;
                    }
                }
                catch (SystemException e)
                {
                    ModLoader.GetMod(Constants.ModName).Logger.InfoFormat(e.ToString());
                }

            if (item.defense > 0 || item.accessory)
            {
                item.ClearNameOverride();
                if (item.accessory)
                    PrefixAccessory(item);
                else
                    PrefixArmour(item);
            }
            else if (item.damage > 0)
            {
                if (UpgradeLevel == 255) return this;
                ApplyUpgradeLevel(item);
                Prefix(item);
                Elementalize(item);
            }

            item.favorited = fav;
            return this;
        }

        public void ApplyUpgradeLevel(Item item)
        {
            double animationDps = 60f * item.damage / item.useAnimation;
            double useTimeDps = 60f * item.damage / item.useTime;
            double dpsModifier;
            switch (UpgradeLevel)
            {
                default:
                    dpsModifier = 0.85;
                    break;
                case 1:
                    dpsModifier = 1;
                    break;
                case 2:
                    dpsModifier = 1.12;
                    break;
                case 3:
                    dpsModifier = 1.25;
                    break;
                case 4:
                    dpsModifier = 1.4;
                    break;
                case 5:
                    dpsModifier = 1.6;
                    break;
                case 6:
                    dpsModifier = 1.8;
                    break;
                case 7:
                    dpsModifier = 2;
                    break;
                case 8:
                    dpsModifier = 2.25;
                    break;
            }

            animationDps *= dpsModifier;
            useTimeDps *= dpsModifier;

            item.damage = (int)Math.Round(animationDps / 60 * item.useAnimation);

            int i = item.useTime - item.useAnimation;

            item.useAnimation = (int)Math.Round(60 / animationDps * item.damage);
            item.useTime = (int)Math.Round(60 / useTimeDps * item.damage);

            if (i >= 0 && item.useTime < item.useAnimation)
                item.useTime = item.useAnimation + i;
            if (i >= 0 && item.useTime < item.useAnimation)
                item.useTime = item.useAnimation;

            item.crit = item.crit + UpgradeLevel - 1;

            if (UpgradeLevel < 3)
                item.scale *= 0.95f;
            else if (UpgradeLevel >= 6 && UpgradeLevel < 8)
                item.scale *= 1.05f;
            else if (UpgradeLevel == 8)
                item.scale *= 1.1f;
        }

        public override bool CanPickup(Item item, Player player)
        {
            return ItemSpace(item, player);
        }

        public override bool CanUseItem(Item item, Player player)
        {
            return item.healMana <= 0 || player.GetModPlayer<PlayerCharacter>().CanHealMana;
        }

        public void ClearPrefixes()
        {
            ElementalDamage = new Dictionary<Element, int> { { Element.Fire, 0 }, { Element.Cold, 0 }, { Element.Lightning, 0 }, { Element.Shadow, 0 } };
            StatBonus = new Dictionary<PlayerStats, int> { { PlayerStats.Resilience, 0 }, { PlayerStats.Quickness, 0 }, { PlayerStats.Potency, 0 }, { PlayerStats.Wits, 0 } };
            ResBonus = new Dictionary<Element, int> { { Element.Fire, 0 }, { Element.Cold, 0 }, { Element.Lightning, 0 }, { Element.Shadow, 0 } };
            BonusEva = 0;
            BonusDef = 0;
            BonusLife = 0;
            BonusMana = 0;
            BonusAccuracy = 0;
            BonusLeech = 0f;
            BonusCrit = 0;
            BonusMult = 0f;
            BonusRegen = 0f;
            BonusAllres = 0;
            PrefixTooltips = new List<string>();
        }

        public override GlobalItem Clone(Item item, Item itemClone)
        {
            kItem copy = (kItem)base.Clone(item, itemClone);
            if (itemClone.type == 0) return copy;
            copy.ElementalDamage = new Dictionary<Element, int>
            {
                {Element.Fire, ElementalDamage[Element.Fire]},
                {Element.Cold, ElementalDamage[Element.Cold]},
                {Element.Lightning, ElementalDamage[Element.Lightning]},
                {Element.Shadow, ElementalDamage[Element.Shadow]}
            };
            copy.StatBonus = new Dictionary<PlayerStats, int>
            {
                {PlayerStats.Resilience, StatBonus[PlayerStats.Resilience]},
                {PlayerStats.Quickness, StatBonus[PlayerStats.Quickness]},
                {PlayerStats.Potency, StatBonus[PlayerStats.Potency]},
                {PlayerStats.Wits, StatBonus[PlayerStats.Wits]}
            };
            copy.ResBonus = new Dictionary<Element, int>
            {
                {Element.Fire, ResBonus[Element.Fire]},
                {Element.Cold, ResBonus[Element.Cold]},
                {Element.Lightning, ResBonus[Element.Lightning]},
                {Element.Shadow, ResBonus[Element.Shadow]}
            };
            copy.PrefixTooltips = new List<string>();
            foreach (string s in PrefixTooltips)
                copy.PrefixTooltips.Add(s);
            item.SetNameOverride(itemClone.Name);
            copy.UpgradeLevel = UpgradeLevel;
            copy.KPrefix = KPrefix;
            copy.BonusDef = BonusDef;
            copy.BonusEva = BonusEva;
            copy.BonusLife = BonusLife;
            copy.BonusMana = BonusMana;
            copy.BonusAccuracy = BonusAccuracy;
            copy.BonusLeech = BonusLeech;
            copy.BonusCrit = BonusCrit;
            copy.BonusMult = BonusMult;
            copy.BonusRegen = BonusRegen;
            copy.BonusAllres = BonusAllres;
            return copy;
        }

        public void Destroy(Item item)
        {
            Main.NewText("Failed to upgrade - item was destroyed", 255, 0, 0);
            item.SetDefaults(0, true);
        }

        public void Downgrade(Item item)
        {
            SetUpgradeLevel(item, (byte)Math.Max(UpgradeLevel - 1, 0));
            Main.NewText("Failed to upgrade - item was downgraded to +" + UpgradeLevel, 255, 0, 0);
        }

        public void Elementalize(Item item)
        {
            if (item.type == mod.GetItem("ProceduralStaff").item.type)
            {
                ProceduralStaff staff = (ProceduralStaff)item.modItem;
                float totalReduction = 0f;
                int totalElements = 0;
                foreach (Element element in Enum.GetValues(typeof(Element)))
                {
                    if (!(staff.EleDamage[element] > 0f))
                        continue;
                    ElementalDamage[element] += Math.Max((int)(item.damage * staff.EleDamage[element]), 1);
                    totalReduction += staff.EleDamage[element];
                    totalElements += 1;
                }

                item.damage -= Math.Max((int)(item.damage * totalReduction), totalElements);
            }
            else if (item.type == mod.GetItem("ProceduralSword").item.type)
            {
                ProceduralSword sword = (ProceduralSword)item.modItem;
                float totalReduction = 0f;
                int totalElements = 0;
                foreach (Element element in Enum.GetValues(typeof(Element)))
                {
                    if (!(sword.EleDamage[element] > 0f))
                        continue;
                    ElementalDamage[element] += Math.Max((int)(item.damage * sword.EleDamage[element]), 1);
                    totalReduction += sword.EleDamage[element];
                    totalElements += 1;
                }

                item.damage -= Math.Max((int)(item.damage * totalReduction), totalElements);
            }

            if (item.type == mod.GetItem("EyeOnAStick").item.type)
                ElementalDamage[Element.Shadow] += 10;

            if (item.Name.Contains("Hellfire") || item.Name.Contains("Molten") || item.Name.Contains("Fiery"))
            {
                ElementalDamage[Element.Fire] += (int)(item.damage * 0.5);
                item.damage = (int)(item.damage * 0.5);
            }

            if (item.Name.Contains("Clockwork") || item.type == ItemID.BreakerBlade || item.Name.Contains("Phase") || item.Name.Contains("Thunder"))
            {
                ElementalDamage[Element.Lightning] += (int)(item.damage * 0.3);
                item.damage = (int)(item.damage * 0.7);
            }

            if (item.damage < 1) item.damage = 1;
        }

        public int GetEleDamage(Item item, Player player, bool ignoreModifiers = false)
        {
            Dictionary<Element, int> elements = GetIndividualElements(item, player, ignoreModifiers);
            return elements[Element.Fire] + elements[Element.Cold] + elements[Element.Lightning] + elements[Element.Shadow];
        }

        public Dictionary<Element, int> GetIndividualElements(Item item, Player player, bool ignoreModifiers = false)
        {
            Dictionary<Element, int> dictionary = new Dictionary<Element, int>();
            if (player.GetModPlayer<PlayerCharacter>().Rituals[Ritual.DemonPact])
            {
                foreach (Element element in Enum.GetValues(typeof(Element)))
                    dictionary[element] = 0;

                dictionary[Element.Shadow] = GetEleDamage(item, player);
            }
            else
            {
                foreach (Element element in Enum.GetValues(typeof(Element)))
                    dictionary[element] = (int)Math.Round(ElementalDamage[element] * (ignoreModifiers
                                                               ? 1
                                                               : player.GetModPlayer<PlayerCharacter>().DamageMultiplier(element, item.melee, item.ranged,
                                                                   item.magic, item.thrown, item.summon)));
            }

            return dictionary;
        }

        //public Mod getMod()
        //{
        //    return mod;
        //}

        /*public override void PostReforge(Item item)
        {
            item.SetItemDefaults(item.type);
            if (item.type == mod.ItemType<ProceduralSword>())
            {
                ProceduralSword sword = (ProceduralSword)item.modItem;
                sword.hilt = reforgehilt;
                sword.blade = reforgeblade;
                sword.accent = reforgeaccent;
                sword.dps = reforgedps;
                sword.enemyDef = reforgedef;
                sword.Initialize();
            }
            else if (item.type == mod.ItemType<ProceduralStaff>())
            {
                ProceduralStaff staff = (ProceduralStaff)item.modItem;
                staff.staff = reforgestaff;
                staff.gem = reforgegem;
                staff.ornament = reforgeornament;
                staff.dps = reforgedps;
                staff.enemyDef = reforgedef;
                staff.Initialize();
            }
            else if (item.modItem is RangedWeapon)
            {
                RangedWeapon weapon = (RangedWeapon)item.modItem;
                weapon.dps = reforgedps;
                weapon.enemyDef = reforgedef;
                weapon.name = reforgename;
                weapon.SetStats();
            }

            if (item.accessory)
                kPrefix = (byte)(Main.rand.Next(27) + 1);
            else if (item.damage > 0)
                kPrefix = (byte)(Main.rand.Next(29) + 1); 
            else if (item.defense > 0)
                kPrefix = (byte)(Main.rand.Next(19) + 1);
            upgradeLevel = reforgelevel;
            ApplyStats(item);
        }*/

        public void Initialize(Item item)
        {
            if (!NeedsSaving(item)) return;
            //if (reset)
            //    if (item.type != ModContent.ItemType<ProceduralSword>() && item.type != ModContent.ItemType<ProceduralStaff>() && !(item.modItem is RangedWeapon))
            //        item.SetItemDefaults(item.type);

            Random rand = new Random();

            if (item.accessory)
            {
                KPrefix = (byte)(rand.Next(27) + 1);
            }

            else if (item.damage > 0)
            {
                bool randomize = item.modItem is ProceduralItem || item.modItem is RangedWeapon || item.value <= 1000 || item.type == 881;
                UpgradeLevel = (byte)(RandomizeUpgradeLevel(item, randomize) + (randomize ? item.modItem is RangedWeapon ? 2 : 0 : 3));
                KPrefix = (byte)(rand.Next(29) + 1);
            }

            else if (item.defense > 0)
            {
                KPrefix = (byte)(rand.Next(19) + 1);
            }

            ApplyStats(item, true);
        }

        public override bool ItemSpace(Item newItem, Player player)
        {
            PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
            try
            {
                if (ItemID.Sets.NebulaPickup[newItem.type] || newItem.type == ItemID.Heart || newItem.type == ItemID.CandyApple ||
                    newItem.type == ItemID.CandyCane || newItem.type == ItemID.Star || newItem.type == ItemID.SoulCake || newItem.type == ItemID.SugarPlum ||
                    newItem.type == ModContent.ItemType<PermanenceCrown>() || newItem.type == ModContent.ItemType<BlacksmithCrown>())
                    return true;
                int num = 50;
                if (newItem.type == 71 || newItem.type == 72 || newItem.type == 73 || newItem.type == 74)
                    num = 54;
                for (int i = 0; i < num; i++)
                {
                    Item item = player.inventory[i];
                    if ((item.type == 0 || item.stack == 0) && (!kConfig.ConfigLocal.ClientSide.ManualInventory || num > 50 || character.ActiveInvPage == 0) ||
                        item.type > 0 && item.stack > 0 && item.stack < item.maxStack && newItem.IsTheSameAs(item))
                        return true;
                }

                for (int i = 0; i < character.Inventories.Length; i++)
                    for (int j = 0; j < character.Inventories[i].Length; j += 1)
                    {
                        Item item = character.Inventories[i][j];
                        if ((item.type == 0 || item.stack == 0) && (!kConfig.ConfigLocal.ClientSide.ManualInventory || i == 0) ||
                            item.type > 0 && item.stack > 0 && item.stack < item.maxStack && newItem.IsTheSameAs(item))
                            //Main.NewText((!kConfig.configLocal.clientSide.manualInventory) + "||" + (i == 0));
                            return true;
                    }
            }
            catch (SystemException e)
            {
                ModLoader.GetMod(Constants.ModName).Logger
                    .InfoFormat(
                        "ItemSpace() failed - TO FIX THE PROBLEM: Delete the kRPG_Settings.json file in Documents/My Games/Terraria/ModLoader. Full error trace: " +
                        e);
            }

            return false;
        }

        public override void Load(Item item, TagCompound tag)
        {
            UpgradeLevel = tag.GetByte("upgrade level");
            KPrefix = tag.GetByte("prefix");
            ApplyStats(item);
        }

        //Seems to work fine now
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (Main.netMode == 2) return;
            if (item.defense > 0 || item.accessory)
            {
                foreach (Element element in Enum.GetValues(typeof(Element)))
                {
                    if (ResBonus[element] == 0)
                        continue;
                    string color = ElementNames[element].Substring(0, 9);
                    string eleName = ElementNames[element].Substring(9);
                    TooltipLine line = new TooltipLine(mod, "Res" + element, ResBonus[element] + eleName + " resistance")
                    {
                        overrideColor = new Color(int.Parse(color.Substring(0, 3)), int.Parse(color.Substring(3, 3)), int.Parse(color.Substring(6, 3)))
                    };
                    tooltips.Insert(1, line);
                }

                foreach (PlayerStats stat in Enum.GetValues(typeof(PlayerStats)))
                    if (StatBonus[stat] != 0)
                    {
                        string color = StatNames[stat].Substring(0, 9);
                        string statName = StatNames[stat].Substring(9);
                        TooltipLine line = new TooltipLine(mod, "Stat" + stat, StatBonus[stat] + statName)
                        {
                            overrideColor = new Color(int.Parse(color.Substring(0, 3)), int.Parse(color.Substring(3, 3)), int.Parse(color.Substring(6, 3)))
                        };
                        tooltips.Insert(1, line);
                    }

                if (BonusEva > 0)
                {
                    TooltipLine line = new TooltipLine(mod, "Evasion", BonusEva + " evasion rating") { overrideColor = new Color(159, 159, 159) };
                    tooltips.Insert(1, line);
                }

                if (BonusMana > 0)
                {
                    TooltipLine line = new TooltipLine(mod, "Mana", BonusMana + " maximum mana") { overrideColor = new Color(0, 63, 255) };
                    tooltips.Insert(1, line);
                }

                if (BonusLife > 0)
                {
                    TooltipLine line = new TooltipLine(mod, "Life", BonusLife + " maximum life") { overrideColor = new Color(255, 31, 31) };
                    tooltips.Insert(1, line);
                }

                for (int i = 0; i < PrefixTooltips.Count; i += 1)
                {
                    TooltipLine line = new TooltipLine(mod, "prefixline" + i, PrefixTooltips[i]) { overrideColor = new Color(127, 191, 127) };
                    tooltips.Add(line);
                }
            }

            else if (item.damage > 0)
            {
                foreach (Element element in Enum.GetValues(typeof(Element)))
                {
                    int eleDamage = GetIndividualElements(item, Main.player[Main.myPlayer])[element];
                    if (eleDamage != 0)
                    {
                        string color = ElementNames[element].Substring(0, 9);
                        string eleName = ElementNames[element].Substring(9);
                        TooltipLine line = new TooltipLine(mod, "Element" + element, eleDamage + eleName + "damage")
                        {
                            overrideColor = new Color(int.Parse(color.Substring(0, 3)), int.Parse(color.Substring(3, 3)), int.Parse(color.Substring(6, 3)))
                        };
                        tooltips.Insert(tooltips.FindIndex(tooltip => tooltip.Name == "Damage"), line);
                    }
                }

                for (int i = 0; i < PrefixTooltips.Count; i += 1)
                {
                    TooltipLine line = new TooltipLine(mod, "prefixline" + i, PrefixTooltips[i]) { overrideColor = new Color(127, 191, 127) };
                    tooltips.Add(line);
                }
            }
        }

        public override bool NeedsSaving(Item item)
        {
            return item.maxStack == 1 && (item.damage > 0 || item.defense > 0 || item.accessory);
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            UpgradeLevel = reader.ReadByte();
            KPrefix = reader.ReadByte();
            ApplyStats(item);
        }

        public override void NetSend(Item item, BinaryWriter writer)
        {
            writer.Write(UpgradeLevel);
            writer.Write(KPrefix);
        }

        public override bool NewPreReforge(Item item)
        {
            //if (item.type == mod.ItemType<ProceduralSword>())
            //{
            //    ProceduralSword sword = (ProceduralSword)item.modItem;
            //    reforgehilt = sword.hilt;
            //    reforgeblade = sword.blade;
            //    reforgeaccent = sword.accent;
            //    reforgedps = sword.dps;
            //    reforgedef = sword.enemyDef;
            //}
            //else if (item.type == mod.ItemType<ProceduralStaff>())
            //{
            //    ProceduralStaff staff = (ProceduralStaff)item.modItem;
            //    reforgestaff = staff.staff;
            //    reforgegem = staff.gem;
            //    reforgeornament = staff.ornament;
            //    //reforgedps = staff.dps;
            //    //reforgedef = staff.enemyDef;
            //}
            //else if (item.modItem is RangedWeapon)
            //{
            //    RangedWeapon weapon = (RangedWeapon)item.modItem;
            //    reforgedps = weapon.dps;
            //    reforgedef = weapon.enemyDef;
            //    reforgename = weapon.name;
            //}
            //reforgelevel = upgradeLevel;

            //if (item.type == mod.ItemType<ProceduralSword>())
            //    ((ProceduralSword)item.modItem).Initialize();
            //else if (item.type == mod.ItemType<ProceduralStaff>())
            //    ((ProceduralStaff)item.modItem).Initialize();
            //else if (item.modItem is RangedWeapon)
            //    ((RangedWeapon)item.modItem).SetStats();
            //else
            //    item.SetItemDefaults(item.type);

            if (item.accessory)
                KPrefix = (byte)(Main.rand.Next(27) + 1);
            else if (item.damage > 0)
                KPrefix = (byte)(Main.rand.Next(29) + 1);
            else if (item.defense > 0)
                KPrefix = (byte)(Main.rand.Next(19) + 1);

            ApplyStats(item);
            return false;
        }

        public override void OnCraft(Item item, Recipe recipe)
        {
            if (NeedsSaving(item))
                Initialize(item);
        }

        public override bool OnPickup(Item item, Player player)
        {
            PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();

            try
            {
                if (player.whoAmI == Main.myPlayer && (player.inventory[player.selectedItem].type != 0 || player.itemAnimation <= 0))
                {
                    if (ItemID.Sets.NebulaPickup[item.type])
                    {
                        SoundManager.PlaySound(Sounds.Grass, player.position);
                        //Main.PlaySound(7, (int) player.position.X, (int) player.position.Y);
                        item = new Item();
                        if (Main.netMode == 1)
                        {
                            NetMessage.SendData((int)PacketTypes.NebulaLevelUp, -1, -1, null, player.whoAmI, item.buffType, player.Center.X, player.Center.Y);
                            NetMessage.SendData((int)PacketTypes.ItemDrop, -1, -1, null, item.whoAmI);
                        }
                        else
                        {
                            player.NebulaLevelup(item.buffType);
                        }
                    }

                    switch (item.type)
                    {
                        case ItemID.Heart:
                        case ItemID.CandyApple:
                        case ItemID.CandyCane:
                            {
                                //Main.PlaySound(7, (int) player.position.X, (int) player.position.Y);
                                SoundManager.PlaySound(Sounds.Grass, player.position);
                                int healAmount = 10 + player.GetModPlayer<PlayerCharacter>().Level / 2;
                                player.statLife += healAmount;
                                if (Main.myPlayer == player.whoAmI)
                                    player.HealEffect(healAmount);
                                if (player.statLife > player.statLifeMax2)
                                    player.statLife = player.statLifeMax2;
                                item = new Item();
                                if (Main.netMode == 1)
                                    NetMessage.SendData((int)PacketTypes.ItemDrop, -1, -1, null, item.whoAmI);

                                break;
                            }
                        case ItemID.Star:
                        case ItemID.SoulCake:
                        case ItemID.SugarPlum:
                            {
                                //Main.PlaySound(7, (int) player.position.X, (int) player.position.Y);
                                SoundManager.PlaySound(Sounds.Grass, player.position);
                                int healAmount = 5 + character.TotalStats(PlayerStats.Wits);
                                character.Mana += healAmount;
                                player.statMana += healAmount;
                                if (Main.myPlayer == player.whoAmI)
                                    player.ManaEffect(healAmount);
                                item = new Item();
                                if (Main.netMode == 1)
                                    NetMessage.SendData((int)PacketTypes.ItemDrop, -1, -1, null, item.whoAmI);

                                break;
                            }
                        default:
                            {
                                if (item.type == ModContent.ItemType<PermanenceCrown>())
                                {
                                    character.Permanence += 1;
                                    ItemText.NewText(item, item.stack);
                                    //Main.PlaySound(7, player.position);
                                    SoundManager.PlaySound(Sounds.Grass, player.position);
                                    return false;
                                }

                                if (item.type == ModContent.ItemType<BlacksmithCrown>())
                                {
                                    character.Transcendence += 1;
                                    ItemText.NewText(item, item.stack);
                                    //Main.PlaySound(7, player.position);
                                    SoundManager.PlaySound(Sounds.Grass, player.position);
                                    return false;
                                }

                                item = player.GetItem(item);
                                if (Main.netMode == 1)
                                    NetMessage.SendData((int)PacketTypes.ItemDrop, -1, -1, null, item.whoAmI);

                                break;
                            }
                    }
                }
            }
            catch (SystemException e)
            {
                ModLoader.GetMod(Constants.ModName).Logger
                    .InfoFormat(
                        "OnPickup() failed - TO FIX THE PROBLEM: Delete the kRPG_Settings.json file in Documents/My Games/Terraria/ModLoader. Full error trace: " +
                        e);
            }

            return false;
        }

        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor,
            Vector2 origin, float scale)
        {
            Player player = Main.LocalPlayer;
            bool flag = false;
            if (player.chest >= 0)
                if (Main.chest[player.chest].item.Contains(item))
                    flag = true;
            if (NeedsSaving(item) && !Enhanced && !WorldGen.gen && !Main.gameMenu &&
                (player.inventory.Contains(item) || flag || player.bank.item.Contains(item) || player.bank2.item.Contains(item) ||
                 player.bank3.item.Contains(item)))
                Initialize(item);
            PlayerCharacter character = Main.player[Main.myPlayer].GetModPlayer<PlayerCharacter>();

            if (Upgradeable(item) && Main.mouseRight && Main.mouseRightRelease &&
                new Rectangle((int)position.X, (int)position.Y, 38, 38).Contains(Main.mouseX, Main.mouseY))
                character.AnvilGui.AttemptSelectItem(this, item);
        }

        

        public void Prefix(Item item)
        {
            foreach (Element element in Enum.GetValues(typeof(Element)))
                ElementalDamage[element] = 0;

            PrefixTooltips.Clear();
            float speed = 1f;

            switch (KPrefix - 1)
            {
                case 0:
                    item.SetNameOverride("Gleaming " + item.Name);
                    item.damage += (int)(item.damage * 0.15);
                    item.rare += 1;
                    PrefixTooltips.Add("+15% damage");
                    break;
                case 1:
                    item.SetNameOverride("Sulfuric " + item.Name);
                    ElementalDamage[Element.Fire] = Math.Max((int)(item.damage * 0.25), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 1);
                    PrefixTooltips.Add("+5% damage");
                    break;
                case 2:
                    item.SetNameOverride("Windswept " + item.Name);
                    ElementalDamage[Element.Cold] = Math.Max((int)(item.damage * 0.25), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 1);
                    PrefixTooltips.Add("+5% damage");
                    break;
                case 3:
                    item.SetNameOverride("Buzzing " + item.Name);
                    ElementalDamage[Element.Lightning] = Math.Max((int)(item.damage * 0.25), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 1);
                    PrefixTooltips.Add("+5% damage");
                    break;
                case 4:
                    item.SetNameOverride("Demonic " + item.Name);
                    ElementalDamage[Element.Shadow] = Math.Max((int)(item.damage * 0.2), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 1);
                    break;
                case 5:
                    item.SetNameOverride("Rending " + item.Name);
                    ElementalDamage[Element.Fire] = Math.Max((int)(item.damage * 0.15), 1);
                    ElementalDamage[Element.Cold] = Math.Max((int)(item.damage * 0.15), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 1);
                    PrefixTooltips.Add("+10% damage");
                    break;
                case 6:
                    item.SetNameOverride("Blazing " + item.Name);
                    ElementalDamage[Element.Fire] = Math.Max((int)(item.damage * 0.15), 1);
                    ElementalDamage[Element.Lightning] = Math.Max((int)(item.damage * 0.15), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 2);
                    PrefixTooltips.Add("+10% damage");
                    break;
                case 7:
                    item.SetNameOverride("Dazzling " + item.Name);
                    ElementalDamage[Element.Cold] = Math.Max((int)(item.damage * 0.15), 1);
                    ElementalDamage[Element.Lightning] = Math.Max((int)(item.damage * 0.15), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 2);
                    PrefixTooltips.Add("+10% damage");
                    break;
                case 8:
                    item.SetNameOverride("Prismatic " + item.Name);
                    ElementalDamage[Element.Fire] = Math.Max((int)(item.damage * 0.11), 1);
                    ElementalDamage[Element.Cold] = Math.Max((int)(item.damage * 0.11), 1);
                    ElementalDamage[Element.Lightning] = Math.Max((int)(item.damage * 0.11), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 2);
                    item.rare += 1;
                    PrefixTooltips.Add("+13% damage");
                    break;
                case 9:
                    item.SetNameOverride("Infernal " + item.Name);
                    ElementalDamage[Element.Fire] = Math.Max((int)(item.damage * 0.15), 1);
                    ElementalDamage[Element.Shadow] = Math.Max((int)(item.damage * 0.1), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 2);
                    PrefixTooltips.Add("+5% damage");
                    break;
                case 10:
                    item.SetNameOverride("Glacial " + item.Name);
                    ElementalDamage[Element.Cold] = Math.Max((int)(item.damage * 0.15), 1);
                    ElementalDamage[Element.Shadow] = Math.Max((int)(item.damage * 0.1), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 2);
                    PrefixTooltips.Add("+5% damage");
                    break;
                case 11:
                    item.SetNameOverride("Ominous " + item.Name);
                    ElementalDamage[Element.Lightning] = Math.Max((int)(item.damage * 0.15), 1);
                    ElementalDamage[Element.Shadow] = Math.Max((int)(item.damage * 0.1), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 2);
                    PrefixTooltips.Add("+5% damage");
                    break;
                case 12:
                    item.SetNameOverride("Volcanic " + item.Name);
                    ElementalDamage[Element.Fire] = Math.Max((int)(item.damage * 0.15), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.15), 1);
                    item.crit += 10;
                    PrefixTooltips.Add("+10% critical strike chance");
                    break;
                case 13:
                    item.SetNameOverride("Cryonic " + item.Name);
                    ElementalDamage[Element.Cold] = Math.Max((int)(item.damage * 0.15), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.15), 1);
                    item.crit += 10;
                    PrefixTooltips.Add("+10% critical strike chance");
                    break;
                case 14:
                    item.SetNameOverride("Stilling " + item.Name);
                    ElementalDamage[Element.Lightning] = Math.Max((int)(item.damage * 0.15), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.15), 1);
                    item.crit += 10;
                    PrefixTooltips.Add("+10% critical strike chance");
                    break;
                case 15:
                    item.SetNameOverride("Ruthless " + item.Name);
                    item.damage += (int)(item.damage * 0.1);
                    item.crit += 5;
                    PrefixTooltips.Add("+10% damage");
                    PrefixTooltips.Add("+5% critical strike chance");
                    break;
                case 16:
                    item.SetNameOverride("Ancient " + item.Name);
                    ElementalDamage[Element.Fire] = Math.Max((int)(item.damage * 0.11), 1);
                    ElementalDamage[Element.Cold] = Math.Max((int)(item.damage * 0.11), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.16), 2);
                    item.crit += 5;
                    PrefixTooltips.Add("+6% damage");
                    PrefixTooltips.Add("+5% critical strike chance");
                    break;
                case 17:
                    item.SetNameOverride("Malevolent " + item.Name);
                    ElementalDamage[Element.Fire] = Math.Max((int)(item.damage * 0.11), 1);
                    ElementalDamage[Element.Lightning] = Math.Max((int)(item.damage * 0.11), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.16), 2);
                    item.crit += 5;
                    PrefixTooltips.Add("+6% damage");
                    PrefixTooltips.Add("+5% critical strike chance");
                    break;
                case 18:
                    item.SetNameOverride("Incandescent " + item.Name);
                    ElementalDamage[Element.Cold] = Math.Max((int)(item.damage * 0.11), 1);
                    ElementalDamage[Element.Lightning] = Math.Max((int)(item.damage * 0.11), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.16), 2);
                    item.crit += 5;
                    PrefixTooltips.Add("+6% damage");
                    PrefixTooltips.Add("+5% critical strike chance");
                    break;
                case 19:
                    item.SetNameOverride("Searing " + item.Name);
                    ElementalDamage[Element.Fire] = Math.Max((int)(item.damage * 0.2), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 1);
                    speed = 1.1f;
                    PrefixTooltips.Add("+10% speed");
                    break;
                case 20:
                    item.SetNameOverride("Arctic " + item.Name);
                    ElementalDamage[Element.Cold] = Math.Max((int)(item.damage * 0.2), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 1);
                    speed = 1.1f;
                    PrefixTooltips.Add("+10% speed");
                    break;
                case 21:
                    item.SetNameOverride("Thunderous " + item.Name);
                    ElementalDamage[Element.Lightning] = Math.Max((int)(item.damage * 0.2), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 1);
                    speed = 1.1f;
                    PrefixTooltips.Add("+10% speed");
                    break;
                case 22:
                    item.SetNameOverride("Sacrificial " + item.Name);
                    ElementalDamage[Element.Shadow] = Math.Max((int)(item.damage * 0.2), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 1);
                    speed = 1.1f;
                    PrefixTooltips.Add("+10% speed");
                    break;
                case 23:
                    item.SetNameOverride("Hateful " + item.Name);
                    ElementalDamage[Element.Fire] = Math.Max((int)(item.damage * 0.2), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 1);
                    speed = 1.1f;
                    PrefixTooltips.Add("+10% speed");
                    item.crit += 5;
                    PrefixTooltips.Add("+5% critical strike chance");
                    break;
                case 24:
                    item.SetNameOverride("Forgotten " + item.Name);
                    ElementalDamage[Element.Cold] = Math.Max((int)(item.damage * 0.2), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 1);
                    speed = 1.1f;
                    PrefixTooltips.Add("+10% speed");
                    item.crit += 5;
                    PrefixTooltips.Add("+5% critical strike chance");
                    break;
                case 25:
                    item.SetNameOverride("Unreal " + item.Name);
                    ElementalDamage[Element.Lightning] = Math.Max((int)(item.damage * 0.2), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 1);
                    speed = 1.1f;
                    PrefixTooltips.Add("+10% speed");
                    item.crit += 5;
                    PrefixTooltips.Add("+5% critical strike chance");
                    break;
                case 26:
                    item.SetNameOverride("Zealous " + item.Name);
                    item.crit += 15;
                    PrefixTooltips.Add("+15% critical strike chance");
                    break;
                case 27:
                    item.SetNameOverride("Legendary " + item.Name);
                    item.damage += (int)(item.damage * 0.1);
                    PrefixTooltips.Add("+10% damage");
                    speed = 1.1f;
                    PrefixTooltips.Add("+10% speed");
                    item.crit += 5;
                    PrefixTooltips.Add("+5% critical strike chance");
                    item.rare += 1;
                    break;
                case 28:
                    item.SetNameOverride("Mythical " + item.Name);
                    speed = 1.1f;
                    PrefixTooltips.Add("+10% speed");
                    item.crit += 10;
                    PrefixTooltips.Add("+10% critical strike chance");
                    item.rare += 1;
                    break;
                case 29:
                    item.SetNameOverride("Transcendent " + item.Name);
                    ElementalDamage[Element.Fire] = Math.Max((int)(item.damage * 0.1), 1);
                    ElementalDamage[Element.Cold] = Math.Max((int)(item.damage * 0.1), 1);
                    ElementalDamage[Element.Lightning] = Math.Max((int)(item.damage * 0.1), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.18), 2);
                    PrefixTooltips.Add("+12% damage");
                    speed = 1.1f;
                    PrefixTooltips.Add("+10% speed");
                    item.crit += 5;
                    PrefixTooltips.Add("+5% critical strike chance");
                    item.rare += 1;
                    break;
            }

            int i = item.useTime - item.useAnimation;

            item.useAnimation = (int)Math.Round(item.useAnimation / speed);
            item.useTime = (int)Math.Round(item.useTime / speed);

            if (i >= 0 && item.useTime < item.useAnimation)
                item.useTime = item.useAnimation + (i > 1 ? (int)(i / speed) : i);
            if (i >= 0 && item.useTime < item.useAnimation)
                item.useTime = item.useAnimation;

            item.SetNameOverride(item.Name + "+" + UpgradeLevel);

            if (item.damage < 1) item.damage = 1;
        }

        public void PrefixAccessory(Item item)
        {
            ClearPrefixes();

            switch (KPrefix - 1)
            {
                case 0:
                    item.SetNameOverride("Sturdy " + item.Name);
                    StatBonus[PlayerStats.Resilience] = 2;
                    break;
                case 1:
                    item.SetNameOverride("Elusive " + item.Name);
                    StatBonus[PlayerStats.Quickness] = 2;
                    break;
                case 2:
                    item.SetNameOverride("Volatile " + item.Name);
                    StatBonus[PlayerStats.Potency] = 2;
                    break;
                case 3:
                    item.SetNameOverride("Cunning " + item.Name);
                    StatBonus[PlayerStats.Wits] = 2;
                    break;
                case 4:
                    item.SetNameOverride("Sage " + item.Name);
                    StatBonus[PlayerStats.Wits] = 3;
                    item.rare += 1;
                    break;
                case 5:
                    item.SetNameOverride("Armored " + item.Name);
                    BonusDef = 4;
                    PrefixTooltips.Add("+4 defense");
                    break;
                case 6:
                    item.SetNameOverride("Elegant " + item.Name);
                    BonusEva = 5;
                    break;
                case 7:
                    item.SetNameOverride("Vampiric " + item.Name);
                    BonusLeech = 0.02f;
                    PrefixTooltips.Add("+2% life Leech");
                    break;
                case 8:
                    item.SetNameOverride("Focused " + item.Name);
                    BonusAccuracy = 3;
                    PrefixTooltips.Add("+3 accuracy");
                    break;
                case 9:
                    item.SetNameOverride("Precise " + item.Name);
                    BonusAccuracy = 4;
                    PrefixTooltips.Add("+4 accuracy");
                    break;
                case 10:
                    item.SetNameOverride("Single-minded " + item.Name);
                    BonusAccuracy = 6;
                    item.rare += 1;
                    PrefixTooltips.Add("+6 accuracy");
                    break;
                case 11:
                    item.SetNameOverride("Organic " + item.Name);
                    BonusRegen = 1.4f;
                    PrefixTooltips.Add("+1.4 life regen");
                    break;
                case 12:
                    item.SetNameOverride("Arcane " + item.Name);
                    BonusMana = 20;
                    break;
                case 13:
                    item.SetNameOverride("Frenetic " + item.Name);
                    BonusCrit = 4;
                    PrefixTooltips.Add("+4 crit chance");
                    break;
                case 14:
                    item.SetNameOverride("Brutal " + item.Name);
                    BonusMult = 0.20f;
                    PrefixTooltips.Add("+40 crit multiplier");
                    break;
                case 15:
                    item.SetNameOverride("Warding " + item.Name);
                    BonusAllres = 3;
                    PrefixTooltips.Add("+4 to all resistances");
                    break;
                case 16:
                    item.SetNameOverride("Flame Retardant " + item.Name);
                    ResBonus[Element.Fire] = 10;
                    break;
                case 17:
                    item.SetNameOverride("Waterproof " + item.Name);
                    ResBonus[Element.Cold] = 10;
                    break;
                case 18:
                    item.SetNameOverride("Insulated " + item.Name);
                    ResBonus[Element.Lightning] = 10;
                    break;
                case 19:
                    item.SetNameOverride("Hexproof " + item.Name);
                    ResBonus[Element.Shadow] = 7;
                    break;
                case 20:
                    item.SetNameOverride("Hardened " + item.Name);
                    BonusDef = 3;
                    PrefixTooltips.Add("+3 defense");
                    break;
                case 21:
                    item.SetNameOverride("Protective " + item.Name);
                    BonusAllres = 2;
                    PrefixTooltips.Add("+3 to all resistances");
                    break;
                case 22:
                    item.SetNameOverride("Lava-Infused " + item.Name);
                    ResBonus[Element.Fire] = 15;
                    item.rare += 1;
                    break;
                case 23:
                    item.SetNameOverride("Snowforged " + item.Name);
                    ResBonus[Element.Cold] = 15;
                    item.rare += 1;
                    break;
                case 24:
                    item.SetNameOverride("Lightning-Coiled " + item.Name);
                    ResBonus[Element.Lightning] = 15;
                    item.rare += 1;
                    break;
                case 25:
                    item.SetNameOverride("Blackheart " + item.Name);
                    ResBonus[Element.Shadow] = 12;
                    item.rare += 1;
                    break;
                case 26:
                    item.SetNameOverride("Enchanted " + item.Name);
                    BonusAllres = 4;
                    PrefixTooltips.Add("+5 to all resistances");
                    item.rare += 1;
                    break;
            }
        }

        public void PrefixArmour(Item item)
        {
            ClearPrefixes();

            switch (KPrefix - 1)
            {
                case 0:
                    item.SetNameOverride("Carnelian " + item.Name);
                    StatBonus[PlayerStats.Resilience] = 1 + item.rare / 2;
                    break;
                case 1:
                    item.SetNameOverride("Viridian " + item.Name);
                    StatBonus[PlayerStats.Quickness] = 1 + item.rare / 2;
                    break;
                case 2:
                    item.SetNameOverride("Cerulean " + item.Name);
                    StatBonus[PlayerStats.Potency] = 1 + item.rare / 2;
                    break;
                case 3:
                    item.SetNameOverride("Arylide " + item.Name);
                    StatBonus[PlayerStats.Wits] = 1 + item.rare / 3;
                    break;
                case 4:
                    item.SetNameOverride("Glaucous " + item.Name);
                    BonusDef = 2 + item.rare;
                    PrefixTooltips.Add("+" + BonusDef + " defence");
                    break;
                case 5:
                    item.SetNameOverride("Cinereous " + item.Name);
                    BonusDef = 1 + item.rare / 2;
                    PrefixTooltips.Add("+" + BonusDef + " defence");
                    BonusLife = 10 + item.rare * 5;
                    break;
                case 6:
                    item.SetNameOverride("Sanguine " + item.Name);
                    BonusLife = 15 + item.rare * 10;
                    break;
                case 7:
                    item.SetNameOverride("Azure " + item.Name);
                    BonusMana = 10 + item.rare * 3;
                    break;
                case 8:
                    item.SetNameOverride("Cerise " + item.Name);
                    BonusMana = 6 + item.rare * 2;
                    BonusLife = 10 + item.rare * 5;
                    break;
                case 9:
                    item.SetNameOverride("Amaranth " + item.Name);
                    BonusLife = 10 + item.rare * 4;
                    StatBonus[PlayerStats.Resilience] = 1 + item.rare / 3;
                    break;
                case 10:
                    item.SetNameOverride("Icterine " + item.Name);
                    BonusLife = 10 + item.rare * 3;
                    StatBonus[PlayerStats.Quickness] = 1 + item.rare / 3;
                    break;
                case 11:
                    item.SetNameOverride("Byzantine " + item.Name);
                    BonusLife = 8 + item.rare * 3;
                    StatBonus[PlayerStats.Potency] = 1 + item.rare / 3;
                    break;
                case 12:
                    item.SetNameOverride("Silver " + item.Name);
                    BonusEva = 2 + item.rare;
                    break;
                case 13:
                    item.SetNameOverride("Ebony " + item.Name);
                    BonusEva = 1 + item.rare / 2;
                    BonusLife = 9 + item.rare * 5;
                    break;
                case 14:
                    item.SetNameOverride("Onyx " + item.Name);
                    BonusEva = 1 + item.rare / 2;
                    BonusDef = 1 + item.rare / 2;
                    PrefixTooltips.Add("+" + BonusDef + " defence");
                    break;
                case 15:
                    item.SetNameOverride("Scarlet " + item.Name);
                    ResBonus[Element.Fire] = 6 + item.rare * 2;
                    break;
                case 16:
                    item.SetNameOverride("Celeste " + item.Name);
                    ResBonus[Element.Cold] = 6 + item.rare * 2;
                    break;
                case 17:
                    item.SetNameOverride("Golden " + item.Name);
                    ResBonus[Element.Lightning] = 6 + item.rare * 2;
                    break;
                case 18:
                    item.SetNameOverride("Prismatic " + item.Name);
                    BonusAllres = 3 + item.rare / 2;
                    PrefixTooltips.Add("+" + BonusAllres + " to all resistances");
                    break;
            }
        }

        public byte RandomizeUpgradeLevel(Item item, bool bonus)
        {
            if (Main.rand.Next(2) != 1)
                return 0;
            if (Main.rand.Next(3) != 1)
                return 1;
            if (Main.rand.Next(4) == 1 && bonus) return 3;

            return 2;
        }

        public override TagCompound Save(Item item)
        {
            return new TagCompound { { "upgrade level", UpgradeLevel }, { "prefix", KPrefix } };
        }

        public override void SetDefaults(Item item)
        {
            int h = item.healLife;
            if (h > 0)
                item.healLife = 10 * (int)Math.Round((73.56 - 0.4 * h + 0.018 * h * h) / 10.0);
            h = item.healMana;
            if (h > 0)
                item.healMana /= 2;
        }

        public void SetUpgradeLevel(Item item, byte level)
        {
            UpgradeLevel = level;
            ApplyStats(item);
        }

        public override void UpdateEquip(Item item, Player player)
        {
            PlayerCharacter modPlayer = player.GetModPlayer<PlayerCharacter>();

            if (!Enhanced) return;
            if (BonusDef > 0)
                player.statDefense += BonusDef;
            if (BonusLife > 0)
                modPlayer.BonusLife += BonusLife;
            if (BonusMana > 0)
                modPlayer.BonusMana += BonusMana;
            if (BonusAccuracy > 0)
                modPlayer.Accuracy += BonusAccuracy;
            if (BonusLeech > 0f)
                modPlayer.LifeLeech += BonusLeech;
            if (BonusCrit > 0)
                modPlayer.CritBoost += BonusCrit;
            if (BonusMult > 0f)
                modPlayer.CritMultiplier += BonusMult;
            if (BonusRegen > 0f)
                modPlayer.LifeRegen += BonusRegen;
            foreach (PlayerStats stat in Enum.GetValues(typeof(PlayerStats)))
                if (StatBonus[stat] > 0)
                    modPlayer.TempStats[stat] += StatBonus[stat];
            foreach (Element element in Enum.GetValues(typeof(Element)))
                modPlayer.Eleres[element] += ResBonus[element];
            modPlayer.Allres += BonusAllres;
            if (BonusEva > 0)
                modPlayer.Evasion += BonusEva;
        }

        public void Upgrade(Item item)
        {
            SetUpgradeLevel(item, (byte)(UpgradeLevel + 1));
            Main.NewText("Successfully upgraded item to +" + UpgradeLevel, 0, 255, 0);
        }

        public bool Upgradeable(Item item)
        {
            return item.damage > 0 && item.ammo == 0;
        }

        public override bool UseItem(Item item, Player player)
        {
            if (item.healMana <= 0)
                return false;
            PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
            character.Mana = Math.Min(player.statManaMax2, character.Mana + item.healMana);
            player.statMana = character.Mana;
            player.AddBuff(ModContent.BuffType<ManaCooldown>(), 600);
            return true;
        }
    }
}