# 04 — Conditional bindings

Two implementations of one contract, kept apart two different ways. `WithId` makes the id half of
the binding's identity, so a request without an id can never be served by a binding that carries
one. `WhenInjectedInto` narrows a binding to one consumer; ask for it anywhere else and the
container throws rather than quietly handing over the wrong instance.
