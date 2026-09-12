namespace LaM0uette.UniJect
{
    public sealed class OptionalConsumer
    {
        [InjectOptional] private IInventory _inventory;

        public IInventory Inventory
        {
            get { return _inventory; }
        }
    }
}
