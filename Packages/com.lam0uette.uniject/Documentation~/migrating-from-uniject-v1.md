# Migrating from UniJect v1

Most of the syntax is unchanged: `Bind<T>()`, `Bind<TInterface, TImplementation>()`, the `From*`
verbs, `Name` / `Parent` / `Root` / `Position` / `Rotation` / `Scale`, `AsSingleton`,
`AsTransient`, `NonLazy`, `WithId`, the `When*` family, `IFactory`, `PlaceholderFactory`,
`IParams`, `IInjectable`, `MonoInstaller.Install()`, `SceneContext.Container`, `DIContainer`.
Chains that compiled before still compile.

Read this page anyway, because the first section changes behaviour **without producing a single
compile error**.

## Silent behaviour changes — read these

**An unregistered type throws.** It used to resolve to `null`, and as a last resort take the first
binding it found with a warning. Both are gone. Wherever you relied on a `null`, say so explicitly:
`TryResolve`, `[InjectOptional]`, or `GetService()`.

**Conditions are now binding.** Because the "take the first binding" fallback is gone, a binding
restricted with `WhenInjectedInto<Foo>()` is no longer handed to `Bar`. It throws.

**`WithId` is part of the identity.** A request without an id can no longer be served by a binding
that carries one, and the reverse. In v1 this worked by declaration order, so swapping two lines
silently changed which instance you got.

**An unconfigured Mono binding creates instead of hijacking.** It used to find an existing component
and take it over, discarding `Name` / `Parent` / `Transform`.

**`FromComponentOnGameObject` only searches.** It throws when the component is absent, where it used
to add one. Use `FromNewComponentOnGameObject` when you want it added — that one always adds.

**`FromComponentInHierarchy` only searches.** It throws instead of quietly creating.

**Transforms are no longer overwritten.** `BindTransform` components are nullable and an unset one
is never written. In v1 every created GameObject came out at scale zero with an invalid quaternion,
and every prefab lost its authored transform. If you were compensating for that somewhere, remove
the compensation.

## Compile errors you will get, and the fix

**`Container.Resolve<T>()` inside `Install()`** no longer compiles: `Container` is an
`IContainerBuilder`. Use `builder.OnBuilt(container => ...)` if you really need the built container.

**`.Name("x")` on a non-component** no longer compiles. It used to compile and throw at runtime.

**`Resolve<T>(target, id)`** lost its two parameters. `target` is supplied by the machinery now; use
`ResolveId<T>(id)` for the id.

**`Scope.Singleton` / `Scope.Transient`** became `Lifetime.Singleton` / `Lifetime.Transient`, with
`Lifetime.Scoped` added. "Scope" now means a container in the tree.

**`PlaceholderFactory`'s `BindInfo`** is gone, and so is `IPlaceholderFactory.Initialize(BindInfo)`.
Post-construction configuration of a product is an ordinary `[Inject]` method now.

**`IParams` is flat.** `IParams<T1,T2,T3>` no longer derives from `IParams<T1,T2>`. If you relied on
that assignability, it was satisfying the wrong binding silently.

**Arity caps.** Factories and params stop at 6 parameters, `IInjectable` at 8. Measured against real
usage, the highest in practice was 2.

**The namespace is `LaM0uette.UniJect`**, not `MGA.UniJect`. One find-and-replace.

## What you gain

Cycle detection with the path printed. `Validate()` and a gate that blocks entering play on a broken
graph. Deterministic release in reverse creation order — v1 released nothing, ever. A scope tree with
a project context. `IInitializable` and the tickables. Error messages that name the container, the
installers, the resolution chain and every near match with its `file:line`.

## Suggested order

1. Find-and-replace the namespace.
2. Compile, and fix what the compiler points at — that list is short and mechanical.
3. Turn on `ValidateOnBuild` (it is on by default in the editor) and fix what it reports before
   pressing play.
4. Search your code for anything that treated a resolve result as possibly `null`, and make it
   explicit.
5. Remove any workaround for the scale-zero transform bug.
