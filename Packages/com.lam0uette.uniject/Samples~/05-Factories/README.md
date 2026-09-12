# 05 — Factories

Two ways to make something on demand. `BindFactory<Bullet, int>()` gives you an
`IFactory<Bullet, int>` for free — no class to write — and the argument lands on the product's
constructor. A `PlaceholderFactory` is the same thing when you want a named type you can override:
`Create` stays `virtual`, so `DoubleDamageFactory` can wrap it. Both go through the same activation
path as any other binding.
