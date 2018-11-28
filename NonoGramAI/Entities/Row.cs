using System.Collections.Generic;
using System.Linq;
using NonoGramAI.Properties;

namespace NonoGramAI.Entities
{
    public class Row
    {
        public List<Tile> Tiles { get; }
        public int Index { get; }
        public int RowScore => GetRowScore();
        private List<int> Hints { get; }
        private List<bool> Solution { get; }

        public Row(List<Tile> tiles, int index, List<int> hints, List<bool> solution)
        {
            Tiles = tiles;
            Index = index;
            Hints = hints;
            Solution = solution;
        }

        private int GetRowScore()
        {
            var score = 0;
            if (Settings.Default.SolutionScoring)
            {
                score = Tiles.Where((t, j) => t.State == Solution[j] && t.State).Count();

                if (score == Solution.Count(s => s))
                    score++;
            }
            else
            {
                var consecutiveList = new List<int>();
                var count = 0;
                foreach (var t in Tiles)
                {
                    if (t.State)
                        count++;
                    else if (count > 0)
                    {
                        consecutiveList.Add(count);
                        count = 0;
                    }
                }
                if (count > 0) consecutiveList.Add(count);

                if (consecutiveList.Count > Hints.Count) return score;
                //Adds one to score for every hint that has the coorsponding size
                score = Hints
                    .TakeWhile((t, x) => x < consecutiveList.Count)
                    .Where((t, x) => consecutiveList[x] == t).Count();
                //If all a row's hints are satisfied, up score
                if (score == Hints.Count) score++;
            }

            return score;
        }

        public override int GetHashCode()
        {
            var id = Tiles
                .Aggregate("", (current, tile) => current + (tile.State ? 1 : 0));
            id += Index;
            return id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Grid tmp)) return false;
            return GetHashCode() == tmp.GetHashCode();
        }
    }
}