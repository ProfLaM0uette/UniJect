# 02 — Scene context and installers

Two installers on one scene context, each owning its own slice of the graph. The order they sit in
the `Installers` array does not matter: `Container` is a builder, not a resolver, so nothing can be
resolved while installing. Swap the two entries in the inspector and the scene behaves identically —
which is the whole point of the two-phase split.
