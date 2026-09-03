# Fonts — GUI Pro: Casual Game

This pack ships two different kinds of TextMeshPro font asset. They are **not** interchangeable,
so please read this before swapping fonts or regenerating atlases.

## 1. `LilitaOne-Regular SDF` — true SDF

| | |
|---|---|
| Render mode | `SDFAA` (Distance Field) |
| Atlas | Embedded, single channel |
| Shader | `TextMeshPro/Distance Field` |
| Scaling | Resolution independent — use at any size |
| Outline / shadow | Add them yourself via the material (Outline, Underlay) |

Use this one for general-purpose text.

## 2. `LilitaOne-Regular Outline <size> Bitmap` — pre-baked raster

| | |
|---|---|
| Render mode | `SMOOTH` / `SMOOTH_HINTED` (**raster, not a distance field**) |
| Atlas | External RGBA colour PNG (`... Bitmap Atlas.png`) |
| Shader | `Layer Lab/TMP Bitmap Custom Atlas (Clip Fix)` |
| Scaling | **Size specific** — pick the variant closest to your point size |
| Outline / shadow | Already baked into the atlas pixels |

The thick cartoon outline and drop shadow of this style cannot be reproduced cleanly with SDF
material properties, so each one is baked at a fixed point size. Variants are provided at
32 / 40 / 50 / 54 / 64 / 72 / 120 / 210 pt, each in a Basic Latin and an `Extended ASCII` flavour.

> **Note on naming.** Before v1.x these assets were named `... Outline <size> SDF`, because that is
> the default suffix TMP's Font Asset Creator appends. They were never distance-field assets. The
> suffix is now `Bitmap` to match the shader they actually use. Asset GUIDs are unchanged, so
> existing scenes and prefabs keep working after the update.

### Why the custom shader?

These assets use a bitmap-family shader. Unity's stock `TextMeshPro/Bitmap Custom Atlas` shader
computes its `RectMask2D` clipping from the diagonal terms of the projection matrix only:

```hlsl
pixelSize /= abs(float2(_ScreenParams.x * UNITY_MATRIX_P[0][0], _ScreenParams.y * UNITY_MATRIX_P[1][1]));
```

Under Android Vulkan **pre-rotation** a 90° rotation is composed into the projection matrix, those
two terms become 0, and the resulting division by zero makes the text fully transparent — but only
inside a `RectMask2D`, and only in a player build. Unity's own `TMP_SDF.shader` already uses the
safe form, which is why SDF text was never affected.

`Layer Lab/TMP Bitmap Custom Atlas (Clip Fix)` is a copy of Unity's shader with that one line
changed to the safe form:

```hlsl
pixelSize /= abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));
```

It lives in `ResourcesData/Shader & Materials/` and ships with the pack, so no edits to
TMP Essentials are required. Everything else about the shader is identical to Unity's.

## Regenerating a font asset

If you regenerate one of the `Bitmap` variants with TMP's Font Asset Creator, note that:

- choosing an `SDFAA` render mode discards the baked outline and shadow — you would have to
  rebuild them from material properties, and the result will not match this art style;
- a regenerated asset is bound to Unity's stock bitmap shader again, which reintroduces the
  Android clipping bug above. Re-assign `Layer Lab/TMP Bitmap Custom Atlas (Clip Fix)` afterwards.
