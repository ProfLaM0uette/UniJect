namespace LaM0uette.UniJect
{
    public readonly struct BindingOrigin
    {
        private static readonly char[] SEPARATORS = { '/', '\\' };

        public static readonly BindingOrigin Unknown = new BindingOrigin(null, 0);

        public string SourceFile { get; }
        public int SourceLine { get; }

        public bool IsKnown
        {
            get { return SourceFile != null; }
        }

        public BindingOrigin(string sourceFile, int sourceLine)
        {
            SourceFile = sourceFile;
            SourceLine = sourceLine;
        }

        public override string ToString()
        {
            if (SourceFile == null)
                return "<unknown origin>";

            int lastSeparator = SourceFile.LastIndexOfAny(SEPARATORS);
            string fileName = lastSeparator < 0 ? SourceFile : SourceFile.Substring(lastSeparator + 1);
            return fileName + ":" + SourceLine;
        }
    }
}
