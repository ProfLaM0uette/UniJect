# UniJect — development project

This repository is the Unity project that hosts and tests **UniJect**, a zero-dependency dependency
injection container for Unity. The shippable artifact is the embedded package:

```
Packages/com.lam0uette.uniject/
```

Everything outside that folder — the URP template scene, the settings, this project's
`ProjectSettings` — exists only to exercise the package. Consumers install the package alone.

| | |
| --- | --- |
| Editor | Unity 6000.6.0f1 |
| Package floor | Unity 6000.3 (LTS), verified green on 6000.6 |
| Language | C# 9.0 |
| Dependencies | none |

The package is floored on the LTS rather than on the editor it is developed with, so that a 6.3
project can consume it. 6.6 is an Update release and loses support when 6.7 ships.

## Getting started

1. Open the project with Unity 6000.6.0f1 (Unity Hub).
2. `Window > General > Test Runner` lists the package's test assemblies — the package is registered
   under `testables` in `Packages/manifest.json`, which is what makes package tests visible.

The host project is deliberately bare: Built-in Render Pipeline, one scene
(`Assets/Scenes/Sandbox.unity`) holding a camera and a light, and four packages beyond the built-in
modules — `test-framework`, `ide.rider`, and the two the Unity CLI needs, `pipeline` and
`inputsystem`. Test fixtures belong in the package's test assemblies, not in `Assets/`.

## Running the tests

### Unity CLI, against the running editor

`com.unity.pipeline` exposes the open editor over a local HTTP API and the `unity` CLI drives it.
This is the fast path — no `Library` lock, no editor boot per run.

```bash
unity command                                  # list every command the editor exposes
unity command recompile                        # recompile, then poll recompile_status
unity command run_tests --filter <TestClass>   # run tests in the live editor
```

### Batch mode, for CI

The editor must be **closed**: batch mode needs the `Library` lock.

```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.6.0f1\Editor\Unity.exe" `
  -runTests -batchmode `
  -projectPath "D:\Projets\Unity\Packages\UniJect" `
  -testPlatform EditMode `
  -testResults "results.xml" `
  -logFile "unity-tests.log"
```

`-testPlatform PlayMode` runs the play-mode assembly instead. `-assemblyNames` narrows to one
assembly, `-testFilter` to a single test.

## Layout

```
Packages/com.lam0uette.uniject/
  Runtime/Core/      LaM0uette.UniJect              engine-free core
  Runtime/Unity/     LaM0uette.UniJect.Unity        the Unity adapter
  Editor/            LaM0uette.UniJect.Editor       window, validation gate, link.xml
  Tests/Core/        LaM0uette.UniJect.Core.Tests   references the core only
  Tests/Unity/       LaM0uette.UniJect.Unity.Tests
  Tests/Editor/      LaM0uette.UniJect.Editor.Tests
  Samples~/
```

The core declares `noEngineReferences: true` and `references: []`. It never learns that
`GameObject` exists; the Unity assembly plugs into it through the seams the core declares
(`IActivatorSource`, `IInstanceReleaser`, `IInstanceLivenessPolicy`, `ITickRegistry`). The core
test assembly referencing nothing but the core is the compiler-enforced proof of that.

## Where to go next

- [ROADMAP.md](ROADMAP.md) — the eight phases, each one leaving a shippable library.
- [CLAUDE.md](CLAUDE.md) — the working rules for this repository.
- [Packages/com.lam0uette.uniject/README.md](Packages/com.lam0uette.uniject/README.md) — the
  library's own README, the one consumers read.

## License

MIT — see [LICENSE.md](Packages/com.lam0uette.uniject/LICENSE.md).
