namespace LaM0uette.UniJect.Samples.Hello
{
    public sealed class GreetingService : IGreetingService
    {
        #region Methods

        public string Greet(string name)
        {
            return "Hello, " + name + ".";
        }

        #endregion
    }
}
