# UniJect demo scene

Open `UniJectDemo.unity` and press play. Everything this scene proves is printed to the console.

## What the scene contains

```
UniJect Context          SceneContext, with both installers in its Installers array
  Demo Installer         DemoInstaller     — services, conditions, ids, collections, lifecycle
  Spawn Installer        DemoSpawnInstaller — GameObject creation, placement, factories
Spawn Root               where the container parents what it creates
Demo Report              DemoReport — reads everything back and logs it
Main Camera
```

Two installers rather than one, on purpose: swap them in the `Installers` array and the scene behaves
identically. `Container` inside `Install()` is a builder with no `Resolve`, so nothing can depend on
another installer having run first.

## What the console prints

```
Spinner.Awake ran, and its dependency was already there: True
GameClock initialised before the first frame, with no MonoBehaviour.

interface binding      IDemoLogger -> ConsoleDemoLogger
unconditional          Knight got the sword
WhenInjectedInto       Archer got the bow
WithId("backup")       bow
collection             Referee sees 2 rules, total 15
factory                plain Create(10) -> damage 10
placeholder factory    overridden Create(10) -> damage 30
FromNewGameObject      Spinner (made by the container), parent Spawn Root
injected before Awake  True
InjectOptional         unbound IDisposable left as null
TryResolve             unbound IComparable found: False
self binding           resolved the container itself: True

GameClock disposed after N ticks.
```

## Line by line, and where it comes from

| Line | Shows | Written in |
| --- | --- | --- |
| `interface binding` | `Bind<IDemoLogger, ConsoleDemoLogger>().AsSingleton()` | `DemoInstaller` |
| `unconditional` | the plain `IWeapon` binding wins for anyone with no condition | `DemoInstaller` |
| `WhenInjectedInto` | `Bind<IWeapon, Bow>().WhenInjectedInto<Archer>()` — a satisfied condition beats an unconditional binding | `DemoInstaller` |
| `WithId("backup")` | the id is part of the identity, so it never leaks into an id-less request | `DemoInstaller` |
| `collection` | an `IEnumerable<IScoreRule>` constructor parameter receives every binding of `IScoreRule` | `Referee` |
| `factory` | `BindFactory<Projectile, int>()` — an `IFactory<Projectile, int>` with no class to write | `DemoSpawnInstaller` |
| `placeholder factory` | `CriticalProjectileFactory` overrides `Create` and triples the damage | `CriticalProjectileFactory` |
| `FromNewGameObject` | the container creates the GameObject, names it and parents it | `DemoSpawnInstaller` |
| `injected before Awake` | the host is created inactive, injected, placed, then activated | `Spinner.Awake` |
| `InjectOptional` | an unresolved optional field is left alone rather than throwing | `DemoReport` |
| `TryResolve` | asking for something unbound returns false instead of throwing | `DemoReport` |
| `self binding` | `DIContainer`, `IResolver`, `IInstantiator` and `IServiceProvider` resolve to the container that resolves them | `DemoReport` |
| `GameClock initialised` / `disposed` | `IInitializable`, `ITickable` and `IDisposable` on a plain C# class, ticking with no `MonoBehaviour` | `GameClock` |

## Things worth trying

- Delete `Bind<IWeapon, Sword>()` and press play: the failure names the container, its installers,
  the resolution chain and every near match with its `file:line`.
- Add a second unconditional `IWeapon`: it throws at resolution showing both candidates, rather than
  picking one.
- Open `Window > UniJect > Container` to see every binding listed, in edit mode and in play mode.
