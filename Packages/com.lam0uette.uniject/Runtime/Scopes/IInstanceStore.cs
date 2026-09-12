namespace LaM0uette.UniJect
{
    internal interface IInstanceStore
    {
        bool TryGet(int slot, out object instance);

        void Set(int slot, object instance);

        void Evict(int slot);

        void Clear();
    }
}
