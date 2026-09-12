using System;

namespace LaM0uette.UniJect
{
    internal static class FactoryDraft
    {
        public static BindingDraft Declare(
            IContainerBuilder builder,
            Type contractType,
            Type concreteType,
            string sourceFile,
            int sourceLine)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (!(builder is ContainerBuilder sink))
            {
                throw new InvalidOperationException(
                    "UniJect: BindFactory needs a ContainerBuilder, but the builder is " +
                    builder.GetType().Name + ".");
            }

            BindingDraft draft = new BindingDraft
            {
                ConcreteType = concreteType,
                Origin = new BindingOrigin(sourceFile, sourceLine)
            };

            draft.ContractTypes.Add(contractType);
            sink.AddDraft(draft);

            return draft;
        }
    }
}
