
### PR
https://github.com/lotsandlotsofrobots/glade/pull/83

### Implementation details
Basically, the way this works is:
- There is a Minimap canvas that renders a culled image that only shows things in the `MinimapIcon` layer from a top-down Minimap camera
- Everything that needs to exist on the minimap has a sprite on top of it, added to a `MinimapIcon` layer
- The main camera culls away the `MinimapIcon` layer so the icons aren't seen in regular gameplay

### How to integrate minimap into regular `Level1`
We will need to do this eventually when we are sure there won't be conflict issues. This is how to do it (mostly documentation for me):
1. Add `Minimap` prefab to the scene as a top level game object
2. Add `MinimapCamera` prefab as child of `Player` game object
3. Add `PlayerMinimapIcon` prefab as child of `PlayerModel` game object
4. Ensure the `MainCamera` Culling Mask does not have `MinimapIcon` selected

Note that 2-4 could be done by updating the `Player` prefab but I avoided doing this to avoid conflicts.