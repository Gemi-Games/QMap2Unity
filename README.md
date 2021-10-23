# QMap2Unity converter

## Mostly abandoned for now

A Unity engine extension that convert a Quake style '.map' file into mesh, collider and entity objects for use in Unity.

## Features
- Creates game objects by linking Unity prefabs to Quake classnames
- Quake style entity framework. Use UEntity and UEntityManager to trigger objects with a 'targetname'.
- Select specific Layers and settings for the meshes, colliders and objects using ScriptableObjects
- Automatically create secondary UVs for lightmapping //NOTE: using Unity's built-in method which is quite slow
- Works for both URP Lit and Standard materials
- Select between 3 materials for each texture: standard, alpha cutout or emissive
- Automatically create additional area light meshes on specific textures for greater lighting control
- Constructs texture mapped meshes, per brush convex colliders and total convex colliders set on a per entity basis
- Configure entity parameters inside the TrenchBroom editor for one-button conversions into ready-to-use Unity scenes

## Features roadmap
- Custom materials per texture
- Save levels as prefabs
- Automatically bake Navigation Mesh and Occlusion upon conversion
- Automatically generate lighting upon conversion (maybe not with URP?)

## Notes
- This extension has only been tested with *.map files created from the Trenchbroom 2.0 Quake engine level editor
- Map files must currently be changed to *.txt files for use with the Unity default TextAsset type
- The map parser can only read texture information that the Quake 2 map format provides and not the Quaternion style texture information that afaik is used for GoldSource engine maps
- Sorry there's no docs! There's a basic demo game included that has the bare bones on how to set up a game.
- Also, the conversion code is an absolute state!

Email me at marios.kalogerou@gemi-games.com if you have any questions whatsoever or make anything with it!
