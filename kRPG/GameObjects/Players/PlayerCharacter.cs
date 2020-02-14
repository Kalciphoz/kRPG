using System;
using System.Collections.Generic;
using System.Linq;
using kRPG.Enums;
using kRPG.GameObjects.Buffs;
using kRPG.GameObjects.GUI;
using kRPG.GameObjects.GUI.Base;
using kRPG.GameObjects.Items;
using kRPG.GameObjects.Items.Dusts;
using kRPG.GameObjects.Items.Glyphs;
using kRPG.GameObjects.Items.Projectiles;
using kRPG.GameObjects.Items.Projectiles.Base;
using kRPG.GameObjects.Items.Weapons.Melee;
using kRPG.GameObjects.NPCs;
using kRPG.GameObjects.Spells;
using kRPG.Packets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

// ReSharper disable StringLiteralTypo

namespace kRPG.GameObjects.Players
{
    public class PlayerCharacter : ModPlayer
    {
        public const int DefaultMaxUpgradeLevel = 7;

        public PlayerCharacter()
        {
            foreach (PlayerStats stat in Enum.GetValues(typeof(PlayerStats)))
            {
                BaseStats[stat] = 0;
                TempStats[stat] = 0;
            }

            Permanence = 0;
            Transcendence = 0;
            for (int i = 0; i < Abilities.Length; i += 1)
            {
                Abilities[i] = new ProceduralSpell(mod);
                for (int j = 0; j < Abilities[i].Glyphs.Length; j += 1)
                {
                    Abilities[i].Glyphs[j] = new Item();
                    Abilities[i].Glyphs[j].SetDefaults(0, true);
                }
            }

            Abilities[0].Key = Keys.Z;
            Abilities[1].Key = Keys.X;
            Abilities[2].Key = Keys.C;
            Abilities[3].Key = Keys.V;

            Inventories = new Item[3][];
            for (int inventoryPage = 0; inventoryPage < Inventories.Length; inventoryPage += 1)
            {
                Inventories[inventoryPage] = new Item[40];
                for (int inventorySlot = 0; inventorySlot < Inventories[inventoryPage].Length; inventorySlot += 1)
                {
                    Inventories[inventoryPage][inventorySlot] = new Item();
                    Inventories[inventoryPage][inventorySlot].SetDefaults(0, true);
                }
            }
        }

        public ProceduralSpell[] Abilities { get; set; } = new ProceduralSpell[4];
        public AbilitiesGui AbilitiesGui { get; set; }
        public int Accuracy { get; set; }

        public float AccuracyCounter { get; set; } = 0.5f;
        public int ActiveInvPage { get; set; }

        public Dictionary<Element, int> AilmentIntensity { get; set; } = new Dictionary<Element, int>
        {
            {Element.Fire, 0}, {Element.Cold, 0}, {Element.Lightning, 0}, {Element.Shadow, 0}
        };

        public Dictionary<Element, Type> Ailments { get; set; } = new Dictionary<Element, Type>
        {
            {Element.Fire, typeof(Fire)}, {Element.Cold, typeof(Cold)}, {Element.Lightning, typeof(Lightning)}, {Element.Shadow, typeof(Shadow)}
        };

        public int AllResists { get; set; }
        public AnvilGui AnvilGui { get; set; }

        public Dictionary<PlayerStats, int> BaseStats { get; set; } = new Dictionary<PlayerStats, int>();
        public int BigCritCounter { get; set; } = 50;
        public int BigHitCounter { get; set; } = 50;

        public int BonusLife { get; set; }
        public int BonusMana { get; set; }
        public bool CanHealMana { get; set; } = true;
        public List<ProceduralSpellProj> CirclingProtection { get; set; } = new List<ProceduralSpellProj>();
        public float CritAccuracyCounter { get; set; } = 0.5f;

        public int CritBoost { get; set; }
        public int CritCounter { get; set; } = 50;

        public float CritHitChance {
            get {
                float diff = 4f + Level / 12f;
                return 1f - diff * (1f - 0.8f) / (Accuracy + diff);
            }
        }

        public float CritMultiplier { get; set; } = 1f;

        public float DamageMultiplierPercent => 1f + TotalStats(PlayerStats.Potency) * 0.05f + Math.Min(0.09f, TotalStats(PlayerStats.Potency) * 0.06f);
        private float DegenTimer { get; set; }

        public Dictionary<Element, int> ElementalResists { get; set; } = new Dictionary<Element, int>
        {
            {Element.Fire, 0}, {Element.Cold, 0}, {Element.Lightning, 0}, {Element.Shadow, 0}
        };

        public int Evasion { get; set; } = 2;
        public int EvasionCounter { get; set; } = 50;
        public int Experience { get; set; }

        public Dictionary<Element, bool> HasAilment { get; set; } = new Dictionary<Element, bool>
        {
            {Element.Fire, false}, {Element.Cold, false}, {Element.Lightning, false}, {Element.Shadow, false}
        };

        public float HitChance {
            get {
                float diff = 7f + Level / 40f;
                return 1f - diff * (1f - 0.85f) / (Accuracy + diff);
            }
        }

        private bool Initialized { get; set; }

        public Item[][] Inventories { get; set; } 
        public InventoryGui InventoryGui { get; set; }

        public double ItemRotation { get; set; }
        public Item LastSelectedWeapon { get; set; }
        private int LeechCooldown { get; set; }

        public int Level { get; set; } = 1;

        private int LevelAnimation { get; set; }
        public LevelGui LevelGui { get; set; }
        public float LifeDegen { get; set; }

        public float LifeLeech { get; set; }

        public float LifeRegen { get; set; } = 1f;

        public int Mana { get; set; }
        public float ManaRegen { get; set; }
        private float ManaRegenTimer { get; set; }
        public List<ProceduralMinion> Minions { get; set; } = new List<ProceduralMinion>();
        public int Permanence { get; set; }

        public int PointsAllocated {
            get { return Enum.GetValues(typeof(PlayerStats)).Cast<PlayerStats>().Sum(stat => BaseStats[stat]); }
        }

        private float RegenTimer { get; set; }

        public Dictionary<Element, int> Resistance {
            get {
                Dictionary<Element, int> dict = new Dictionary<Element, int>();
                foreach (Element element in Enum.GetValues(typeof(Element)))
                    dict[element] = ElementalResists[element] + AllResists;
                return dict;
            }
        }

        public Dictionary<Ritual, bool> Rituals { get; set; } = new Dictionary<Ritual, bool>();
        public ProceduralSpell SelectedAbility { get; set; } = null;
        public SpellCraftingGui SpellCraftingGui { get; set; }
        public List<SpellEffect> SpellEffects { get; set; } = new List<SpellEffect>();
        public bool StatPage { get; set; } = true;
        public StatusBar StatusBar { get; set; }
        public Dictionary<PlayerStats, int> TempStats { get; set; } = new Dictionary<PlayerStats, int>();
        public List<Trail> Trails { get; set; } = new List<Trail>();
        public int Transcendence { get; set; }

        public void AddXp(int xp)
        {
            if (Main.gameMenu)
                return;
            if (xp == 0) 
                return;
            Experience += xp;

        Check:
            if (Experience >= ExperienceToLevel())
            {
                Experience -= ExperienceToLevel();
                LevelUp();
                goto Check;
            }

            CombatText.NewText(player.getRect(), new Color(127, 159, 255), xp + " XP");
        }

        public float DamageMultiplier(Element? element, bool melee, bool ranged = false, bool magic = false, bool thrown = false, bool minion = false)
        {
            float dmgModifier = 1f;
            if (melee) dmgModifier *= player.meleeDamage;
            if (ranged) dmgModifier *= player.rangedDamage;
            if (magic) dmgModifier *= player.magicDamage;
            if (thrown) dmgModifier *= player.thrownDamage;
            if (minion) dmgModifier *= player.minionDamage;
            return dmgModifier;
        }

        public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (Main.netMode == Constants.NetModes.Server || Main.myPlayer != player.whoAmI) return;
            if (player.statLife < 1) return;
            if (HasAilment[Element.Fire])
            {
                if (Main.rand.Next(2) == 0)
                {
                    int dust = Dust.NewDust(player.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, DustID.Fire, player.velocity.X * 0.4f,
                        player.velocity.Y * 0.4f, 100, default, 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                }

                Lighting.AddLight(player.position, 0.7f, 0.4f, 0.1f);
                fullBright = true;
            }

            if (HasAilment[Element.Cold])
            {
                if (Main.rand.Next(2) == 0)
                {
                    int dust = Dust.NewDust(player.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, ModContent.GetInstance<Ice>().Type,
                        player.velocity.X, player.velocity.Y, 100, Color.White, 1.5f);
                    Main.dust[dust].noGravity = true;
                }

                Lighting.AddLight(player.position, 0f, 0.4f, 1f);
            }

            if (HasAilment[Element.Lightning])
            {
                if (Main.rand.Next(2) == 0)
                {
                    int dust = Dust.NewDust(player.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, DustID.Electric, player.velocity.X,
                        player.velocity.Y, 100, default, 0.5f);
                    Main.dust[dust].noGravity = true;
                }

                Lighting.AddLight(player.position, 0.5f, 0.5f, 0.5f);
                fullBright = true;
            }

            if (HasAilment[Element.Shadow])
                if (Main.rand.Next(3) < 2)
                {
                    int dust = Dust.NewDust(player.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, DustID.Shadowflame, player.velocity.X,
                        player.velocity.Y, 100, default, 1.5f);
                    Main.dust[dust].noGravity = true;
                }

            if (Main.netMode == Constants.NetModes.Server) return;
            SpriteBatch spriteBatch = Main.spriteBatch;

            foreach (Trail trail in Trails.ToArray())
                trail.Draw(spriteBatch, player);

            if (LevelAnimation >= 60)
                return;
            if (LevelAnimation < 24)
            {
                fullBright = true;
                Lighting.AddLight(player.position, 0.9f, 0.9f, 0.9f);
            }
            else
            {
                Lighting.AddLight(player.position, 0.4f, 0.4f, 0.4f);
            }

            spriteBatch.Draw(GFX.GFX.LevelUp, player.Bottom - new Vector2(48, 108) - Main.screenPosition, new Rectangle(0, LevelAnimation / 3 * 96, 96, 96),
                Color.White);
            LevelAnimation += 1;
        }

        public int ExperienceToLevel()
        {
            if (Level < 5)
                return 80 + Level * 20;
            if (Level < 10)
                return Level * 40;
            if (Level < 163)
                return (int)(280 * Math.Pow(1.09, Level - 5) + 3 * Level);
            return (int)(2000000000 - 288500000000 / Level);
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            foreach (Projectile projectile in Main.projectile)
                if (projectile.modProjectile is ProceduralSpear || projectile.modProjectile is ProceduralMinion)
                    projectile.Kill();
            foreach (ProceduralSpellProj spell in CirclingProtection)
                spell.projectile.Kill();
            CirclingProtection.Clear();
        }

        private void LeechLife(Item item, int damage)
        {
            if (LeechCooldown != 0)
                return;
            int leechAmount = Math.Min((int)(damage * LifeLeech), (int)(player.inventory[player.selectedItem].damage / 2f * (1 + LifeLeech)));
            leechAmount = Math.Min(leechAmount, (int)(player.statLifeMax2 * LifeLeech * 0.2));
            if (leechAmount > 1)
            {
                player.statLife += leechAmount;
                player.HealEffect(leechAmount);
                LeechCooldown = item.useAnimation * 3;
            }

            else if (LifeLeech > 0f)
            {
                player.statLife += 1;
                player.HealEffect(1);
                LeechCooldown = (int)(item.useAnimation * (3 - Math.Min(1.4f, LifeLeech * 10f)));
            }
        }

        public void LevelUp()
        {
            Level += 1;
            if (!Main.gameMenu)
                GFX.GFX.SfxLevelUp.Play(0.5f * Main.soundVolume, 0f, 0f);
            SyncLevelPacket.Write(player.whoAmI, Level);
            LevelAnimation = 0;
            Main.NewText("Congratulations! You are now level " + Level, 255, 223, 63);
        }

        private void ModifyDamage(ref int damage, ref bool crit, NPC target, Item item = null, Projectile proj = null)
        {
            if (Rituals[Ritual.WarriorOath])
            {
                crit = false;
                float damageBoost = 1f + TotalStats(PlayerStats.Resilience) * 0.04f;
                damageBoost += Math.Min(0.1f, TotalStats(PlayerStats.Resilience) * 0.02f);
                damage = (int)(damage * damageBoost);
            }

            Dictionary<Element, int> eleDmg = new Dictionary<Element, int>();

            if (item != null)
            {
                kItem ki = item.GetGlobalItem<kItem>();
                damage += ki.GetEleDamage(item, player);
                eleDmg = ki.GetIndividualElements(item, player);
            }
            else if (proj != null)
            {
                kProjectile kp = proj.GetGlobalProjectile<kProjectile>();
                damage += kp.GetEleDamage(proj, player);
                eleDmg = kp.GetIndividualElements(proj, player);
            }

            if (HasAilment[Element.Shadow])
                damage = Math.Min(damage * 2 / 5, damage - AilmentIntensity[Element.Shadow]);
            //    damage = damage * (20 + 9360 / (130 + ailmentIntensity[ELEMENT.SHADOW])) / 100;

            kNPC victim = target.GetGlobalNPC<kNPC>();

            if (!crit && Main.netMode == Constants.NetModes.SinglePlayer)
                crit = Main.rand.Next(500) < 50 + victim.AilmentIntensity[Element.Cold];

            if (crit)
            {
                damage = (int)(damage / DamageMultiplierPercent * (DamageMultiplierPercent + CritMultiplier));
                if (Rituals[Ritual.EldritchFury])
                {
                    int i = damage;
                    int c = target.boss ? 7 : 2;
                    damage += Math.Min(Mana * c, i);
                    Mana = Math.Max(0, Mana - i / c);
                }
            }

            if (item == null && proj == null) return;

            if (victim.HasAilment[Element.Lightning])
                damage += 1 + victim.AilmentIntensity[Element.Lightning];

            foreach (Element element in Enum.GetValues(typeof(Element)))
            {
                if (Main.rand.Next(target.boss ? 500 : 200) >= 30 + eleDmg[element])
                    continue;

                if (eleDmg[element] <= 0)
                    continue;

                //Type t = Ailments[element];

                ModBuff buff;

                if (Ailments[element] == typeof(Fire))
                    buff = ModContent.GetInstance<Fire>();

                else if (Ailments[element] == typeof(Cold))
                    buff = ModContent.GetInstance<Cold>();

                else if (Ailments[element] == typeof(Lightning))
                    buff = ModContent.GetInstance<Lightning>();

                else
                    buff = ModContent.GetInstance<Shadow>();

                target.AddBuff(buff.Type, target.boss ? 30 + Math.Min(eleDmg[element], 30) * 3 : 120 + Math.Min(eleDmg[element], 15) * 12);

                victim.AilmentIntensity[element] = target.boss ? eleDmg[element] / 2 : eleDmg[element];

                victim.HasAilment[element] = true;
            }
        }

        public void ModifyDamageTakenFromNpc(ref int damage, ref bool crit, Dictionary<Element, int> eleDmg)
        {
            double dmg = 0.5 * Math.Pow(damage, 1.35);
            Dictionary<Element, int> originalEle = eleDmg;
            foreach (Element element in Enum.GetValues(typeof(Element)))
                eleDmg[element] = (int)(0.5 * Math.Pow(eleDmg[element], 1.35));
            if (!Main.expertMode)
            {
                dmg = dmg * 1.3;
                foreach (Element element in Enum.GetValues(typeof(Element)))
                    eleDmg[element] = (int)(eleDmg[element] * 1.3);
            }

            damage = (int)Math.Round(Math.Min(dmg, (double)damage * 3));
            foreach (Element element in Enum.GetValues(typeof(Element)))
                eleDmg[element] = Math.Min(originalEle[element] * 3, eleDmg[element]);
            bool bossFight = false;
            foreach (NPC n in Main.npc)
                if (n.active)
                    if (n.boss)
                        bossFight = true;
            int eleCount = Enum.GetValues(typeof(Element)).Cast<Element>().Count(element => eleDmg[element] > 0);
            if (eleCount > 0) damage = (int)Math.Round(damage * (kNPC.EleDmgModifier + 1) / 2);
            foreach (Element element in Enum.GetValues(typeof(Element)))
            {
                damage -= Math.Min(Resistance[element], eleDmg[element] * 3 / 5);
                if (Main.rand.Next(player.statLifeMax2 + Resistance[element] * 20) >= 15 + eleDmg[element] * (bossFight ? 2 : 8) || Main.netMode == Constants.NetModes.Server)
                    continue;
                if (eleDmg[element] <= 0)
                    continue;
                //Type t = Ailments[element];
                ModBuff buff;
                if (Ailments[element] == typeof(Fire))
                    buff = ModContent.GetInstance<Fire>();
                else if (Ailments[element] == typeof(Cold))
                    buff = ModContent.GetInstance<Cold>();
                else if (Ailments[element] == typeof(Lightning))
                    buff = ModContent.GetInstance<Lightning>();
                else
                    buff = ModContent.GetInstance<Shadow>();
                player.AddBuff(buff.Type, bossFight ? 90 : 210);
                int intensity = eleDmg[element] * 3 / 2;
                AilmentIntensity[element] = Main.expertMode ? intensity * 2 / 3 : intensity;
                HasAilment[element] = true;
            }

            if (Main.rand.Next(player.statLifeMax2 + player.statDefense) < damage * 3)
                player.AddBuff(ModContent.BuffType<Physical>(), 15 + Math.Min(30, damage * 30 / player.statLifeMax2));
            if (HasAilment[Element.Lightning])
                damage += 1 + AilmentIntensity[Element.Lightning];
        }

        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            //if (kRPG2.Overhaul != null)
            //    return;
            for (int layerId = 0; layerId < layers.Count; layerId += 1)
                if (layers[layerId].Name.Contains("Held"))
                    layers.Insert(layerId + 2, new PlayerLayer(Constants.ModName, "ProceduralItem", drawInfo =>
                    {
                        if (player.itemAnimation <= 0)
                            return;
                        if (player.HeldItem.type == mod.GetItem("ProceduralStaff").item.type)
                        {
                            if (Main.gameMenu) return;

                            ProceduralStaff staff = (ProceduralStaff)player.HeldItem.modItem;

                            Vector2 pos = player.Center - Main.screenPosition;
                            staff.DrawHeld(drawInfo, Lighting.GetColor((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f)),
                                player.itemRotation + (float)Constants.Tau * player.direction / 8, staff.item.scale, pos);
                        }
                        else if (player.HeldItem.type == mod.GetItem("ProceduralSword").item.type)
                        {
                            if (Main.gameMenu) return;

                            ProceduralSword sword = (ProceduralSword)player.HeldItem.modItem;

                            if (sword.Spear) return;

                            Vector2 pos = player.Center - Main.screenPosition;
                            sword.DrawHeld(drawInfo, Lighting.GetColor((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f)),
                                player.itemRotation + (float)Constants.Tau, sword.item.scale, pos);
                        }
                    }));
        }

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            Dictionary<Element, int> dict = new Dictionary<Element, int>();
            foreach (Element element in Enum.GetValues(typeof(Element)))
                dict[element] = npc.GetGlobalNPC<kNPC>().ElementalDamage[element];
            ModifyDamageTakenFromNpc(ref damage, ref crit, dict);
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            Dictionary<Element, int> dict = new Dictionary<Element, int>();
            foreach (Element element in Enum.GetValues(typeof(Element)))
                dict[element] = proj.GetGlobalProjectile<kProjectile>().GetIndividualElements(proj, player)[element];
            ModifyDamageTakenFromNpc(ref damage, ref crit, dict);
        }

        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            ModifyDamage(ref damage, ref crit, target, item);
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockBack, ref bool crit, ref int hitDirection)
        {
            if (proj.modProjectile is ProceduralSpellProj)
                ModifyDamage(ref damage, ref crit, target, null, proj);
            else
                ModifyDamage(ref damage, ref crit, target, null, proj);
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockBack, bool crit)
        {
            LeechLife(item, damage);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockBack, bool crit)
        {
            Item item = player.inventory[player.selectedItem];
            LeechLife(item, damage);
            if (item.type == ModContent.ItemType<ProceduralStaff>())
            {
                ProceduralStaff staff = (ProceduralStaff)item.modItem;
                bool proceed = false;
                if (proj.type == item.shoot)
                    proceed = true;
                else if (proj.type == ModContent.ProjectileType<ProceduralSpellProj>())
                    proceed = ((ProceduralSpellProj)proj.modProjectile).Source == null;
                if (proceed)
                    staff.Ornament?.OnHit?.Invoke(player, target, item, damage, crit);
            }
            else if (proj.type == ModContent.ProjectileType<ProceduralSpear>() && item.type == ModContent.ItemType<ProceduralSword>())
            {
                ProceduralSword spear = (ProceduralSword)item.modItem;
                spear.Accent?.OnHit?.Invoke(player, target, spear, damage, crit);
            }
        }

        public void OpenInventoryPage(int page)
        {
            for (int inventorySlot = 0; inventorySlot < 40; inventorySlot += 1)
                player.inventory[inventorySlot + 10] = Inventories[page][inventorySlot];

            ActiveInvPage = page;

            StatPage = false;

            Recipe.FindRecipes();

            for (int i = 0; i < 50; i += 1)

                if (player.inventory[i].type == ItemID.SilverCoin || player.inventory[i].type == ItemID.CopperCoin || player.inventory[i].type == ItemID.GoldCoin || player.inventory[i].type == ItemID.PlatinumCoin)
                    player.DoCoins(i);
        }

        public override void PlayerConnect(Player playerObj)
        {
            SyncLevelPacket.Write(playerObj.whoAmI,Level,true);
        }

        public override void PostItemCheck()
        {
            if (Main.netMode == Constants.NetModes.Client)
                if (player.whoAmI != Main.myPlayer)
                    return;

            try
            {
                Item item = player.inventory[player.selectedItem];
                if (item.type != ModContent.ItemType<ProceduralStaff>() || item.shoot > 0)
                    return;
                player.releaseUseItem = true;
                if (player.itemAnimation == 1 && item.stack > 0)
                {
                    if (player.whoAmI != Main.myPlayer && player.controlUseItem)
                    {
                        player.itemAnimation = (int)(item.useAnimation / PlayerHooks.TotalMeleeSpeedMultiplier(player, item));
                        player.itemAnimationMax = player.itemAnimation;
                        player.reuseDelay = (int)(item.reuseDelay / PlayerHooks.TotalUseTimeMultiplier(player, item));
                        if (item.UseSound != null)
                            Main.PlaySound(item.UseSound, player.Center);
                    }
                    else
                    {
                        player.itemAnimation = 0;
                    }
                }

                if (player.itemTime < 2)
                {
                    Vector2 pos = player.RotatedRelativePoint(player.MountedCenter);
                    Vector2 relativeMousePos = Main.MouseWorld - pos;
                    ItemRotation = Math.Atan2(relativeMousePos.Y * player.direction, relativeMousePos.X * player.direction) - player.fullRotation;
                    NetMessage.SendData((int)PacketTypes.PlayerUpdate, -1, -1, null, player.whoAmI);
                    NetMessage.SendData((int)PacketTypes.PlayerAnimation, -1, -1, null, player.whoAmI);
                }

                float scaleFactor = 6f;
                if (player.itemAnimation > 0)
                    player.itemRotation = (float)ItemRotation;
                player.itemLocation = player.MountedCenter;
                player.itemLocation += player.itemRotation.ToRotationVector2() * scaleFactor * player.direction;
            }
            catch (SystemException e)
            {
                ModLoader.GetMod(Constants.ModName).Logger.InfoFormat(e.ToString());
            }
        }

        public override void PostUpdate()
        {
            switch (Main.netMode)
            {
                case 2:
                case 1 when Main.myPlayer != player.whoAmI:
                    return;
            }

            Item item = player.inventory[player.selectedItem];
            if (item.damage > 0 && (item.melee || !item.noMelee || item.modItem is ProceduralSword))
                LastSelectedWeapon = item;

            switch (item.modItem)
            {
                case ProceduralSword s:
                    {
                        //if (Main.itemTexture[item.type] != s.LocalTexture)
                        Main.itemTexture[item.type] = s.LocalTexture;
                        break;
                    }
                case ProceduralStaff st:
                    {
                        //if (Main.itemTexture[ModContent.ItemType<ProceduralStaff>()] != st.LocalTexture)
                        Main.itemTexture[ModContent.ItemType<ProceduralStaff>()] = st.LocalTexture;
                        break;
                    }
            }

            //for (int i = 0; i < 40; i += 1)
            //    inventories[activeInvPage][i] = playerObj.inventory[i + 10];

            //API.FindRecipes();
        }

        public override void PostUpdateEquips()
        {
            if (!Initialized)
            {
                InitializeGui();
                Initialized = true;
            }

            UpdateStats();
            if (LifeRegen > 0 && !player.bleed && !player.onFire && !player.poisoned && !player.onFire2 && !player.venom && !player.onFrostBurn)
                RegenTimer += 1f;
            if (RegenTimer > 60f / LifeRegen)
            {
                player.statLife = Math.Min(player.statLife + (int)(RegenTimer / (60f / LifeRegen)), player.statLifeMax2);
                RegenTimer = RegenTimer % (60f / LifeRegen);
            }

            if (LifeDegen > 0) DegenTimer += 1f;
            if (DegenTimer >= 20f && HasAilment[Element.Fire])
            {
                int amount = (int)Math.Round(LifeDegen / 3, 1);
                player.statLife = player.statLife - amount;
                CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(255, 95, 31),
                    amount);
                DegenTimer = 0;
                if (player.statLife <= 0) player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " burned to death."), amount, 0);
            }

            ManaRegenTimer += 1f;

            if (Main.chatRelease && !Main.drawingPlayerChat && !Main.editSign && !Main.editChest && Main.netMode != Constants.NetModes.Server)
                for (int abilityIndex = 0; abilityIndex < Abilities.Length; abilityIndex += 1)
                    if (Abilities[abilityIndex].CompleteSkill())
                    {
                        bool useable = true;
                        foreach (Item item in Abilities[abilityIndex].Glyphs)
                        {
                            Glyph glyph = (Glyph)item.modItem;
                            if (!glyph.CanUse()) useable = false;
                        }

                        if (!Main.keyState.IsKeyDown(Abilities[abilityIndex].Key) || !Main.keyState.IsKeyUp(Keys.LeftShift) || Abilities[abilityIndex].Remaining != 0 || !useable ||
                            player.statMana < Abilities[abilityIndex].ManaCost(this))
                            continue;
                        if (Main.netMode != Constants.NetModes.Server)
                            Abilities[abilityIndex].UseAbility(player, Main.MouseWorld);
                        player.statMana -= Abilities[abilityIndex].ManaCost(this);
                    }

            for (int spellEffectIndex = 0; spellEffectIndex < SpellEffects.Count; spellEffectIndex += 1)
                SpellEffects[spellEffectIndex].Update(this);

            if (Main.mapStyle == 0 && kConfig.ConfigLocal.ClientSide.ArpgMiniMap) Main.mapStyle += 1;
        }

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound,
            ref bool genGore, ref PlayerDeathReason damageSource)
        {
            bool enemyCrit = Main.rand.Next(5) == 0 && Main.netMode == Constants.NetModes.SinglePlayer;
            int max = 80;
            int diff = 52;

            if (TotalStats(PlayerStats.Quickness) > 0 && !Rituals[Ritual.StoneAspect])
            {
                if (damage < (Level + 10) * 5)
                {
                    EvasionCounter += 100 - max + max * diff / (diff + Evasion);
                    if (EvasionCounter >= 100)
                    {
                        EvasionCounter -= 100;
                        if (enemyCrit)
                        {
                            CritCounter += 100 - max + max * diff / (diff + Evasion);
                            if (CritCounter >= 100)
                                CritCounter -= 100;
                            else
                                enemyCrit = false;

                            if (enemyCrit) damage = (int)(damage * 1.65);
                        }
                    }

                    else
                    {
                        player.NinjaDodge(40);
                        return false;
                    }
                }
                else
                {
                    max = 90;
                    diff = 38;
                    BigHitCounter += 100 - max + max * diff / (diff + Evasion);
                    if (BigHitCounter >= 100)
                    {
                        BigHitCounter -= 100;
                        if (enemyCrit)
                        {
                            BigCritCounter += 100 - max + max * diff / (diff + Evasion + TotalStats(PlayerStats.Wits) * 5);
                            if (BigCritCounter >= 100)
                                BigCritCounter -= 100;
                            else
                                enemyCrit = false;

                            if (enemyCrit) damage = (int)(damage * 1.3);
                        }
                    }

                    else
                    {
                        player.NinjaDodge(40 + TotalStats(PlayerStats.Wits) * 5);
                        return false;
                    }
                }
            }

            if (Rituals[Ritual.MindFortress])
            {
                int i = (int)Math.Round(damage * 0.25);
                if (Mana > i)
                {
                    damage -= i;
                    Mana -= i;
                }
                else
                {
                    damage -= Mana;
                    Mana = 0;
                }
            }

            return true;
        }

        public override void PreUpdate()
        {
            if (Main.chatRelease && !Main.drawingPlayerChat && !Main.editSign && !Main.editChest && Main.netMode != Constants.NetModes.Server)
            {
                if (PlayerInput.Triggers.Current.QuickHeal)
                    if (!PlayerInput.Triggers.Old.QuickHeal)
                    {
                        player.ApiQuickHeal();
                        PlayerInput.Triggers.Old.QuickHeal = true;
                    }

                if (PlayerInput.Triggers.Current.QuickMana)
                    if (!PlayerInput.Triggers.Old.QuickMana)
                    {
                        player.ApiQuickMana();
                        PlayerInput.Triggers.Old.QuickMana = true;
                    }

                if (PlayerInput.Triggers.Current.QuickBuff)
                    if (!PlayerInput.Triggers.Old.QuickBuff)
                    {
                        player.ApiQuickBuff();
                        PlayerInput.Triggers.Old.QuickBuff = true;
                    }
            }

            //int selectedBinding3 = playerObj.QuicksRadial.SelectedBinding;
            player.QuicksRadial.Update();

            if (player.QuicksRadial.SelectedBinding == -1 || !PlayerInput.Triggers.JustReleased.RadialQuickbar ||
                PlayerInput.MiscSettingsTEMP.HotbarRadialShouldBeUsed)
                return;

            switch (player.QuicksRadial.SelectedBinding)
            {
                case 0:
                    player.ApiQuickHeal();
                    break;
                case 1:
                    player.ApiQuickBuff();
                    break;
                case 2:
                    player.ApiQuickMana();
                    break;
            }

            PlayerInput.Triggers.JustReleased.RadialQuickbar = false;
        }

        public override void PreUpdateBuffs()
        {
        }

        public override void ResetEffects()
        {
            foreach (PlayerStats stat in Enum.GetValues(typeof(PlayerStats)))
                TempStats[stat] = 0;
            Evasion = 2;
            Accuracy = 0;
            BonusLife = 0;
            BonusMana = 0;
            LifeRegen = 1;
            LifeDegen = 0;
            ManaRegen = 0;
            CanHealMana = true;

            CritBoost = 0;
            CritMultiplier = 0f;
            LifeLeech = 0f;
            AllResists = 0;

            if (LeechCooldown > 0) LeechCooldown--;

            foreach (Element element in Enum.GetValues(typeof(Element)))
            {
                ElementalResists[element] = 0;
                if (!HasAilment[element]) AilmentIntensity[element] = 0;
                HasAilment[element] = false;
            }

            if (Math.Abs(Main.time % 300) < .01)
                SyncStatsPacket.Write(player.whoAmI,Level, BaseStats[PlayerStats.Resilience], BaseStats[PlayerStats.Quickness], BaseStats[PlayerStats.Potency], BaseStats[PlayerStats.Wits]);

        }

        public override void SetupStartInventory(IList<Item> items, bool mediumCoreDeath)
        {
            Random rand = new Random();
            switch (rand.Next(8))
            {
                default:
                    items[0].SetDefaults(rand.Next(2) == 0 ? ItemID.TinBroadsword : ItemID.CopperBroadsword, true);
                    break;
                case 1:
                    items[0].SetDefaults(ItemID.Spear, true);
                    break;
                case 2:
                    items[0].SetDefaults(ItemID.WoodenBoomerang, true);
                    break;
                case 3:
                    items[0].SetDefaults(rand.Next(2) == 0 ? ItemID.TopazStaff : ItemID.AmethystStaff, true);
                    break;
                case 4:
                    items[0].SetDefaults(rand.Next(2) == 0 ? ItemID.TinBow : ItemID.CopperBow, true);
                    Item arrows = new Item();
                    arrows.SetDefaults(rand.Next(2) == 0 ? ItemID.FlamingArrow : ItemID.WoodenArrow, true);
                    arrows.stack = rand.Next(2) == 0 ? 150 : 200;
                    items.Add(arrows);
                    break;
                case 5:
                    items[0].SetDefaults(ItemID.Shuriken, true);
                    items[0].stack = rand.Next(2) == 0 ? 150 : 100;
                    Item knives = new Item();
                    knives.SetDefaults(rand.Next(2) == 0 ? ItemID.PoisonedKnife : ItemID.ThrowingKnife, true);
                    knives.stack = 50;
                    items.Add(knives);
                    break;
                case 6:
                    items[0].SetDefaults(ItemID.WoodYoyo, true);
                    break;
                case 7:
                    items[0].SetDefaults(ItemID.ChainKnife, true);
                    break;
            }

            items[1].SetDefaults(rand.Next(3) == 0 ? ItemID.TinPickaxe : rand.Next(2) == 0 ? ItemID.CactusPickaxe : ItemID.CopperPickaxe, true);
            items[1].GetGlobalItem<kItem>().Initialize(items[1]);
            items[2].SetDefaults(rand.Next(2) == 0 ? ItemID.TinAxe : ItemID.CopperAxe);
            items[2].GetGlobalItem<kItem>().Initialize(items[2]);

            Item star = new Item();
            star.SetDefaults(ModContent.ItemType<Star_Blue>(), true);
            Item cross = new Item();
            switch (rand.Next(4))
            {
                default:
                    cross.SetDefaults(ModContent.ItemType<Cross_Red>(), true);
                    break;
                case 1:
                    cross.SetDefaults(ModContent.ItemType<Cross_Orange>(), true);
                    break;
                case 2:
                    cross.SetDefaults(ModContent.ItemType<Cross_Yellow>(), true);
                    break;
                case 3:
                    cross.SetDefaults(ModContent.ItemType<Cross_Green>(), true);
                    break;
            }

            ((Glyph)cross.modItem).Randomize();
            Item moon = new Item();
            switch (rand.Next(5))
            {
                default:
                    moon.SetDefaults(ModContent.ItemType<Moon_Yellow>(), true);
                    break;
                case 1:
                    moon.SetDefaults(ModContent.ItemType<Moon_Green>(), true);
                    break;
                case 2:
                    moon.SetDefaults(ModContent.ItemType<Moon_Blue>(), true);
                    break;
                case 3:
                    moon.SetDefaults(ModContent.ItemType<Moon_Violet>(), true);
                    break;
                case 4:
                    moon.SetDefaults(ModContent.ItemType<Moon_Purple>(), true);
                    break;
            }

            ((Glyph)moon.modItem).Randomize();
            items.Add(star);
            items.Add(cross);
            items.Add(moon);
        }

        public int TotalStats(PlayerStats playerStats)
        {
            if (Rituals[Ritual.DemonPact] && playerStats == PlayerStats.Potency)
                return BaseStats[playerStats] + TempStats[playerStats] + BaseStats[PlayerStats.Resilience];

            if (Rituals[Ritual.DemonPact] && playerStats == PlayerStats.Resilience)
                return TempStats[playerStats];

            return BaseStats[playerStats] + TempStats[playerStats];
        }

        public bool UnspentPoints()
        {
            return PointsAllocated < Level - 1;
        }

        public void UpdateStats()
        {
            float lifeMultiplier = 1f + (player.statLifeMax - 100f) / 400f;
            int addedLife = player.statLifeMax2 - player.statLifeMax;
            player.statLifeMax2 += 115 + TotalStats(PlayerStats.Resilience) * 10 + Level * 5 + BonusLife - player.statLifeMax;
            player.statLifeMax2 = (int)Math.Round(player.statLifeMax2 * lifeMultiplier) + addedLife;
            float manaMultiplier = 1f + (player.statManaMax - 20f) / 200f * 1.5f;
            int addedMana = player.statManaMax2 - player.statManaMax;
            player.statManaMax2 += 19 + Level + BonusMana + TotalStats(PlayerStats.Wits) * 3 - player.statManaMax;
            player.statManaMax2 = (int)Math.Round(player.statManaMax2 * manaMultiplier) + addedMana;
            player.statDefense += TotalStats(PlayerStats.Resilience);
            AllResists += TotalStats(PlayerStats.Wits);
            Evasion += TotalStats(PlayerStats.Quickness);
            Accuracy += TotalStats(PlayerStats.Wits);
            if (Rituals[Ritual.StoneAspect]) player.statDefense += TotalStats(PlayerStats.Quickness);
            LifeRegen += TotalStats(PlayerStats.Resilience) * 0.3f + TotalStats(PlayerStats.Wits) * 0.2f;
            if (HasAilment[Element.Fire])
                LifeDegen = AilmentIntensity[Element.Fire] / 2f;
            ManaRegen = player.statManaMax2 * 0.06f + TotalStats(PlayerStats.Wits) * 0.6f;

            if (Main.netMode != Constants.NetModes.Server && Main.myPlayer == player.whoAmI)
            {
                if (Mana < 0) Mana = 0;
                if (player.statMana < 0) player.statMana = 0;
                if (player.statMana < Mana)
                    Mana = player.statMana;
                if (Rituals[Ritual.ElanVital] && Mana < player.statManaMax2)
                {
                    if (player.statLife > player.statLifeMax2 * 0.4 + player.statManaMax2 - Mana)
                    {
                        player.statLife -= player.statManaMax2 - Mana;
                        Mana = player.statManaMax2;
                    }
                    else if (player.statLife > player.statLifeMax2 * 0.4)
                    {
                        Mana += (int)(player.statLife - player.statLifeMax2 * 0.4);
                        player.statLife = (int)(player.statLifeMax2 * 0.4);
                    }
                }

                if (player.statMana == player.statManaMax2 && Mana == player.statMana - 1)
                    Mana = player.statMana;
                else player.statMana = Mana;
                if (ManaRegenTimer > 60f / ManaRegen)
                {
                    Mana = Math.Min(Mana + (int)(ManaRegenTimer / (60f / ManaRegen)), player.statManaMax2);
                    ManaRegenTimer = ManaRegenTimer % (60f / ManaRegen);
                }

                player.statMana = Mana;
            }

            CritMultiplier += TotalStats(PlayerStats.Potency) * 0.04f;
            LifeLeech += TotalStats(PlayerStats.Potency) * 0.002f;
            LifeLeech += Math.Min(0.006f, TotalStats(PlayerStats.Potency) * 0.002f);

            player.meleeDamage *= DamageMultiplierPercent;
            player.rangedDamage *= DamageMultiplierPercent;
            player.magicDamage *= DamageMultiplierPercent;
            player.minionDamage *= DamageMultiplierPercent;
            player.thrownDamage *= DamageMultiplierPercent;

            player.moveSpeed *= 1f + Math.Min(1.2f, TotalStats(PlayerStats.Quickness) * 0.02f + Math.Min(Level * 0.005f, 0.5f));
            player.meleeSpeed *= 1f + TotalStats(PlayerStats.Quickness) * 0.01f;
            player.jumpSpeedBoost += Math.Min(5f, TotalStats(PlayerStats.Quickness) * 0.2f + Math.Min(Level * 0.05f, 2f));

            CritBoost += Math.Min(TotalStats(PlayerStats.Quickness), Math.Max(4, TotalStats(PlayerStats.Quickness) / 2 + 2));
            player.magicCrit += CritBoost;
            player.meleeCrit += CritBoost;
            player.rangedCrit += CritBoost;
            player.thrownCrit += CritBoost;
        }

        #region Saving and loading

        public override void Initialize()
        {
            BaseStats = new Dictionary<PlayerStats, int>();
            TempStats = new Dictionary<PlayerStats, int>();

            Permanence = 0;
            Transcendence = 0;

            Level = 1;
            Experience = 0;

            foreach (PlayerStats stat in Enum.GetValues(typeof(PlayerStats)))
            {
                BaseStats[stat] = 0;
                TempStats[stat] = 0;
            }

            Inventories = new Item[3][];
            for (int inventoryPageId = 0; inventoryPageId < Inventories.Length; inventoryPageId += 1)
            {
                Inventories[inventoryPageId] = new Item[40];
                for (int inventorySlotId = 0; inventorySlotId < Inventories[inventoryPageId].Length; inventorySlotId += 1)
                    Inventories[inventoryPageId][inventorySlotId] = new Item();
            }

            Rituals = new Dictionary<Ritual, bool>();
            foreach (Ritual rite in Enum.GetValues(typeof(Ritual)))
                Rituals[rite] = false;

            for (int abilityIndexId = 0; abilityIndexId < Abilities.Length; abilityIndexId += 1)
            {
                Abilities[abilityIndexId] = new ProceduralSpell(mod);
                for (int j = 0; j < Abilities[abilityIndexId].Glyphs.Length; j += 1)
                    Abilities[abilityIndexId].Glyphs[j] = new Item();
            }

            Abilities[0].Key = Keys.Z;
            Abilities[1].Key = Keys.X;
            Abilities[2].Key = Keys.C;
            Abilities[3].Key = Keys.V;
        }

        public void InitializeGui()
        {
            if (Main.netMode == Constants.NetModes.Server)
                return;
            BaseGui.GuiElements.Clear();
            SpellCraftingGui = new SpellCraftingGui();
            AnvilGui = new AnvilGui(this);
            LevelGui = new LevelGui(this, mod);
            StatusBar = new StatusBar(this) { GuiActive = true };
            InventoryGui = new InventoryGui(this);
            AbilitiesGui = new AbilitiesGui { GuiActive = true };
        }

        public void CloseGuIs()
        {
            AnvilGui.CloseGui();
            LevelGui.CloseGui();
            SpellCraftingGui.CloseGui();
        }

        public override void OnEnterWorld(Player playerObj)
        {
            //kRPG.PlayerEnteredWorld = true;
            InitializeGui();

            if (playerObj.whoAmI == Main.myPlayer)
                kRPG.CheckForUpdates();
        }

        public override TagCompound Save()
        {
            TagCompound tagCompound = new TagCompound
            {
                {"level", Level},
                {"Experience", Experience},
                {"baseRESILIENCE", BaseStats[PlayerStats.Resilience]},
                {"baseQUICKNESS", BaseStats[PlayerStats.Quickness]},
                {"basePOTENCY", BaseStats[PlayerStats.Potency]},
                {"baseWITS", BaseStats[PlayerStats.Wits]},
                {"RITUAL_DEMON_PACT", Rituals[Ritual.DemonPact]},
                {"RITUAL_WARRIOR_OATH", Rituals[Ritual.WarriorOath]},
                {"RITUAL_ELAN_VITAL", Rituals[Ritual.ElanVital]},
                {"RITUAL_STONE_ASPECT", Rituals[Ritual.StoneAspect]},
                {"RITUAL_ELDRITCH_FURY", Rituals[Ritual.EldritchFury]},
                {"RITUAL_MIND_FORTRESS", Rituals[Ritual.MindFortress]},
                {"RITUAL_BLOOD_DRINKING", Rituals[Ritual.BloodDrinking]},
                {"life", player.statLife},
                {"permanence", Permanence},
                {"transcendence", Transcendence}
            };

            try
            {
                for (int abilityIndexId = 0; abilityIndexId < Abilities.Length; abilityIndexId += 1)
                {
                    if (Abilities[abilityIndexId] == null) 
                        return tagCompound;

                    tagCompound.Add("abilities" + abilityIndexId + "_key", Abilities[abilityIndexId].Key.ToString());

                    for (int abilityGlyphIndexId = 0; abilityGlyphIndexId < Abilities[abilityIndexId].Glyphs.Length; abilityGlyphIndexId += 1)

                        if (Abilities[abilityIndexId].Glyphs[abilityGlyphIndexId] != null)
                            tagCompound.Add("ability" + abilityIndexId + abilityGlyphIndexId, ItemIO.Save(Abilities[abilityIndexId].Glyphs[abilityGlyphIndexId]));
                }
            }
            catch (SystemException e)
            {
                ModLoader.GetMod(Constants.ModName).Logger.InfoFormat("@Abilities :: " + e);
            }

            try
            {
                for (int inventoryPageId = 0; inventoryPageId < Inventories.Length; inventoryPageId += 1)
                    for (int inventorySlotId = 0; inventorySlotId < Inventories[inventoryPageId].Length; inventorySlotId += 1)
                        tagCompound.Add("item" + inventoryPageId + inventorySlotId, ItemIO.Save(Inventories[inventoryPageId][inventorySlotId]));
            }
            catch (SystemException e)
            {
                ModLoader.GetMod(Constants.ModName).Logger.InfoFormat("@Inventories :: " + e);
            }

            return tagCompound;
        }

        public override void Load(TagCompound tag)
        {
            try
            {
                Level = tag.GetInt("level");
                Experience = tag.GetInt("Experience");
            }
            catch (SystemException e)
            {
                ModLoader.GetMod(Constants.ModName).Logger.InfoFormat("@Level&XP :: " + e);
            }

            try
            {
                foreach (PlayerStats stat in Enum.GetValues(typeof(PlayerStats)))
                    BaseStats[stat] = tag.GetInt("base" + stat.ToString().ToUpper());
                foreach (Ritual rite in Enum.GetValues(typeof(Ritual)))
                    Rituals[rite] = tag.GetBool("RITUAL_" + rite.ToString().ToUpper());
            }
            catch (SystemException e)
            {
                ModLoader.GetMod(Constants.ModName).Logger.InfoFormat("@Stats&Rituals :: " + e);
            }

            try
            {
                Abilities = new ProceduralSpell[4];
                for (int abilityIndexId = 0; abilityIndexId < Abilities.Length; abilityIndexId += 1)
                {
                    Abilities[abilityIndexId] = new ProceduralSpell(mod)
                    {
                        Key = (Keys)Enum.Parse(typeof(Keys), tag.GetString("abilities" + abilityIndexId + "_key"))
                    };
                    for (int abilityGlyphIndexId = 0; abilityGlyphIndexId < Abilities[abilityIndexId].Glyphs.Length; abilityGlyphIndexId += 1)
                        if (tag.ContainsKey("ability" + abilityIndexId + abilityGlyphIndexId))
                            Abilities[abilityIndexId].Glyphs[abilityGlyphIndexId] = ItemIO.Load(tag.GetCompound("ability" + abilityIndexId + abilityGlyphIndexId));
                }
            }
            catch (SystemException e)
            {
                ModLoader.GetMod(Constants.ModName).Logger.InfoFormat("@Abilities :: " + e);
            }

            try
            {
                for (int inventoryPageId = 0; inventoryPageId < Inventories.Length; inventoryPageId += 1)
                    for (int inventorySlotId = 0; inventorySlotId < Inventories[inventoryPageId].Length; inventorySlotId += 1)
                        Inventories[inventoryPageId][inventorySlotId] = ItemIO.Load(tag.GetCompound("item" + inventoryPageId + inventorySlotId));
                OpenInventoryPage(0);
            }
            catch (SystemException e)
            {
                ModLoader.GetMod(Constants.ModName).Logger.InfoFormat("@Inventory :: " + e);
            }

            try
            {
                player.statLife = tag.GetInt("life");
                Permanence = tag.GetInt("permanence");
                Transcendence = tag.GetInt("transcendence");

                Mana = 10 + (Level - 1) * 3;
            }
            catch (SystemException e)
            {
                ModLoader.GetMod(Constants.ModName).Logger.InfoFormat("@Miscellaneous :: " + e);
            }
        }

        #endregion
    }
}