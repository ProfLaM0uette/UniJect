# Diagnostics

Around 90 % of real container failures are a typo, a forgotten `.WithId`, an interface bound to the
wrong contract, or a condition that silently excluded everything. So the error message is the
feature, not decoration.

Every exception prints the full resolution chain, a **near matches** block giving each rejected
candidate's reason, and the `file:line` of every binding involved — carried by `[CallerFilePath]`,
so it costs nothing at runtime.

## Reading a failure

```
UniJect: resolving GameController -> IPlayerService -> IInventory: no binding for IInventory.

Resolution chain:
  <root>
    GameController
      IPlayerService                (field '_playerService')
        IInventory                  (ctor parameter 'inventory')

Near matches for IInventory:
  IInventory (id: "remote")         RemoteInstaller.cs:31 — rejected: id mismatch
  IInventory                        DebugInstaller.cs:12 — rejected: WhenInjectedInto<DebugPanel>

Container: SceneContext 'Level01'
Installers: GameInstaller, UiInstaller, RemoteInstaller, DebugInstaller

Fix: bind IInventory in one of your installers, mark the injection site [InjectOptional],
     or use TryResolve if it is optional.
```

The chain reads top down: the thing you asked for, then what it needed, down to what was missing.
The near matches are the bindings that *could* have answered and why they did not.

## The codes

`Validate()` and `ValidateOnBuild` report issues rather than throwing them one at a time. Errors
block the build; warnings and info never do.

### UJ001 — Error — no binding for a required dependency

Something in the graph needs a contract nobody bound. Bind it, mark the site `[InjectOptional]`, or
give the parameter a default value.

### UJ002 — Error — duplicate binding

The same `(contract, id, concrete type)` was declared twice. Use `.WithId(...)` to tell them apart,
or `.IfNotBound()` if the second one is a default.

### UJ003 — Error — abstract concrete type with no source

You bound an interface or abstract class without telling the container how to build it. Add a
`From*` verb, or bind a concrete type.

### UJ004 — Warning — two construction sources

Two different `From*` verbs on one binding. The last one wins, which is probably not what you meant.

### UJ005 — Error — `.AsTransient()` with `.NonLazy()`

There is no single instance to create eagerly. Pick one.

### UJ006 — Error — `.FromInstance(...)` with `.AsTransient()`

One instance cannot be transient.

### UJ007 — Error — `.FromResolve()` pointing at itself

The binding aliases its own `(contract, id)` and would resolve to itself forever. Point it at the
concrete type, or at a different id.

### UJ008 — Warning — `.DontDestroyOnLoad()` with an explicit `.Parent(...)`

Unity promotes the parent's **whole root hierarchy**, not just this object. It works, and it is
rarely what you meant. Left as a warning because it is genuinely used.

### UJ009 — Warning — `.DontDestroyOnLoad()` on a scene container

The binding lives in a scene scope, so every scene load makes another survivor. Move it to the
project context.

### UJ010 — Info — binding not statically validatable

A `FromMethod` or `FromInstance` binding cannot be checked ahead of time. *(Not yet emitted.)*

### UJ011 — Info — transient `IDisposable` resolved from the root

It is tracked by the root container, so it lives for the whole process. *(Not yet emitted.)*

### UJ012 — Error — captive dependency

A singleton depends on a scoped service, so the scoped instance is captured for the singleton's
lifetime and outlives its own scope. Make the dependency a singleton, make the consumer scoped, or
inject a factory. This is the most common DI bug in Unity.

### UJ013 — Error — factory parameters match no product constructor

`BindFactory<Bullet, string>()` when no `Bullet` constructor takes a `string`. Caught when the
container is built, not when something first calls `Create`.

### UJ014 — Warning — IL2CPP with aggressive stripping

The build is IL2CPP with managed stripping above `Low`. Types reached only through `[Inject]` can be
stripped. See [il2cpp-and-stripping.md](il2cpp-and-stripping.md).

### UJ015 — Warning — dead binding

Nothing resolves it and no injection site asks for it. *(Not yet emitted.)*

## Cycles

A cycle where **every edge is a constructor parameter** is always an error: it genuinely cannot be
built. A cycle with at least one field, property or method edge between non-transient bindings is
**allowed** — the container caches the instance before injecting it, so the second arm finds the
partially built peer. Between transients it is an error again, because there is no cache to break it.

The message prints the path with the edge kinds, and names the ways out: an `IFactory<T>`, a
`PlaceholderFactory`, or moving one dependency from the constructor to an `[Inject]` field.

## The container window

`Window > UniJect > Container` lists every binding with its contract, concrete type, lifetime, id,
condition, eagerness, state, injection count and clickable `file:line`. It works against the live
containers in play mode, and against a preview built from your installers in edit mode.

Entering play with a binding that cannot be resolved is blocked and the whole report is logged. Turn
that off in `Project Settings > UniJect`.
