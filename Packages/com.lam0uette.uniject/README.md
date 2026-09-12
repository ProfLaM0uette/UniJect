# UniJect

A zero-dependency dependency injection container for Unity.

UniJect keeps the fluent binding grammar you write by hand and replaces the internals with an
immutable container: bindings are declared during installation, analysed once at build time, and
resolved from a memoized call-site graph. Reflection is cached per type for the lifetime of the
domain. A missing binding throws with the full resolution chain instead of silently returning
`null`.

> **Status: pre-release.** The public surface is specified but not yet implemented. See
> [ROADMAP.md](../../ROADMAP.md) for the eight phases and what each one delivers.

## Requirements

| | |
| --- | --- |
| Unity | 6000.3 (LTS) or newer — developed on 6000.6 |
| Language | C# 9.0 |
| Dependencies | none |
| Scripting backends | Mono and IL2CPP (no expression trees, no IL emit) |

## Installation

Unity Package Manager, *Add package from git URL*:

```
https://github.com/LaM0uette/UniJect.git?path=/Packages/com.lam0uette.uniject
```

Or pin a tag:

```
https://github.com/LaM0uette/UniJect.git?path=/Packages/com.lam0uette.uniject#v1.0.0
```

## A first container

```csharp
public class GameInstaller : MonoInstaller
{
    public override void Install()
    {
        Container.Bind<ILogger, UnityLogger>().AsSingleton();
        Container.Bind<GameState>().AsSingleton().NonLazy();
        Container.Bind<IClock>().FromMethod(resolver => new SystemClock()).AsSingleton();
    }
}
```

```csharp
public class Hud : MonoBehaviour
{
    [Inject] private ILogger _logger;

    private void Start()
    {
        _logger.Log("injected");
    }
}
```

`Container` is an `IContainerBuilder` during installation: it has no `Resolve`, no
`CreateInstance` and no `Inject`, which is what makes installer order irrelevant.

## Assemblies

| Assembly | Engine | Role |
| --- | --- | --- |
| `LaM0uette.UniJect` | no | registration, activation, resolution, scopes, release |
| `LaM0uette.UniJect.Unity` | yes | GameObject sources, placement, contexts, installers, PlayerLoop |
| `LaM0uette.UniJect.Editor` | yes | container window, validation gate, `link.xml` |

The core never learns that `GameObject` exists. The Unity assembly plugs into it through
`IActivatorSource`, `IInstanceReleaser`, `IInstanceLivenessPolicy` and `ITickRegistry`. Both sides
share the same root namespace, so one `using LaM0uette.UniJect;` is all a consumer writes.

## License

MIT — see [LICENSE.md](LICENSE.md).
