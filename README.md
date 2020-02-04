# kRPG
Kalciphoz's RPG Mod

This work is derived from https://forums.terraria.org/index.php?threads/kalciphozs-rpg-mod.63348/

This mod aims to overhaul Terraria by adding RPG elements, and includes a leveling system, an item upgrade system, an elemental damage system, procedurally generated weapons, and much more. For more detailed information, see the wiki.

Features
The mod features a leveling system with XP and level display, and rewards you with stat points when leveling up. In order to compensate for gradually increasing player strength, enemies have seen a drastic power increase, especially later in the game.

To further emphasize these changes, I have added prefixes to armour and overhauled accessory prefixes to provide these new stats, including accuracy, leech, critical strike damage multiplier, evasion, and more.

![alt text](https://i.imgur.com/YbDNAU1.png "Logo Title Text 1")

Enemies drop weapons, many of which are procedurally generated. To compensate for the consumption of inventory space, you now have two additional pages of inventory, giving you 80 extra slots. Weapons and enemies now deal elemental damage, which has a chance to inflict elemental status ailments - powerful combat-related debuffs.

I have also added an item system to upgrade weapons. It primarily serves as an item sink, since in a modded playthrough, it is very common for the player to have more weapons than needed. The upgrade system gambles your weapon at a risk of destruction and at a cost. Rare upgrade crowns make this process less painful and protect weapons you hold dear.

Installation
The mod can be found in the Mod Browser of tModLoader. You can search for the mod or find it by its icon. Alternatively, you can download it here. Simply move it in the Mods directory at Documents/My Games/Terraria/ModLoader/Mods

![alt text](https://i.imgur.com/YoJx5BF.png "Logo Title Text 1")
![alt text](https://i.imgur.com/YvpbqwM.png "Logo Title Text 1")
![alt text](https://i.imgur.com/4J4uXcd.png "Logo Title Text 1")

Special thanks goes to raydeejay, Mirsario, jopojelly and bluemagic123 for helping me fix bugs during the mods development. I want to thank also Mirsario, Alena, HellPhoenix, Jofairden, NuovaPrime, Snorlaxxo, Hozlocos, Mop Guy, Randie Marsh, and Robbie for being my beta testers and for helping me stay motivated. Art and code was made by me, but I would like to credit the tModLoader dev team with making that possible in the first place.

If I have forgotten anybody from this list, I sincerely apologize. Please inform me and I will correct it asap.

[![YouTube Video]](https://youtu.be/QldqCHEaqCc)

Open Source
The mod is open source. The source code can be acquired by three means: Going to the github page here, or by using ModLoader to decompile it, or by using ILSpy. Any of my content may be used, but please give proper attribution (credits) and if using a larger part of my mod (such as a major mechanic), please refer people to the original. If you're unsure about anything, you may contact me through PM or in this thread.


Version 2.0
  -Upgraded code to be compatible with latest version of TMOD.

Version 1.2

Fixed in patch:
- The eyes in the stat allocation interface are now properly displayed on all resolutions
- The crown buttons in the anvil interface are now properly displayed on all resolutions
- Game would lag when many crafting recipes were available, eg. when near your chests
- Fixed a bug where certain scepters and glyph-modifiers would not work as intended
- Fixed crashes upon reaching lv 189, courtesy of Saaland Pfarr
- Items will no longer have prefixes and upgrade levels in the crafting menu
- Fixed bug where players were unable to switch inventory tab in multiplayer
- Fixed bug where players were unable to allocate stats in multiplayer
- Fixed bug where the level, mana, and life displays in the status bar would be wrong in multiplayers
- Fixed a frequent crash in multiplayer relating to projectiles
- Fixed a bug where NPCs would teleport in multiplayer. Another bug that causes them to ocasionally vanish persits.
- Procedural weapon textures are now properly unloaded
- Fixed bug where broken procedurally generated items would crash the game in multiplayer
- Fixed bug where crit multiplier would scale quadratically with ordinary damage multiplier (This is an extremely large nerf to Potency)
- Fixed the routine errors you'd see in your log file
- Fixed a bug where procedurally generated items would cause crashes and other errors in multiplayer
- Fixed a bug where the stat allocation interface could not be accessed through the inventory in multiplayer
- Fixed a bug where the spell creation interface would not show up in multiplayer
- Fixed a bug where having more than 2147 platinum would make you unable to upgrade your items

Tweaked in patch:
- Quickness now only provides half as much crit chance, except for the first few points
- Potency now provides less life leech and critical strike multiplier.
- Ground enemies now move faster, making them more dangerous.
- NPC damage scaling now has a cap to prevent ridiculous post-endgame damage numbers with mods like Calamity
Taken together, these changes severely curtail the survivability of offensive builds as well as their offence itself. This should encourage
players to allocate more points to Resilience, which was previously drastically underused
- Upgrading crowns now have much higher drop chance
- Item upgrades cost a bit more currency
- Improved handling of enemy projectile damage somewhat
- Hotbar now looks different, less buggy in MP and more in line with the general aesthetic of the mod

Added in patch:
- New hilt with autoswing added to the hardmode sword tier
- New explosion and smoke bomb animations, larger and higher resolution
- Enemies can now have prefixes
- Automatic update checks, courtesy of Mirsario
- NPCs that deal elemental damage now have visual indications

Compatibility:
- Compatibility with Overhaul mod fixed, courtesy of Saaland Pfarr
- Mod now has much better multiplayer performance, courtesy of Mirsario
- Mod is generally compatible with multiplayer, though there are still a few rough edges here and there.

Version 1.1

Fixed in patch:
- Ranged weapons would get corrupted by reforging and sometimes by upgrading
- Some players experienced crashes upon entering worlds
- UI now properly scales with resolution
- Levelup sound is now properly affected by sound volume
- Projectiles owned by the player would deal too little elemental damage
- Crit multiplier now increases damage by the intended amount
- Fixed issue that utterly demolished crit chance in the previous version
- Sources of increased life from other mods now work properly
- Items would not be assigned a prefix immediately with autopause enabled
- Players playing with autopause enabled would sometimes lose items
- Quick Heal, Quick Mana and Quick Buff now works across inventory tabs
- You can now use items across inventory pages when crafting

Mod compatibility:
- Mod is now compatible with WeaponOut
- Mod is now compatible with Magic Storage
- Mod is now compatible with Autotrash (thanks Jopojelly)
- Mod now sorta kinda works in multiplayer

Tweaked in patch:
- Sword hitbox is slightly more accurate
- Changed texture of Shortbow
- Enemies now deal less damage at the upper end
- Dodge effects are less extra
- Players now have short duration of immunity after their attacks are dodged.
- All sources of base life have been lowered dramatically
- Players now start with more life and mana
- Players now have a wider variety of possible starting weapons
- Multiple projectiles from the same ability cannot hit the same enemy in quick succession.
- Clicking the stat page button while the stat display is active will now reopen the previously closed inventory page
- Glyph modifier "Vanish" renamed to "Discord" and now works like the Rod of Discord, but also deals much more damage. Use with care.
- Glyph modifier "Thorny Chains" now much more useful.

Added in patch:
- Crowns now displayed in inventory
- Heart Crystals now scale life via a percentage bonus. Life fruits do the same, as do Mana Crystals.
- New glyph: Purple Moon Glyph, which aimlessly hurls projectiles skyward, very impractical but lots of fun



