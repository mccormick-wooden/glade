# SpaceCowboys_Glade_README





## Table of Contents


## Start Scene File
## How To Play
## Game Requirements Achieved
## Known Problem Areas
## Manifest
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
