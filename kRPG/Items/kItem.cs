using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System;
using System.Collections.Generic;
using kRPG.Items.Weapons;
using Terraria.ID;
using Terraria.Utilities;
using kRPG.Buffs;
using kRPG.Items.Weapons.RangedDrops;

namespace kRPG.Items
{
    public class kItem : GlobalItem
    {
        static byte reforgelevel = 0;
        public static SwordHilt reforgehilt;
        public static SwordBlade reforgeblade;
        public static SwordAccent reforgeaccent;
        public static Staff reforgestaff;
        public static StaffGem reforgegem;
        public static StaffOrnament reforgeornament;
        public static float reforgedps;
        public static int reforgedef;
        public static string reforgename;
        public static Dictionary<ELEMENT, string> elementNames = new Dictionary<ELEMENT, string>()
        {
            {ELEMENT.FIRE, "255063000 fire "},
            {ELEMENT.COLD, "063127255 cold "},
            {ELEMENT.LIGHTNING, "255239000 lightning "},
            {ELEMENT.SHADOW, "095000191 shadow "}
        };

        public static Dictionary<STAT, string> statNames = new Dictionary<STAT, string>()
        {
            {STAT.RESILIENCE, "223000000 resilience"},
            {STAT.QUICKNESS, "000191031 quickness"},
            {STAT.POTENCY, "027063255 potency"},
            {STAT.WITS, "239223031 wits"}
        };

        public override bool InstancePerEntity { get { return true; } }

        public byte upgradeLevel = 255;
        public byte kPrefix = 0;
        public bool enhanced
        {
            get
            {
                return kPrefix != 0;
            }
        }

        public Dictionary<ELEMENT, int> elementalDamage = new Dictionary<ELEMENT, int>()
        {
            {ELEMENT.FIRE, 0},
            {ELEMENT.COLD, 0},
            {ELEMENT.LIGHTNING, 0},
            {ELEMENT.SHADOW, 0}
        };
        public Dictionary<STAT, int> statBonus = new Dictionary<STAT, int>()
        {
            {STAT.RESILIENCE, 0},
            {STAT.QUICKNESS, 0},
            {STAT.POTENCY, 0},
            {STAT.WITS, 0}
        };
        public Dictionary<ELEMENT, int> resBonus = new Dictionary<ELEMENT, int>()
        {
            {ELEMENT.FIRE, 0},
            {ELEMENT.COLD, 0},
            {ELEMENT.LIGHTNING, 0},
            {ELEMENT.SHADOW, 0}
        };
        public int bonusDef;
        public int bonusEva;
        public int bonusAccuracy;
        public int bonusLife;
        public int bonusMana;
        public float bonusLeech;
        public int bonusCrit;
        public float bonusMult;
        public float bonusRegen;
        public int bonusAllres;

        private List<string> prefixTooltips = new List<string>();

        public kItem()
        {
            upgradeLevel = 255;
            kPrefix = 0;
            
            ClearPrefixes();
        }

        public override void OnCraft(Item item, Recipe recipe)
        {
            if (NeedsSaving(item))
            {
                Initialize(item);
            }
        }

        public override GlobalItem Clone(Item item, Item itemClone)
        {
            kItem copy = (kItem)base.Clone(item, itemClone);
            if (itemClone.type == 0) return copy;
            copy.elementalDamage = new Dictionary<ELEMENT, int>()
            {
                {ELEMENT.FIRE, elementalDamage[ELEMENT.FIRE]},
                {ELEMENT.COLD, elementalDamage[ELEMENT.COLD]},
                {ELEMENT.LIGHTNING, elementalDamage[ELEMENT.LIGHTNING]},
                {ELEMENT.SHADOW, elementalDamage[ELEMENT.SHADOW]},
            };
            copy.statBonus = new Dictionary<STAT, int>()
            {
                {STAT.RESILIENCE, statBonus[STAT.RESILIENCE]},
                {STAT.QUICKNESS, statBonus[STAT.QUICKNESS]},
                {STAT.POTENCY, statBonus[STAT.POTENCY]},
                {STAT.WITS, statBonus[STAT.WITS]},
            };
            copy.resBonus = new Dictionary<ELEMENT, int>()
            {
                {ELEMENT.FIRE, resBonus[ELEMENT.FIRE]},
                {ELEMENT.COLD, resBonus[ELEMENT.COLD]},
                {ELEMENT.LIGHTNING, resBonus[ELEMENT.LIGHTNING]},
                {ELEMENT.SHADOW, resBonus[ELEMENT.SHADOW]},
            };
            copy.prefixTooltips = new List<string>();
            foreach (string s in prefixTooltips)
                copy.prefixTooltips.Add(s);
            item.SetNameOverride(itemClone.Name);
            copy.upgradeLevel = upgradeLevel;
            copy.kPrefix = kPrefix;
            copy.bonusDef = bonusDef;
            copy.bonusEva = bonusEva;
            copy.bonusLife = bonusLife;
            copy.bonusMana = bonusMana;
            copy.bonusAccuracy = bonusAccuracy;
            copy.bonusLeech = bonusLeech;
            copy.bonusCrit = bonusCrit;
            copy.bonusMult = bonusMult;
            copy.bonusRegen = bonusRegen;
            copy.bonusAllres = bonusAllres;
            return copy;
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
                kPrefix = (byte)(Main.rand.Next(27) + 1);
            else if (item.damage > 0)
                kPrefix = (byte)(Main.rand.Next(29) + 1);
            else if (item.defense > 0)
                kPrefix = (byte)(Main.rand.Next(19) + 1);

            ApplyStats(item);
            return false;
        }

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

        public void Initialize(Item item, bool reset = true)
        {
            if (!NeedsSaving(item)) return;
            if (reset)
                if (item.type != mod.ItemType<ProceduralSword>() && item.type != mod.ItemType<ProceduralStaff>() && !(item.modItem is RangedWeapon))
                    item.SetItemDefaults(item.type);
            
            Random rand = new Random();

            if (item.accessory)
            {
                kPrefix = (byte)(rand.Next(27) + 1);
            }

            else if (item.damage > 0)
            {
                bool randomize = item.modItem is ProceduralItem || item.modItem is RangedWeapon || item.value <= 1000 || item.type == 881;
                upgradeLevel = (byte)(RandomizeUpgradeLevel(item, randomize) + (randomize ? (item.modItem is RangedWeapon ? 2 : 0) : 3));
                kPrefix = (byte)(rand.Next(29) + 1);
            }
            
            else if (item.defense > 0)
            {
                kPrefix = (byte)(rand.Next(19) + 1);
            }

            ApplyStats(item, true);
        }

        public kItem ApplyStats(Item item, bool clean = false)
        {
            if (item.maxStack > 1) return this;
            bool fav = item.favorited;

            if (!clean)
            {
                if (item.modItem is ProceduralSword)
                    ((ProceduralSword)item.modItem).ResetStats();
                else if (item.modItem is ProceduralStaff)
                    ((ProceduralStaff)item.modItem).ResetStats();
                else if (item.modItem is RangedWeapon)
                    ((RangedWeapon)item.modItem).SetStats();
                else
                    item.SetItemDefaults(item.type);
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
                if (upgradeLevel == 255) return this;
                ApplyUpgradeLevel(item);
                Prefix(item);
                Elementalize(item);
            }

            item.favorited = fav;
            return this;
        }

        public void Elementalize(Item item)
        {
            if (item.type == mod.GetItem("ProceduralStaff").item.type)
            {
                ProceduralStaff staff = (ProceduralStaff)item.modItem;
                float totalReduction = 0f;
                int totalElements = 0;
                foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                {
                    if (staff.eleDamage[element] > 0f)
                    {
                        elementalDamage[element] += Math.Max((int)(item.damage * staff.eleDamage[element]), 1);
                        totalReduction += staff.eleDamage[element];
                        totalElements += 1;
                    }
                }
                item.damage -= Math.Max((int)(item.damage * totalReduction), totalElements);
            }
            else if (item.type == mod.GetItem("ProceduralSword").item.type)
            {
                ProceduralSword sword = (ProceduralSword)item.modItem;
                float totalReduction = 0f;
                int totalElements = 0;
                foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                {
                    if (sword.eleDamage[element] > 0f)
                    {
                        elementalDamage[element] += Math.Max((int)(item.damage * sword.eleDamage[element]), 1);
                        totalReduction += sword.eleDamage[element];
                        totalElements += 1;
                    }
                }
                item.damage -= Math.Max((int)(item.damage * totalReduction), totalElements);
            }

            if (item.type == mod.GetItem("EyeOnAStick").item.type)
            {
                elementalDamage[ELEMENT.SHADOW] += 10;
            }

            if (item.Name.Contains("Hellfire") || item.Name.Contains("Molten") || item.Name.Contains("Fiery"))
            {
                elementalDamage[ELEMENT.FIRE] += (int)(item.damage * 0.5);
                item.damage = (int)(item.damage*0.5);
            }

            if (item.Name.Contains("Clockwork") || item.type == Terraria.ID.ItemID.BreakerBlade || item.Name.Contains("Phase") || item.Name.Contains("Thunder"))
            {
                elementalDamage[ELEMENT.LIGHTNING] += (int)(item.damage * 0.3);
                item.damage = (int)(item.damage * 0.7);
            }

            if (item.damage < 1) item.damage = 1;
        }

        public void ClearPrefixes()
        {
            elementalDamage = new Dictionary<ELEMENT, int>()
            {
                {ELEMENT.FIRE, 0},
                {ELEMENT.COLD, 0},
                {ELEMENT.LIGHTNING, 0},
                {ELEMENT.SHADOW, 0},
            };
            statBonus = new Dictionary<STAT, int>()
            {
                {STAT.RESILIENCE, 0},
                {STAT.QUICKNESS, 0},
                {STAT.POTENCY, 0},
                {STAT.WITS, 0},
            };
            resBonus = new Dictionary<ELEMENT, int>()
            {
                {ELEMENT.FIRE, 0},
                {ELEMENT.COLD, 0},
                {ELEMENT.LIGHTNING, 0},
                {ELEMENT.SHADOW, 0},
            };
            bonusEva = 0;
            bonusDef = 0;
            bonusLife = 0;
            bonusMana = 0;
            bonusAccuracy = 0;
            bonusLeech = 0f;
            bonusCrit = 0;
            bonusMult = 0f;
            bonusRegen = 0f;
            bonusAllres = 0;
            prefixTooltips = new List<string>();
        }

        public void PrefixAccessory(Item item)
        {
            ClearPrefixes();

            switch (kPrefix - 1)
            {
                case 0:
                    item.SetNameOverride("Sturdy " + item.Name);
                    statBonus[STAT.RESILIENCE] = 2;
                    break;
                case 1:
                    item.SetNameOverride("Elusive " + item.Name);
                    statBonus[STAT.QUICKNESS] = 2;
                    break;
                case 2:
                    item.SetNameOverride("Volatile " + item.Name);
                    statBonus[STAT.POTENCY] = 2;
                    break;
                case 3:
                    item.SetNameOverride("Cunning " + item.Name);
                    statBonus[STAT.WITS] = 2;
                    break;
                case 4:
                    item.SetNameOverride("Sage " + item.Name);
                    statBonus[STAT.WITS] = 3;
                    item.rare += 1;
                    break;
                case 5:
                    item.SetNameOverride("Armored " + item.Name);
                    bonusDef = 4;
                    prefixTooltips.Add("+4 defence");
                    break;
                case 6:
                    item.SetNameOverride("Elegant " + item.Name);
                    bonusEva = 5;
                    break;
                case 7:
                    item.SetNameOverride("Vampiric " + item.Name);
                    bonusLeech = 0.02f;
                    prefixTooltips.Add("+2% life Leech");
                    break;
                case 8:
                    item.SetNameOverride("Focused " + item.Name);
                    bonusAccuracy = 3;
                    prefixTooltips.Add("+3 accuracy");
                    break;
                case 9:
                    item.SetNameOverride("Precise " + item.Name);
                    bonusAccuracy = 4;
                    prefixTooltips.Add("+4 accuracy");
                    break;
                case 10:
                    item.SetNameOverride("Single-minded " + item.Name);
                    bonusAccuracy = 6;
                    item.rare += 1;
                    prefixTooltips.Add("+6 accuracy");
                    break;
                case 11:
                    item.SetNameOverride("Organic " + item.Name);
                    bonusRegen = 1.4f;
                    prefixTooltips.Add("+1.4 life regen");
                    break;
                case 12:
                    item.SetNameOverride("Arcane " + item.Name);
                    bonusMana = 20;
                    break;
                case 13:
                    item.SetNameOverride("Frenetic " + item.Name);
                    bonusCrit = 4;
                    prefixTooltips.Add("+4 crit chance");
                    break;
                case 14:
                    item.SetNameOverride("Brutal " + item.Name);
                    bonusMult = 0.20f;
                    prefixTooltips.Add("+40 crit multiplier");
                    break;
                case 15:
                    item.SetNameOverride("Warding " + item.Name);
                    bonusAllres = 3;
                    prefixTooltips.Add("+3 to all resistances");
                    break;
                case 16:
                    item.SetNameOverride("Flame Retardant " + item.Name);
                    resBonus[ELEMENT.FIRE] = 10;
                    break;
                case 17:
                    item.SetNameOverride("Waterproof " + item.Name);
                    resBonus[ELEMENT.COLD] = 10;
                    break;
                case 18:
                    item.SetNameOverride("Insulated " + item.Name);
                    resBonus[ELEMENT.LIGHTNING] = 10;
                    break;
                case 19:
                    item.SetNameOverride("Hexproof " + item.Name);
                    resBonus[ELEMENT.SHADOW] = 7;
                    break;
                case 20:
                    item.SetNameOverride("Hardened " + item.Name);
                    bonusDef = 3;
                    prefixTooltips.Add("+3 defence");
                    break;
                case 21:
                    item.SetNameOverride("Protective " + item.Name);
                    bonusAllres = 2;
                    prefixTooltips.Add("+2 to all resistances");
                    break;
                case 22:
                    item.SetNameOverride("Lava-Infused " + item.Name);
                    resBonus[ELEMENT.FIRE] = 15;
                    item.rare += 1;
                    break;
                case 23:
                    item.SetNameOverride("Snowforged " + item.Name);
                    resBonus[ELEMENT.COLD] = 15;
                    item.rare += 1;
                    break;
                case 24:
                    item.SetNameOverride("Lightning-Coiled " + item.Name);
                    resBonus[ELEMENT.LIGHTNING] = 15;
                    item.rare += 1;
                    break;
                case 25:
                    item.SetNameOverride("Blackheart " + item.Name);
                    resBonus[ELEMENT.SHADOW] = 12;
                    item.rare += 1;
                    break;
                case 26:
                    item.SetNameOverride("Enchanted " + item.Name);
                    bonusAllres = 4;
                    prefixTooltips.Add("+4 to all resistances");
                    item.rare += 1;
                    break;
            }
        }

        public void PrefixArmour(Item item)
        {
            ClearPrefixes();
            
            switch (kPrefix - 1)
            {
                case 0:
                    item.SetNameOverride("Carnelian " + item.Name);
                    statBonus[STAT.RESILIENCE] = 1 + item.rare / 2;
                    break;
                case 1:
                    item.SetNameOverride("Viridian " + item.Name);
                    statBonus[STAT.QUICKNESS] = 1 + item.rare / 2;
                    break;
                case 2:
                    item.SetNameOverride("Cerulean " + item.Name);
                    statBonus[STAT.POTENCY] = 1 + item.rare / 2;
                    break;
                case 3:
                    item.SetNameOverride("Arylide " + item.Name);
                    statBonus[STAT.WITS] = 1 + item.rare / 3;
                    break;
                case 4:
                    item.SetNameOverride("Glaucous " + item.Name);
                    bonusDef = 2 + item.rare;
                    prefixTooltips.Add("+" + bonusDef + " defence");
                    break;
                case 5:
                    item.SetNameOverride("Cinereous " + item.Name);
                    bonusDef = 1 + item.rare / 2;
                    prefixTooltips.Add("+" + bonusDef + " defence");
                    bonusLife = 10 + item.rare * 5;
                    break;
                case 6:
                    item.SetNameOverride("Sanguine " + item.Name);
                    bonusLife = 15 + item.rare * 10;
                    break;
                case 7:
                    item.SetNameOverride("Azure " + item.Name);
                    bonusMana = 10 + item.rare * 3;
                    break;
                case 8:
                    item.SetNameOverride("Cerise " + item.Name);
                    bonusMana = 6 + item.rare * 2;
                    bonusLife = 10 + item.rare * 5;
                    break;
                case 9:
                    item.SetNameOverride("Amaranth " + item.Name);
                    bonusLife = 10 + item.rare * 4;
                    statBonus[STAT.RESILIENCE] = 1 + item.rare / 3;
                    break;
                case 10:
                    item.SetNameOverride("Icterine " + item.Name);
                    bonusLife = 10 + item.rare * 3;
                    statBonus[STAT.QUICKNESS] = 1 + item.rare / 3;
                    break;
                case 11:
                    item.SetNameOverride("Byzantine " + item.Name);
                    bonusLife = 8 + item.rare * 3;
                    statBonus[STAT.POTENCY] = 1 + item.rare / 3;
                    break;
                case 12:
                    item.SetNameOverride("Silver " + item.Name);
                    bonusEva = 2 + item.rare;
                    break;
                case 13:
                    item.SetNameOverride("Ebony " + item.Name);
                    bonusEva = 1 + item.rare / 2;
                    bonusLife = 9 + item.rare * 5;
                    break;
                case 14:
                    item.SetNameOverride("Onyx " + item.Name);
                    bonusEva = 1 + item.rare / 2;
                    bonusDef = 1 + item.rare / 2;
                    prefixTooltips.Add("+" + bonusDef + " defence");
                    break;
                case 15:
                    item.SetNameOverride("Scarlet " + item.Name);
                    resBonus[ELEMENT.FIRE] = 6 + item.rare * 2;
                    break;
                case 16:
                    item.SetNameOverride("Celeste " + item.Name);
                    resBonus[ELEMENT.COLD] = 6 + item.rare * 2;
                    break;
                case 17:
                    item.SetNameOverride("Golden " + item.Name);
                    resBonus[ELEMENT.LIGHTNING] = 6 + item.rare * 2;
                    break;
                case 18:
                    item.SetNameOverride("Prismatic " + item.Name);
                    bonusAllres = 3 + item.rare / 2;
                    prefixTooltips.Add("+" + bonusAllres + " to all resistances");
                    break;
            }
        }

        public void Prefix(Item item)
        {
            foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                elementalDamage[element] = 0;

            prefixTooltips.Clear();
            float speed = 1f;

            switch (kPrefix - 1)
            {
                case 0:
                    item.SetNameOverride("Gleaming " + item.Name);
                    item.damage += (int)(item.damage*0.15);
                    item.rare += 1;
                    prefixTooltips.Add("+15% damage");
                    break;
                case 1:
                    item.SetNameOverride("Sulfuric " + item.Name);
                    elementalDamage[ELEMENT.FIRE] = Math.Max((int)(item.damage * 0.25), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 1);
                    prefixTooltips.Add("+5% damage");
                    break;
                case 2:
                    item.SetNameOverride("Windswept " + item.Name);
                    elementalDamage[ELEMENT.COLD] = Math.Max((int)(item.damage * 0.25), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 1);
                    prefixTooltips.Add("+5% damage");
                    break;
                case 3:
                    item.SetNameOverride("Buzzing " + item.Name);
                    elementalDamage[ELEMENT.LIGHTNING] = Math.Max((int)(item.damage * 0.25), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 1);
                    prefixTooltips.Add("+5% damage");
                    break;
                case 4:
                    item.SetNameOverride("Demonic " + item.Name);
                    elementalDamage[ELEMENT.SHADOW] = Math.Max((int)(item.damage * 0.2), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 1);
                    break;
                case 5:
                    item.SetNameOverride("Rending " + item.Name);
                    elementalDamage[ELEMENT.FIRE] = Math.Max((int)(item.damage * 0.15), 1);
                    elementalDamage[ELEMENT.COLD] = Math.Max((int)(item.damage * 0.15), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 1);
                    prefixTooltips.Add("+10% damage");
                    break;
                case 6:
                    item.SetNameOverride("Blazing " + item.Name);
                    elementalDamage[ELEMENT.FIRE] = Math.Max((int)(item.damage * 0.15), 1);
                    elementalDamage[ELEMENT.LIGHTNING] = Math.Max((int)(item.damage * 0.15), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 2);
                    prefixTooltips.Add("+10% damage");
                    break;
                case 7:
                    item.SetNameOverride("Dazzling " + item.Name);
                    elementalDamage[ELEMENT.COLD] = Math.Max((int)(item.damage * 0.15), 1);
                    elementalDamage[ELEMENT.LIGHTNING] = Math.Max((int)(item.damage * 0.15), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 2);
                    prefixTooltips.Add("+10% damage");
                    break;
                case 8:
                    item.SetNameOverride("Prismatic " + item.Name);
                    elementalDamage[ELEMENT.FIRE] = Math.Max((int)(item.damage * 0.11), 1);
                    elementalDamage[ELEMENT.COLD] = Math.Max((int)(item.damage * 0.11), 1);
                    elementalDamage[ELEMENT.LIGHTNING] = Math.Max((int)(item.damage * 0.11), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 2);
                    item.rare += 1;
                    prefixTooltips.Add("+13% damage");
                    break;
                case 9:
                    item.SetNameOverride("Infernal " + item.Name);
                    elementalDamage[ELEMENT.FIRE] = Math.Max((int)(item.damage * 0.15), 1);
                    elementalDamage[ELEMENT.SHADOW] = Math.Max((int)(item.damage * 0.1), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 2);
                    prefixTooltips.Add("+5% damage");
                    break;
                case 10:
                    item.SetNameOverride("Glacial " + item.Name);
                    elementalDamage[ELEMENT.COLD] = Math.Max((int)(item.damage * 0.15), 1);
                    elementalDamage[ELEMENT.SHADOW] = Math.Max((int)(item.damage * 0.1), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 2);
                    prefixTooltips.Add("+5% damage");
                    break;
                case 11:
                    item.SetNameOverride("Ominous " + item.Name);
                    elementalDamage[ELEMENT.LIGHTNING] = Math.Max((int)(item.damage * 0.15), 1);
                    elementalDamage[ELEMENT.SHADOW] = Math.Max((int)(item.damage * 0.1), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 2);
                    prefixTooltips.Add("+5% damage");
                    break;
                case 12:
                    item.SetNameOverride("Volcanic " + item.Name);
                    elementalDamage[ELEMENT.FIRE] = Math.Max((int)(item.damage * 0.15), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.15), 1);
                    item.crit += 10;
                    prefixTooltips.Add("+10% critical strike chance");
                    break;
                case 13:
                    item.SetNameOverride("Cryonic " + item.Name);
                    elementalDamage[ELEMENT.COLD] = Math.Max((int)(item.damage * 0.15), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.15), 1);
                    item.crit += 10;
                    prefixTooltips.Add("+10% critical strike chance");
                    break;
                case 14:
                    item.SetNameOverride("Stilling " + item.Name);
                    elementalDamage[ELEMENT.LIGHTNING] = Math.Max((int)(item.damage * 0.15), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.15), 1);
                    item.crit += 10;
                    prefixTooltips.Add("+10% critical strike chance");
                    break;
                case 15:
                    item.SetNameOverride("Ruthless " + item.Name);
                    item.damage += (int)(item.damage * 0.1);
                    item.crit += 5;
                    prefixTooltips.Add("+10% damage");
                    prefixTooltips.Add("+5% critical strike chance");
                    break;
                case 16:
                    item.SetNameOverride("Ancient " + item.Name);
                    elementalDamage[ELEMENT.FIRE] = Math.Max((int)(item.damage * 0.11), 1);
                    elementalDamage[ELEMENT.COLD] = Math.Max((int)(item.damage * 0.11), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.16), 2);
                    item.crit += 5;
                    prefixTooltips.Add("+6% damage");
                    prefixTooltips.Add("+5% critical strike chance");
                    break;
                case 17:
                    item.SetNameOverride("Malevolent " + item.Name);
                    elementalDamage[ELEMENT.FIRE] = Math.Max((int)(item.damage * 0.11), 1);
                    elementalDamage[ELEMENT.LIGHTNING] = Math.Max((int)(item.damage * 0.11), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.16), 2);
                    item.crit += 5;
                    prefixTooltips.Add("+6% damage");
                    prefixTooltips.Add("+5% critical strike chance");
                    break;
                case 18:
                    item.SetNameOverride("Incandescent " + item.Name);
                    elementalDamage[ELEMENT.COLD] = Math.Max((int)(item.damage * 0.11), 1);
                    elementalDamage[ELEMENT.LIGHTNING] = Math.Max((int)(item.damage * 0.11), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.16), 2);
                    item.crit += 5;
                    prefixTooltips.Add("+6% damage");
                    prefixTooltips.Add("+5% critical strike chance");
                    break;
                case 19:
                    item.SetNameOverride("Searing " + item.Name);
                    elementalDamage[ELEMENT.FIRE] = Math.Max((int)(item.damage * 0.2), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 1);
                    speed = 1.1f;
                    prefixTooltips.Add("+10% speed");
                    break;
                case 20:
                    item.SetNameOverride("Arctic " + item.Name);
                    elementalDamage[ELEMENT.COLD] = Math.Max((int)(item.damage * 0.2), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 1);
                    speed = 1.1f;
                    prefixTooltips.Add("+10% speed");
                    break;
                case 21:
                    item.SetNameOverride("Thunderous " + item.Name);
                    elementalDamage[ELEMENT.LIGHTNING] = Math.Max((int)(item.damage * 0.2), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 1);
                    speed = 1.1f;
                    prefixTooltips.Add("+10% speed");
                    break;
                case 22:
                    item.SetNameOverride("Sacrificial " + item.Name);
                    elementalDamage[ELEMENT.SHADOW] = Math.Max((int)(item.damage * 0.2), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 1);
                    speed = 1.1f;
                    prefixTooltips.Add("+10% speed");
                    break;
                case 23:
                    item.SetNameOverride("Hateful " + item.Name);
                    elementalDamage[ELEMENT.FIRE] = Math.Max((int)(item.damage * 0.2), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 1);
                    speed = 1.1f;
                    prefixTooltips.Add("+10% speed");
                    item.crit += 5;
                    prefixTooltips.Add("+5% critical strike chance");
                    break;
                case 24:
                    item.SetNameOverride("Forgotten " + item.Name);
                    elementalDamage[ELEMENT.COLD] = Math.Max((int)(item.damage * 0.2), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 1);
                    speed = 1.1f;
                    prefixTooltips.Add("+10% speed");
                    item.crit += 5;
                    prefixTooltips.Add("+5% critical strike chance");
                    break;
                case 25:
                    item.SetNameOverride("Unreal " + item.Name);
                    elementalDamage[ELEMENT.LIGHTNING] = Math.Max((int)(item.damage * 0.2), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.2), 1);
                    speed = 1.1f;
                    prefixTooltips.Add("+10% speed");
                    item.crit += 5;
                    prefixTooltips.Add("+5% critical strike chance");
                    break;
                case 26:
                    item.SetNameOverride("Zealous " + item.Name);
                    item.crit += 15;
                    prefixTooltips.Add("+15% critical strike chance");
                    break;
                case 27:
                    item.SetNameOverride("Legendary " + item.Name);
                    item.damage += (int)(item.damage * 0.1);
                    prefixTooltips.Add("+10% damage");
                    speed = 1.1f;
                    prefixTooltips.Add("+10% speed");
                    item.crit += 5;
                    prefixTooltips.Add("+5% critical strike chance");
                    item.rare += 1;
                    break;
                case 28:
                    item.SetNameOverride("Mythical " + item.Name);
                    speed = 1.1f;
                    prefixTooltips.Add("+10% speed");
                    item.crit += 10;
                    prefixTooltips.Add("+10% critical strike chance");
                    item.rare += 1;
                    break;
                case 29:
                    item.SetNameOverride("Transcendent " + item.Name);
                    elementalDamage[ELEMENT.FIRE] = Math.Max((int)(item.damage * 0.1), 1);
                    elementalDamage[ELEMENT.COLD] = Math.Max((int)(item.damage * 0.1), 1);
                    elementalDamage[ELEMENT.LIGHTNING] = Math.Max((int)(item.damage * 0.1), 1);
                    item.damage -= Math.Max((int)(item.damage * 0.18), 2);
                    prefixTooltips.Add("+12% damage");
                    speed = 1.1f;
                    prefixTooltips.Add("+10% speed");
                    item.crit += 5;
                    prefixTooltips.Add("+5% critical strike chance");
                    item.rare += 1;
                    break;
            }
            int i = item.useTime - item.useAnimation;

            item.useAnimation = (int)Math.Round(item.useAnimation / speed);
            item.useTime = (int)Math.Round(item.useTime / speed);

            if (i >= 0 && item.useTime < item.useAnimation)
                item.useTime = item.useAnimation + (i > 1 ? (int)(i/speed) : i);
            if (i >= 0 && item.useTime < item.useAnimation)
                item.useTime = item.useAnimation;

            item.SetNameOverride(item.Name + "+" + upgradeLevel.ToString());
            
            if (item.damage < 1) item.damage = 1;
        }

        //Seems to work fine now
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (Main.netMode == 2) return;
            if (item.defense > 0 || item.accessory)
            {

                foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                {
                    if (resBonus[element] != 0)
                    {
                        string color = elementNames[element].Substring(0, 9);
                        string eleName = elementNames[element].Substring(9);
                        TooltipLine line = new TooltipLine(mod, "Res" + element.ToString(), resBonus[element].ToString() + eleName + " resistance");
                        line.overrideColor = new Color(Int32.Parse(color.Substring(0, 3)), Int32.Parse(color.Substring(3, 3)), Int32.Parse(color.Substring(6, 3)));
                        tooltips.Insert(1, line);
                    }
                }

                foreach (STAT stat in Enum.GetValues(typeof(STAT)))
                {
                    if (statBonus[stat] != 0)
                    {
                        string color = statNames[stat].Substring(0, 9);
                        string statName = statNames[stat].Substring(9);
                        TooltipLine line = new TooltipLine(mod, "Stat" + stat.ToString(), statBonus[stat].ToString() + statName);
                        line.overrideColor = new Color(Int32.Parse(color.Substring(0, 3)), Int32.Parse(color.Substring(3, 3)), Int32.Parse(color.Substring(6, 3)));
                        tooltips.Insert(1, line);
                    }
                }

                if (bonusEva > 0)
                {
                    TooltipLine line = new TooltipLine(mod, "Evasion", bonusEva.ToString() + " evasion rating");
                    line.overrideColor = new Color(159, 159, 159);
                    tooltips.Insert(1, line);
                }

                if (bonusMana > 0)
                {
                    TooltipLine line = new TooltipLine(mod, "Mana", bonusMana.ToString() + " maximum mana");
                    line.overrideColor = new Color(0, 63, 255);
                    tooltips.Insert(1, line);
                }

                if (bonusLife > 0)
                {
                    TooltipLine line = new TooltipLine(mod, "Life", bonusLife.ToString() + " maximum life");
                    line.overrideColor = new Color(255, 31, 31);
                    tooltips.Insert(1, line);
                }

                for (int i = 0; i < prefixTooltips.Count; i += 1)
                {
                    TooltipLine line = new TooltipLine(mod, "prefixline" + i, prefixTooltips[i]);
                    line.overrideColor = new Color(127, 191, 127);
                    tooltips.Add(line);
                }
            }

            else if (item.damage > 0)
            {
                foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                {
                    int eleDamage = GetIndividualElements(item, Main.player[Main.myPlayer])[element];
                    if (eleDamage != 0)
                    {
                        string color = elementNames[element].Substring(0, 9);
                        string eleName = elementNames[element].Substring(9);
                        TooltipLine line = new TooltipLine(mod, "Element" + element.ToString(), eleDamage.ToString() + eleName + "damage");
                        line.overrideColor = new Color(Int32.Parse(color.Substring(0, 3)), Int32.Parse(color.Substring(3, 3)), Int32.Parse(color.Substring(6, 3)));
                        tooltips.Insert(tooltips.FindIndex(tooltip => tooltip.Name == "Damage"), line);
                    }
                }
                for (int i = 0; i < prefixTooltips.Count; i += 1)
                {
                    TooltipLine line = new TooltipLine(mod, "prefixline" + i, prefixTooltips[i]);
                    line.overrideColor = new Color(127, 191, 127);
                    tooltips.Add(line);
                }
            }
        }

        public void ApplyUpgradeLevel(Item item)
        {
            double animationDPS = 60 * item.damage / item.useAnimation;
            double usetimeDPS = 60 * item.damage / item.useTime;
            double dpsModifier = 1.0;
            switch (upgradeLevel)
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
            animationDPS *= dpsModifier;
            usetimeDPS *= dpsModifier;

            item.damage = (int)Math.Round(animationDPS / 60 * item.useAnimation);

            int i = item.useTime - item.useAnimation;
            
            item.useAnimation = (int)Math.Round(60 / animationDPS * item.damage);
            item.useTime = (int)Math.Round(60 / usetimeDPS * item.damage);

            if (i >= 0 && item.useTime < item.useAnimation)
                item.useTime = item.useAnimation + i;
            if (i >= 0 && item.useTime < item.useAnimation)
                item.useTime = item.useAnimation;

            item.crit = item.crit + upgradeLevel - 1;

            if (upgradeLevel < 3)
                item.scale *= 0.95f;
            else if (upgradeLevel >= 6 && upgradeLevel < 8)
                item.scale *= 1.05f;
            else if (upgradeLevel == 8)
                item.scale *= 1.1f;
        }  

        public byte RandomizeUpgradeLevel(Item item, bool bonus)
        {
            if (Main.rand.Next(2) == 1)
            {
                if (Main.rand.Next(3) == 1)
                {
                    if (Main.rand.Next(4) == 1 && bonus) return 3;

                    else return 2;
                }
                else return 1;
            }
            else return 0;
        }

        public void Upgrade(Item item)
        {
            SetUpgradeLevel(item, (byte)(upgradeLevel + 1));
            Main.NewText("Successfully upgraded item to +" + upgradeLevel.ToString(), 0, 255, 0);
        }

        public void SetUpgradeLevel(Item item, byte level)
        {
            upgradeLevel = level;
            ApplyStats(item);
        }

        public void Downgrade(Item item)
        {
            SetUpgradeLevel(item, (byte)Math.Max(upgradeLevel - 1, 0));
            Main.NewText("Failed to upgrade - item was downgraded to +" + upgradeLevel.ToString(), 255, 0, 0);
        }

        public void Destroy(Item item)
        {
            Main.NewText("Failed to upgrade - item was destroyed", 255, 0, 0);
            item.SetDefaults(0,true);
        }

        public bool Upgradeable(Item item)
        {
            return item.damage > 0 && item.ammo == 0;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            PlayerCharacter modPlayer = player.GetModPlayer<PlayerCharacter>();

            if (!enhanced) return;
            if (bonusDef > 0)
                player.statDefense += bonusDef;
            if (bonusLife > 0)
                modPlayer.bonusLife += bonusLife;
            if (bonusMana > 0)
                modPlayer.bonusMana += bonusMana;
            if (bonusAccuracy > 0)
                modPlayer.accuracy += bonusAccuracy;
            if (bonusLeech > 0f)
                modPlayer.lifeLeech += bonusLeech;
            if (bonusCrit > 0)
                modPlayer.critBoost += bonusCrit;
            if (bonusMult > 0f)
                modPlayer.critMultiplier += bonusMult;
            if (bonusRegen > 0f)
                modPlayer.lifeRegen += bonusRegen;
            foreach (STAT stat in Enum.GetValues(typeof(STAT)))
                if (statBonus[stat] > 0)
                    modPlayer.tempStats[stat] += statBonus[stat];
            foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                modPlayer.eleres[element] += resBonus[element];
            modPlayer.allres += bonusAllres;
            if (bonusEva > 0)
                modPlayer.evasion += bonusEva;
        }

        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (NeedsSaving(item) && !enhanced && !WorldGen.gen && !Main.gameMenu)
                Initialize(item);
            PlayerCharacter character = Main.player[Main.myPlayer].GetModPlayer<PlayerCharacter>(mod);

            if (Upgradeable(item) && Main.mouseRight && Main.mouseRightRelease && new Rectangle((int)position.X, (int)position.Y, 38, 38).Contains(Main.mouseX, Main.mouseY))
            {
                character.anvilGUI.AttemptSelectItem(this, item);
            }
        }

        public override bool CanPickup(Item item, Player player)
        {
            return ItemSpace(item, player);
        }

        public override bool ItemSpace(Item newItem, Player player)
        {
            PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
            if (newItem.type == mod.ItemType<PermanenceCrown>() || newItem.type == mod.ItemType<BlacksmithCrown>())
                return true;
            
            int num = 50;
            if (newItem.type == 71 || newItem.type == 72 || newItem.type == 73 || newItem.type == 74)
            {
                num = 54;
            }
            for (int i = 0; i < num; i++)
            {
                Item item = player.inventory[i];
                if ((item.type == 0 || item.stack == 0) && (!kConfig.configLocal.clientside.manualInventory || num > 50 || character.activeInvPage == 0) || item.type > 0 && item.stack > 0 && item.stack < item.maxStack && newItem.IsTheSameAs(item))
                {
                    return true;
                }
            }
            for (int i = 0; i < character.inventories.Length; i++)
            {
                for (int j = 0; j < character.inventories[i].Length; j += 1)
                {
                    Item item = character.inventories[i][j];
                    if ((item.type == 0 || item.stack == 0) && (!kConfig.configLocal.clientside.manualInventory || i == 0) || item.type > 0 && item.stack > 0 && item.stack < item.maxStack && newItem.IsTheSameAs(item))
                    {
                        //Main.NewText((!kConfig.configLocal.clientside.manualInventory) + "||" + (i == 0));
                        return true;
                    }
                }
            }
            return false;
        }

        public override bool OnPickup(Item item, Player player)
        {
            PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
            if (player.whoAmI == Main.myPlayer && (player.inventory[player.selectedItem].type != 0 || player.itemAnimation <= 0))
            {
                if (ItemID.Sets.NebulaPickup[item.type])
                {
                    Main.PlaySound(7, (int)player.position.X, (int)player.position.Y, 1, 1f, 0f);
                    item = new Item();
                    if (Main.netMode == 1)
                    {
                        NetMessage.SendData(102, -1, -1, null, player.whoAmI, (float)item.buffType, player.Center.X, player.Center.Y, 0, 0, 0);
                        NetMessage.SendData(21, -1, -1, null, item.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                    }
                    else
                    {
                        player.NebulaLevelup(item.buffType);
                    }
                }
                if (item.type == ItemID.Heart || item.type == ItemID.CandyApple || item.type == ItemID.CandyCane)
                {
                    Main.PlaySound(7, (int)player.position.X, (int)player.position.Y, 1, 1f, 0f);
                    int healAmount = 10 + player.GetModPlayer<PlayerCharacter>().level / 2;
                    player.statLife += healAmount;
                    if (Main.myPlayer == player.whoAmI)
                    {
                        player.HealEffect(healAmount);
                    }
                    if (player.statLife > player.statLifeMax2)
                    {
                        player.statLife = player.statLifeMax2;
                    }
                    item = new Item();
                    if (Main.netMode == 1)
                    {
                        NetMessage.SendData(21, -1, -1, null, item.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                    }
                }
                else if (item.type == ItemID.Star || item.type == ItemID.SoulCake || item.type == ItemID.SugarPlum)
                {
                    Main.PlaySound(7, (int)player.position.X, (int)player.position.Y, 1, 1f, 0f);
                    int healAmount = 5 + character.TotalStats(STAT.WITS);
                    character.mana += healAmount;
                    player.statMana += healAmount;
                    if (Main.myPlayer == player.whoAmI)
                    {
                        player.ManaEffect(healAmount);
                    }
                    item = new Item();
                    if (Main.netMode == 1)
                    {
                        NetMessage.SendData(21, -1, -1, null, item.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                    }
                }
                else if (item.type == mod.ItemType<PermanenceCrown>())
                {
                    character.permanence += 1;
                    ItemText.NewText(item, item.stack);
                    Main.PlaySound(7, player.position);
                    return false;
                }
                else if (item.type == mod.ItemType<BlacksmithCrown>())
                {
                    character.transcendence += 1;
                    ItemText.NewText(item, item.stack);
                    Main.PlaySound(7, player.position);
                    return false;
                }
                else
                {
                    item = player.GetItem(item);
                    if (Main.netMode == 1)
                    {
                        NetMessage.SendData(21, -1, -1, null, item.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                    }
                }
            }

            return false;
        }

        public override bool NeedsSaving(Item item)
        {
            return item.maxStack == 1 && (item.damage > 0 || item.defense > 0 || item.accessory);
        }

        public override TagCompound Save(Item item)
        {
            return new TagCompound
            {
                {"upgrade level", upgradeLevel},
                {"prefix", kPrefix}
            };
        }

        public override void NetSend(Item item, BinaryWriter writer)
        {
            writer.Write(upgradeLevel);
            writer.Write(kPrefix);
        }

        public override void Load(Item item, TagCompound tag)
        {
            upgradeLevel = tag.GetByte("upgrade level");
            kPrefix = tag.GetByte("prefix");
            ApplyStats(item);
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            upgradeLevel = reader.ReadByte();
            kPrefix = reader.ReadByte();
            ApplyStats(item);
        }

        public Mod getMod()
        {
            return mod;
        }

        public int GetEleDamage(Item item, Player player, bool ignoreModifiers = false)
        {
            Dictionary<ELEMENT, int> ele = new Dictionary<ELEMENT, int>();
            ele = GetIndividualElements(item, player, ignoreModifiers);
            return ele[ELEMENT.FIRE] + ele[ELEMENT.COLD] + ele[ELEMENT.LIGHTNING] + ele[ELEMENT.SHADOW];
        }

        public Dictionary<ELEMENT, int> GetIndividualElements(Item item, Player player, bool ignoreModifiers = false)
        {
            Dictionary<ELEMENT, int> dictionary = new Dictionary<ELEMENT, int>();
            if (player.GetModPlayer<PlayerCharacter>().rituals[RITUAL.DEMON_PACT])
            {
                foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                    dictionary[element] = 0;

                dictionary[ELEMENT.SHADOW] = GetEleDamage(item, player);
            }
            else foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
            {
                dictionary[element] = (int)Math.Round(elementalDamage[element] * (ignoreModifiers ? 1 : player.GetModPlayer<PlayerCharacter>(mod).DamageMultiplier(element, item.melee, item.ranged, item.magic, item.thrown, item.summon)));
            }
            return dictionary;
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

        public override bool UseItem(Item item, Player player)
        {
            if (item.healMana > 0)
            {
                PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
                character.mana = Math.Min(player.statManaMax2, character.mana + item.healMana);
                player.statMana = character.mana;
                player.AddBuff(mod.BuffType<ManaCooldown>(), 600);
                return true;
            }
            return false;
        }

        public override bool CanUseItem(Item item, Player player)
        {
            if (item.healMana > 0)
                return player.GetModPlayer<PlayerCharacter>().canHealMana;
            return true;
        }
    }
}
