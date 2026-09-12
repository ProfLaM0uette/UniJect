# 01 — Hello container

The smallest thing that works: one installer declares one binding, and one scene `MonoBehaviour`
reads it from a private field it never assigns. Put `HelloInstaller` and `Greeter` on the same
GameObject as a `SceneContext`, drag the installer into the context's `Installers` list, and press
play. Nothing else is needed — no service locator, no `FindObjectOfType`, no static.
