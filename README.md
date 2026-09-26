# Piñata Poppers

You shoot piñatas into a pot, and two of the same size merge into a bigger one. Get a high score before the pot overflows. Inspired by Suika Game.

- Play: [itch.io](https://unitedfailures.itch.io/pinata-poppers)
- Made: October 2023 to July 2024. It started as a Con Latinidad game jam entry.
- Team: [@mfclinton](https://github.com/mfclinton) (programming), [CelestialJoy](https://celestialjoy.itch.io) (art), [@MrAozora](https://github.com/MrAozora) (music), [Marekuma](https://marekuma.itch.io) (QA)
- Engine: Unity, C#

This is an export of a private repo with only the code we wrote. Art, audio, the Unity project files, and third party plugins aren't included. The history is squashed into one commit.

## What I built

- The merging. When two piñatas of the same size touch, the faster one slides into the slower one and the next size up spawns in its place. Both get locked while they merge so a third can't join in, and piñatas that end up overlapping get pushed apart.
- The paper frills on every piñata swing on a spring. They hang with gravity when the piñata sits still, even as it rolls, and swing with its movement when it's flying. I also made an editor tool that scatters the frill sprites over a piñata with a little random color.
- The effects in Shader Graph. Merging piñatas flash toward a highlight color as they slide together, and the frills flash with them, while every piñata still shares one material. The background scrolls and wipes between the menu and the game as the camera moves.
- The cannon and next-shot queue, menus and animated scores in UI Toolkit, object pooling, and a Newgrounds scoreboard.
