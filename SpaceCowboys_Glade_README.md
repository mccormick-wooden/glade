# SpaceCowboys_Glade_README

Team Members
- Chris Dail - cdail7@gatech.edu
- Eric Gilligan - eric.gilligan@gatech.edu
- Thomas Lagrange - tlagrange3@gatech.edu
- McCormick Wooden - mwooden3@gatech.edu
- Daniel Zuniga - daniel.zuniga@gatech.edu



## Table of Contents


## Start Scene File
## How To Play
## Game Requirements Achieved
## Known Problem Areas
## Manifest

**Primary contributors** are listed first. Primary contributors are defined as the person that instantiated the idea and contributed the majority of the work.
**Secondary contributors** are listed after primaries. Secondary contributors are defined as contributing something to the file, either adding additional features or refactoring. Order here doesn't matter

### Features / Non-Script Assets

### Scripts

To regenerate tree for new files:
- `apt-get install tree`
- `cd <projectdir>/Assets/Scripts && tree -I '*.meta'`
- Copy the specific part of the tree that's new, and insert into existing tree (don't overwrite existing manifest)

.
├── Abstract
│   ├── BaseCrystalEffect.cs - *(eric.gilligan)*
│   ├── BaseDamageable.cs - *(mwooden3, cdail7, eric.gilligan, tlagrange3)*
│   ├── BaseDevCommand.cs - *(mwooden3)*
│   ├── BaseEnemy.cs - *(cdail7)*
│   ├── BaseLevelStateManager.cs - *(mwooden3)*
│   ├── BaseStateManager.cs - *(mwooden3, cdail7)*
│   └── BaseWeapon.cs - *(mwooden3, cdail7)*
├── AngryChestBump.cs - *(mwooden3)*
├── AppEvents
│   ├── PlayMusicEvent.cs - *(cdail7)*
│   └── SwordSwingEvent.cs - *(cdail7)*
├── Arrow.cs - *(cdail7)*
├── AudioEventManager.cs - *(cdail7)*
├── Beacons
│   ├── BeaconFall.cs - *(tlagrange3)*
│   ├── BeaconManager.cs - *(tlagrange3, cdail7, eric.gilligan, mwooden3)*
│   ├── BeaconOrbiter.cs - *(tlagrange3)*
│   ├── BeaconSpawner.cs - *(tlagrange3, mwooden3)*
│   ├── CrashedBeacon.cs - *(tlagrange3, mwooden3)*
│   └── dev
│       └── TriggerBeaconSpawn.cs - *(tlagrange3)*
├── BillboardController.cs - *(daniel.zuniga, mwooden3, eric.gilligan)*
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
│   ├── CrystalController.cs - *(eric.gilligan)*
│   └── CrystalHealEffect.cs - *(eric.gilligan)*
├── Damageable
│   ├── AnimateDamageable.cs - *(tlagrange3, mwooden3)*
│   └── DisappearDamageable.cs - *(tlagrange3, mwooden3)*
├── Enemy
│   ├── DummyBeaconDefenderEnemy.cs - *(cdail7)*
│   ├── DummyRangedAttackEnemy.cs - *(cdail7)*
│   ├── DummySpinAttackEnemy.cs - *(cdail7)*
│   ├── HackTestEnemy.cs - *(mwooden3)*
│   └── SwordEnemy.cs - *(mwooden3)*
├── EnemySpawner.cs - *(cdail7)*
├── EventSound3D.cs - *(cdail7)*
├── FootIK.cs - *(cdail7)*
├── GameManagement
│   ├── EventManager.cs - (from course)
│   ├── GameManager.cs - *(mwooden3)*
│   ├── Level1StateManager.cs - *(mwooden3)*
│   ├── MainMenuStateManager.cs - *(mwooden3)*
│   ├── NewGameStateManager.cs - *(mwooden3)*
│   ├── PauseMenuManager.cs - *(mwooden3)*
│   └── TrainingStateManager.cs - *(mwooden3)*
├── HealthBarController.cs - *(daniel.zuniga, eric.gilligan)*
├── Helper
│   ├── AnimationEventDispatcher.cs - ([from StackOverflow](https://gamedev.stackexchange.com/questions/117423/unity-detect-animations-end))
│   ├── DontDestroyThisOnLoad.cs - *(mwooden3)*
│   ├── Quitter.cs - *(mwooden3)*
│   ├── SceneLoader.cs - *(mwooden3)*
│   ├── TimeScaleToggle.cs - *(mwooden3)*
│   └── Utility.cs - *(mwooden3)*
├── IKFootPlacement.cs - *(cdail7)*
├── Interfaces
│   ├── IDamageable.cs - *(mwooden3)*
│   ├── IDevCommand.cs - *(mwooden3)*
│   ├── IDevCommandResult.cs - *(mwooden3)*
│   └── IWeapon.cs - *(mwooden3)*
├── Movement
│   └── CameraRelativeRootMovement.cs - *(cdail7)*
├── Player.cs - *(cdail7, eric.gilligan, tlagrange3, mwooden3, Daniel Zuniga)*
├── PlayerControls.cs - *(cdail7)*
├── Shield.cs - *(cdail7)*
├── Sword.cs - *(cdail7)*
├── ThirdPersonCamera.cs - *(daniel.zuniga)*
└── VelocityReporter.cs - (from course)

### 3rd Party Assets

The entirety of the 3rd Party assets that are in use are contained in `Assets/3rdParty/`:

- [UI Button Pack 2](https://assetstore.unity.com/packages/2d/gui/icons/ui-button-pack-2-1200-button-130422) - Used for main menu and pause menu buttons.
- [Pyro Particles](need link) - Used for Beacon meteorite effect.
- [Nature Starter Kit 2](https://assetstore.unity.com/packages/3d/environments/nature-starter-kit-2-52977) - Used for environments / terrain.
- [Casual Fantasy - Ent](https://assetstore.unity.com/packages/3d/characters/creatures/ent-casual-fantasy-206323) - Used for Ancient Tree Spirit character
- [Polygonal Metalon](https://assetstore.unity.com/packages/3d/characters/creatures/meshtint-free-polygonal-metalon-151383) - Used for "Boss" character
- [Toby Fredson](need link) - No idea
- [BizulkaProduction](need link) - No idea
- (where are crystals from?)


## Internal Documentation
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

#### Known-Issues

- ???

### Camera Sensitivity

The camera sensitivity can be controlled and fine-tuned in two different places:

1. In the Inspector View of the `3rdPersonCamera` object (under the `CameraParent` in the `Player` prefab).
    - Under **Axis Control**, the *Speed*, *Accel Time*, and *Decel Time* fields can be used to tweak the camera's overall responsiveness.
    - These settings affect both Gamepad and Mouse behavior.
    - Additionally, the camera's FOV and Orbit blending can also be changed from here.

2. In the **CharacterCameraControls** Input Action under `~Assets/InputSystem/CharacterCameraControls`.
    - There's a *Look* action defined and mapped to both Gamepad and Mouse.
    - New **Processors** can be individually added to each, to individually scale control sensitivity.
