namespace LaM0uette.UniJect
{
    public interface IInstanceReleaser
    {
        bool TryRelease(object instance, Ownership ownership);
    }
}
