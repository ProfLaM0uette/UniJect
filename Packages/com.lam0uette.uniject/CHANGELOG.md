# Changelog

All notable changes to this package are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project
adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Package skeleton: `package.json` with an empty `dependencies`, six assembly definitions,
  `link.xml`, vendored JetBrains annotations, the C# 9 rules, and the licence-free `dotnet` test
  project.
- Two-phase construction: `IContainerBuilder` declares, `BuildPipeline` finalises, `DIContainer`
  resolves. The builder exposes no `Resolve`, so installer order no longer matters.
- Binding entry points `Bind<T>`, `Bind<TContract, TConcrete>` and `BindInstance`, each carrying its
  own `file:line`.
- Sources `FromNew`, `FromInstance`, `FromMethod` and `FromResolve`, lifetimes `AsSingleton` and
  `AsTransient`, and the eagerness verbs `NonLazy` and `Lazy`. Every verb returns the same binder,
  in any order.
- Cached reflection injection: constructors, fields, properties and `[Inject]` methods, base class
  first, with one plan per type held for the lifetime of the domain.
- `[Inject]`, `[InjectOptional]` and a home-made `PreserveAttribute` that makes every injection site
  survive IL2CPP stripping without an engine reference.
- Deterministic release: `Ownership`, a disposal tracker that runs in reverse creation order, and a
  liveness policy that keeps a destroyed `Component` from being served from cache.
- Diagnostics: twelve exception types, all carrying structured data, all printing the full
  resolution chain and a near-matches block.
- Unity layer: `SceneContext`, `MonoInstaller`, scene-scoped injection with `SceneInjectionMode`,
  and the Unity releaser and liveness policy.
- `WithId`, which makes the id half of a binding's identity: a request without an id can no longer
  be served by a binding that carries one, and the reverse.
- The condition family — `When`, `WhenInjectedInto`, `WhenInjectedIntoInstance`, their `WhenNot*`
  counterparts and `IfNotBound` — accumulating as AND. Conditions that can be answered at
  build time cost nothing at resolution; the others keep their call site out of the cache.
- Collection contracts: an `IEnumerable<T>`, `IReadOnlyList<T>`, `IList<T>`, `List<T>` or `T[]`
  dependency receives every binding of `T`, innermost container first, conditions still filtering.
  No match is an empty collection, never an exception.
- `Validate()` and `ValidateOnBuild`, walking the whole dependency graph by type and instantiating
  nothing. Failures are collected into a `ValidationReport` rather than thrown one at a time.
- Build-time cycle detection with the path printed, aware of lifetimes: a constructor cycle is
  always an error, a member-injection cycle between non-transient bindings is allowed.
- A scope tree: `CreateChildBuilder()`, `AsScoped()`, and the rule that the nearest container wins.
  A singleton is cached where it was declared, a scoped service once per container that resolves it.
  Disposing a container disposes its children first and never touches its parent's instances.
- `ProjectContext`, a root container created before the first scene loads, configured by a
  `ProjectContextSettings` asset in Preloaded Assets and its `ScriptableObjectInstaller` list.
- `SceneContext` now builds a child of the project container, or of another scene context for
  additive scenes, and registers itself in a scene-keyed registry for the duration of the scene.
- `IInitializable`, run after the eager-loading and scene-injection passes in creation order.
- `UJ012`: a singleton that depends on a scoped service is reported as a captive dependency when
  scope checking is on.
- A statics reset table that runs at subsystem registration, so entering play mode repeatedly with
  domain reload disabled behaves identically every time.
- GameObject sources, all constrained to `Component` at compile time: `FromNewGameObject`,
  `FromNewPrefab`, `FromNewPrefabResource`, `FromNewComponentOnGameObject`,
  `FromComponentInHierarchy` and `FromComponentOnGameObject`.
- Placement verbs `Name`, `Parent`, `Root`, `DontDestroyOnLoad`, `Transform`, `Position`, `Rotation`
  and `Scale`, usable before a `From*` verb, after it, or with no `From*` verb at all.
- A component spawned by the container reads its injected dependencies inside `Awake`: the host is
  frozen before the component is added and re-activated only once injection and placement are done.

### Fixed

- A created GameObject no longer comes out at scale zero with an invalid rotation, and a prefab
  keeps its authored transform. Every pose component is nullable and an unset one is never written.
- `FromComponentOnGameObject` and `FromComponentInHierarchy` search only and throw when nothing
  matches, where they used to create silently. `FromNewComponentOnGameObject` always adds.
- Destroying a container destroys the GameObjects it created and nothing else: a component it merely
  found on a scene object is never destroyed.

### Added

- Factories: `BindFactory`, `BindFactoryTo`, `BindPlaceholderFactory` and `BindPlaceholderFactoryTo`,
  each up to six parameters, with the product-first generic order preserved.
- The `IFactory`, `PlaceholderFactory`, `IParams`, `Params` and `IInjectable` families, emitted by a
  hand-run generator whose output is committed. Re-running it produces no change.
- Placement verbs on a placeholder factory whose product is a `Component`, so a spawned product can
  be named, parented and posed from the binding.
- `UJ013`: a factory whose parameter list matches no constructor of its product is reported when the
  container is built, not when something first calls `Create`.

### Fixed

- `BindFactory<T>().FromMethod(...)` now calls the delegate it was given. The v1 cast to
  `Func<TProduct>` was a conversion that cannot exist, so it was always null and every such binding
  silently ignored its lambda.
- `IParams` is flat instead of an inheritance tower, so an `IParams<string, int, float>` can no
  longer satisfy a binding that asked for `IParams<string, int>`.
- `IInjectable<...>` is recognised by exact open-generic match against the generated type list,
  never by a name prefix, so a user interface that merely starts with `IInjectable` is left alone.

### Fixed

- An unregistered type now throws instead of resolving to `null`, and there is no last-resort
  "take the first binding" fallback.
- Injection no longer recurses without a visited set, so mutually injected singletons complete
  instead of killing the editor with an uncatchable `StackOverflowException`.
- `FromResolve()` returns the instance rather than a delegate wrapping another delegate, and points
  at the concrete type instead of its own contract.
