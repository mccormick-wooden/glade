# Glade

![image](https://user-images.githubusercontent.com/60236744/178626491-22030cd6-4e1c-4e34-b48a-cbdade6d27ab.png)

## Team
- Chris
- Eric
- Thomas
- McCormick
- Daniel

## Build instructions

1. [Download Unity for your operating system.](https://unity.com/download)
   - Unity Version should be 2020.3.34F1 LTS
2. From a terminal, run this command to clone this repository:
```
git clone https://github.com/mccormick-wooden/glade-mirror.git
```
3. From Unity Hub, select `Open > Add project from disk` and select local cloned project directory.
![image](https://user-images.githubusercontent.com/60236744/178601965-42d9025b-6b65-4a36-ace0-e89d634ff3d4.png)
4. In the Unity Editor, navigate to `File > Build Settings` and ensure the build settings are configured as below.
   - Modify the `Target Platform` dropdown to match your operating system
   - Linux builds are unfortunately not supported for this project
![image](https://user-images.githubusercontent.com/60236744/178602025-e51007af-91de-438a-816f-ca437be2bbb2.png)
5. Click `Build`
6. Run the executable!


## Team

#### Chris
- Player control / animations
- Enemy AI
- Enemy Spawning
- Enemy Animation
- Audio framework
- IK apple pickup / healing

#### Eric
- Crystals - anything and everything related to crystals
- Mana bar / mana mechanic
- Environment design
- Bunch of bug fixes across all parts
- Project Icon

#### Thomas
- Combat animation and implementation
- Special attack mechanic
- Lock on and Strafe Mechanics
- Powerup Mechanic
- Beacon spawning / flying / management
- Level / terrain design
- Boss design / logic

#### McCormick
- Menus - Main Menu + Pause Menu
- Story introduction crawl
- Tutorial / Training scene
- Game state management
- Win / Loss conditions
- Damage / Combat framework
- Scene Transitions
- Minimap
- Glade Health System

#### Daniel Zuniga
- Camera
- Player Health Bar
- Player control/root motion
- Player Animator: Fall-Landing sequence


## 3rd party assets

The majority of the 3rd Party assets that are in use are contained in `Assets/3rdParty/`:

- [UI Button Pack 2](https://assetstore.unity.com/packages/2d/gui/icons/ui-button-pack-2-1200-button-130422) - Used for main menu and pause menu buttons.
- [Pyro Particles](https://assetstore.unity.com/packages/vfx/particles/fire-explosions/fire-spell-effects-36825) - Used for Beacon meteorite effect.
- [Nature Starter Kit 2](https://assetstore.unity.com/packages/3d/environments/nature-starter-kit-2-52977) - Used for environments / terrain.
- [Casual Fantasy - Ent](https://assetstore.unity.com/packages/3d/characters/creatures/ent-casual-fantasy-206323) - Used for Ancient Tree Spirit character
- [Polygonal Metalon](https://assetstore.unity.com/packages/3d/characters/creatures/meshtint-free-polygonal-metalon-151383) - Used for "Boss" character
- [Toby Fredson](https://assetstore.unity.com/publishers/11721) - Used for the Terrain textures
- [BizulkaProduction](https://assetstore.unity.com/packages/3d/characters/creatures/fuga-spiders-with-destructible-eggs-and-mummy-151921) - Used for the crashed beacon model
- [SkythianCat](https://assetstore.unity.com/packages/3d/environments/hand-painted-nature-kit-lite-69220#description) - Used for the training interaction stump the ent stands on
- [SineVFX - Transluscent Crystals](https://assetstore.unity.com/packages/3d/environments/fantasy/translucent-crystals-106274)
- [Hovl Studio](https://assetstore.unity.com/packages/vfx/particles/spells/epic-toon-vfx-2-157651) - Used for wind sword tornado particle effect, model, and textures
- [Hit Impact Effects](https://assetstore.unity.com/packages/vfx/particles/hit-impact-effects-free-218385) - Used for visualizing hit impacts in combat
- [minicrap](https://github.com/Srfigie/Unity-3d-TopDownMovement) - Some prebuilt art for minimap.
- [ExplosiveLLC](https://assetstore.unity.com/packages/3d/animations/warrior-pack-bundle-1-free-36405) - Tornado (Wind Sword) attack animation