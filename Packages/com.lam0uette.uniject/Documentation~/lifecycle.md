# Lifecycle

## The four interfaces

```csharp
public interface IInitializable { void Initialize(); }
public interface ITickable      { void Tick(float deltaTime); }
public interface IFixedTickable { void FixedTick(float fixedDeltaTime); }
public interface ILateTickable  { void LateTick(float deltaTime); }
```

All four live in the engine-free core, and `System.IDisposable` is the release interface — there is
no `UniJect.IDisposable`.

## Why not just use Update()

N `MonoBehaviour.Update()` methods mean N native-to-managed transitions per frame, in an order Unity
does not define. UniJect installs **one** PlayerLoop node per phase and walks a snapshot, so it is
one transition, in registration order. That is the strongest practical reason to use DI in Unity.

```csharp
public sealed class GameLoop : IInitializable, ITickable, IDisposable
{
    public void Initialize() { }
    public void Tick(float deltaTime) { }
    public void Dispose() { }
}
```

```csharp
Container.Bind<GameLoop>().AsSingleton().NonLazy();
```

No `MonoBehaviour` anywhere, and it ticks once per frame.

## The order things happen

When a `SceneContext` awakes:

1. every installer runs, in the order of the `Installers` array;
2. the container is built, and validation runs if it is on;
3. the **non-lazy pass** creates every `NonLazy` binding, in declaration order;
4. the **scene injection pass** injects every `MonoBehaviour` in that scene;
5. the **initialize pass** calls `Initialize()` on everything created so far, in creation order;
6. tick registration.

Non-lazy runs before scene injection, which is what makes "`NonLazy` means built at container
startup" literally true.

## Ticking rules

- A tickable that throws is logged and **does not stop the others**. The container itself never
  logs; the PlayerLoop pump does.
- Adding or removing a tickable during a tick takes effect on the **next** tick: each pass runs over
  a snapshot.
- Only instances the container actually created are ticked. A binding that is never resolved never
  ticks.

## Release

`container.Dispose()` releases children first, then its own instances in **reverse creation order**,
so anything an object depends on is still alive when its `Dispose` runs. A failure in one release
never stops the rest; they are collected and thrown together.

What gets released is decided by `Ownership`, which the activator declares and the tracker executes:

| Ownership | Disposed | Destroyed |
| --- | --- | --- |
| `None` | no | no |
| `Managed` | yes, if `IDisposable` | no |
| `UnityComponent` | yes, if `IDisposable` | the component |
| `UnityGameObject` | yes, if `IDisposable` | the whole GameObject |

`FromInstance`, `FromResolve`, `FromComponentInHierarchy` and `FromComponentOnGameObject` are all
`None`: the container never destroys something it did not create.
