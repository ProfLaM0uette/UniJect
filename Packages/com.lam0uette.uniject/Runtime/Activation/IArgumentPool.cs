namespace LaM0uette.UniJect
{
    internal interface IArgumentPool
    {
        object[] Rent(int size);

        void Return(object[] arguments);
    }
}
