# cs6457-glade

## Table of Contents

1. [Beacons](#beacons)
2. [Character Controllers](#character-controllers)
3. [Camera Sensitivity](#camera-sensitivity)

## Beacons

Beacons are special seeds sent as forward recon/attack positions by the invading enemy. The player must find and destroy beacons to progress towards the boss fight.

Beacons are spawned in by a `BeaconSpawner` that will spawn an additional beacon on death of another (up to a concurrent and total maximum count of beacons). It will also spawn beacons after a random interval of time.

### How to Use

- Add the `BeaconParent` prefab to the scene
  - You can inspect the `BeaconSpawner` child to set parameters relating to spawn times and max beacons to spawn
  - The `BeaconSpawner` spawns a `BeaconManager` - so called because it manages the state of the Beacon, crashed vs. not crashed
  - Not crashed beacons fly towards the earth at a randomized angle
    - This behavior is defined on the 3rd party `Firebolt` Prefab
  - Crashed beacons turn into gooey eggs that inherit `BaseDamageable`

### Known-Issues

- Hitting the beacon with the sword does not always disable even if it 'looks' like it should. This is an issue with the sword and/or the fact that the beacon is making sure the sword is swinging.
- The beacons can spawn in and crash somewhere inaccessible
- The beacons tend to favor one quadrant of the map when spawning

## Character Controllers

In addition to our WIP player character there is another used for dev testing available in the `CameraRelativePlayerRootMotion` prefab. Drop it in to any scene to try it out - the prefab includes a preconfigured camera so you may need to remove/disable any existing cameras.

This prefab is _very_ sparse - currently only has movement and camera controls for Gamepad. It does use root motion though so it's a decent base to tweak from to get the animations to look right.

## Camera Sensitivity

The camera sensitivity can be controlled and fine-tuned in two different places:

1. In the Inspector View of the `3rdPersonCamera` object (under the `CameraParent` in the `Player` prefab).
    - Under **Axis Control**, the *Speed*, *Accel Time*, and *Decel Time* fields can be used to tweak the camera's overall responsiveness.
    - These settings affect both Gamepad and Mouse behavior.
    - Additionally, the camera's FOV and Orbit blending can also be changed from here.

2. In the **CharacterCameraControls** Input Action under `~Assets/InputSystem/CharacterCameraControls`.
    - There's a *Look* action defined and mapped to both Gamepad and Mouse.
    - New **Processors** can be individually added to each, to individually scale control sensitivity.
