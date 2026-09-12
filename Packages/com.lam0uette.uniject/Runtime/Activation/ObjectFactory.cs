using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public delegate object ObjectFactory(IResolutionContext context, IReadOnlyList<object> arguments);
}
