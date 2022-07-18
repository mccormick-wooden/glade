# Crystal Prefabs

## Crystal
   
Crystal uses the included animation and animation controller. It sends
CrystalEffect events once per second when active and is activated by anything
with a Crystal Effect script attached. 

Crystals will dynamically spawn near other crystals per the parameters set in
the CrystalSpawner script in the prefab. If the CrystalManager has a
beaconSpawner reference defined, a crystal will spawn near a new beacon when it
lands.

### How to use

1. Add a CrystalEffect script to anything you want to be affected by the crystals. 
   * For CrystalDamageEffect with a lightning effect, set a lightning target if
   you want the lightning effect from a crystal to hit a certain spot on the
   GameObject, such as an empty game object at the location on the model you
   want it to hit.
2. Add a CrystalManager prefab in the scene and optionally provide it a
reference to the BeaconSpawner to have a crystal spawn whenever a new beacon
lands.
3. (optional) Drop in a Crystal prefab anywhere to start with a crystal in the scene.

#### Crystal Effects
1. CrystalHealEffect
   * Receive health when in the vicinity of a crystal
   * Requires that the GameObject have a Damageable script added, since it depends on that to update the HP.
   * The Damageable script should have the IsHealable checkbox checked.
2. CrystalDamageEffect
   * Take damage when in the vicinity of a crystal
   * Requires that the GameObject have a Damageable script added, since it depends on that to update the HP.

## Known Issues
* Crystals will all be clones of the prefab, but eventually want to have a way
to set various parameters on dynamically spawned crystals to change their size,
strength, effect radius, health, etc.

## References

### SineVFX TransluscentCrystals

This prefab uses modified model and shader assets from SineVFX's
[TransluscentCrystals](https://assetstore.unity.com/packages/3d/environments/fantasy/translucent-crystals-106274).

### ZapSplat Sounds

Uses audio clips from Zapsplat.com, one of which has been modified into a loopable.

