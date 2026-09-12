using System;
using System.Collections.Generic;
using System.Text;

namespace LaM0uette.UniJect
{
    public sealed class ResolutionPath
    {
        #region Statements

        private readonly List<ResolutionFrame> _frames;

        public int Count
        {
            get { return _frames.Count; }
        }

        public IReadOnlyList<ResolutionFrame> Frames
        {
            get { return _frames; }
        }

        public ResolutionPath()
        {
            _frames = new List<ResolutionFrame>();
        }

        private ResolutionPath(List<ResolutionFrame> frames)
        {
            _frames = frames;
        }

        #endregion

        #region Methods

        public void Push(in ResolutionFrame frame)
        {
            _frames.Add(frame);
        }

        public void Pop()
        {
            if (_frames.Count > 0)
                _frames.RemoveAt(_frames.Count - 1);
        }

        public bool Contains(in ServiceIdentifier identifier)
        {
            for (int i = 0; i < _frames.Count; i++)
            {
                if (_frames[i].Identifier.Equals(identifier))
                    return true;
            }

            return false;
        }

        public ResolutionPath Snapshot()
        {
            return new ResolutionPath(new List<ResolutionFrame>(_frames));
        }

        public List<Type> ToTypeList()
        {
            List<Type> types = new List<Type>(_frames.Count);

            for (int i = 0; i < _frames.Count; i++)
                types.Add(_frames[i].Identifier.ContractType);

            return types;
        }

        public string Describe()
        {
            if (_frames.Count == 0)
                return "<root>";

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < _frames.Count; i++)
            {
                if (i > 0)
                    builder.Append(" -> ");

                builder.Append(_frames[i].Identifier);
            }

            return builder.ToString();
        }

        public string DescribeIndented()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("  <root>");

            for (int i = 0; i < _frames.Count; i++)
            {
                builder.AppendLine();
                builder.Append(' ', 4 + (i * 2));
                builder.Append(_frames[i].Identifier.ToString().PadRight(30));

                string site = _frames[i].DescribeSite();

                if (site != null)
                    builder.Append('(').Append(site).Append(')');
            }

            return builder.ToString();
        }

        #endregion
    }
}
