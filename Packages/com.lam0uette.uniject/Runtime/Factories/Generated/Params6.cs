namespace LaM0uette.UniJect
{
    [System.CodeDom.Compiler.GeneratedCode("UniJect.ArityGenerator", "1.0")]
    public class Params<T1, T2, T3, T4, T5, T6> : IParams<T1, T2, T3, T4, T5, T6>
    {
        public T1 P1 { get; }
        public T2 P2 { get; }
        public T3 P3 { get; }
        public T4 P4 { get; }
        public T5 P5 { get; }
        public T6 P6 { get; }

        public Params(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
            P4 = p4;
            P5 = p5;
            P6 = p6;
        }
    }
}
