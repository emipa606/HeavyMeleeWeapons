# GitHub Copilot Instructions for RimWorld Mod: Heavy Melee Weapons (Continued)

## Mod Overview and Purpose

The **Heavy Melee Weapons (Continued)** mod is an enhancement of a previously existing mod, officially updated with permission. The mod introduces a variety of powerful melee weapons designed to enhance melee combat in RimWorld. It offers players the ability to wield oversized swords, incendiary cannons, and psychic warhammers to combat enemies, particularly mechanoids. This mod aims to make melee combat more engaging and strategic.

## Key Features and Systems

- **Oversized Weapons**: The mod requires the "EnableOversizedWeapons" mod, enhancing the game's weaponry available to players.
- **Unique Weapons**: Introduces new and unique weapon types such as gravity lances, psychic shockwave hammers, and flame-based swords.
- **Compatibility Patches**: Contains compatibility patches for other popular RimWorld mods, such as "Combat Extended" and "Vanilla Weapons Expanded - Heavy Weapons".
- **Craftable Persona Variants**: Adds the ability to craft personalized variants of the heavy melee weapons.

## Coding Patterns and Conventions

- **C# Classes**: Organized by weapon and effect. Classes are named with suffixes such as `Command_`, `Comp_`, `DamageWorker_`, and `Verb_` to denote their function.
- **Methods**: Methods are prefixed with access modifiers like `public` and `private`, and are named with verbs to signify actions (`GetTargetCommand`, `ApplyDamage`, `KeepDisplaying`).
- **File Organization**: Files are organized by functionality, such as weapons, shields, and damage mechanics, supporting modularity and easier maintenance.

## XML Integration

The mod integrates XML for defining weapon properties, balancing characteristics, and setting up compatibility patches. XML files define weapon stats and tags that dictate how these weapons function in the game environment.

## Harmony Patching

- **Harmony**: The mod uses Harmony for patching methods to ensure compatibility with other mods and to enhance or modify existing game behavior. For example, `Harmony_ExosuitHeavyWeapon` enables pawns with specific equipment to use heavy melee weapons effectively.
  
## Suggestions for Copilot

- **Auto-Complete XML**: Suggest XML snippets to define new weapons or modify existing ones, including balance parameters like damage, range, and cooldowns.
- **Generate Harmony Patches**: Assist in creating Harmony patches for compatibility with other mods, indicating which methods to prefix, postfix, or transpile.
- **Code Commenting**: Encourage inline comments within C# classes to describe the purpose of complex methods, enhancing code readability and maintainability.
- **Design Patterns**: Suggest design patterns for implementing new weapon effects or integration with pawn abilities, leveraging existing class and method structures.
- **Debug Aids**: Propose methods for logging and debugging, such as code snippets for writing mod-specific logs to assist in identifying errors during playtesting.
- **Testing Assistance**: Recommend unit tests or simulation scenarios to validate the performance and balance of new weapon features introduced by the mod.

This document intends to guide contributors and users in optimizing the development and enhancement of the Heavy Melee Weapons mod with GitHub Copilot's assistance.
