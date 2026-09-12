# C# — UniJect

This file is the authority for C# style in this repository. It is the global
`~/.claude/rules/csharp.md` carried over in full, plus the amendments Unity forces. Read this one,
not the global copy.

## The Unity amendment — language level

**Unity 6 compiles C# 9.0.** Verified in the generated `Assembly-CSharp.csproj`
(`<LangVersion>9.0</LangVersion>`) and true for 6000.3 as for 6000.6. Anything newer is a
compilation error, not a preference:

| Forbidden here | Why | Write instead |
| --- | --- | --- |
| `[]`, `[a, b, c]` collection expressions | C# 12 | `new List<T>()`, `Array.Empty<T>()`, `new[] { a, b, c }` |
| `namespace X;` file-scoped | C# 10 | `namespace X { … }` with braces |
| `record`, `record struct` | needs `IsExternalInit` | `class` / `struct` with a constructor |
| `init` accessors | needs `IsExternalInit` | `readonly` field set in the constructor, or a private setter |
| `required`, `global using`, raw strings, list patterns | C# 11+ | — |

Target-typed `new()`, pattern matching enhancements, `static` lambdas and top-level natives of C# 9
are all fine.

**The intent of the global collection rule is unchanged.** Never a spread — there is no `..` to
write anyway — and always materialize explicitly: `.ToList()`, `.ToArray()`, `.ToHashSet()`,
`new HashSet<T>(items)`. An empty collection is `Array.Empty<T>()` for arrays and a plain
`new List<T>()` otherwise.

## The Unity amendment — runtime

- **No expression trees, no IL emit, ever.** Under IL2CPP `Expression.Compile()` does not throw:
  it returns an interpreted lambda slower than reflection, which then throws on value types. The
  Editor runs Mono, so the fast path is the only one ever exercised. A CI test forbids it.
- **Reflection lives in exactly one file.** `GetFields` / `GetProperties` / `GetMethods` /
  `GetCustomAttributes` appear only in `ReflectionInjectionPlanProvider`. One plan per type, cached
  for the lifetime of the domain.
- **No string-keyed reflection.** No `"Create"`, no `StartsWith("IInjectable")`.
- **Every static cache declares its reset.** Domain reload is off by default in 6.6, so a static
  field survives Play. Register the reset in the static reset table; a forgotten one only shows up
  on the *second* Play without an edit.
- **Never cache a `UnityEngine.Object` as if `!= null` meant alive.** A destroyed object is
  `!= null` under `ReferenceEquals`. Liveness goes through `IInstanceLivenessPolicy`.

## Namespaces

Every assembly carries `rootNamespace: LaM0uette.UniJect`, and the public surface lives in that one
namespace — one `using` is all a consumer writes. The core / Unity / Editor split is a build
concern, never a namespace concern.

| Folder | Namespace |
| --- | --- |
| `Packages/com.lam0uette.uniject/Runtime/Core/**` | `LaM0uette.UniJect` |
| `Packages/com.lam0uette.uniject/Runtime/Unity/**` | `LaM0uette.UniJect` |
| `Packages/com.lam0uette.uniject/Editor/**` | `LaM0uette.UniJect` |
| `Packages/com.lam0uette.uniject/Tests/**` | `LaM0uette.UniJect` |

A role subfolder adds no namespace segment. A deeper namespace is allowed only for a sub-feature
whose types are all `internal`.

## Types and language features

- Always use **explicit types** — no `var`.
- Materialize collections explicitly (see the amendment above).

## Constants

`const` naming is **UPPER_CASE** — `MAX_RETRY`, `DEFAULT_TIMEOUT` — and `private` unless another
class reads it.

## Files

**One type per file** — no second class, enum or record beside it.

## Enum values

Always assign **explicit integer values with gaps** (0, 10, 20, 100 …) so a member can be
intercalated later without shifting the ones already stored.

## Transport suffixes

`*Request` going in, `*Response` coming out. Where a `DTO` suffix is genuinely warranted — a shape
parsed from a third party's payload, which is not this application's own transport type — it is
written in **all caps**: `CardEnDTO`, never `Dto`.

## Events

An event name does **not** start with `On`. The handler method is what gets the prefix.

```csharp
public event Action CultureChanged;               // event
Localizer.CultureChanged += OnCultureChanged;     // subscription
```

## Blank lines inside a method body

A blank line separates **two logical blocks**, and nothing else. It is never mechanical: not after
the declarations, not before a `return`, not between every pair of statements.

**A `return` belongs to the block that produced its value.** It stays glued to the lines it
concludes, and only takes a blank line before it when it genuinely opens a new step — typically
after a guard clause.

```csharp
// avoid — one block artificially cut in two
Registration registration = _callSiteFactory.GetRegistration(contract);

return registration;

// prefer
Registration registration = _callSiteFactory.GetRegistration(contract);
return registration;
```

**A guard clause is its own block**, so it is surrounded by blank lines.

```csharp
Registration registration = _callSiteFactory.Find(contract, id);

if (registration == null)
    throw new BindingNotFoundException(contract, id, _chain);

return Activate(registration);
```

**Group statements by what they accomplish**, then breathe between the groups — a run of related
setup lines is one block, the work it feeds is the next, and the value it hands back closes the
last.

- **A short body stays in one piece.** Two or three lines reading as a single step take no blank
  line at all.
- **Never two blank lines in a row inside a method body.**
- Single-line `if` bodies go on the **next line without braces**.
- **2 blank lines** only at the boundary between public and private methods, to signal the
  access-level shift. Every other method separation is 1 blank line.

## Control flow

**Invert the condition.** Prefer a guard clause with an early `return` / `continue` / `throw` over
wrapping the main logic in an `if`: handle the exceptional or empty case first and exit, keeping the
happy path un-nested.

```csharp
// avoid
if (registration != null)
{
    // ... main logic
}
return null;

// prefer
if (registration == null)
    return null;

// ... main logic
```

## `#region`

A region appears only where the type has **both statements and methods** — never in an enum, an
entity, a DTO, a model, a record, a simple data carrier or a test file. Where regions apply, exactly
three names are allowed, in this order:

```
#region Statements
    const (public → private), events, indexers,
    properties (public → private), constructors,
    framework lifecycle overrides
#endregion

#region Methods
    methods (public → private)
#endregion

#region <InterfaceName>
    one region per explicitly implemented interface (IDisposable, IAsyncDisposable, …)
#endregion
```

No other region name. No `#region Fields`, no `#region Private`, no `#region Helpers`, and no region
per shape.

## Unit tests

- Tests cover the **public contract**, not internals.
- Naming: `MethodName_StateUnderTest_ExpectedBehavior`.
- No `#region` in a test file.
- Group tests by tested method, **2 blank lines** between groups and 1 between tests in a group.
- A fake used only by tests lives in the test assembly.
- `LaM0uette.UniJect.Core.Tests` references the core assembly and nothing else from this package.
  That restriction is the compiler-enforced proof that the core is engine-free — never relax it.
