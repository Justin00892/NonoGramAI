using System.Collections.Generic;
using System.Linq;

namespace NonoGramAI.Entities
{
    public class Grid
    {
        public Tile[,] Tiles { get; set; }
        public List<Hint> TopHints { get; }
        public List<Hint> SideHints { get; }
        public int Size { get; }
        public int Score => Algorithm.CheckWholeScore(this);

        public Grid(int size, Tile[,] tiles, List<Hint> top, List<Hint> side)
        {
            Tiles = tiles;
            TopHints = top;
            SideHints = side;
            Size = size;
        }

        public int Shaded(int row)
        {
            var score = 0;
            var hints = SideHints.Skip(row).FirstOrDefault();
                foreach (var h2 in hints.Hints)
                    score += h2;

            return score;
        }

        public static Tile[,] GenerateNewTiles(int Size)
        {
            var tiles = new Tile[Size, Size];
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    var tile = new Tile(i, j);
                    tiles[i, j] = tile;
                }
            }
            return tiles;
        }

        public void Scoot(int row)
        {
            
        }

        public List<int> GetConsecutiveList(int position, bool isRow)
        {
            var consecutiveList = new List<int>();
            var count = 0;
            if (isRow)
            {
                for (int col = 0; col < Size; col++)
                {
                    if (Tiles[position, col].State)
                        count++;
                    else if (count > 0)
                    {
                        consecutiveList.Add(count);
                        count = 0;
                    }
                }
            }
            else
            {
                for (int row = 0; row < Size; row++)
                {
                    if (Tiles[row, position].State)
                        count++;
                    else if (count > 0)
                    {
                        consecutiveList.Add(count);
                        count = 0;
                    }
                }
            }
            if (count > 0) consecutiveList.Add(count);
            return consecutiveList;
        }

        public void ClearTiles()
        {
            foreach (var tile in Tiles)
            {
                tile.State = false;
            }
        }

        public List<Tile> GetTiles()
        {
            var list = new List<Tile>();
            for (var i = 0; i < Size; i++)
                for(var j = 0; j < Size; j++)
                    list.Add(Tiles[i,j]);
            return list;
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