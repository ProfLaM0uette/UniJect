# UniJect — Roadmap

Eight phases. **Every phase ends on a working library**: stopping after any one of them leaves
something shippable. Test counts are indicative — without them, count roughly 40 % less effort.

The authoritative specification is the design document *Conception UniJect v2*
([artifact](https://claude.ai/code/artifact/338242cf-8e87-4c1a-8ae5-4ec9716b5043)): fifteen
decisions, the frozen public API, the resolution algorithms, the fifteen `UJxxx` validation codes,
and the 46 audited defects of v1. This file is the execution order only.

| # | Phase | Files | Tests | Effort | Status |
| --- | --- | --- | --- | --- | --- |
| 0 | [Foundations](#phase-0--foundations) | 14 | 3 | 0.5 d | **done** |
| 1 | [Vertical slice](#phase-1--vertical-slice) | 68 | 95 | 8 d | **done, tests partial** |
| 2 | [Identity, conditions, validation](#phase-2--identity-conditions-validation) | 26 | 90 | 8 d | **done, tests partial** |
| 3 | [Scope tree](#phase-3--scope-tree) | 20 | 50 | 5 d | **done, tests partial** |
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

**Status — the definition of done is met.** 105 runtime files, 26 test files, 24 EditMode tests and
4 PlayMode tests green, zero compiler warnings. Each clause is covered by a test:
`Resolve_UnboundContract_ThrowsInsteadOfReturningNull`,
`Resolve_UnregisteredNestedDependency_ExposesTheFullResolutionPath`,
`Dispose_OwnedSingletons_ReleasesInReverseCreationOrder`,
`Inject_OnASceneBehaviour_FillsThePrivateInjectField`, and
`ContainerBuilder_PublicSurface_ExposesNoResolveDuringInstall` (reflection over `IContainerBuilder`,
standing in for the compile error until the phase 2 compilation tests exist).

**Deliberately left for later:**

- **Tests: 28 of the planned ~95.** The set that covers the definition of done and the critical
  defects. The rest is deferrable per D-11.
- `DIContainer.Validate()`, `ValidationReport` and the `ValidateOnBuild` graph walk — phase 2 owns
  them, so `CallSite.Dependencies` is declared but never populated and no build-time cycle walk runs.
  Runtime cycle detection is in place and tested.
- `UJ004` (two distinct sources) is counted on the draft but not reported: warnings need the phase 2
  `ValidationReport`.
- The `Container:` and `Installers:` lines of the `BindingNotFoundException` message. The core
  cannot know a `SceneContext` name; that block needs a seam the Unity layer fills, which belongs
  with the phase 7 diagnostics work. Everything else in the message — first line, resolution chain,
  near matches, fix — is there.
- `ContainerBuilderServiceExtensions` (the twelve .NET one-liners) — not in the phase 1 scope list.

**Two deviations from the dossier, both deliberate:**

- `PreserveAttribute` is **not** `sealed`. Both `02-API.md` and `EN-12` declare it `sealed` *and*
  derive `InjectAttributeBase` from it; that does not compile. Dropping `sealed` is the only reading
  that preserves the stated intent (every `[Inject]` site preserves itself under IL2CPP).
- `DEVELOPMENT_BUILD` is replaced by `DEBUG` in `ContainerOptions.Default` and `MainThreadGuard`.
  Unity 6.6 deprecates the symbol (`warning UAC0009`) and points at the managed-code-variant
  directives. `DEBUG` is defined in the editor and in development builds, so the semantics the
  dossier asks for are unchanged.

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
advisory), D8 (no cycle detection at all), D31 (`WithId` not counted as identity).

**Status — the definition of done is met.** 121 runtime files, 37 test files, 52 EditMode tests and
4 PlayMode tests green, zero compiler warnings. Each clause has its test:
`Resolve_TwoUnconditionalBindingsOfOneContract_ThrowsShowingBothCandidates` (and asserts the
`file:line` of both candidates is in the message),
`Resolve_WhenInjectedIntoAnotherConsumer_ThrowsInsteadOfHandingItOver`,
`Build_WithValidateOnBuildAndAConstructorCycle_ThrowsWithThePath`, and
`Validate_AHealthyGraph_ReturnsAnEmptyReport` paired with
`Validate_AHealthyGraph_InstantiatesNothing`.

`CallSite.Dependencies` is now populated: `CallSiteFactory` recurses through constructor parameters
and every member site, which is what makes both build-time validation and build-time cycle detection
possible. The lifetime-aware cycle policy is in: an all-constructor cycle is always an error, a
member-edge cycle between non-transient registrations is allowed because the cache write in
`CreateAndStore` precedes injection.

**Deliberately left for later:**

- **Tests: 56 of the planned ~185 for phases 1 and 2 together.** The set that covers both definitions
  of done and the critical defects.
- `BindInterfacesTo` / `BindInterfacesAndSelfTo` — listed under this phase in the summary but not in
  `08-PLAN.md`'s scope paragraph, and one `Registration` carrying N contracts already works. The two
  verbs are sugar over it; they land with the `ContainerBuilderServiceExtensions` batch.
- `CaptiveDependencyValidator` and `ValidateScopes` — phase 3 owns them, so `ValidateScopes` is a
  flag nothing reads yet.
- `UJ004`, `UJ010`, `UJ015` — the warning and info codes. Only errors are reported so far;
  `ValidationReport` already carries severity and counts them.

**Two deviations from the dossier, both forced by C#:**

- `CompositeCondition` is split in two. The tree says it should be "`IStaticCondition` iff every part
  is", which no single C# type can express. `CompositeCondition` implements `IStaticCondition`,
  `DynamicCompositeCondition` does not, and `CompositeCondition.Create` picks. Every other site keeps
  the single `is IStaticCondition` test.
- `WhenInjectedInto<T>()` and `WhenNotInjectedInto<T>()` are instance methods on `Binder<,>`, not
  extension methods. As extensions they would need all three type arguments spelled out at the call
  site, which C# requires all-or-nothing. The `params Type[]` overloads stay extensions.

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

**Status — the definition of done is met for everything a test can reach.** 130 runtime files, 49
test files, 63 EditMode and 11 PlayMode tests green, zero compiler warnings. Covered:
`Resolve_AParentSingletonFromTwoChildren_ReturnsTheSameInstance`,
`Resolve_AChildBindingThatShadowsTheParent_PrefersTheNearestScope`,
`AsScoped_ResolvedFromTwoChildren_GivesEachChildItsOwnInstance`,
`Dispose_AChild_ReleasesOnlyItsOwnInstances`, `Dispose_AParent_DisposesItsChildrenFirst`,
`Build_ASingletonDependingOnAScopedService_IsACaptiveDependencyError` (UJ012),
`Awake_TwoSceneContexts_GetSeparateContainersUnderOneProjectRoot`, and the three
`UniJectStaticsResetTests`.

**Two real bugs the scope tree exposed in the phase 1 and 2 code**, both found by writing the tests
rather than by reading:

- **Store slots were numbered per container.** A `Singleton` declared in a child was stored in the
  root's array at the child's index, and a `Scoped` declared in a parent was stored in the child's
  array at an index past its end — an `ArgumentOutOfRangeException` the moment a scope tree exists.
  Slots are now numbered continuously across the tree, and a `CallSite` carries the container that
  *declares* the registration, so a singleton is cached where it was declared rather than blindly at
  the root.
- **The cycle chain was per container.** A resolution crossing a scope boundary split its chain in
  two, so a cycle passing through a parent could not be seen. The chain is now shared down from the
  root, which is correct because resolution is single-threaded by contract.

**One deviation from the dossier.** `MonoInstallerBase.IsEnabled` returns
`enabled && gameObject.activeInHierarchy` rather than `isActiveAndEnabled`. They mean the same
thing, but `isActiveAndEnabled` is still false for a component added to an inactive GameObject at
the moment `SceneContext.Awake` runs — `SceneContext` has execution order −9999, so it awakes before
the installer's own `OnEnable`. The explicit form keeps the intent the dossier asks for (an
installer on an inactive GameObject does not install) without depending on a transient flag.

**Deliberately left for later:**

- **Tests: 74 of the planned ~235 across phases 1 to 3.**
- `OnBuilt` callbacks run, but nothing in the Unity layer uses them yet.
- `ProjectContextSettings` is discovered with `Resources.FindObjectsOfTypeAll`, which picks up
  Preloaded Assets without a `Resources/` folder, as the dossier requires. There is no editor tooling
  yet to create the asset or add it to Preloaded Assets — that is phase 7's settings provider.
- The full additive-scene scenario is exercised through two `SceneContext` instances sharing one
  project root, not through two genuinely additively-loaded scenes. `SceneScopeRegistry` is keyed by
  the `Scene` struct and tested, so the remaining gap is the scene-loading harness, which belongs
  with the phase 4 PlayMode fixture.
- The "enter Play three times without editing" protocol is covered structurally by
  `UniJectStaticsResetTests` rather than by three real Play sessions.

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
