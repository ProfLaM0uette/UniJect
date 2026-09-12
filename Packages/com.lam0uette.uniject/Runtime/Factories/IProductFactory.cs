using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public interface IProductFactory
    {
        object Create(IReadOnlyList<object> arguments);
    }
}
