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

### Fixed

- An unregistered type now throws instead of resolving to `null`, and there is no last-resort
  "take the first binding" fallback.
- Injection no longer recurses without a visited set, so mutually injected singletons complete
  instead of killing the editor with an uncatchable `StackOverflowException`.
- `FromResolve()` returns the instance rather than a delegate wrapping another delegate, and points
  at the concrete type instead of its own contract.
