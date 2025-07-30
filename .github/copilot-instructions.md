# Copilot Instructions for RimWorld Heavy Melee Mod Project

## Mod Overview and Purpose
The Heavy Melee mod is designed to enhance the melee combat experience in RimWorld by adding new weapon systems, combat mechanics, and utilities. The purpose of the mod is to introduce more dynamic and strategic melee fighting options, providing players with tools like enhanced shields and unique weapon abilities. 

## Key Features and Systems
- **Extended Shields**: Provides additional defense mechanics for pawns, including improved feedback through `Gizmo_EnergyShieldExtendedStatus`.
- **Heavy Melee Weapons**: Adds various melee weapons with distinct abilities, such as the `Verb_Cleave` and `Verb_SelfDamagingMelee`.
- **Custom Verb Systems**: Implements unique interactions and cooldown systems with custom verbs like `Verb_Cleave` and `Verb_PoweredMelee`.
- **Explosive and Gravity-based Attacks**: Features like `Comp_GravityLanceExplode` introduce powerful area-of-effect damage dynamics.

## Coding Patterns and Conventions
- **Class Structure**: The mod uses a consistent class structure where each weapon or ability is defined by a specific class, typically inheriting from RimWorld's vanilla classes like `ThingComp` or `Verb`.
- **Method Naming**: Public methods use PascalCase, while private methods are prefixed with an underscore to indicate their visibility.
- **Interface Implementation**: The mod employs interfaces such as `IVerbTick` and `IVerbCooldown` to define shared behavior among different verb classes.

## XML Integration
While specific XML data was not provided in the summary, typical integration would involve defining new `ThingDefs`, `HediffDefs`, and other relevant RimWorld XML entries to register the new items and abilities in the game. XML tags should align with the C# class definitions to ensure seamless functionality.

## Harmony Patching
The mod includes `Harmony_ExosuitHeavyWeapon` class to patch existing RimWorld methods, allowing for modifications without altering the base game code. Harmony patches are applied statically, and appropriate safety checks should be used when patching to avoid conflicts with other mods.

## Suggestions for Copilot
1. **Shield Mechanics**: Suggest ways to enhance shield visuals or effects, such as linking shield health to color intensity.
2. **Dynamic AI Use**: Propose logic for NPCs to optimally use special abilities provided by the mod, based on combat scenarios.
3. **Improved Feedback Systems**: Recommend adding more detailed feedback to players, such as sound effects or UI highlights when using powerful melee attacks or shields.
4. **Optimization Tips**: Offer performance optimization tips, particularly for real-time updates during combat (e.g., efficient usage of Tick methods).
5. **Error Handling and Logging**: Provide guidelines to implement robust error handling and logging for unexpected behaviors in combat interactions.

By following these instructions, Copilot should assist in generating efficient, functional, and scalable code to ensure the Heavy Melee mod provides a seamless and engaging experience for RimWorld players.
