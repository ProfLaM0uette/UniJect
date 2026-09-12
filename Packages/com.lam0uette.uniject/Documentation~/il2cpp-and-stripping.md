# IL2CPP and stripping

IL2CPP is not a corner case: it is the runtime on console and mobile, and it stays that way. Unity 7
replaces Mono with CoreCLR, not IL2CPP.

## What the container already does

**No expression trees, no IL emit, anywhere.** This is not a preference, it is a trap:
`Expression.Compile()` under IL2CPP does not throw. It returns an *interpreted* lambda slower than
reflection, which then throws on value types. The editor runs Mono, so the fast path would be the
only one ever exercised in development. A test scans the runtime sources and fails the build if
`Expression.Compile`, `Reflection.Emit`, `DynamicMethod` or `ILGenerator` ever appears.

**Every `[Inject]` site preserves itself.** The Unity linker honours any attribute named exactly
`PreserveAttribute`, whatever its namespace. `InjectAttributeBase` derives from a home-made
`PreserveAttribute`, so the core gets stripping resistance with no engine reference.

**A static `link.xml`** ships with the package and preserves both runtime assemblies.

**A generated `link.xml`** is added at build time by `LinkXmlGenerator`, listing every type in the
project that carries an `[Inject]` site.

**Generic instantiations are static.** Factories never use `MakeGenericType` or
`Activator.CreateInstance` on a runtime-built generic. Each generated `BindFactory` overload closes
over its own adapter constructor, so every instantiation the container needs already exists at the
call site where you wrote the binding.

## What you still have to do

**Keep managed stripping at `Low`.** Above that, a type only ever reached through `[Inject]` can
disappear. The package logs `UJ014` at build time when it sees IL2CPP paired with a higher level.

**Watch constructor injection on types nothing references statically.** The generated `link.xml`
covers types with an `[Inject]` member, but a concrete type only ever named in a binding — for
example `Bind<IFoo, Foo>()` where `Foo` has a plain constructor and no `[Inject]` — is reached by
the linker through the generic argument, which is usually enough. If you strip aggressively and see
a missing constructor at runtime, that is the first place to look.

## Status, honestly

This is the one part of the package that is **written and reasoned about but not demonstrated**. No
IL2CPP player build has been produced, and there is no `AotConformanceTests` suite yet. Everything
above follows from the architecture and from Unity's documented linker behaviour, but if you are
shipping to IL2CPP, make a development build early and look for it rather than trusting this page.
