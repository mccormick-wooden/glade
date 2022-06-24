# Crystal Prefabs

## Crystal
   
Crystal uses the included animation and animation controller. It sends
CrystalEffect events once per second when active and is activated by anything
with the CrystalHealer script attached. CrystalHealers subscribe to the
CrystalEffect events and apply healing when the event received and they are
within range.

### How to use

Drop in a prefab and add the CrystalHealer script to anything you want to be
healed by the crystals. The CrystalHealer requires that the GameObject have a
Damageable script added, since it depends on that to update the HP.

## Citations

### SineVFX TransluscentCrystals

This prefab uses model and shader assets from SineVFX's TransluscentCrystals.

