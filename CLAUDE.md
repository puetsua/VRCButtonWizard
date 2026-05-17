# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

`vrchat.puetsuaworkshop.buttonwizard` is a Unity **Editor-only** VPM/UPM package (Unity 2019.4+) that generates VRChat avatar toggles. There is no runtime code, no test suite, and no build script — all C# lives under `Editor/` and compiles via the `Puetsua.VRCButtonWizard.Editor` asmdef when Unity opens the project. Iteration is done by editing files and letting Unity recompile.

Entry point in the editor: **Tools → VRChat Button Wizard** (`ButtonWizardWindow.OpenPreferredWindow`, menu priority `1123`).

## Dependencies

- `com.vrchat.avatars` `>=3.1.0 <4.0.0` (VRC SDK3 Avatars) — the package uses `VRCAvatarDescriptor`, `VRCExpressionsMenu`, `VRCExpressionParameters`.
- `com.unity.nuget.newtonsoft-json` `2.0.0` — used only by `ButtonWizardConst` to parse `package.json` for the displayed version string.

## Release

Releases are produced by `.github/workflows/release.yaml` (manual `workflow_dispatch` only). It reads `version` from `package.json`, zips the repo, builds a `.unitypackage` from `.meta` files, and creates a GitHub release tagged with the version. **Bumping the version in `package.json` is the trigger** — there is no separate changelog or tag step.

`ButtonWizardConst.Version` reads `Packages/vrchat.puetsuaworkshop.buttonwizard/package.json` at runtime via a **hardcoded path**. Renaming the package folder will break the in-tool version display (it falls back to `"dev"`).

## Architecture

### Window hierarchy

Two editor windows share state and helpers via a common base:

- `ButtonWizardWindowBase` — abstract-ish base (not `abstract` keyword, but never opened directly). Holds all shared fields (`avatar`, `targetAnimator`, `targetAnimatorController`, `targetProperties`, `vrcParameters`, `vrcTargetMenu`, `menuName`, `parameterName`, `folderPath`, etc.) and all `Show*` field-drawing helpers. Also contains the `CreateToggle` / `CreateToggleClipsOnly` / `CreateToggleAnimatorLayerOnly` / `CreateVrcToggle` orchestration methods.
- `ButtonWizardWindow` — the "simple" UI. Auto-derives `folderPath` from the avatar's prefab/scene location + `/Animations`, hides advanced fields, always calls the full `CreateToggle` + `CreateVrcToggle` flow.
- `AdvancedButtonWizardWindow` — exposes every field individually and lets the user pick which subset to generate (clips only / animator layer only / VRC toggle only / all).

`ButtonWizardPref.AlwaysAdvanced` decides which of the two opens from the menu item. Both windows offer a context menu (`IHasCustomMenu`) to toggle this and to switch to the other window.

When adding a new editable field, add the `Show*` helper to `ButtonWizardWindowBase` (so both windows can reuse it) and call it from the relevant window's `OnGUI`. Don't duplicate field state across the subclasses.

### Toggle generation pipeline

`CreateToggle` in `ButtonWizardWindowBase` wires four utility classes together. The shape matters because each utility writes sub-assets back into the same animator controller asset:

1. `AnimationClipUtil.ToggleCreate` — creates (or reuses, by name match within the folder) an `On`/`Off` `AnimationClip` for each `ToggleProperty`. Numeric/bool values use `AnimationCurve.Constant`; `Object`-reference bindings (`isPPtrCurve`) use `ObjectReferenceKeyframe`.
2. `AnimatorStateUtil.ToggleCreate` — creates two `AnimatorState`s (Show/Hide) as sub-assets of the controller via `AssetDatabase.AddObjectToAsset`.
3. `AnimatorStateMachineUtil.ToggleCreate` — wraps the two states in an `AnimatorStateMachine` with `defaultState = stateOff`, also added as a sub-asset.
4. `AnimatorStateUtil.ToggleLink` — adds the two `If` / `IfNot` transitions between Show and Hide based on the bool parameter.
5. The state machine is wrapped in a new `AnimatorControllerLayer` (named after the menu entry) and appended to the controller; `AnimatorControllerUtil.TryAddParameter` adds the bool parameter (guarded against duplicates).
6. `CreateVrcToggle` calls `VrcExpressionParameterExtension.AddToggle` and `VrcExpressionMenuExtension.AddToggle` to register the parameter and a menu control.

`invertToggle` swaps which clip/state is treated as "on". The default `ToggleProperty` binding is `GameObject.m_IsActive` (see `PropertyNameConst`) — extracted from `AnimationUtility.GetAnimatableBindings` against the avatar root.

The animator controller is resolved from `avatar.baseAnimationLayers[4]` (FX layer) when the user picks an avatar. This index is hardcoded.

### Preferences storage

`ProjectPrefs` is **not** Unity's `EditorPrefs`. It serializes a `PrefData` list-of-key-value-pairs to `Assets/project_pref_VRCButtonWizard.json` (under `Application.dataPath`), so preferences live per-Unity-project, not per-user. Anything new that needs persistence should go through `ButtonWizardPref` → `ProjectPrefs` → `ProjectPrefConst` key constants, not directly `EditorPrefs`/`PlayerPrefs`.

### Localization

`LocalizedTextDataset` is a `partial class` split across `LocalizedTextDataset.cs` (the string field list + `SetLanguage` switch + the `static primary` singleton) and `LocalizedTextDataset.Data.cs` (the actual `English` / `ChineseTraditional` instances). All UI strings go through `Localized.<fieldName>`. To add a new string:

1. Add the field name to the big `public string ...;` block in `LocalizedTextDataset.cs`.
2. Initialize it in **both** language instances in `LocalizedTextDataset.Data.cs`. C# will not warn if you forget — the field will just be `null` at runtime.
3. Add a matching language entry to `SupportedLanguage` and the `SetLanguage` switch if adding a new language.

### VRC-SDK extension methods

`VrcExpressionMenuExtension` and `VrcExpressionParameterExtension` are extension-method holders that wrap `VRCExpressionsMenu` / `VRCExpressionParameters` with idempotent `AddToggle` / `HasToggleWithParameter` helpers. New VRC-SDK interactions should follow the same pattern (extension class per VRC type, idempotent by name/parameter check, `EditorUtility.SetDirty` at the call site, not inside the extension).

## Conventions

- All code lives in namespace `Puetsua.VRCButtonWizard.Editor`.
- Utility classes are `internal static`; window classes and `ToggleProperty` are `public`.
- The `ButtonWizardWindowBase.debug` static flag gates verbose `Debug.Log` output — wrap noisy logs in `if (debug)`.
- `CreateFolderIfNotExist` recursively creates Unity asset folders via `AssetDatabase.CreateFolder` — use it instead of `Directory.CreateDirectory` for anything under `Assets/`, otherwise the folder won't show up in the project window without a refresh.
- The package supports a **legacy folder migration**: `package.json` maps `Assets\PuetsuaWorkshop\VRCButtonWizard` (GUID `7b90b926c73474b45a40da4840c494da`) → this package. Don't change that GUID.
