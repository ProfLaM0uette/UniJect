# 06 — Lifecycle

`GameLoop` is a plain C# class with no `MonoBehaviour` anywhere, and it still initialises, ticks
once per frame and disposes. N `Update()` methods mean N native-to-managed transitions per frame in
undefined order; one PlayerLoop node means one transition, in registration order. Disposal runs in
reverse creation order, so anything this object depends on is still alive when its `Dispose` runs.
