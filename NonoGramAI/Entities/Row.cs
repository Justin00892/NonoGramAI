using System.Linq;

namespace NonoGramAI.Entities
{
    public class Row
    {
        public Tile[] Tiles { get; }
        public int Index { get; }

        public Row(Tile[] tiles, int index)
        {
            Tiles = tiles;
            Index = index;
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