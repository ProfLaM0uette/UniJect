# FAQ

## Why does `.Name(...)` not exist on my binder?

Because your concrete type is not a `UnityEngine.Component`. The placement verbs — `Name`, `Parent`,
`Root`, `Transform`, `Position`, `Rotation`, `Scale`, `DontDestroyOnLoad` — are extension methods
constrained `where TConcrete : Component`. `Bind<PlainClass>().Name("x")` is a compile error on
purpose; in v1 it compiled and threw at runtime.

If you meant to bind a component, make sure the *concrete* type is the component:
`Bind<IWeapon, SwordBehaviour>()`, not `Bind<IWeapon>()`.

## Why can't I call `Resolve` inside `Install()`?

`Container` there is an `IContainerBuilder`, which has no `Resolve`, no `CreateInstance` and no
`Inject`. That is deliberate: it is what makes the order of your installers irrelevant. If an
installer could resolve, it would depend on other installers having run first.

If you genuinely need the built container, use `builder.OnBuilt(container => ...)`, which runs after
`Build()` and before the non-lazy pass.

## Why does resolving throw instead of returning null?

Because a `null` surfaces hundreds of stack frames later as a `NullReferenceException` in gameplay
code, with nothing pointing at the real cause. When you actually want optional, say so:
`TryResolve`, `[InjectOptional]`, or `GetService()` which returns `null` per the documented
`IServiceProvider` contract.

## My two bindings of the same contract throw. Why doesn't the first one win?

Because "first one wins" turns into a lottery the moment installers sit in an inspector array across
two additive scenes. Give one an id, or narrow one with `WhenInjectedInto<T>()`. An ambiguity is an
error you can see, not a coin toss you cannot.

## Does the order of verbs in a chain matter?

No. Every verb returns the same binder and writes to the same draft; `Build()` validates the result.
`.AsSingleton().FromNew()` and `.FromNew().AsSingleton()` are identical. The only "last one wins"
rules are between two sources, and between two lifetimes.

## Is my component injected before `Awake`?

Yes, when the container creates it. The host is created or instantiated inactive, injected, placed,
then activated — so `Awake` fires with everything already in place. A component that was already in
the scene is injected by the scene pass instead, which runs after the container is built but
after that component's own `Awake`.

## Why is my scene `MonoBehaviour` not injected?

Check three things: the `SceneContext` is in the same scene, its injection mode is `WholeScene` (or
the object has a `UniJectInjectRoot` marker when the mode is `MarkedRootsOnly`), and the field
carries `[Inject]`. Scene injection is scoped to one scene on purpose — it never sweeps other
additive scenes or the DontDestroyOnLoad scene.

## What happens to my objects when a scene unloads?

The scene's container is disposed: children first, then its own instances in reverse creation order.
It destroys the GameObjects it created and nothing else — a component it merely found on a scene
object is left alone. Project-scope singletons are untouched.

## Do I need a ProjectContext?

Only if you want bindings shared across scenes. It always exists, and is empty and invisible until
you configure it: create a `ProjectContextSettings` asset from
`Create > UniJect > Project Context Settings`, add your `ScriptableObjectInstaller`s to it, and make
sure it is in Preloaded Assets — the button in `Project Settings > UniJect` does that for you.

## Is it fast?

Reflection is cached per type for the lifetime of the domain, call sites are memoized, and there is
no LINQ on the resolution path. The container is immutable once built. The expensive part of DI —
analysis — happens at `Build()`, not per resolution.

There is no benchmark suite and no consumer has been profiled, so treat that as design intent rather
than a measurement.

## Can I use it from a background thread?

No. Resolution is single-threaded by contract, and `MainThreadGuard` asserts it in the editor and in
development builds.
