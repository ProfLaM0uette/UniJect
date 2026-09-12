# 03 — GameObjects and prefabs

Every placement verb is constrained to `Component` at compile time, so `.Name("x")` on a plain class
does not compile. A component spawned by the container reads its dependencies inside `Awake`: the
host is created inactive, injected, placed, and only then activated. Note what is *not* written —
no `.Position(...)` here means the position is left exactly as authored, never reset to zero.
