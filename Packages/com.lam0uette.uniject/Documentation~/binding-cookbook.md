# Binding cookbook

## The grammar

Exactly one entry point comes first, then any number of verbs, **in any order**.

```
builder
  .Bind<TContract>()                     ─┐
  .Bind<TContract, TConcrete>()           │ exactly one, first
  .BindInstance(obj)                      │
  .BindInterfacesTo<TConcrete>()          │
  .BindInterfacesAndSelfTo<TConcrete>()  ─┘

  SOURCE (last one wins)
      .FromNew()  .FromInstance(o)  .FromMethod(f)  .FromResolve()
      .FromNewGameObject()  .FromNewPrefab(go)  .FromNewPrefabResource(s)      <Component>
      .FromNewComponentOnGameObject(go)
      .FromComponentInHierarchy(bool)  .FromComponentOnGameObject(go, bool, bool)

  PLACEMENT                                                                    <Component>
      .Name(s)  .Parent(t)  .Root()  .DontDestroyOnLoad()
      .Transform(t)  .Position(v)  .Rotation(q)  .Scale(v)

  IDENTITY / CONDITION (conditions accumulate as AND)
      .WithId(o)
      .When(f)  .When(request => ...)
      .WhenInjectedInto<T>()  .WhenInjectedInto(types)  .WhenInjectedIntoInstance(o)
      .WhenNotInjectedInto<T>()  .WhenNotInjectedInto(types)  .WhenNotInjectedIntoInstance(o)

  LIFETIME (last one wins)
      .AsSingleton()  .AsScoped()  .AsTransient()

  EAGERNESS / IDEMPOTENCE
      .NonLazy()  .Lazy()  .IfNotBound()
```

`<Component>` marks the verbs available only when the concrete type derives from
`UnityEngine.Component`. That is enforced at **compile time**: `Bind<PlainClass>().Name("x")` does
not compile.

## Recipes

**One instance for the whole game**

```csharp
Container.Bind<IInventory, Inventory>().AsSingleton();
```

**One instance shared by every interface it implements**

```csharp
Container.BindInterfacesTo<SaveSystem>().AsSingleton();
```

One `Registration` carrying N contracts, so one instance. Use `BindInterfacesAndSelfTo` when you
also want to resolve the concrete type.

**Something already built**

```csharp
Container.BindInstance(myConfig);
```

The container never disposes or destroys it, but it does inject it once.

**Something built by hand**

```csharp
Container.Bind<IClock>().FromMethod(resolver => new SystemClock()).AsSingleton();
```

The closure is allocated once, at install time, never per resolution.

**Two implementations of one contract**

```csharp
Container.Bind<IWeapon, Sword>().WithId("melee").AsSingleton();
Container.Bind<IWeapon, Bow>().WithId("ranged").AsSingleton();
```

The id is part of the binding's identity, so a request without an id can never be served by a
binding that carries one. Ask with `[Inject("melee")]` or `ResolveId<IWeapon>("melee")`.

**One implementation only for one consumer**

```csharp
Container.Bind<IWeapon, Sword>().AsSingleton();
Container.Bind<IWeapon, Bow>().WhenInjectedInto<Archer>().AsSingleton();
```

A binding whose condition is satisfied beats an unconditional one. Two conditions that both match is
an error, not a contest.

**Every implementation at once**

```csharp
Container.Bind<IRule, ScoreRule>().AsSingleton();
Container.Bind<IRule, TimeRule>().AsSingleton();

public Referee(IEnumerable<IRule> rules) { }
```

`IEnumerable<T>`, `IReadOnlyList<T>`, `IList<T>`, `List<T>` and `T[]` all work. No match is an empty
collection, never an exception.

**A component the container creates**

```csharp
Container.Bind<Turret>().FromNewGameObject().Name("Turret").Parent(_root).AsSingleton();
```

What you do not write is not written: no `.Position(...)` means the position is left exactly as it
was, never reset to zero.

**A prefab**

```csharp
Container.Bind<Enemy>().FromNewPrefab(_enemyPrefab).AsTransient();
```

The clone is instantiated inactive, its whole subtree is injected, then it is activated — so `Awake`
runs with every dependency already in place.

**A component already in the scene**

```csharp
Container.Bind<Camera>().FromComponentInHierarchy().AsSingleton();
```

Searches only. Throws if absent, never creates silently, and never destroys it on teardown.

**Something made on demand**

```csharp
Container.BindFactory<Bullet, int>();

public Gun(IFactory<Bullet, int> bullets) { bullets.Create(7); }
```

No factory class to write. For a named type you can override, derive from
`PlaceholderFactory<Bullet, int>` and use `BindPlaceholderFactory`.

**A default the application can override**

```csharp
Container.Bind<ILogger, ConsoleLogger>().IfNotBound().AsSingleton();
```

## The plain .NET façade

If you prefer the shape you already know:

```csharp
builder
    .AddSingleton<ILogger, UnityLogger>()
    .AddScoped<ISession, Session>()
    .AddTransient<ICommand, MoveCommand>();

builder.TryAddSingleton<IClock, SystemClock>();
builder.Replace<ILogger, NullLogger>(Lifetime.Singleton);
```

Every one of these is a one-liner over the fluent grammar. They are the same bindings, written
differently — mixing the two styles in one installer is fine.

## Precedence, once

**The id is part of the identity. The nearest container wins. A binding whose condition is satisfied
beats an unconditional one. Everything else is an error.** There is no specificity score.
