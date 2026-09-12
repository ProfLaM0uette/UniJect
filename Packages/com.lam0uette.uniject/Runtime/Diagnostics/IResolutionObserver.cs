namespace LaM0uette.UniJect
{
    public interface IResolutionObserver
    {
        void ResolutionCompleted(in ResolutionRequest request, Registration registration, object instance);

        void InjectionCompleted(object target, Registration registration);
    }
}
