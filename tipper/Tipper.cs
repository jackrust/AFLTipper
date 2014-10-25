using ArtificialNeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using AustralianRulesFootball;

namespace Tipper
{
    public class Tipper
    {
        public static int NUM_INPUTS = 18;
        public static int NUM_OUTPUTS = 4;
        public static int DEFAULT_HIDDENS = 5;
        public static int RELEVANT_ROUND_HISTORY = 7;
        public List<Team> Teams;
        public Ladder Ladder;
        public RecursiveLadder RLadder;
        public ContemporaryLadder CLadder;
        public League League;
        public Network net;

        public Tipper()
        {
            Teams = Util.getTeams();
            League = League.Load();
            Refresh(NUM_INPUTS, new List<int>() { DEFAULT_HIDDENS }, NUM_OUTPUTS);
        }

        public void Refresh(int maxEpochs, double targetError, int inputs, List<int> hiddens, int outputs)
        {
            Refresh(inputs, hiddens, outputs);
            net.MaxEpochs = maxEpochs;
            net.TargetError = targetError;
        }

        public void Refresh(int inputs, List<int> hiddens, int outputs)
        {
            net = new Network(inputs, hiddens, outputs);
            Ladder = new Ladder(Teams);
            RLadder = new RecursiveLadder(Teams);
            CLadder = new ContemporaryLadder(Teams);
        }

        public Data LearnFromScratchFromTo(int fromYear, int fromRound, int toYear, int toRound)
        {
            Refresh(NUM_INPUTS, new List<int>() { DEFAULT_HIDDENS }, NUM_OUTPUTS);
            return LearnFromTo(fromYear, fromRound, toYear, toRound);
        }

        public ArtificialNeuralNetwork.Data LearnFromTo(RoundShell roundFrom, RoundShell roundTo)
        {
            return LearnFromTo(roundFrom.Year, roundFrom.Number, roundTo.Year, roundTo.Number);
        }

        public ArtificialNeuralNetwork.Data LearnFromTo(int fromYear, int fromRound, int toYear, int toRound)
        {
            List<string> references = new List<string>();
            List<List<double>> inputs = new List<List<double>>();
            List<List<double>> targets = new List<List<double>>();
            var rounds = League.GetRounds(fromYear, fromRound, toYear, toRound).Where(x => x.Matches.Count > 0).ToList();

            for (var i = RELEVANT_ROUND_HISTORY; i < rounds.Count - 1; i++)
            {
                var s = new Season();
                var Ladder = new Ladder(Teams);
                var RLadder = new RecursiveLadder(Teams);
                var CLadder = new ContemporaryLadder(Teams);
                for (var j = i - RELEVANT_ROUND_HISTORY; j < i; j++)
                {
                    s.Rounds.Add(rounds[j]);
                    //Build Ladder
                    Ladder.addRound(rounds[j]);
                    RLadder.addRound(rounds[j]);
                    CLadder.addRound(rounds[j]);
                }

                foreach (var m in rounds[i+1].Matches)//predict one match ahead of collected data
                {
                    inputs.Add(new List<double>() {
                        //Ladder input
                        Numbery.Normalise(Ladder.GetRow(m.Home).LadderPoints(), GetMaxLadderPoints(Ladder.Rows[0].Played())), 
                        Numbery.Normalise(Ladder.GetRow(m.Away).LadderPoints(), GetMaxLadderPoints(Ladder.Rows[0].Played())),
                        Numbery.Normalise(RLadder.getRecursiveLadderRow(m.Home).RecursiveLadderPoints(), GetMaxRLadderPoints(Ladder.Rows[0].Played())), 
                        Numbery.Normalise(RLadder.getRecursiveLadderRow(m.Away).RecursiveLadderPoints(), GetMaxRLadderPoints(Ladder.Rows[0].Played())),
                        Numbery.Normalise(CLadder.getContemporaryLadderRow(m.Home).ContemporaryLadderPoints(), GetMaxCLadderPoints(Ladder.Rows[0].Played())), 
                        Numbery.Normalise(CLadder.getContemporaryLadderRow(m.Away).ContemporaryLadderPoints(), GetMaxCLadderPoints(Ladder.Rows[0].Played())),

                        //Scores By Team
                        Numbery.Normalise(s.GetMatches(0, i).Sum(x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals(s.GetMatches(0, i).Count())),
                        Numbery.Normalise(s.GetMatches(0, i).Sum(x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints(s.GetMatches(0, i).Count())),
                        Numbery.Normalise(s.GetMatches(0, i).Sum(x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals(s.GetMatches(0, i).Count())),
                        Numbery.Normalise(s.GetMatches(0, i).Sum(x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints(s.GetMatches(0, i).Count())),
                        Numbery.Normalise(s.GetMatches(0, i).Sum(x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals(s.GetMatches(0, i).Count())),
                        Numbery.Normalise(s.GetMatches(0, i).Sum(x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints(s.GetMatches(0, i).Count())),
                        Numbery.Normalise(s.GetMatches(0, i).Sum(x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals(s.GetMatches(0, i).Count())),
                        Numbery.Normalise(s.GetMatches(0, i).Sum(x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints(s.GetMatches(0, i).Count())),

                        //Scores By Ground
                        Numbery.Normalise(s.GetMatches(0, i).Where(x => x.Ground.Equals(m.Ground)).Sum(x => x.HomeScore().Goals), 
                            GetMaxSeasonGoals(s.GetMatches(0, i).Count(x => x.Ground.Equals(m.Ground)))), 
                        Numbery.Normalise(s.GetMatches(0, i).Where(x => x.Ground.Equals(m.Ground)).Sum(x => x.HomeScore().Points), 
                            GetMaxSeasonPoints(s.GetMatches(0, i).Count(x => x.Ground.Equals(m.Ground)))),
                        Numbery.Normalise(s.GetMatches(0, i).Where(x => x.Ground.Equals(m.Ground)).Sum(x => x.AwayScore().Goals), 
                            GetMaxSeasonGoals(s.GetMatches(0, i).Count(x => x.Ground.Equals(m.Ground)))),
                        Numbery.Normalise(s.GetMatches(0, i).Where(x => x.Ground.Equals(m.Ground)).Sum(x => x.AwayScore().Points), 
                            GetMaxSeasonPoints(s.GetMatches(0, i).Count(x => x.Ground.Equals(m.Ground)))),
                    });
                    targets.Add(new List<double>() {
                        Numbery.Normalise(m.HomeScore().Goals, Util.MAX_GOALS),
                        Numbery.Normalise(m.HomeScore().Points, Util.MAX_POINTS),
                        Numbery.Normalise(m.AwayScore().Goals, Util.MAX_GOALS),
                        Numbery.Normalise(m.AwayScore().Points, Util.MAX_POINTS),
                    });
                    references.Add(m.Home.APIName + " Vs " + m.Away.APIName);
                }
            }
            ArtificialNeuralNetwork.Data data = new Data()
            {
                References = references,
                Inputs = inputs,
                Outputs = targets
            };
            return data;
        }



        public List<Match> Predict(RoundShell round)
        {
            return Predict(round.Year, round.Number, false);
        }

        public List<Match> Predict(RoundShell round, bool print)
        {
            return Predict(round.Year, round.Number, print);
        }

        public List<Match> Predict(int year, int round, bool print)
        {
            List<Match> results = new List<Match>();
            var rounds = League.GetRounds(year, round-RELEVANT_ROUND_HISTORY, year, round).Where(x => x.Matches.Count > 0).ToList();

            var s = new Season();
            var Ladder = new Ladder(Teams);
            var RLadder = new RecursiveLadder(Teams);
            var CLadder = new ContemporaryLadder(Teams);
            for (var j = 0; j < rounds.Count; j++)
            {
                s.Rounds.Add(rounds[j]);
                //Build Relevant Ladder
                Ladder.addRound(rounds[j]);
                RLadder.addRound(rounds[j]);
                CLadder.addRound(rounds[j]);
            }

            //TODO League.GetCurrentSeason() should be pulled out
            foreach (var m in League.GetCurrentSeason().Rounds[round].Matches)
            {
                List<double> test = new List<double>() {
                    //Ladder Input
                    Numbery.Normalise(Ladder.GetRow(m.Home).LadderPoints(), GetMaxLadderPoints(Ladder.Rows[0].Played())), 
                    Numbery.Normalise(Ladder.GetRow(m.Away).LadderPoints(), GetMaxLadderPoints(Ladder.Rows[0].Played())),
                    Numbery.Normalise(RLadder.getRecursiveLadderRow(m.Home).RecursiveLadderPoints(), GetMaxRLadderPoints(Ladder.Rows[0].Played())), 
                    Numbery.Normalise(RLadder.getRecursiveLadderRow(m.Away).RecursiveLadderPoints(), GetMaxRLadderPoints(Ladder.Rows[0].Played())),
                    Numbery.Normalise(CLadder.getContemporaryLadderRow(m.Home).ContemporaryLadderPoints(), GetMaxCLadderPoints(CLadder.Rows[0].Played())), 
                    Numbery.Normalise(CLadder.getContemporaryLadderRow(m.Away).ContemporaryLadderPoints(), GetMaxCLadderPoints(CLadder.Rows[0].Played())),

                    //Scores By Team
                    Numbery.Normalise(s.GetMatches().Sum(x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals(s.GetMatches().Count())),
                    Numbery.Normalise(s.GetMatches().Sum(x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints(s.GetMatches().Count())),
                    Numbery.Normalise(s.GetMatches().Sum(x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals(s.GetMatches().Count())),
                    Numbery.Normalise(s.GetMatches().Sum(x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints(s.GetMatches().Count())),
                    Numbery.Normalise(s.GetMatches().Sum(x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals(s.GetMatches().Count())),
                    Numbery.Normalise(s.GetMatches().Sum(x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints(s.GetMatches().Count())),
                    Numbery.Normalise(s.GetMatches().Sum(x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals(s.GetMatches().Count())),
                    Numbery.Normalise(s.GetMatches().Sum(x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints(s.GetMatches().Count())),

                    //Scores By Ground
                    Numbery.Normalise(s.GetMatches().Where(x => x.Ground.Equals(m.Ground)).Sum(x => x.HomeScore().Goals), 
                        GetMaxSeasonGoals(s.GetMatches().Count(x => x.Ground.Equals(m.Ground)))), 
                    Numbery.Normalise(s.GetMatches().Where(x => x.Ground.Equals(m.Ground)).Sum(x => x.HomeScore().Points), 
                        GetMaxSeasonGoals(s.GetMatches().Count(x => x.Ground.Equals(m.Ground)))),
                    Numbery.Normalise(s.GetMatches().Where(x => x.Ground.Equals(m.Ground)).Sum(x => x.AwayScore().Goals), 
                        GetMaxSeasonGoals(s.GetMatches().Count(x => x.Ground.Equals(m.Ground)))),
                    Numbery.Normalise(s.GetMatches().Where(x => x.Ground.Equals(m.Ground)).Sum(x => x.AwayScore().Points), 
                        GetMaxSeasonGoals(s.GetMatches().Count(x => x.Ground.Equals(m.Ground)))),

                };

                var result = net.Run(test);
                results.Add(new Match(
                    m.Home,
                    new Score(0, 0),
                    new Score(0, 0),
                    new Score(0, 0),
                    new Score(
                        Numbery.Denormalise(result[0], Util.MAX_GOALS), 
                        Numbery.Denormalise(result[1], Util.MAX_POINTS)
                        ),
                    m.Away,
                    new Score(0, 0),
                    new Score(0, 0),
                    new Score(0, 0),
                    new Score(
                        Numbery.Denormalise(result[2], Util.MAX_GOALS),
                        Numbery.Denormalise(result[3], Util.MAX_POINTS)
                        ),
                    new Ground(), new DateTime()));

                if(print)
                    Console.WriteLine(m.Home.Mascot + " Vs " + m.Away.Mascot + ": " + 
                        printlayer(new double[]{
                            results.Last<Match>().HomeScore().Goals, 
                            results.Last<Match>().HomeScore().Points, 
                            results.Last<Match>().HomeScore().Total(),
                            results.Last<Match>().AwayScore().Goals, 
                            results.Last<Match>().AwayScore().Points,
                            results.Last<Match>().AwayScore().Total()
                        }));
            }
            return results;
        }

        public int MarkRound(int round, List<Match> tips)
        {
            int mark = 0;
            var matches = League.GetCurrentSeason().Rounds[round].Matches;
            if(tips.Count != matches.Count)
                return 0;
            for(var i = 0; i < matches.Count; i++)
            {
                for (var j = 0; j< tips.Count; j++)
                {
                    if (matches[i].getLosingTeam().Equals(tips[j].getLosingTeam()))
                        mark++;
                }
            }
            return mark;
        }

        public static double GetMaxLadderPoints(double rounds)
        {
            return rounds * 4;
        }
        public static double GetMaxRLadderPoints(double rounds)
        {
            return rounds * 4 + rounds * rounds * 2;
        }
        public static double GetMaxCLadderPoints(double rounds)
        {
            return Numbery.Factorial(rounds);
        }
        public static double GetMaxSeasonScores(double rounds)
        {
            return Util.MAX_SCORE * rounds;
        }
        public static double GetMaxSeasonGoals(double rounds)
        {
            return Util.MAX_GOALS * rounds;
        }
        public static double GetMaxSeasonPoints(double rounds)
        {
            return Util.MAX_POINTS * rounds;
        }

        public String printLadders()
        {
            String s = "";
            s += "\nGeneric Ladder:";
            s += Ladder.ToString();
            s += "\nRecursive Ladder:";
            s += RLadder.ToString();
            s += "\nContemporary Ladder:";
            s += CLadder.ToString();
            return s;
        }

        public static String printlayer(double[] vals)
        {
            String result = "{";
            for (int i = 0; i < vals.Length; i++)
            {
                result += String.Format("{0:N1}, ", vals[i]);
            }
            return result + "}";
        }
    }
}
