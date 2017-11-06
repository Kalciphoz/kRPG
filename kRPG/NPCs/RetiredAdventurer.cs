using kRPG.Items;
using kRPG.Items.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace kRPG.NPCs
{
    [AutoloadHead]
    public class RetiredAdventurer : ModNPC
    {
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

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            for (int i = 0; i < 255; i += 1)
            {
                Player player = Main.player[i];
                if (player.active)
                {
                    PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
                    if (character.level >= 20)
                        return true;
                }
            }
            return false;
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[nextSlot++].SetDefaults(mod.ItemType<ScintillatingBloodLacrima>());
            shop.item[nextSlot++].SetDefaults(mod.ItemType<EyeOnAStick>());
            shop.item[nextSlot++].SetDefaults(mod.ItemType<Scythe>());
            shop.item[nextSlot++].SetDefaults(mod.ItemType<Arbalest>());
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

        public override string GetChat()
        {
            int nurse = NPC.FindFirstNPC(NPCID.Nurse);
            int guide = NPC.FindFirstNPC(NPCID.Guide);
            if (nurse > 0 && Main.rand.Next(9) == 0)
                return "I'm so grateful to " + Main.npc[nurse].GivenName + " for removing that stinger from my knee.";
            else if (guide > 0 && Main.rand.Next(8) == 0)
            {
                if (Main.rand.Next(2) == 0)
                    return Main.npc[guide].GivenName + " is looking sharp today.";
                else
                    return "You know, when I started adventuring, I didn't have a " + Main.npc[guide].GivenName + " around.";
            }
            else if (npc.GivenName == "Brian Alvarez" && Main.rand.Next(7) == 0)
            {
                switch (Main.rand.Next(3))
                {
                    default:
                        return "You don't know what we do here, and neither do I.";
                    case 1:
                        return "Everything is free.";
                    case 2:
                        return "My son Emil tells me to move to a retirement home, but I feel like I'm not old enough for that.";
                }
            }
            else if (npc.GivenName == "Dylan Alvarez" && Main.rand.Next(7) == 0)
            {
                if (Main.rand.Next(2) == 0)
                    return "I once fought four ritual archmagicians at the same time. I won.";
                else
                    return "Oh, if you think you're the protagonist of this story, you're dead wrong.";
            }
            else if (npc.GivenName == "Grian" && Main.rand.Next(7) == 0)
            {
                if (Main.rand.Next(2) == 0)
                    return "This is a very nice place. Have you considered becoming an architect?";
                else
                    return "I've dabbled in architecture you know. I was quite skilled.";
            }
            else if (npc.GivenName == "Colonel Zabini" && Main.rand.Next(7) == 0)
            {
                if (Main.rand.Next(2) == 0)
                    return "You lot are hopeless without me.";
                else
                    return "It's just a game, really. And games are supposed to be fun. So how about if I just do whatever I feel like?";

            }
            else if (npc.GivenName == "General Raynor" && Main.rand.Next(7) == 0)
                return "Do me a favor, don't shoot me this time.";
            else if (npc.GivenName == "Red Cloud" && Main.rand.Next(7) == 0)
                return "I've done enough adventuring for a lifetime.";
            else if (npc.GivenName == "Arnold" && Main.rand.Next(7) == 0)
                return "Get into the choppa!";
            else if (npc.GivenName == "Dominus" && Main.rand.Next(7) == 0)
                return "This world is an illusion.";
            else if (npc.GivenName == "Avarius" && Main.rand.Next(7) == 0)
                return "I was a High Templar in Oriath. I was a god!";
            else if (npc.GivenName == "Dentarthurdent" && Main.rand.Next(7) == 0)
            {
                if (Main.rand.Next(2) == 0)
                    return "It's a tough world. If you wanna survive out here, you gotta know where your gel is.";
                else
                    return "I would never want to go anywhere without my grappling hook.";
            }
            else if (npc.GivenName == "Harry" && Main.rand.Next(7) == 0)
                return "I must not tell lies.";
            else if (npc.GivenName == "Voldy" && Main.rand.Next(7) == 0)
                return "Avada Kedav- Oh, it's you!";
            switch (Main.rand.Next(6))
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
            }
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Lang.inter[28].Value;
        }
        
        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                shop = true;
            }
        }
    }
}
