namespace LaM0uette.UniJect
{
    public sealed class PlayerService
    {
        public IInventory Inventory { get; }

        public PlayerService(IInventory inventory)
        {
            Inventory = inventory;
        }
    }
}
