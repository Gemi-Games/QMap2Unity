# QMap2Unity converter

# Still under construction

A Unity engine extension that converts Quake style *.map files into mesh, collider and entity objects for use in a Unity scene.

## Features
- Creates game objects by linking Unity prefabs to Quake classnames
- Select specific Layers and settings for the visible meshes, colliders and top level objects in your entities
- Automatically create secondary UVs for lightmapping
- Works for both HDRF Lit and Standard materials
- Select between 3 materials for each texture: standard, alpha cutout or emissive
- Automatically create additional area light meshes on specific textures for greater lighting control
- Constructs texture mapped meshes, per brush convex colliders and total convex colliders set on a per entity basis
- Configure entity parameters inside the TrenchBroom editor for one-button conversions into ready-to-use Unity scenes

## Features roadmap
- Custom materials per texture
- Save levels as prefabs
- Automatically bake Navigation Mesh and Occlusion upon conversion
- Automatically generate lighting upon conversion

## Notes
- This extension has only been tested with *.map files created from the Trenchbroom 2.0 Quake engine level editor
- Map files must currently be changed to *.txt files for use with the Unity default TextAsset type
- The map parser can only read texture information that the Quake 2 map format provides and not the Quaternion style texture information that I believe is used for GoldSource engine maps