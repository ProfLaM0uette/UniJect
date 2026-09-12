namespace LaM0uette.UniJect
{
    internal static class GameObjectSourceAccess
    {
        public static GameObjectSource GetOrCreate(IProductSourceHolder holder)
        {
            if (holder.ProductSource is GameObjectSource existing)
                return existing;

            GameObjectSource source = new GameObjectSource();
            holder.ProductSource = source;

            return source;
        }

        public static GameObjectSource GetOrCreate(BindingDraft draft)
        {
            if (draft.Source is GameObjectSource existing)
                return existing;

            GameObjectSource source = new GameObjectSource();

            if (draft.Source == null || draft.Source is ConstructorActivatorSource)
            {
                draft.Source = source;
                return source;
            }

            draft.SetSource(source);
            return source;
        }
    }
}
