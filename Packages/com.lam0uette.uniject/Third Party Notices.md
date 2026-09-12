# Third Party Notices

This package vendors the following third party code.

## JetBrains.Annotations

`Runtime/Annotations/MeansImplicitUseAttribute.cs`, `Runtime/Annotations/ImplicitUseKindFlags.cs`
and `Runtime/Annotations/ImplicitUseTargetFlags.cs` are declarations taken from
JetBrains.Annotations, reproduced under their canonical `JetBrains.Annotations` namespace and
marked `internal` so they never reach this package's public surface.

They are vendored rather than referenced because the package contract is zero dependency: the v1
library declared `"references": []` in its assembly definition yet only compiled because the Rider
package happened to ship the `JetBrains.Annotations` assembly.

Copyright 2016 JetBrains s.r.o.

Licensed under the MIT License. See <https://github.com/JetBrains/JetBrains.Annotations>.
