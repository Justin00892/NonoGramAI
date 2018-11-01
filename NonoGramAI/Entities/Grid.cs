using System.Collections.Generic;
using System.Linq;

namespace NonoGramAI.Entities
{
    public class Grid
    {
        public Tile[,] Tiles { get; }
        public List<Hint> TopHints { get; }
        public List<Hint> SideHints { get; }
        public int Size { get; }
        public int Score => Algorithm.CheckWholeScore(Tiles, TopHints, SideHints);

        public Grid(int size, Tile[,] tiles, List<Hint> top, List<Hint> side)
        {
            Tiles = tiles;
            TopHints = top;
            SideHints = side;
            Size = size;
        }

        public override int GetHashCode()
        {
            var id = Tiles.Cast<Tile>()
                .Aggregate("", (current, tile) => current + (tile.State ? 1 : 0));
            return id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Grid tmp)) return false;
            return GetHashCode() == tmp.GetHashCode();
        }
    }
}