using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tipper
{
    public enum DataInterpretationRuleType
    {
        UNKOWN = 0,
        SCORES_BY_TEAM = 1,
        SCORES_BY_GROUND = 2,
        SCORES_BY_STATE = 3,
        SCORES_BY_DAY = 4,
        SCORES_BY_SHARED_OPPONENTS = 5,
        SCORES_BY_QUALITY_OF_OPPONENTS = 6,
        WINS = 7,
        SCORING_SHOTS = 8,
        KICKS = 9,
        HANDBALLS = 10,
        MARKS = 11,
        HITOUTS = 12,
        TACKLES = 13,
        FREES = 14
    }
    public class DataInterpretationRule
    {
        /*public static int SCORES_BY_TEAM = 1;
        public static int SCORES_BY_GROUND = 2;
        STATE
        public static int SCORES_BY_DAY = 3;
        public static int SCORES_BY_SHARED_OPPONENTS = 4;
        public static int SCORES_BY_QUALITY_OF_OPPONENTS = 5;
        public static int WINS_BY_TEAM = 6;
        public static int SCORING_SHOTS = 7;
        public static int KICKS = 8;
        public static int HANDBALLS = 9;
        public static int MARKS = 10;
        public static int HITOUTS = 11;
        public static int TACKLES = 12;
        public static int FREES = 13;

        public string RuleName = "";*/
        public DataInterpretationRuleType Type = DataInterpretationRuleType.UNKOWN;
        public List<int> Periods = new List<int>();

        public DataInterpretationRule(DataInterpretationRuleType type, List<int> periods)
        {
            Type = type;
            Periods = periods;
        }
    }
    public class DataInterpretation
    {
        public List<DataInterpretationRule> Rules;

        public DataInterpretation(List<DataInterpretationRule> rules)
        {
            Rules = rules;
        }
    }
}
