using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public interface IBindingValidation
    {
        void Validate(BindingOrigin origin, bool isChildScope, List<ValidationIssue> issues);
    }
}
