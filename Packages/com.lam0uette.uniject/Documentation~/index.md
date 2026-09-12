# UniJect

A zero-dependency dependency injection container for Unity. The container is immutable once built,
bindings are analysed at build time, reflection is cached per type for the lifetime of the domain,
and a missing binding throws with the full resolution chain instead of returning `null`.

## Install

Unity Package Manager, *Add package from git URL*:

```
https://github.com/ProfLaM0uette/UniJect.git?path=/Packages/com.lam0uette.uniject
```

## The whole thing in thirty lines

```csharp
public sealed class GameInstaller : MonoInstaller
{
    public override void Install()
    {
        Container.Bind<ILogger, UnityLogger>().AsSingleton();
        Container.Bind<GameState>().AsSingleton().NonLazy();
        Container.Bind<IClock>().FromMethod(resolver => new SystemClock()).AsSingleton();
    }
}

public sealed class Hud : MonoBehaviour
{
    [Inject] private ILogger _logger;

    private void Start()
    {
        _logger.Log("injected before this ran");
    }
}
```

Put `GameInstaller` and a `SceneContext` in the scene, drag the installer into the context's
`Installers` list, press play.

## The five things worth knowing

1. **`Container` inside `Install()` is a builder, not a resolver.** It has no `Resolve`, no
   `CreateInstance`, no `Inject`. That is why the order of your installers does not matter.
2. **A missing binding throws.** There is no silent `null`, and no "take the first binding"
   fallback. Use `TryResolve`, `[InjectOptional]` or `GetService()` when something really is
   optional.
3. **Every binder verb returns the same binder, in any order.** `.AsSingleton().FromNew()` and
   `.FromNew().AsSingleton()` are the same binding.
4. **A component spawned by the container is injected before its `Awake` runs.** The host is created
   inactive, injected, placed, then activated.
5. **Release is deterministic**: reverse creation order, children before parents, and the container
   only destroys what it created.

## Where to go next

| | |
| --- | --- |
| [binding-cookbook.md](binding-cookbook.md) | every verb, with a recipe per situation |
| [lifecycle.md](lifecycle.md) | initialize, tick and dispose without a `MonoBehaviour` |
| [diagnostics.md](diagnostics.md) | one section per `UJxxx` code |
| [il2cpp-and-stripping.md](il2cpp-and-stripping.md) | what AOT forces, and what is already handled |
| [faq.md](faq.md) | including why `.Name(...)` does not exist on your binder |
| [migrating-from-uniject-v1.md](migrating-from-uniject-v1.md) | the changes that produce no compile error |
