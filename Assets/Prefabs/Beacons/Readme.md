# Beacon Prefabs

## BeaconParent

Drag this into the scene to set-up everything needed for the current state of beacon mechanics. This includes the `BeaconSpawner` script which will spawn instances of a `BeaconManager`.

## BeaconManager

This prefab links to a prefab/model of a falling beacon and a crashed beacon. This prefab currently uses the `Firebolt` as a falling beacon and the `CrashedBeacon` as the beacon once it is on the ground. Note that the `Firebolt` is loaded from the 3rd party folder. 

## CrashedBeacon

Crashed beacon is a limited functionality at this time, it simply renders the beacon on the ground and extends `BaseDamageable`.

## Known Issues

- See top-level README for more
- The falling beacon is expecting an object with label `Terrain` by default to collide with. See the Terrain prefab README for compatible prefabs. Alternatively you can change what *singular* Layer the falling beacon should collide with on the `Firebolt` prefab. Some lowhanging fruit for improvement here is to change the `Firebolt`'s collision handling to take in a `LayerMask` to _ignore_ which would make it more broadly compatible.
