using System.Collections.Generic;

namespace UniJect.Demo
{
    public sealed class Referee
    {
        #region Statements

        private readonly IReadOnlyList<IScoreRule> _rules;

        public Referee(IEnumerable<IScoreRule> rules)
        {
            _rules = new List<IScoreRule>(rules);
        }

        #endregion

        #region Methods

        public int Total()
        {
            int total = 0;

            for (int i = 0; i < _rules.Count; i++)
                total += _rules[i].Points;

            return total;
        }

        public int RuleCount()
        {
            return _rules.Count;
        }

        #endregion
    }
}
