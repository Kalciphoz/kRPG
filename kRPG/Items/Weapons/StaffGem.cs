using System;
using System.Collections.Generic;
using kRPG.Enums;
using kRPG.Items.Glyphs;
using kRPG.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.Items.Weapons
{
    public class StaffGem : StaffPart
    {
        public static StaffGem amber;
        public static StaffGem amberLarge;
        public static StaffGem amethyst;
        public static StaffGem amethystDark;
        public static StaffGem crystalDark;
        public static StaffGem crystalGreen;
        public static StaffGem diamond;
        public static StaffGem emerald;
        public static StaffGem fire;
        public static StaffGem fireOrb;

        public static Dictionary<int, StaffGem> gems;
        public static Dictionary<STAFFTHEME, List<StaffGem>> gemsByTheme;
        public static StaffGem ruby;
        public static StaffGem sapphire;
        public static StaffGem shattered;

        private static readonly Action<Player, Item, int, Cross, float> spellEffect =
            delegate(Player player, Item item, int projCount, Cross cross, float scale)
            {
                var mod = ModLoader.GetMod("kRPG");
                float spread = 0.030f - projCount * 0.002f;
                var unitVelocity = Main.MouseWorld - player.Center;
                unitVelocity.Normalize();
                var velocity = unitVelocity * item.shootSpeed;
                for (int i = 0; i < projCount; i += 1)
                {
                    var projectile =
                        Main.projectile[
                            Projectile.NewProjectile(player.Center, velocity.RotatedBy(API.Tau * (-spread / 2f + i * spread + spread / 2f)),
                                ModContent.ProjectileType<ProceduralSpellProj>(), item.damage, item.knockBack, player.whoAmI)];
                    var ps = (ProceduralSpellProj) projectile.modProjectile;
                    ps.origin = projectile.position;
                    if (cross.GetAiAction() != null)
                        ps.ai.Add(cross.GetAiAction());
                    if (cross.GetInitAction() != null)
                        ps.init.Add(cross.GetInitAction());
                    if (cross.GetImpactAction() != null)
                        ps.impact.Add(cross.GetImpactAction());
                    if (cross.GetKillAction() != null)
                        ps.kill.Add(cross.GetKillAction());
                    ps.caster = player;
                    ps.projectile.minion = false;
                    ps.Initialize();
                    if (projCount == 1) projectile.tileCollide = true;
                    projectile.scale *= scale;
                }
            };

        public bool back;
        public int critBonus;
        public float dpsModifier = 1f;

        public Dictionary<ELEMENT, float> eleDamage = new Dictionary<ELEMENT, float>
        {
            {ELEMENT.FIRE, 0f}, {ELEMENT.COLD, 0f}, {ELEMENT.LIGHTNING, 0f}, {ELEMENT.SHADOW, 0f}
        };

        public float knockBack;
        public float mana = 1f;
        public string name = "";

        public Action<Player, Item> projectile;
        public int shoot;
        public float speedModifier = 1f;

        public int type;

        public StaffGem(string texture, int origin_X, int origin_Y, string name, int shoot, bool back = false, float mana = 1f, float dpsModifier = 1f,
            float speedModifier = 1f, float knockBack = 0f, int critBonus = 0)
        {
            type = gems.Count + 1;
            if (Main.netMode != 2)
                this.texture = ModLoader.GetMod("kRPG").GetTexture("GFX/Items/Gemstones/" + texture);
            origin = new Vector2(origin_X, origin_Y);
            this.dpsModifier = dpsModifier;
            this.speedModifier = speedModifier;
            this.knockBack = knockBack;
            this.critBonus = critBonus;
            this.name = name;
            this.mana = mana;
            this.shoot = shoot;
            this.back = back;
            if (!gems.ContainsKey(type))
                gems.Add(type, this);
        }

        public static void Initialize()
        {
            gems = new Dictionary<int, StaffGem>();

            ruby = new StaffGem("Ruby", 0, 6, "Ruby Staff", ProjectileID.RubyBolt).SetEleDamage(new Dictionary<ELEMENT, float>
            {
                {ELEMENT.FIRE, 0.3f}, {ELEMENT.COLD, 0f}, {ELEMENT.LIGHTNING, 0f}, {ELEMENT.SHADOW, 0f}
            });
            diamond = new StaffGem("Diamond", 1, 5, "Diamond Staff", ProjectileID.DiamondBolt, false, 0.9f);
            emerald = new StaffGem("Emerald", 1, 4, "Emerald Staff", ProjectileID.EmeraldBolt, true);
            amber = new StaffGem("Amber", 1, 5, "Amber Staff", ProjectileID.AmberBolt);
            sapphire = new StaffGem("Sapphire", 2, 5, "Sapphire Staff", ProjectileID.SapphireBolt, false, 0.9f, 1.1f, 1.1f, 1f);
            amethyst = new StaffGem("BrightAmethyst", 2, 7, "Mana Scepter", ProjectileID.DiamondBolt);
            amethystDark = new StaffGem("DarkAmethyst", 2, 7, "Dungeon Scepter", ProjectileID.AmethystBolt, false, 1.1f, 1.2f, 1.1f).SetEleDamage(
                new Dictionary<ELEMENT, float> {{ELEMENT.FIRE, 0f}, {ELEMENT.COLD, 0f}, {ELEMENT.LIGHTNING, 0f}, {ELEMENT.SHADOW, 0.2f}});
            amberLarge = new StaffGem("LargeAmber", 2, 7, "Runic Scepter", ProjectileID.AmberBolt);
            crystalDark = new StaffGem("DarkCrystal", 2, 7, "Shadow Scepter", 0, false, 1.1f, 1.1f, 1.2f)
                .SetEleDamage(new Dictionary<ELEMENT, float> {{ELEMENT.FIRE, 0f}, {ELEMENT.COLD, 0f}, {ELEMENT.LIGHTNING, 0f}, {ELEMENT.SHADOW, 0.2f}})
                .SetProjectile(delegate(Player player, Item item) { spellEffect(player, item, 1, new Cross_Violet(), 1f); });
            crystalGreen = new StaffGem("GreenCrystal", 2, 7, "Crystal Scepter", ProjectileID.EmeraldBolt, false, 1f, 1f, 1.4f, 2f, 5);
            fire = new StaffGem("FireCrystal", 2, 7, "Flame Scepter", ProjectileID.BallofFire, false, 1f, 1.2f, 0.9f, 1f, 5).SetEleDamage(
                new Dictionary<ELEMENT, float> {{ELEMENT.FIRE, 0.5f}, {ELEMENT.COLD, 0f}, {ELEMENT.LIGHTNING, 0f}, {ELEMENT.SHADOW, 0f}});
            fireOrb = new StaffGem("FireOrb", 2, 7, "Immolation Scepter", ProjectileID.InfernoFriendlyBolt, false, 1.2f, 0.6f, 1.2f, 2f).SetEleDamage(
                new Dictionary<ELEMENT, float> {{ELEMENT.FIRE, 0.5f}, {ELEMENT.COLD, 0f}, {ELEMENT.LIGHTNING, 0f}, {ELEMENT.SHADOW, 0f}});
            shattered = new StaffGem("Shatter", 1, 8, "Lava Staff", 0, false, 1f, 0.65f)
                .SetEleDamage(new Dictionary<ELEMENT, float> {{ELEMENT.FIRE, 0.4f}, {ELEMENT.COLD, 0f}, {ELEMENT.LIGHTNING, 0f}, {ELEMENT.SHADOW, 0f}})
                .SetProjectile(delegate(Player player, Item item) { spellEffect(player, item, 3, new Cross_Red(), 0.8f); });

            gemsByTheme = new Dictionary<STAFFTHEME, List<StaffGem>>
            {
                {
                    STAFFTHEME.WOODEN, new List<StaffGem>
                    {
                        ruby,
                        diamond,
                        emerald,
                        amber,
                        sapphire
                    }
                },
                {
                    STAFFTHEME.DUNGEON, new List<StaffGem>
                    {
                        amethyst,
                        amethystDark,
                        amberLarge,
                        crystalDark,
                        crystalGreen
                    }
                },
                {STAFFTHEME.UNDERWORLD, new List<StaffGem> {fire, fireOrb, shattered, crystalDark}}
            };
        }

        public static StaffGem RandomGem(STAFFTHEME theme)
        {
            return gemsByTheme[theme].Random();
        }

        public StaffGem SetEleDamage(Dictionary<ELEMENT, float> eleDamage)
        {
            this.eleDamage = eleDamage;
            return this;
        }

        public StaffGem SetProjectile(Action<Player, Item> projectile)
        {
            this.projectile = projectile;
            return this;
        }

        public static void Unload()
        {
            foreach (var gem in gems.Values)
                gem.texture = null;
        }
    }
}