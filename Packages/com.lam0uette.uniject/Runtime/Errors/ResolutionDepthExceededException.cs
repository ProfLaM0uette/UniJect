using System;

namespace LaM0uette.UniJect
{
    public sealed class ResolutionDepthExceededException : UniJectException
    {
        public int Depth { get; }
        public ResolutionPath Path { get; }

        public ResolutionDepthExceededException(int depth, Type contractType, ResolutionPath path)
            : base(ResolutionMessage.ResolutionDepthExceeded(depth, contractType, path))
        {
            Depth = depth;
            Path = path;
        }
    }
}
