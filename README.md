# Fnt2TMPro

Unity Editor tool to convert BMFont `.fnt` bitmap fonts (text or XML) into TextMesh Pro `TMP_FontAsset` using an existing texture atlas.
Based on [Napbla's fnt2TMPro](https://github.com/napbla/fnt2TMPro) but with some improvements. Maybe if that project didn't work for you, then this one might.
### Features
- Parse both text and XML `.fnt` formats
- Map glyphs and metrics to `TMP_FontAsset`
- Assign custom bitmap shader and atlas texture

### Requirements
- Unity with TextMesh Pro installed

### Installation
1. Copy the `Fnt2TMPro` folder into your Unity project Editor folder (e.g., under `Assets/Editor/Fnt2TMPro`).
2. Ensure TextMesh Pro is installed in the project.

### Usage
1. Open the window: `Window > Bitmap Font Converter`.
2. Set inputs:
   - Font Texture: your bitmap font atlas (`Texture2D`).
   - Source Font File: your `.fnt` file (`TextAsset`, text or XML).
   - Destination Font File: an existing `TMP_FontAsset` to populate.
3. Click `Convert`.

The tool will:
- Parse glyphs and metrics from the `.fnt` file
- Fill `glyphTable` and `characterTable`
- Assign the atlas texture and set atlas size
- Update `faceInfo` (line height, ascent, baseline, point size)

### Notes
- The tool tries to load shader at `Assets/Plugins/TextMesh Pro/Shaders/TMP_Bitmap-Custom-Atlas.shader`. If it is missing, it falls back to a default TMP shader.
- Kerning from `.fnt` is parsed but not currently applied to `TMP_FontAsset`.
- The destination font asset must already exist; the converter updates it in place.

