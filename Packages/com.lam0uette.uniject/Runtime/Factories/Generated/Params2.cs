namespace LaM0uette.UniJect
{
    [System.CodeDom.Compiler.GeneratedCode("UniJect.ArityGenerator", "1.0")]
    public class Params<T1, T2> : IParams<T1, T2>
    {
        public T1 P1 { get; }
        public T2 P2 { get; }

        public Params(T1 p1, T2 p2)
        {
            P1 = p1;
            P2 = p2;
        }
    }
}
