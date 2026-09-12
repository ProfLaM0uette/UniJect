using System;

namespace LaM0uette.UniJect
{
    public interface IProductSourceHolder
    {
        Type ProductType { get; }
        IActivatorSource ProductSource { get; set; }
    }
}
