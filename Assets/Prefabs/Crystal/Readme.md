# Crystal Prefabs

## Crystal
   
Crystal uses the included animation and animation controller. It sends
CrystalEffect events once per second when active and is activated by anything
with a Crystal Effect script attached. 

### How to use

1. Add a CrystalEffect script to anything you want to be affected by the crystals. 
2. (optional) Drop in a Crystal prefab
3. (optional) Add a CrystalManager prefab in the scene and provide it a
reference to the BeaconSpawner to have a crystal spawn whenever a new beacon
lands.

#### Crystal Effects
1. CrystalHealEffect
   * Receive health when in the vicinity of a crystal
   * Requires that the GameObject have a Damageable script added, since it depends on that to update the HP.
   * The Damageable script should have the IsHealable checkbox checked.
2. CrystalDamageEffect
   * Take damage when in the vicinity of a crystal
   * Requires that the GameObject have a Damageable script added, since it depends on that to update the HP.

## Known Issues
* Crystal's sphere collider erroneously applies damage if hit by a sword. This
sphere collider is intended to just be used for determining when an activator
is in the vicinity in order to animate the crystal.
* Crystal does not yet have any audio attached.

## References

### SineVFX TransluscentCrystals

This prefab uses modified model and shader assets from SineVFX's
[TransluscentCrystals](https://assetstore.unity.com/packages/3d/environments/fantasy/translucent-crystals-106274).

