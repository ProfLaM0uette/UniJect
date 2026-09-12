# UniJect — Roadmap

Eight phases. **Every phase ends on a working library**: stopping after any one of them leaves
something shippable. Test counts are indicative — without them, count roughly 40 % less effort.

The authoritative specification is the design document *Conception UniJect v2*
([artifact](https://claude.ai/code/artifact/338242cf-8e87-4c1a-8ae5-4ec9716b5043)): fifteen
decisions, the frozen public API, the resolution algorithms, the fifteen `UJxxx` validation codes,
and the 46 audited defects of v1. This file is the execution order only.

| # | Phase | Files | Tests | Effort | Status |
| --- | --- | --- | --- | --- | --- |
| 0 | [Foundations](#phase-0--foundations) | 14 | 3 | 0.5 d | in progress |
| 1 | [Vertical slice](#phase-1--vertical-slice) | 68 | 95 | 8 d | not started |
| 2 | [Identity, conditions, validation](#phase-2--identity-conditions-validation) | 26 | 90 | 8 d | not started |
| 3 | [Scope tree](#phase-3--scope-tree) | 20 | 50 | 5 d | not started |
| 4 | [GameObjects, prefabs, placement](#phase-4--gameobjects-prefabs-placement) | 27 | 60 | 8 d | not started |
| 5 | [Factories and arity families](#phase-5--factories-and-arity-families) | 45 | 45 | 5 d | not started |
| 6 | [Tickables and PlayerLoop](#phase-6--tickables-and-playerloop) | 9 | 18 | 2.5 d | not started |
| 7 | [Diagnostics and editor tooling](#phase-7--diagnostics-and-editor-tooling) | 15 | 25 | 8 d | not started |
| 8 | [Extraction and release](#phase-8--extraction-and-release) | — | — | 0.5 d | not started |
| | **v1.0 total** | **~224** | **~386** | **~45 d** | |

**If the scale becomes a problem**, the profitable cut is phases 0 to 5 with no tickables and no
editor window: about 20 days, and the container is complete on bindings and factories.

---

## Phase 0 — Foundations

Package layout, six assemblies, the C# 9 rules, a CI that runs without a Unity licence.

**Lands**

- `Packages/com.lam0uette.uniject/` as an embedded package, `package.json` floored at Unity 6000.3.
- Six assembly definitions: core, Unity adapter, Editor, and one test assembly per production
  assembly. The core is `noEngineReferences: true` with `references: []`.
- `.claude/rules/csharp.md` — the global rules plus the Unity C# 9 amendment.
- `.gitignore`, root `README.md`, package `README.md`, `CHANGELOG.md`, `LICENSE.md`.
- The host project stripped to a bare shell: URP removed (Built-in RP, no render pipeline asset),
  one scene `Assets/Scenes/Sandbox.unity` with a camera and a light, and the template packages gone.
  63 resolved packages down to 46. Kept beyond the built-in modules: `test-framework`, `ide.rider`,
  and `pipeline` + `inputsystem`, which the Unity CLI needs.

**Definition of done**

- ✅ Unity imports the project with zero errors and lists the six assemblies (verified with
  `-quit -batchmode`, exit code 0).
- ✅ The embedded package resolves and `testables` makes its test assemblies visible.
- `LaM0uette.UniJect.Core.Tests` compiles while referencing the core assembly only — provable once
  phase 1 puts a script in it.
- A test project pinned to `LangVersion 9.0` turns CI red if a `[]` collection expression appears.
- CI runs the core tests with no Unity licence.

**Remaining**: the `LangVersion 9.0` guard project and the CI workflow.

---

## Phase 1 — Vertical slice

One container you can ship a scene with. This is the phase where the v1 bugs lived, so it is the
one phase whose tests are not deferrable.

**Lands**

- `Registration` — eight read-only members, nothing writes to it after `Build()`.
- `IContainerBuilder` — mutable, alive only during construction. No `Resolve`, no `CreateInstance`,
  no `Inject`. `Bind<T>()` is an extension method, so installer code compiles byte for byte.
- `IActivator` — how an instance comes into existence, knowing nothing about lifetime.
- `CallSiteFactory` — selection, recursion, memoization. One IR class, no visitor.
- `DIContainer` + `Ownership` — the activator declares ownership, the tracker executes, release runs
  in reverse creation order.
- `ReflectionInjectionPlanProvider` — the single file in the library that touches reflection.
- `MonoInstaller`, `SceneContext`, `[Inject]`, `[InjectOptional]`, `TryResolve`.

**Fixes** D1 (unregistered type resolves to `null`), D4 (recursive `InjectAll` with no visited set,
giving an uncatchable `StackOverflowException`), D5 (`FromResolve()` returns a `Func<object>`), D7,
D9, D10, D35.

**Definition of done**

```csharp
public override void Install()
{
    Container.Bind<ILogger, UnityLogger>().AsSingleton();
    Container.Bind<GameState>().AsSingleton().NonLazy();
    Container.Bind<IClock>().FromMethod(resolver => new SystemClock()).AsSingleton();

    // Container.Resolve<IClock>();   <- must no longer compile, and that is the point
}
```

Plus: a scene `MonoBehaviour` reads a non-null `[Inject] private ILogger` in its `Start()`, a
missing binding throws with the full resolution chain, and `Dispose()` releases in reverse creation
order.

---

## Phase 2 — Identity, conditions, validation

**Lands**

- `WithId`, counted as part of the identity, so an id-carrying binding can no longer satisfy an
  id-less request.
- The `When*` family, accumulating as AND: `When`, `WhenInjectedInto<T>`,
  `WhenInjectedIntoInstance`, and their `WhenNot*` counterparts.
- `ResolveAll`, `BindInterfacesTo`, `BindInterfacesAndSelfTo`, `.IfNotBound()`.
- Cycle detection with the chain printed.
- `Validate()` and the fifteen `UJ001`-`UJ015` codes.

**Precedence, stated once**: the id is part of the identity; the nearest container wins; a binding
whose condition is satisfied beats an unconditional one. Everything else is an error. No
specificity score — two conditional bindings that both match is an exception, not a contest.

**Fixes** D2 (the "as a last resort, take the first binding" fallback, which made every condition
advisory), D8, D31.

---

## Phase 3 — Scope tree

**Lands**

- `ProjectContext`, parent/child containers, `Lifetime.Scoped`.
- Additive scene support.
- The static reset table — domain reload is off by default in 6.6, so every static cache declares
  its reset or it survives Play.
- `IInitializable`.

Adding a third lifetime later is the change that cannot be made, which is why scopes land before
the GameObject work.

---

## Phase 4 — GameObjects, prefabs, placement

**Lands**

- Sources: `FromNewGameObject`, `FromNewPrefab`, `FromNewPrefabResource`,
  `FromNewComponentOnGameObject`, `FromComponentInHierarchy`, `FromComponentOnGameObject` — each
  now meaning one distinct thing.
- Placement verbs merging into a single `GameObjectPlacement`: `Name`, `Parent`, `Root`,
  `DontDestroyOnLoad`, `Transform`, `Position`, `Rotation`, `Scale`, with a `Space` property
  replacing the world-then-local double write.
- Mono safety enforced by the type system: the placement verbs are extension methods constrained
  `where TConcrete : Component`, so `Bind<Foo>().Name("x")` on a plain class fails to compile.
- The disable / instantiate / inject / re-enable dance, since `Object.Instantiate` runs `Awake` and
  nothing changes that.

**Fixes** D3, the critical one: `BindInfo.Transform` was initialised with `new()`, so the
`!= null` guard was dead and every created GameObject came out at scale 0 with an invalid
quaternion, overwriting each prefab's authored transform. In v2, *not set* means *do not touch*.
Also D13, D18, D21, D22, D23, D32, D33.

**Silent behaviour changes** — these produce no compile error and lead the migration guide: an
unconfigured Mono binding now creates instead of hijacking an existing component;
`FromComponentOnGameObject` only searches and throws if absent while
`FromNewComponentOnGameObject` always adds; `FromComponentInHierarchy` throws instead of quietly
creating.

---

## Phase 5 — Factories and arity families

**Lands**

- `IFactory`, `IParams`, `BindPlaceholderFactory`, `IInjectable` — arity capped at **6** for
  factories and params, **8** for `IInjectable`.
- The 32 remaining arity files generated by a hand-run console executable, output committed. Not a
  Roslyn source generator: if the generator dies, 32 trivial files are editable by hand.
- `Factory`, `ParamsFactory`, `BaseFactory` and `ProductInitializer` — 51 types, ~2 400 lines —
  deleted in favour of two activators.

**Fixes** D6 (`method as Func<TProduct>` is a contravariant conversion that does not exist, so it
was always `null` and every `BindFactory<T>().FromMethod(...)` silently ignored its lambda), D14,
D16, D24, D25, D30.

---

## Phase 6 — Tickables and PlayerLoop

`IInitializable`, `ITickable`, `IFixedTickable`, `ILateTickable` driven by one PlayerLoop node.

N `Update()` methods means N native-to-managed transitions per frame in undefined order; one
PlayerLoop node means one transition, in deterministic order.

**Definition of done**: an `ITickable` written in pure C# ticks without any `MonoBehaviour`.

---

## Phase 7 — Diagnostics and editor tooling

**Lands**

- The container window and the injection observer.
- The validation gate that blocks entering Play mode.
- AOT apparatus: `link.xml` generation, and the `PreserveAttribute` trick — the Unity linker honours
  any attribute named exactly `PreserveAttribute` regardless of namespace, so deriving
  `InjectAttributeBase` from a home-made one makes every `[Inject]` site preserve itself.
- The CI test that forbids `Expression.Compile()` and IL emit outright.

Diagnostics are a feature, not an afterthought: 90 % of real failures are a typo, a forgotten
`.WithId`, an interface bound to the wrong contract, or a condition that silently excluded
everything. Every exception prints the full resolution chain and a *near matches* block giving each
candidate's rejection reason, and every binding carries its own `file:line` via `[CallerFilePath]`
at zero runtime cost.

**Corollary**: the core never logs, it throws.

---

## Phase 8 — Extraction and release

Git repository, tag `v1.0.0`, MGA consumes the package by URL.

---

## Decisions already locked

| | Decision | Resolution |
| --- | --- | --- |
| D-01 | `[]` does not compile under Unity | Local `.claude/rules/csharp.md` carries the C# 9 amendment. The intent of the global rule — never a spread, always materialize — is preserved in full. |
| D-02 | Where the project lives, on which version | This dedicated Unity 6000.6.0f1 project, package floored at `"unity": "6000.3"` (LTS) so MGA stays compatible. The only 6.3 / 6.6 difference that matters is `FindObjectsSortMode`, isolated behind the library's single `#if`. |
| D-03 | `MGA.UniJect` becomes `LaM0uette.UniJect` | Applied. Package id `com.lam0uette.uniject`; the names `UniJect` and `DIContainer` are kept. A find-and-replace reverts it. |

The twelve remaining decisions — arity caps, removing `Resolve` during `Install`, throwing instead
of returning `null`, the Mono binding changes, what is added, what is out of scope, deferred tests,
`.meta` GUIDs, deleting the demos, diagnostics, and the scale — are argued in full in the design
document.
