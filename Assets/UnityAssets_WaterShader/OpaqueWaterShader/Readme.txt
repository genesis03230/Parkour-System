README
================================

Contents
--------------------------------
About
Installation Instructions
Usage
Troubleshooting
FAQs
Support
Leave a Review


About
--------------------------------
A nice-looking opaque water shader for Unity.

Features:
- Made with Shader Graph.
- Supports Caustic Maps, Flow Maps, and Normal Maps.
- Uses custom lighting system to give the best visual results.
- Includes 45 Textures that you can mix-and-match to create over 3000 combinations of water.
- You can control where the Caustics and Tint appear using the Vertex Colors of the mesh.

Configurable Parameters:
- Color
- Reflection Sharpness
- Caustics Map (+ Tiling + Offset)
- Caustics Speed
- Caustics Color
- Flow Map (+ Tiling + Offset)
- Flow Map Speed
- Flow Map Intensity
- Normal Map (+ Tiling + Offset)
- Normal Map Strength
- Normal Map Speed
- Tint Color


Installation Instructions
--------------------------------
1. Import the asset into your project.
2. Add the Opaque Water material to any mesh in your scene.


Usage
--------------------------------
Configure the Opaque Water Material.
Duplicate it and edit the new copy to apply different settings to different meshes.
Use a tool like Polybrush to paint the vertices to use the Tint and Caustics mask features.
Bake a reflection probe near the water to ensure it has reflections.

Caustics appear when the Vertex Color R Channel is 1.
The tint color is used when the Vertex Color G Channel is 0.


FAQs
--------------------------------
For additional FAQs, check our documentation on the website: 
https://www.occasoftware.com/assets/opaque-water-shader


Support
--------------------------------
If you need any support, we are here to help.
Contact us at hello@occasoftware.com, or join our Discord at http://occasoftware.com/discord.


Leave a Review
--------------------------------
We would love to hear your feedback on this asset :) 
Please leave a review on the Unity Asset Store.