namespace LaM0uette.UniJect
{
    public interface IInstanceLivenessPolicy
    {
        bool IsAlive(object instance);
    }
}
