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
| 4 | [GameObjects, prefabs, placement](#phase-4--gameobjects-prefabs-placement) | 27 | 60 | 8 d | **done, tests partial** |
| 5 | [Factories and arity families](#phase-5--factories-and-arity-families) | 45 | 45 | 5 d | **done, tests partial** |
| 6 | [Tickables and PlayerLoop](#phase-6--tickables-and-playerloop) | 9 | 18 | 2.5 d | **done** |
| 7 | [Diagnostics and editor tooling](#phase-7--diagnostics-and-editor-tooling) | 15 | 25 | 8 d | **done, AOT build unverified** |
| 8 | [Extraction and release](#phase-8--extraction-and-release) | — | — | 0.5 d | **prepared, release is yours** |
| | **v1.0 total** | **~224** | **~386** | **~45 d** | |

**If the scale becomes a problem**, the profitable cut is phases 0 to 5 with no tickables and no
editor window: about 20 days, and the container is complete on bindings and factories.

---

## What is left

The eight phases are done, and so is the first round of follow-up: `[CreateAssetMenu]` plus a
*Create and preload* button in `Project Settings > UniJect` (project-scope bindings now work end to
end), `BindInterfacesTo` / `BindInterfacesAndSelfTo`, the twelve-method .NET façade, and the seven
`Documentation~` pages.

This is what is left after that, checked against the disk rather than against memory, ordered by
what would bite a consumer first. **Nothing here blocks using the package in a personal project**
outside IL2CPP.

### Tests — 147 of the planned ~386

Every definition of done and all six critical defects are covered. The named suites still missing:
`ConstructorSelectorTests`, `InjectionOrderTests`, `RegistrationSelectorTests` (the 14 lattice
cases), `DisposalOwnershipTests`, `ContainerEagerLoadTests`, `InjectableInterfaceTests`,
`ReflectionBudgetTests`, `FileConventionTests`, `ServiceExtensionsTests`, `BindTransformTests`,
`PlacementApplierTests`, `HierarchyComponentActivatorTests`, `MonoBinderCompilationTests`,
`LegacyChainCompilationTests` (the 27 audited chains, word for word),
`SceneContextInstallOrderTests`, `SceneInjectionScopeTests`, `DontDestroyOnLoadTests`,
`TeardownOrderTests`.

**Compiled but never exercised by a test**, so the corners most likely to hold a bug:
`FromNewPrefabResource`, the `Position` / `Rotation` / `Transform` placement verbs,
`DontDestroyOnLoad` at runtime, factories above one parameter, `IParams` of 3 to 6,
`IInjectable` of 2 to 8, `ScriptableObjectInstaller`, `ProjectContextSettings`, genuinely additive
scenes, and the container window and play-mode gate in real use.

### Diagnostics not emitted

- `UJ010` (binding not statically validatable), `UJ011` (a transient `IDisposable` resolved from the
  root), `UJ015` (dead binding). All Info or Warning.
- `UJ002` exists as behaviour — `DuplicateBindingException` does throw — but carries no code and has
  no test.

### AOT

- `AotConformanceTests` does not exist and no IL2CPP player build has ever run. `LinkXmlGenerator`
  and `StrippingLevelCheck` are written and compile. Verifying this needs the IL2CPP module for the
  target platform installed, plus a development build with stripping at `Low`.
- **One real hazard was found and fixed by auditing rather than building.** Collections used to be
  materialised with `typeof(List<>).MakeGenericType(...)` plus `Activator.CreateInstance`, which
  throws `ExecutionEngineException` under IL2CPP whenever that exact `List<T>` was never
  instantiated statically — trivially reachable with a value-type element. Interface-shaped
  collections now receive a `T[]` built with `Array.CreateInstance`, with no runtime generic
  construction at all. Asking for a **concrete `List<T>`** is the one shape that still takes the old
  path; it is documented in `il2cpp-and-stripping.md`.
- The only other runtime `Activator.CreateInstance` is the boxed `default(T)` for an unresolved
  `[InjectOptional]` value-type parameter. Not a generic construction, far lower risk.

### CoreCLR (Unity 7)

Nothing to do, as far as can be told without the runtime in hand. `Runtime/` uses no `AppDomain`,
no `Assembly.Load`, no `Marshal`, no `DllImport`, no `unsafe`, and no thread creation — only
mainstream BCL reflection (`GetFields`, `GetProperties`, `GetMethods`, `GetConstructors`,
`GetInterfaceMap`, `GetCustomAttributes`, `SetValue`, `Invoke`) and `ConditionalWeakTable`, all of
which CoreCLR supports better than Mono does. The C# 9 floor only relaxes under a newer compiler.

The two places to re-check when it lands are Unity-side rather than runtime-side: the
`UnityEngine.LowLevel.PlayerLoop` shape used by `PlayerLoopTickPump`, and the domain-reload
semantics the statics reset table depends on. `LinkXmlGenerator` uses
`AppDomain.CurrentDomain.GetAssemblies()`, but it is editor-only.

### Minor

- `InjectionTimelineWindow` — a second window, pure addition.
- `CappedArgumentPool` is written and in the statics reset table but not on the hot path;
  `ReflectionInjector` still allocates its argument arrays. No consumer to profile yet.
- The container window's edit-mode preview reads installers sitting on the `SceneContext`'s own
  GameObject, not its serialised `_installers` array.
- `ContainerWindow` builds its UI in code rather than from `.uxml` / `.uss`.

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
- ✅ A test project pinned to `LangVersion 9.0` turns CI red if a `[]` collection expression appears —
  verified in phase 8 by dropping a probe in and reading back `error CS8773`.
- ✅ CI runs the core tests with no Unity licence: `dotnet test` in `CI~/DotNetTests` runs 87 core
  tests green in 95 ms, and the `core-tests` workflow runs it on every push.

**Phase 0 is fully closed.** Its last two clauses were only demonstrated in phase 8.

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

**Status — the definition of done is met.** 155 runtime files, 54 test files, 63 EditMode and 26
PlayMode tests green, zero compiler warnings. The headline clauses:
`FromNewGameObject_WithNoPoseVerb_LeavesTheTransformAlone` and
`FromNewPrefab_OnATemplateWithAnAuthoredTransform_KeepsIt` are **D3**;
`FromNewPrefab_ASpawnedComponent_ReadsItsDependencyInsideAwake` is the injected-before-`Awake`
guarantee; `Name_WithNoFromVerbAtAll_StillCreatesAndPlacesTheGameObject` and
`ParentThenRoot_LastWriteWins_AndTheObjectEndsAtTheSceneRoot` are the free verb order; and
`PlacementVerbs_AreConstrainedToComponent_SoTheyCannotCompileOnAPlainClass` reads the generic
constraint by reflection, standing in for the compile error.

**How the verb order was actually made free.** The dossier has the placement verbs promote the
source slot, and `FromNew()` set `ConstructorActivatorSource`. Written literally, `.Name("x")`
followed by `.FromNew()` would throw the placement away. Two small decisions fix it without adding
a member to `BindingDraft`: `FromNew()` is the *default* rather than a specific source, so it never
overrides a source already chosen; and the default-source rule fires when the source is absent
**or** is the core's own constructor source, which is what lets `.FromNew()` mean "new GameObject
plus AddComponent" for a `Component` and `new T(...)` for everything else.

**Where the injection actually happens.** `ComponentActivator` splits the six-step dance across
`Create` and `Inject` so that the container's cache write still lands between them — the property
that breaks member-injection cycles. `Create` provides the host, freezes it and adds the component;
`Inject` injects, applies placement and re-activates, which is what makes `Awake` fire with every
dependency already in place. The prefab path injects the whole subtree inside `Create`, because the
component search has to happen after injection.

**Deliberately left for later:**

- **Tests: 89 of the planned ~295 across phases 1 to 4.**
- `UJ008` (`.Parent(t)` with `.DontDestroyOnLoad()`) and `UJ009` are not emitted. Both are warnings,
  and reaching them needs the build pipeline to inspect a `GameObjectSource`, which the core cannot
  do without a validation seam on `IActivatorSource`. That seam belongs with the phase 7 diagnostics
  work, which is also where warnings become visible.
- `InactiveInstantiationScope` is one pooled holder per domain rather than one per container. The
  holder only ever owns an instance between `Instantiate` and the re-parent in the placement step, so
  per-container buys nothing; it is registered in the statics reset table either way.
- `UnityObjectFinder` carries no `#if UNITY_6000_5_OR_NEWER`. The dossier expects one for
  `FindObjectsSortMode`, but the hierarchy search is scene-scoped through `GetRootGameObjects`, so
  `FindObjectsByType` is never called and the conditional has nothing to guard.

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

**Status — the definition of done is met.** 218 runtime files (42 of them generated), 66 test files,
75 EditMode and 29 PlayMode tests green, zero compiler warnings.
`BindFactory_WithFromMethod_ActuallyCallsTheDelegate` counts the invocations, which is **D6**: the
delegate is now `Func<IResolver, TProduct>`, strongly typed, with no cast to fail silently.
`BindFactory_WhoseParameterMatchesNoProductConstructor_ThrowsAtBuild` is UJ013.
`BindPlaceholderFactory_AnOverriddenCreate_IsTheOneThatRuns` keeps `Create` virtual.
`Params_OfThreeArguments_IsNotAssignableToParamsOfTwo` proves `IParams` is flat rather than an
inheritance tower, which is what used to satisfy the wrong binding silently.
`BindPlaceholderFactory_WithParentThenRoot_EndsAtTheSceneRoot` is the placement verbs on a factory.

**The generator is idempotent**, verified by hashing all 42 outputs, re-running it, and diffing:
identical. It takes the package root as its argument —
`dotnet run --project "Tools~/ArityGenerator/ArityGenerator.csproj" -- <package root>`.

**One error in the dossier, found by the compiler.** `BindPlaceholderFactoryTo` is specified as
`where TFactory : TInterface, PlaceholderFactory<TProduct>`. C# requires a class-type constraint to
come first, so that does not compile; the generator emits
`where TFactory : PlaceholderFactory<TProduct>, TInterface`, which means the same thing.

**One addition the dossier's file count omits.** `PlaceholderFactory<TProduct>` is abstract, so
`BindFactory<TProduct>()` — which binds no user factory class — has nothing concrete to hand back
as an `IFactory<TProduct>`. The generator emits an `internal sealed ProductFactoryAdapter` per
arity (7 files) to fill that hole. They are internal, so the public surface is unchanged, and D-04's
deleted public `Factory` class is not resurrected.

**How AOT safety is kept.** No `MakeGenericType`, no `Activator.CreateInstance` on a runtime-built
generic. Each generated `BindFactory` overload closes over its own adapter constructor as a lambda
(`productFactory => new ProductFactoryAdapter<TProduct, P1>(productFactory)`), so every generic
instantiation the container needs is statically present at the call site where the user wrote the
binding.

**One resolution path for every product.** `ActivatorProductFactory` goes through
`DIContainer.ActivateProduct`, which runs the product's `IActivator`. A plain class therefore goes
through `ConstructorActivator`, a `Component` through `ComponentActivator` — placement verbs and
all — and a `FromMethod` product through `DelegateActivator`, with no special-casing anywhere.

**Deliberately left for later:**

- **Tests: 104 of the planned ~340 across phases 1 to 5.**
- `CappedArgumentPool` is written and registered but not yet on the hot path; `ReflectionInjector`
  still allocates its argument arrays. Wiring it in is a contained optimisation with no API change,
  and there is no consumer to profile yet.
- `BindFactoryTo` binds the user's own `IFactory` implementation and lets the container construct
  it, which is all it can mean once the factory class is the user's.

---

## Phase 6 — Tickables and PlayerLoop

`IInitializable`, `ITickable`, `IFixedTickable`, `ILateTickable` driven by one PlayerLoop node.

N `Update()` methods means N native-to-managed transitions per frame in undefined order; one
PlayerLoop node means one transition, in deterministic order.

**Definition of done**: an `ITickable` written in pure C# ticks without any `MonoBehaviour`.

**Status — the definition of done is met, and this phase has its full test count.** 227 runtime
files, 74 test files, 82 EditMode and 35 PlayMode tests green, zero compiler warnings.

Every clause has its test:
`Tick_APureCSharpTickable_TicksOncePerFrameWithNoMonoBehaviour` counts a delta over three frames and
asserts exactly three ticks — no `MonoBehaviour` anywhere in the graph;
`Tick_AThrowingTickable_ReportsTheFailureWithoutStoppingTheOthers` asserts both that the failure is
reported and that the tickable registered after it still ran;
`Tick_ATickableAddedDuringATick_OnlyRunsFromTheNextTick` covers the snapshot rule;
`StartTicking_Once_InsertsExactlyThreeNodes` and
`Reset_AfterRegistration_RemovesExactlyOurThreeNodes` cover the PlayerLoop surgery; and
`Reset_ThenRegisterAgainThreeTimes_NeverStacksExtraNodes` is the "three Play sessions in a row"
protocol, asserting three nodes and one container on every pass rather than two then three.

**The core still never sees `PlayerLoop`, and never logs.** `LifecycleRunner.Tick` takes an
`Action<Exception>` and hands each failure to it; the Unity pump is what calls
`Debug.LogException`. That keeps the "the core throws, it does not log" rule intact while giving
the dossier's "logged and does not stop the others" behaviour, and it is why the failure assertion
can be made in an EditMode test with no console noise.

**Node removal is surgical.** `PlayerLoopTickPump.Reset` walks the current loop and drops only
subsystems whose type is `TickPhase`, `FixedTickPhase` or `LateTickPhase`, then sets the loop back.
It never calls `SetPlayerLoop(GetDefaultPlayerLoop())`, which would silently delete every other
package's nodes. `CountNodes()` exists so a test can assert exactly that.

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

**Status.** 229 runtime files, 12 editor files, 81 test files, 95 EditMode and 39 PlayMode tests
green, zero compiler warnings.

**The three debts left by earlier phases are paid.**

- The `Container:` and `Installers:` lines are in the message. `ContainerOptions` carries a
  `Description` and an `InstallerNames` list that `SceneContext` and `ProjectContext` fill; the
  core reads strings and still knows nothing about scenes.
  `BindingNotFound_WhenTheContainerIsDescribed_NamesItAndItsInstallers` asserts both lines, and
  `BindingNotFound_WithNoDescription_OmitsTheContextBlockEntirely` asserts they vanish when there
  is nothing to say.
- `UJ004`, `UJ008` and `UJ009` are emitted. `IBindingValidation` is the seam: `BuildPipeline` calls
  it while the source is still in hand, collects the issues as warnings, and hands them to the
  container so `Validate()` returns them alongside the graph walk. `GameObjectSource` implements it.
  Warnings never block a build — `Build_AWarningOnly_NeverBlocksTheBuild` pins that.
- `link.xml` is generated. `LinkXmlGenerator` implements `IUnityLinkerProcessor` and writes a
  `<type preserve="all" />` entry for every type carrying an `[Inject]` site, on top of the static
  `link.xml` shipped since phase 0.

**`ForbiddenApiTests` turned out to be an audit of all six earlier phases**, and passed first time:
no `Expression.Compile`, no `Reflection.Emit`, no `DynamicMethod`, no `using UnityEngine` outside
`Runtime/Unity/`, and no `var` anywhere in the runtime.

**One naming hazard found by the compiler.** The core's `BuildPipeline` shadows
`UnityEditor.BuildPipeline` for any file in the `LaM0uette.UniJect` namespace — which is every file
in the package. `StrippingLevelCheck` has to say `UnityEditor.BuildPipeline` explicitly. Worth
knowing before writing more editor code.

**Deliberately left, and what it would take:**

- **The IL2CPP conformance build is unverified.** `StrippingLevelCheck` and `LinkXmlGenerator` are
  written and compile, but proving a StandaloneWindows64 IL2CPP player passes `AotConformanceTests`
  needs the IL2CPP module installed and a real build, which cannot be done from here. This is the
  one clause of the definition of done that is claimed rather than demonstrated.
- `InjectionTimelineWindow` is not written. The container window covers the dossier's listed
  columns; a chronological log is a second window and pure addition.
- `ContainerWindow` builds its `MultiColumnListView` in code rather than from `ContainerWindow.uxml`
  and `.uss`. Same result, one fewer pair of files to keep in sync; moving to UXML later is
  mechanical.
- `PublicApiSnapshotTests` and `PublicApi.Core.approved.txt` are not written. They are most useful
  once the surface stops moving, which is phase 8.
- The window's edit-mode view builds a throwaway container from each `SceneContext`'s installers and
  disposes it immediately, so it lists real registrations with real `file:line` without entering
  play mode. It does not yet read a `SceneContext`'s serialised `_installers` array — only
  installers sitting on the same GameObject.

---

## Phase 8 — Extraction and release

Git repository and tag `v1.0.0`. The dossier also had MGA consume the package by URL; that clause
is void — see below.

**Status — prepared, not released.** The repository already exists at
`git@github.com:ProfLaM0uette/UniJect.git` with the dev project and the package together, so there
is nothing to extract: a consumer installs with
`https://github.com/ProfLaM0uette/UniJect.git?path=/Packages/com.lam0uette.uniject`, which is the
form the package README has documented since day one.

**Landed in this phase**

- The phase 0 promise, finally demonstrated rather than claimed: `dotnet test` in
  `CI~/DotNetTests` runs **87 core tests green in 95 ms with no Unity licence**. That is the
  engine-free core proving itself outside the editor.
- The C# 9 guard verified empirically: a `[1, 2, 3]` collection expression dropped into `Runtime/`
  fails the build with `error CS8773: Feature 'collection expressions' is not available in C# 9.0`.
  The probe was removed straight after.
- Three GitHub workflows. `core-tests` and `package-hygiene` run on every push because they need no
  licence; `package-hygiene` checks that every imported file has a `.meta`, that re-running the
  arity generator produces no diff, and that the core still compiles as C# 9. `unity-tests` runs
  **only** on `workflow_dispatch`, weekly, and on `v*` tags — never on every push, which would burn
  a Personal activation seat.
- `PublicApiSnapshotTests` with `PublicApi.Core.approved.txt`: **136 public core types**, grouped by
  type so the diff is readable. It writes the file on first run and fails, so approving a change is
  a deliberate act.
- The six samples of D-13, each teaching one thing with a one-paragraph README. All 25 scripts were
  compiled against the real package API by staging them under `Assets/` and recompiling, then the
  staging copy was deleted — a sample that does not compile is worse than no sample.

**The MGA half of this phase is void.** The dossier ends with "delete the old library from MGA and
switch its manifest to the git URL", because it assumed MGA was the downstream consumer driving the
rewrite. It is not: MGA was only where the v1 source lived, the starting point this package was
written from. Nothing in MGA is to be touched, now or later, and nothing here depends on it.

**Left to you: tagging.** The work is staged, nothing is committed — committing has been yours each
phase and stays that way. Worth weighing before a `v1.0.0`: the IL2CPP conformance clause is
claimed rather than demonstrated, and the test count stands at 135 of the planned ~386. A `v0.9.x`
would let the package be consumed by URL without freezing a 1.0 contract.

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
