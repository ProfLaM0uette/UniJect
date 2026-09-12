namespace LaM0uette.UniJect
{
    public interface IBindingCondition
    {
        bool Matches(in ResolutionRequest request);

        string Describe();
    }
}
