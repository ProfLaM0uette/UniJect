namespace LaM0uette.UniJect
{
    internal interface IDisposalTracker
    {
        void Track(object instance, Ownership ownership);

        void Forget(object instance);

        void DisposeAll();
    }
}
