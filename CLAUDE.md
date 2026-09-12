# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this repository is

A Unity 6000.6.0f1 project whose only purpose is to host and exercise **UniJect**, a zero-dependency
dependency injection container. The shippable artifact is the embedded package at
`Packages/com.lam0uette.uniject/`; everything else is scaffolding.

The host project is stripped to the bone: the URP template is gone (Built-in Render Pipeline, no
render pipeline asset), and four packages remain beyond the built-in modules: `test-framework`,
`ide.rider`, and the two the Unity CLI needs (`pipeline`, `inputsystem`). **Keep the manifest that
way.**

`Assets/` holds exactly two things, both deliberate:

- `Assets/Scenes/Sandbox.unity` — an empty scene with a camera and a light, for scratch work.
- `Assets/Demo/` — a working demo scene, `UniJectDemo.unity`, that exercises every binding shape and
  prints what it proves to the console. It is the thing to open when someone asks what the library
  looks like in use, and `Assets/Demo/README.md` maps each printed line back to the binding that
  produced it. **Do not delete it**, and update it when the binding grammar changes.

Nothing else belongs in `Assets/`: test fixtures live in the package's test assemblies, and
compile-only examples live in `Samples~/`, which Unity never compiles.

The library is being rewritten from a v1 that lives in another project (MGA). There is no v1 code
here — this is a clean implementation against a frozen specification, built phase by phase.

## Where the detailed conventions live

| Working on | Read |
| --- | --- |
| C# — language level, types, spacing, namespaces, `#region`, tests | `Packages/com.lam0uette.uniject/.claude/rules/csharp.md` **(this copy is the authority, not the global one)** |
| What to build next, in what order | `ROADMAP.md` |
| Why anything is the way it is | the design dossier at `D:\Unity\Projets\UniJect\docs\` — see the map below |

**The dossier is the specification, and it is decision-complete.** Never invent architecture: the
answer is in one of these twelve documents. Read the one you need before writing code.

| Document | Answers |
| --- | --- |
| `00-DECISIONS.md` | the fifteen decisions and why each went the way it did |
| `01-CONTRAINTES.md` | Unity 6.6, C# 9, IL2CPP, domain reload, fake-null |
| `02-API.md` | the frozen public surface, signature by signature |
| `03-ARCHITECTURE.md` | assemblies, layers, the type model |
| `04-ARBORESCENCE.md` | every file to create, with its role — **the authority on where a file goes** |
| `05-ALGORITHMES.md` | build, resolution, injection, prefabs, cycles, teardown, the statics reset table |
| `06-ERREURS.md` | the exceptions and the fifteen `UJxxx` codes |
| `07-AUDIT-V1.md` | the 46 v1 defects, and what is worth keeping |
| `08-PLAN.md` | the eight phases and each one's definition of done |
| `09-TESTS-DEMOS.md` | the deferrable test plan and the six samples |
| `10-RISQUES.md` | eighteen traps with their mitigation — **re-read at the end of every phase** |

`docs/annexes/EN-12-DEFINITIVE-ARCHITECTURE.md` is the long English version of the whole dossier:
exhaustive signatures, code excerpts, mapping tables. It settles any detail the French files leave
open. The v1 source being replaced is at
`D:\Unity\Projets\MGA\Assets\Plugins\LaM0uette\MGA\Lib\UniJect`.

The same content is also published as an artifact:
<https://claude.ai/code/artifact/338242cf-8e87-4c1a-8ae5-4ec9716b5043>.

## Commands

### The Unity CLI — the default way to work here

`com.unity.pipeline` exposes the **running** editor over a local HTTP API, and the `unity` CLI
(`%LOCALAPPDATA%\Unity\bin\unity.exe`) drives it. This is the primary loop: no `Library` lock to
fight, no editor boot per run, and the server keeps the editor ticking while it is unfocused, so a
compile proceeds even when focus is elsewhere.

**The editor must be OPEN for these**, which is the opposite of batch mode.

```bash
unity command                                     # connect and list every command the editor exposes
unity command recompile                           # trigger a domain recompile
unity command recompile_status                    # poll it until it settles
unity command run_tests --filter <TestClass>      # run tests in the live editor
unity command --project-path "D:/Projets/Unity/Packages/UniJect" <name>   # target a specific project
```

The edit→verify loop is: make one logical change (it may span several files) → `command recompile`
→ poll `command recompile_status` → `command run_tests --filter <TestClass>`.

Run `unity command` with no command name to discover the current verb list rather than guessing at
flags. The auth token is the `evalToken` field of `Library/Pipeline/.unity-pipeline-port`, sent as
`Authorization: Bearer <token>`; the CLI handles that for you.

`com.unity.pipeline` is **load-bearing infrastructure, not a project dependency** — never remove it
from `Packages/manifest.json`. Neither `com.unity.inputsystem`, which `Unity.Pipeline.asmdef`
references unconditionally.

### Batch mode — CI, or when the editor is closed

Batch mode takes the `Library` lock, so **the editor must be closed**, and it pays a full editor
boot every run. Use it for CI and for verifying a clean import, not for the inner loop.

```powershell
# EditMode tests
& "C:\Program Files\Unity\Hub\Editor\6000.6.0f1\Editor\Unity.exe" -runTests -batchmode `
  -projectPath "D:\Projets\Unity\Packages\UniJect" -testPlatform EditMode `
  -testResults "results.xml" -logFile "unity-tests.log"

# PlayMode tests: -testPlatform PlayMode
# One assembly:   -assemblyNames LaM0uette.UniJect.Core.Tests
# One test:       -testFilter "LaM0uette.UniJect.DIContainerTests.Resolve_UnboundContract_Throws"

# Clean-import / compile check, no tests
& "…\Unity.exe" -quit -batchmode -nographics -projectPath "…" -logFile "unity-compile.log"
```

`-runTests` exits non-zero on failure; the detail is in `results.xml` (NUnit 3 XML), not in stdout.
`Temp/UnityLockfile` is the reliable check for whether the editor still holds the project.

**Never text-edit `ProjectSettings/*.asset` or an open scene while the editor is running** — Unity
holds them in memory and silently overwrites the file on its next save.

## Architecture

### The one-way dependency

```
LaM0uette.UniJect            noEngineReferences: true, references: []
   ▲                         registration · activation · resolution · scopes · release
   │                         injection · conditions · validation · DSL · factories · lifecycle
   │
LaM0uette.UniJect.Unity      GameObject sources · placement · contexts · installers
   ▲                         PlayerLoop pump · releaser
   │
LaM0uette.UniJect.Editor     container window · observer · validation gate · link.xml
```

**Nothing points back up.** The core never learns that `GameObject` exists. The Unity assembly
plugs in through seams the core declares: `IActivatorSource`, `IInstanceReleaser`,
`IInstanceLivenessPolicy`, `ITickRegistry`. `LaM0uette.UniJect.Core.Tests` references the core
assembly and nothing else from this package — that is the compiler-enforced proof, so never add a
reference to it to make a test compile.

All six assemblies share `rootNamespace: LaM0uette.UniJect`. The split is a build concern; one
`using` is all a consumer writes.

### The six separated responsibilities

The structural defect of v1 was not a bug, it was confused responsibilities. Keep these apart:

| Responsibility | Type | Rule |
| --- | --- | --- |
| Registration model | `Registration` | Eight read-only members. Nothing writes to it after `Build()`. |
| Construction | `IContainerBuilder` | Mutable, alive only during installation. **No `Resolve`.** |
| Activation | `IActivator` | How an instance comes to exist. Never learns anything about lifetime. |
| Lifetime | `CallSite.CacheLocation` | Data carried by the node, never an `if` inside an activator. |
| Resolution | `CallSiteFactory` | Selection, recursion, memoization. One IR class, no visitor. |
| Scopes and release | `DIContainer` + `Ownership` | The activator declares ownership, the tracker executes, in reverse creation order. |

`MonoInstaller.Container` is an `IContainerBuilder`, not a `DIContainer`: no `Resolve`, no
`CreateInstance`, no `Inject`. That is what makes installer order irrelevant, and it is deliberate —
do not add a resolve path to the builder to make something convenient.

### Invariants that are easy to break

- **The core never logs. It throws.** Every exception carries the full resolution chain plus a
  *near matches* block giving each rejected candidate's reason, and every binding carries its
  `file:line` via `[CallerFilePath]` (zero runtime cost).
- **No silent fallback, ever.** An unregistered type throws. Two bindings tied throws. The v1
  "as a last resort, take the first binding" line is the defect that made every condition
  advisory — never reintroduce it. Optional resolution goes through `TryResolve`,
  `[InjectOptional]`, or `GetService()` returning `null` per the documented `IServiceProvider`
  contract.
- **Precedence is three rules and no more.** The id is part of the identity; the nearest container
  wins; a binding whose condition is satisfied beats an unconditional one. No specificity score.
  Two conditional bindings that both match is an exception, not a contest.
- **Every binder verb returns the same binder**, in any order. There is no narrowing ladder of
  binder types. Mono safety is the only thing the type system still enforces, via extension methods
  constrained `where TConcrete : Component`.
- **Not set means do not touch.** Placement (`Name`, `Parent`, `Transform`, `Position`, `Rotation`,
  `Scale`) uses nullable components. v1 initialised them with `new()`, killed its own `!= null`
  guard, and shipped every GameObject at scale 0 with an invalid quaternion.
- **Reflection lives in exactly one file**, `ReflectionInjectionPlanProvider`. One plan per type,
  cached for the domain's lifetime. No string-keyed reflection.
- **Zero dependency.** The asmdefs declare `references: []` on the core for a reason — v1's
  hard dependency on `JetBrains.Annotations` only compiled because the Rider package happened to
  ship it.

## Platform constraints that are not negotiable

| Fact | What it forces |
| --- | --- |
| Unity 6 compiles **C# 9.0** | No `[]`, no file-scoped namespaces, no `record`, no `init`. See `.claude/rules/csharp.md`. |
| `Expression.Compile()` under IL2CPP | Does not throw — returns an interpreted lambda slower than reflection, which then throws on value types. The editor runs Mono, so the broken path is never the one tested. **Expression trees and IL emit are banned outright.** |
| Domain reload off by default in 6.6 | Every static cache is presumed to survive Play. A forgotten reset only shows up on the *second* Play without an edit. |
| `Object.Instantiate` runs `Awake` | No attribute or wrapper changes this, `InstantiateAsync` included. The only lever is `activeInHierarchy == false` — hence disable / inject / re-enable. |
| Unity fake-null | A destroyed object is `!= null` under `ReferenceEquals`. A cached `Component` singleton resolves to a corpse forever. Liveness goes through `IInstanceLivenessPolicy`. |
| Linker stripping | Any attribute named exactly `PreserveAttribute` is honoured whatever its namespace — deriving `InjectAttributeBase` from a home-made one makes every `[Inject]` site preserve itself. |

## Working in this repository

- **Follow `ROADMAP.md` phase by phase.** Each phase ends on a library that still works; do not
  start a phase's work while the previous one is half-landed, and update the status column when a
  phase closes.
- **Never hand-write `.meta` files or GUIDs.** Unity generates them on import. If a `.meta` is
  missing, open the editor rather than inventing one.
- **New test assembly?** A package's tests are only discovered because
  `Packages/manifest.json` lists `com.lam0uette.uniject` under `testables`. Keep that entry.
- The package version in `package.json` stays `0.x` until phase 8; `CHANGELOG.md` gets the entry in
  the same commit as the change.
