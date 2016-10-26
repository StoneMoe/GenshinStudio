**Unity Studio** is a tool for exploring, extracting and exporting assets from Unity games and apps.

It is the continuation of my Unity Importer script for 3ds Max, and comprises all my research and reverse engineering of Unity file formats. It has been thoroughly tested with Unity builds from most platforms, ranging from Web, PC, Linux, MacOS to Xbox360, PS3, Android and iOS, and it is currently maintained to be compatible with Unity builds from 2.5.0 up to the 5.4.x version.

#### Current features

* Extraction of assets that can be used as standalone resources:
  * Textures: 
    * DDS (Alpha8, ARGB4444, RGB24, RGBA32, ARGB32, RGB565, DXT1, DXT5, RGBA4444, BGRA32)
    * PVR (YUY2, PVRTC_RGB2, PVRTC_RGBA2, PVRTC_RGB4, PVRTC_RGBA4, ETC_RGB4, ETC2_RGB, ETC2_RGBA1, ETC2_RGBA8, EAC_R, EAC_R_SIGNED, EAC_RG, EAC_RG_SIGNED)
    * KTX (RHalf, RGHalf, RGBAHalf, RFloat, RGFloat, RGBAFloat, ATC_RGB4, ATC_RGBA8)
    * ASTC (ASTC_RGB_4x4, ASTC_RGB_5x5, ASTC_RGB_6x6, ASTC_RGB_8x8, ASTC_RGB_10x10, ASTC_RGB_12x12, ASTC_RGBA_4x4, ASTC_RGBA_5x5, ASTC_RGBA_6x6, ASTC_RGBA_8x8, ASTC_RGBA_10x10, ASTC_RGBA_12x12)
  * Audio clips: fsb, mp3, ogg, wav, xbox wav (including streams from resource files)
  * Fonts: ttf, otf
  * Text Assets
  * Shaders
* Support convert almost all textures to .bmp .png or .jpeg
* Support convert FSB file to wav
* Export to FBX, with complete hierarchy, transformations, materials and textures. Geometry is exported with normals, tangents, UV coordinates, vertex colors and deformers. Skeleton nodes can be exported either as bones or dummy deformers..
* Real-time preview window for the above-mentioned assets
* Diagnostics mode with useful tools for research


#### UI guide

| Item                          | Action
| :---------------------------- | :----------------------------
| File -> Load file/folder      | Open Assetfiles and load their assets. Load file can also decompress and load bundle files straight into memory
| File -> Extract bundle/folder | Extract Assetfiles from bundle files compressed with lzma or l4z
| Scene Hierarchy search box    | Search nodes using * and ? wildcards. Press Enter to loop through results or Ctrl+Enter to select all matching nodes
| Asset List filter box         | Enter a keyword to filter the list of available assets; wildcards are added automatically
| Diagnostics                   | press Ctrl+Alt+D to bring up a hidden menu and a new list
| Bulid class structures        | Create human-readable structures for each type of Unity asset; available only in Web builds!

Other interface elements have tooltips or are self-explanatory.