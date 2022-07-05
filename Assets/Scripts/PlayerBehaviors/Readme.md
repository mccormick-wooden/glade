# Player Behaviors

The purpose of this folder is to start collecting scripts that are exclusively meant to be attached as components onto a
Player character.

## Player Stats

Used to store and update information about the player's stats - especially as it pertains to power ups. Other behaviors
on the player object have read access to the stats the class is responsible for.

## Player Damageable

Extends the previously used `DisappearDamageable` class to provide the ability to update max health as well as affect
the amount of damage that is ultimately applied to the character.
