namespace LaM0uette.UniJect
{
    public sealed class FrameTickRecorder
    {
        public int TickCount { get; set; }
        public int FixedTickCount { get; set; }
        public int LateTickCount { get; set; }
    }
}
