using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public sealed class Armoury
    {
        public IReadOnlyList<IWeapon> Weapons { get; }

        public Armoury(IEnumerable<IWeapon> weapons)
        {
            List<IWeapon> collected = new List<IWeapon>(weapons);
            Weapons = collected;
        }
    }
}
