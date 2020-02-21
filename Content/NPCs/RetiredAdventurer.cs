﻿using kRPG.Content.Items.Armor;
using kRPG.Content.Items.Weapons.Melee;
using kRPG.Content.Items.Weapons.Ranged;
using kRPG.Content.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

// ReSharper disable StringLiteralTypo

namespace kRPG.Content.NPCs
{
    [AutoloadHead]
    public class RetiredAdventurer : ModNPC
    {
        public override bool CanTownNPCSpawn(int numTownNpcs, int money)
        {
            for (int i = 0; i < 255; i += 1)
            {
                Player player = Main.player[i];
                if (!player.active)
                    continue;
                PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
                if (character.Level >= 20)
                    return true;
            }

            return false;
        }

        public override string GetChat()
        {
            int nurse = NPC.FindFirstNPC(NPCID.Nurse);
            int guide = NPC.FindFirstNPC(NPCID.Guide);
            if (nurse > 0 && Main.rand.Next(10) == 0)
                return "I'm so grateful to " + Main.npc[nurse].GivenName + " for removing that stinger from my knee.";
            if (guide > 0 && Main.rand.Next(5) == 0)
            {
                if (Main.rand.Next(2) == 0)
                    return Main.npc[guide].GivenName + " is looking sharp today.";
                return "You know, when I started adventuring, I didn't have a " + Main.npc[guide].GivenName + " around.";
            }

            switch (npc.GivenName)
            {
                case "Brian Alvarez" when Main.rand.Next(4) == 0:
                    switch (Main.rand.Next(3))
                    {
                        default:
                            return "You don't know what we do here, and neither do I.";
                        case 1:
                            return "Everything is free.";
                        case 2:
                            return "My son Emil tells me to move to a retirement home, but I feel like I'm not old enough for that.";
                    }

                case "Dylan Alvarez" when Main.rand.Next(5) == 0:
                {
                    return Main.rand.Next(2) == 0
                        ? "I once fought four ritual archmagicians at the same time. I won."
                        : "Oh, if you think you're the protagonist of this story, you're dead wrong.";
                }
                case "Grian" when Main.rand.Next(5) == 0:
                {
                    return Main.rand.Next(2) == 0
                        ? "This is a very nice place. Have you considered becoming an architect?"
                        : "I've dabbled in architecture you know. I was quite skilled.";
                }
                case "Colonel Zabini" when Main.rand.Next(5) == 0:
                {
                    return Main.rand.Next(2) == 0
                        ? "You lot are hopeless without me."
                        : "It's just a game, really. And games are supposed to be fun. So how about if I just do whatever I feel like?";
                }
                case "General Raynor" when Main.rand.Next(9) == 0:
                    return "Do me a favor, don't shoot me this time.";
                case "Red Cloud" when Main.rand.Next(9) == 0:
                    return "I've done enough adventuring for a lifetime.";
                case "Arnold" when Main.rand.Next(9) == 0:
                    return "Get into the choppa!";
                case "Dominus" when Main.rand.Next(9) == 0:
                    return "This world is an illusion.";
                case "Avarius" when Main.rand.Next(9) == 0:
                    return "I was a High Templar in Oriath. I was a god!";
                case "Dentarthurdent" when Main.rand.Next(5) == 0:
                {
                    return Main.rand.Next(2) == 0
                        ? "It's a tough world. If you wanna survive out here, you gotta know where your cobweb is."
                        : "I would never want to go anywhere without my wonderful grappling hook.";
                }
                case "Harry" when Main.rand.Next(9) == 0:
                    return "I must not tell lies.";
                case "Voldy" when Main.rand.Next(9) == 0:
                    return "Avada Kedav- Oh, it's you!";
                default:
                    switch (Main.rand.Next(8))
                    {
                        default:
                            return "I used to be an adventurer like you, but then I took a stinger in the knee.";
                        case 1:
                            return "No lollygaggin'!";
                        case 2:
                            return "I've got a jar of dirt!";
                        case 3:
                            return "Do you like Sweet Rolls? I heard there's a rare ultra delicious kind known as an 'Elder Roll'.";
                        case 4:
                            return "I used to hunt a rare breed of unicorn and collect its tears. Want to buy some?";
                        case 5:
                            return "I have a few weapons left from my adventuring days. They're yours if you've got the coins!";
                        case 6:
                            return "Wait for the opportune moment";
                        case 7:
                            return "I'm not certain I can survive any more visits from old friends.";
                    }
            }
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
                shop = true;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
        }

        public override void SetDefaults()
        {
            npc.townNPC = true;
            npc.friendly = true;
            npc.width = 18;
            npc.height = 40;
            npc.aiStyle = 7;
            npc.damage = 10;
            npc.defense = 15;
            npc.lifeMax = 250;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 0.5f;
            animationType = NPCID.Guide;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Retired Adventurer");
            Main.npcFrameCount[npc.type] = 26;
            NPCID.Sets.ExtraFramesCount[npc.type] = NPCID.Sets.ExtraFramesCount[NPCID.Guide];
            NPCID.Sets.AttackFrameCount[npc.type] = NPCID.Sets.AttackFrameCount[NPCID.Guide];
            NPCID.Sets.DangerDetectRange[npc.type] = NPCID.Sets.DangerDetectRange[NPCID.Guide];
            NPCID.Sets.AttackType[npc.type] = NPCID.Sets.AttackType[NPCID.Guide];
            NPCID.Sets.AttackTime[npc.type] = NPCID.Sets.AttackTime[NPCID.Guide];
            NPCID.Sets.AttackAverageChance[npc.type] = NPCID.Sets.AttackAverageChance[NPCID.Guide];
            NPCID.Sets.HatOffsetY[npc.type] = NPCID.Sets.HatOffsetY[NPCID.Guide];
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ScintillatingBloodLacrima>(), true);
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<EyeOnAStick>(), true);
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Scythe>(), true);
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Arbalest>(), true);
        }

        public override string TownNPCName()
        {
            switch (WorldGen.genRand.Next(12))
            {
                default:
                    return "Brian Alvarez";
                case 1:
                    return "Dylan Alvarez";
                case 2:
                    return "Grian";
                case 3:
                    return "Colonel Zabini";
                case 4:
                    return "General Raynor";
                case 5:
                    return "Red Cloud";
                case 6:
                    return "Arnold";
                case 7:
                    return "Dominus";
                case 8:
                    return "Avarius";
                case 9:
                    return "Dentarthurdent";
                case 10:
                    return "Harry";
                case 11:
                    return "Voldy";
            }
        }
    }
}