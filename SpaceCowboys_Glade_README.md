# SpaceCowboys_Glade_README

Team Members
- Chris Dail - cdail7@gatech.edu
- Eric Gilligan - egilligan3@gatech.edu
- Thomas Lagrange - tlagrange3@gatech.edu
- McCormick Wooden - mwooden3@gatech.edu
- Daniel Zuniga - daniel.zuniga@gatech.edu

## Table of Contents
- [SpaceCowboys_Glade_README](#spacecowboys_glade_readme)
  - [Table of Contents](#table-of-contents)
  - [Start Scene File](#start-scene-file)
  - [How To Play](#how-to-play)
    - [To Start Game](#to-start-game)
    - [Play the Game](#play-the-game)
    - [How to Win the Game](#how-to-win-the-game)
    - [Controls](#controls)
    - [Game Requirements Achieved](#game-requirements-achieved)
  - [Known Problem Areas](#known-problem-areas)
  - [Manifest](#manifest)
    - [Features / Non-Script Assets](#features--non-script-assets)
      - [Chris Dail - cdail7](#chris-dail---cdail7)
      - [Eric Gilligan - egilligan3](#eric-gilligan---egilligan3)
      - [Thomas Lagrange - tlagrange3](#thomas-lagrange---tlagrange3)
      - [McCormick Wooden - mwooden3](#mccormick-wooden---mwooden3)
      - [Daniel Zuniga](#daniel-zuniga)
    - [Scripts](#scripts)
    - [3rd Party Assets](#3rd-party-assets)
  - [Internal Team Documentation](#internal-team-documentation)
    - [Beacons](#beacons)
      - [How to Use](#how-to-use)
    - [Camera Sensitivity](#camera-sensitivity)

## Start Scene File
*Requirement from assignment pdf:*
```
i. Start scene file
```

The starting scene is the `Start` scene. This scene bootstraps all global management objects and immediately loads the scene/state specified in the `GameManager` GameObject's `Starting State`. By default, the starting scene/state is `Main Menu`

## How To Play
*Requirement from assignment pdf:*
```
ii. How to play and what parts of the level to observe technology requirements
```

### To Start Game
- Begin the game
- Select `New Game` in menu
- Progress through the story crawl and tutorial
- Follow the instructions in the story crawl and tutorial to play and win the game - just don't die!

### Play the Game
- Find beacons as they land (you will hear them rain down from the sky and can see them too)
- Destroy the beacons with your sword
- Destroying a beacon drops a purple object - approach to interact automatically
- Colliding with the purple object opens a power up menu where the player can pick from certain power ups
- The player must choose a single power up among the three offered
- Destroying the beacon launched another beacon, find it and repeat!

### How to Win the Game
- Find and destroy 2 beacons

### Controls
- Left Stick, WASD = Warden movement
- Right Stick, Mouse = Camera Control
- Right Bumper, Left Click - Attack
- Left Bumper, Right Click - Defend
- Start Button, ESC - Pause Game

### Game Requirements Achieved
- Clearly defined, achievable, objective/goal?
  - *see new game story crawl, tutorial*
- Communication of success or failure to player!
  - *on death game tells you that you lost, on killing all beacons game says you won*
- Start menu to support Starting Action?
  - *press start button on gamepad or ESC*
- Sorry, no first-person perspective games (e.g. FPS) unless briefly used for
  - *self-explanatory*
- Goals/sub-goals effectively communicated to player
    - *see new game story crawl, tutorial*
- Avoid Fun Killers (micromanagement, stagnation, insurmountable obstacles, arbitrary events, etc.)
  - *self explanatory*
- Character control is a predominant part of gameplay and not simply a way to traverse between non-“Game Feel” interactions.
  - *self explanatory*
- No Unity tutorial characters or CS4455 milestone characters allowed. You must use characters other than those in tutorials; that includes the “3DGameKit”
  - *self explanatory*
- Your character is not a complete prepackaged asset from a 3rd party. It is ok to use models and animations from another source, but your team should configure animation control and input processing
  - *self explanatory*
- Utilize a character/vehicle controlled by the player with engaging animations that react to the player’s inputs.
  - *self explanatory*
- Player has Game Feel style control: continuous, dynamic, low latency, variable/analog-style control of character movement majority of time
  - *self explanatory*
- Camera has limited passing through walls, near clipping plane cutting through objects (e.g. player model), etc.
  - *self explanatory*
- Synthesized unique environment
  - *Level is synthesized from NatureStarterKit2 assets*
- Consistent spatial simulation throughout
  - *self explanatory*
- Your AI agents are not complete prepackaged assets from a 3rd party
  - *enemy assets are custom prefabs using existing models/animations*
- Implement a start menu GUI.
  - *visible from the start of the game*
- Implement in-game pause menu with ability to quit game
  - *see controls for pause button - only viable during the main level*
- Ability to exit software at any time
  - *pause menu, start menu lets you quit*
- Transitions between scenes should be done aesthetically
  - *scenes have a fade in/out transition on load*

## Known Problem Areas
*Requirement from assignment pdf:*
```
iii. Known problem areas
```
- If you continue running directly into the beacon, it can push you through the floor in some cases

## Manifest

**Primary contributors** are listed first. Primary contributors are defined as the person that instantiated the idea and contributed the majority of the work.
**Secondary contributors** are listed after primaries. Secondary contributors are defined as contributing something to the file, either adding additional features or refactoring. Order here doesn't matter

### Features / Non-Script Assets
*Requirement from assignment pdf:*
```
iv. Manifest of which files authored by each teammate:
1. Detail who on the team did what
2. For each team member, list each asset implemented.
3. Make sure to list C# script files individually so we can confirm
each team member contributed to code written
```

#### Chris Dail - cdail7
- Player control / animations
- Enemy AI
- Enemy Spawning
- Enemy Animation
- Audio framework
- IK apple pickup / healing

#### Eric Gilligan - egilligan3
- Crystals - anything and everything related to crystals
- Mana bar / mana mechanic
- Environment design
- Bunch of bug fixes across all parts
- Project Icon

#### Thomas Lagrange - tlagrange3
- Combat animation and implementation
- Special attack mechanic
- Lock on and Strafe Mechanics
- Powerup Mechanic
- Beacon spawning / flying / management
- Level / terrain design
- Boss design / logic

#### McCormick Wooden - mwooden3
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

### Scripts

To regenerate tree for new files:
- `apt-get install tree`
- `cd <projectdir>/Assets/Scripts && tree -I '*.meta'`
- Copy the specific part of the tree that's new, and insert into existing tree (don't overwrite existing manifest)

```
.
├── Abstract
│   ├── BaseCrystalEffect.cs - *(egilligan3)*
│   ├── BaseDamageable.cs - *(mwooden3, cdail7, egilligan3, tlagrange3)*
│   ├── BaseDevCommand.cs - *(mwooden3)*
│   ├── BaseEnemy.cs - *(cdail7)*
│   ├── BaseLevelStateManager.cs - *(mwooden3, egilligan3)*
│   ├── BasePowerUp.cs - *(tlagrange3)*
│   ├── BaseStateManager.cs - *(mwooden3, cdail7, egilligan3)*
│   └── BaseWeapon.cs - *(mwooden3, cdail7, tlagrange3)*
├───Animation
│   ├── EnterStrafeBlendTree.cs - *(tlagrange3)*
│   ├── ExitCombatLayerAnimation.cs - *(tlagrange3)*
│   ├── ExitLockedOnLayer.cs - *(tlagrange3)*
│   └── StartComboAnimation.cs - *(tlagrange3)*
├── AppleTreeScript.cs - *(cdail7)*
├── AppEvents
│   ├── AppleHitGrassEvent.cs - *(cdail7)*
│   ├── CampfireStartEvent.cs - *(cdail7)*
│   ├── CrystalCollisionEvent.cs - *(egilligan3)*
│   ├── CrystalDeathEvent.cs - *(egilligan3)*
│   ├── FairyAOEAttackEvent.cs - *(cdail7)*
│   ├── MonsterDieEvent.cs - *(cdail7)*
│   ├── MonsterTakeDamageEvent.cs - *(cdail7)*
│   ├── PlayerEatAppleEvent.cs - *(cdail7)*
│   ├── PlayerFootstepEvent.cs - *(cdail7)*
│   ├── PlayerHurtEvent.cs - *(cdail7)*
│   ├── PlayMusicEvent.cs - *(cdail7)*
│   ├── SwordHitEvent.cs - *(cdail7)*
│   └── SwordSwingEvent.cs - *(cdail7)*
├── Arrow.cs - *(cdail7)*
├── AudioListenerManager.cs - *(egilligan3)*
├── AudioEventManager.cs - *(cdail7, tlagrange3, egilligan3)*
├── Beacons
│   ├── BeaconFall.cs - *(tlagrange3)*
│   ├── BeaconManager.cs - *(tlagrange3, cdail7, egilligan3, mwooden3)*
│   ├── BeaconOrbiter.cs - *(tlagrange3)*
│   ├── BeaconOvershield.cs - *(tlagrange3)*
│   ├── BeaconSpawner.cs - *(tlagrange3, mwooden3, egilligan3)*
│   ├── BeaconSpawnPoint.cs - *(tlagrange3)*
│   ├── CrashedBeacon.cs - *(tlagrange3, mwooden3)*
│   └── dev
│       └── TriggerBeaconSpawn.cs - *(tlagrange3)*
├── BillboardController.cs - *(daniel.zuniga, mwooden3, egilligan3)*
├── Boss.cs - *(tlagrange3)*
├── BossRootMotion.cs - *(tlagrange3)*
├── Console
│   ├── DevCommandResult.cs - *(mwooden3)*
│   ├── DeveloperConsole.cs - *(mwooden3)*
│   ├── DeveloperConsoleBehaviour.cs - *(mwooden3)*
│   ├── EchoCommand.asset - *(mwooden3)*
│   ├── EchoCommand.cs - *(mwooden3)*
│   ├── LoadSceneCommand.asset - *(mwooden3)*
│   └── LoadSceneCommand.cs - *(mwooden3)*
├── Crystal
│   ├── CrystalController.cs - *(egilligan3)*
│   ├── CrystalDamageEffect.cs - *(egilligan3)*
│   ├── CrystalPhsyicsCollision.cs - *(egilligan3)*
│   ├── CrystalHealEffect.cs - *(egilligan3)*
│   ├── CrystalManager.cs - *(egilligan3)*
│   ├── CrystalPhysicsCollision.cs - *(egilligan3)*
│   └── CrystalSpawner.cs - *(egilligan3)*
│   └── CrystalWeapon.cs - *(egilligan3)*
├── Damageable
│   ├── AnimateDamageable.cs - *(tlagrange3, mwooden3)*
│   └── DisappearDamageable.cs - *(tlagrange3, mwooden3)*
├── Enemy
│   ├── DummyBeaconDefenderEnemy.cs - *(cdail7)*
│   ├── DummyRangedAttackEnemy.cs - *(cdail7)*
│   ├── DummySpinAttackEnemy.cs - *(cdail7)*
│   ├── FairyEnemy.cs - *(cdail7)*
│   ├── FrightFlyEnemy.cs - *(cdail7)*
│   ├── HackTestEnemy.cs - *(mwooden3)*
│   ├── MushroomScript.cs - *(cdail7)*
│   ├── PeaShooterEnemy.cs - *(cdail7)*
│   ├── PlantEnemy.cs - *(cdail7)*
│   ├── SwordEnemy.cs - *(mwooden3)*
│   └── VenusScript.cs - *(cdail7)*
├── EventSound3D.cs - (from course)
├── FruitDetector.cs - *(cdail7)*
├── EnemySpawner.cs - *(cdail7, tlagrange3)*
├── GameManagement
│   ├── DialogueController.cs - *(mwooden3)*
│   ├── EventManager.cs - (from course)
│   ├── GameManager.cs - *(mwooden3, egilligan3)*
│   ├── GladeHealthManager.cs - *(mwooden3)*
│   ├── Level1StateManager.cs - *(mwooden3)*
│   ├── LongClickButton.cs - *(mwooden3)*
│   ├── MainMenuStateManager.cs - *(mwooden3)*
│   ├── NewGameStateManager.cs - *(mwooden3)*
│   ├── PauseMenuManager.cs - *(mwooden3, tlagrange3, egilligan3)*
│   ├── TrainingStateManager.cs - *(mwooden3, daniel.zuniga, egilligan3)*
│   ├── TriggerPlane.cs - *(mwooden3, daniel.zuniga)*
│   └── TriggerPlane.cs - *(mwooden3, daniel.zuniga)*
│   └── LongClickButton.cs - *(mwooden3)*
│   ├── EndGameMenu.cs - *(mwooden3)*
│   └── DialogueController.cs - *(mwooden3)*
├── HealthBarController.cs - *(daniel.zuniga, egilligan3, mwooden3, tlagrange3)*
├── HealingApple.cs - *(cdail7)*
├── Helper
│   ├── AnimationEventDispatcher.cs - *(mwooden3, [inspired by StackOverflow](https://gamedev.stackexchange.com/questions/117423/unity-detect-animations-end))*
│   ├── CameraBlendEventDispatcher.cs *(mwooden3, [inspired by thread](https://forum.unity.com/threads/oncameratransition-onblendcomplete-event.520056/))*
│   ├── DontDestroyThisOnLoad.cs - *(mwooden3)*
│   ├── Extensions.cs - *(mwooden3)*
│   ├── MouseSelect.cs - *(egilligan3)*
│   ├── Quitter.cs - *(mwooden3)*
│   ├── SceneLoader.cs - *(mwooden3)*
│   ├── TimeScaleToggle.cs - *(mwooden3)*
│   ├── SyncWithCameraRotation.cs - *(egilligan3)*
│   └── Utility.cs - *(mwooden3, egilligan3)*
│   ├── MouseSelect.cs - *(egilligan3)*
├── Interfaces
│   ├── IDamageable.cs - *(mwooden3, egilligan3)*
│   ├── IDevCommand.cs - *(mwooden3)*
│   ├── IDevCommandResult.cs - *(mwooden3)*
│   └── IWeapon.cs - *(mwooden3)*
├── LockOnIndicatorAnimation.cs - *(tlagrange3)*
├── Minimap
│   ├── MinimapCamera.cs *(mwooden3, egilligan3)*
│   └── MinimapIcon.cs *(mwooden3)*
├── Movement
│   └── CameraRelativeRootMovement.cs *(daniel.zuniga)*
├── NPC
│   └── TreeSpirit.cs - *(mwooden3)*
├── Player.cs - *(cdail7, egilligan3, tlagrange3, mwooden3, Daniel Zuniga)*
├── PlayerBehaviors
│   ├── PlayerCombat.cs - *(tlagrange3, egilligan3)*
│   ├── PlayerDamageable.cs - *(tlagrange3)*
|   ├── PlayerStats.cs - *(tlagrange3)*
│   ├── PlayerWeapon.cs - *(tlagrange3)*
│   ├── PlayerWeaponManager.cs - *(tlagrange3)*
│   └── PlayerWeaponSlot.cs - *(tlagrange3)*
├── PlayerControls.cs - *(cdail7)*
├── PlayerLockOnCamera.cs - *(cdail7)*
├── PowerUps
│   ├── DamageIncreasePowerUp.cs - *(tlagrange3)*
│   ├── DamageResistPowerUp.cs - *(tlagrange3)*
│   ├── MaxHealthPowerUp.cs - *(tlagrange3)*
│   ├── PowerUpMenu.cs - *(tlagrange3, egilligan3)*
│   ├── PowerUpPickup.cs - *(tlagrange3, egilligan3)*
│   └── dev
│       └── TogglePowerUpMenu.cs - *(tlagrange3)*
|── WeaponsAndAttacks
│   ├── AngryChestBump.cs - *(mwooden3)*
|   ├── AOEAttack.cs - *(cdail7)*
|   ├── BiteAttack.cs - *(cdail7)*
|   ├── MushroomExplosion.cs - *(cdail7)*
|   ├── PeaWeapon.cs - *(cdail7)*
|   └── Sword.cs - *(cdail7, tlagrange3)*
├── Shield.cs - *(cdail7)*
├── SpecialEffectWeapon.cs - *(tlagrange3)*
├── PlayerLockOnCamera.cs - *(tlagrange3)*
├── ThirdPersonCamera.cs - *(daniel.zuniga, tlagrange3)*
├── VelocityReporter.cs - (from course)
├── Extensions.cs *(mwooden3)*
└── VelocityReporter.cs - (from course)

```


### 3rd Party Assets

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


## Internal Team Documentation
### Beacons

Beacons are special seeds sent as forward recon/attack positions by the invading enemy. The player must find and destroy beacons to progress towards the boss fight.

Beacons are spawned in by a `BeaconSpawner` that will spawn an additional beacon on death of another (up to a concurrent and total maximum count of beacons). It will also spawn beacons after a random interval of time.

#### How to Use

- Add the `BeaconParent` prefab to the scene
  - You can inspect the `BeaconSpawner` child to set parameters relating to spawn times and max beacons to spawn
  - The `BeaconSpawner` spawns a `BeaconManager` - so called because it manages the state of the Beacon, crashed vs. not crashed
  - Not crashed beacons fly towards the earth at a randomized angle
    - This behavior is defined on the 3rd party `Firebolt` Prefab
  - Crashed beacons turn into gooey eggs that can be destroyed.

### Camera Sensitivity

The camera sensitivity can be controlled and fine-tuned in two different places:

1. In the Inspector View of the `3rdPersonCamera` object (under the `CameraParent` in the `Player` prefab).
    - Under **Axis Control**, the *Speed*, *Accel Time*, and *Decel Time* fields can be used to tweak the camera's overall responsiveness.
    - These settings affect both Gamepad and Mouse behavior.
    - Additionally, the camera's FOV and Orbit blending can also be changed from here.

2. In the **CharacterCameraControls** Input Action under `~Assets/InputSystem/CharacterCameraControls`.
    - There's a *Look* action defined and mapped to both Gamepad and Mouse.
    - New **Processors** can be individually added to each, to individually scale control sensitivity.
