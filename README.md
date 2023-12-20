# GarageDoorFix

A plugin which fixes a bug with Hygrodere behavior.

## Explanation

In the game's code, there are references to "taming" Hygroderes by playing a Boombox near it. This is reinforced by an explicit reference to this in its Terminal entry:

> they have great taste! cause I made a friend with one somehow,, and we think it was my music. 

In-game, being exposed to a Boombox's music speeds up the Hygrodere, and is supposed to trigger code which prevents the Hygrodere from dealing damage. However, this code has a typo which causes it to only work when the Hygrodere has been hit by a player (using a Shovel or Stun Grenade); this increases an "anger" timer, but when the anger timer has elapsed, the taming code now works as expected.

## Issues
Report any issues on the Lethal Company Modding Discord.

## Credits
- EliteMasterEric: Programming
- FoguDragon: Playtesting
- [SoftySapphie](https://twitter.com/SoftySapphie): Icon