---
title: "Team Project Pitch: Glade"
author:
  - Space Cowboys
geometry: margin=2cm
header-includes:
  - \usepackage{setspace}
  - \onehalfspacing
---

# Team Project Pitch: Glade

## Story

The forests of a distant world have a secret: tucked away glades designated by the ancient tree spirits as a neutral space to meet, broker peace in, and maintain healthy relationships between the disparate tree tribes. Now, alien seedlings riding on meteorites are falling from the sky and have targeted these glades for their saplings. Play as the warden of one of these glades: a warrior spirit imbued with the powers of the tree tribes. Will you be able to protect your glade or see it fall to the invading species?

## Gameplay

Glade is a video game featuring a weapon-wielding hero who must scout out and sabotage an ongoing alien invasion. As the hero progresses in sabotaging the invasion they are granted additional abilities/powerups that the player can choose from. Combining these additional powers for maximum effect is imperative as the number and difficulty of enemies scales as they detect the hero's meddling in their invasion. Finally, the invading force will send out an extremely powerful foe which the hero must defeat to thwart the invasion attempt.

## Terminology

- Glade: A relative clearing in the ancient forests now targeted by alien plant-life for colonization
- Tree Tribe: A group of tree spirits associated with one of the many tribes present in the ancient forests
- Warden: the player character, a hero chosen by the tree tribes
- Beacon: alien tree seedling used as a recon/attack tool by the enemy to spearhead their invasion of the glade
- Run: an attempt at protecting a glade from invasion, the game attempts to stop the player from completing a run successfully in which case the player would need to start a new run.

## Core Mechanics

- Combat: the warden must kill the invading foes using weapons
- Recon: the warden must find the beacons sent by the invading force
- Powerup: the warden must collect powerups that will alter/improve its abilities

- Weapon Diversity

  - The warden has a sword, a shield, and a seed shooter available in their arsenal
  - Weapon choice is critical depending on the type of enemy and the current state of combat
  - The player's choice of weapon for a given situtation may change depending on powerups obtained during the run

- Powerups

  - Powerups may alter/improve the warden's ability to stop the invasion by altering:
  - The warden's traits (speed, health points, etc)
  - Their weapons (base damage, extra damage types like splash damage, extra weapon effects like spawning vines that will temporarily root enemies to the ground)
  - The player may discover emergent properties of the powerups when combined. For example, having a powerup that roots enemies to the ground could be paired with a powerup that slows down the players attacks but makes them hit harder.

- Powerup Choice

  - The invasion is spearheaded using beacons placed by the enemy, clearing these restores some power to the local tree tribes
  - The local tree tribes will offer the warden a choice of power ups - the player must choose one among these
  - Choice involves the potential dilemna of liking more than one of the options
  - Choice involves some strategy as some powerups may combine better with existing powerups

- Enemy Scaling

  - The enemy does not expect much resistance at first and does not have many troops on the ground when the invasion of a glade first starts
  - As the warden continues to thwart the invasion the enemies will send more/stronger forces to protect the remaining beacons

- Boss Fights
  - Clearing enough of the beacons triggers the invading force to send an extremely powerful foe in an attempt to stop the warden's sabotage
  - Boss is a unique enemy type that forces the player to combine their abilities for maximum output in order to achieve victory

## Formal Elements

### Rules

- The game is divided into "runs" - each "run" involves a new glade being invaded with a new warden who must start from scratch in thwarting the invasion
- If the player is killed by the enemy, the run is considered over in a loss condition
- If the player kills the boss, the run is considered over in a victory condition
- The warden cannot leave the glade, they are psychologically incapable of desertion

### Characters

- Warden: Player character
- Enemies: Sapient alien plant-life protecting the beacons
- Boss: Super-powered unique enemy sent as a last-ditch attempt to thwart the warden
- Tree Spirits: NPCs that offer powerups upon clearing a beacon

### Objectives

When a run starts the player is prompted to find the first beacon with some clue to its whereabouts. After clearing it and obtaining a powerup the player's objective is to continue the cycle of finding/destroying beacons to scale the warden's power.

The player should aim to combine powers as effectively as possible in anticipation of a boss fight - progression towards the fight is indicated to the player via in game events

## Work Plan

We have chosen to organize the work using a Kanban-like strategy. We meet synchronously to agree on scopes of work and define larger-scale goals.

We use Trello (as the Kanban board) to ansynchronously create work-items encompassing smaller scopes of work that can be individually completed. Individual work is reviewed and merged into the project by the team as a whole.

We will scout free resources for 3D models that will fit the alien/forest theme of the game.

We will also research the use of libraries to tackle some techical challenges critical to this game's development. The outcome of the research will either be to use a library or to develop our own solution. Here are some of the most critical challenges we've identified:

- AI detection of player character based on sound/proximity
- Random placement of objects on the map (specifically the beacons) such that the objects are still visible/interactible by the player
- State management to manage the powerups: for example how to track whether additional effects need to be computed for the player character or the enemies based on the currently enabled powerups.
