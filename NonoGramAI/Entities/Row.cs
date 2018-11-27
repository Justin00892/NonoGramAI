using System.Collections.Generic;
using System.Linq;

namespace NonoGramAI.Entities
{
    public class Row
    {
        public List<Tile> Tiles { get; }
        public int Index { get; }
        public int RowScore => GetRowScore();
        private List<bool> Solution { get; }

        public Row(List<Tile> tiles, int index, List<bool> solution)
        {
            Tiles = tiles;
            Index = index;
            Solution = solution;
        }

        private int GetRowScore()
        {
            var score = Tiles.Where((t, j) => t.State == Solution[j] && t.State).Count();

            if (score == Solution.Count(s => s))
                score++;

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