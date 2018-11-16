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
        public int Score => GetScore();
        public int Stagnant { get; set; }
        public List<List<bool>> Solution { get; set; }
        public Dictionary<Grid, int> ExistingPop { get; set; }

        public Grid(int size, Tile[,] tiles, List<Hint> top, List<Hint> side, List< List<bool>> solution)
        {
            Tiles = tiles;
            TopHints = top;
            SideHints = side;
            Size = size;
            Solution = solution;           
        }

        public int Shaded(int row)
        {
            var score = 0;
            var hints = SideHints.Skip(row).FirstOrDefault();
            if (hints == null) return score;
            foreach (var h2 in hints.Hints)
                score += h2;

            return score;
        }

        public static Tile[,] GenerateNewTiles(int size)
        {
            var tiles = new Tile[size, size];
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    var tile = new Tile(i, j);
                    tiles[i, j] = tile;
                }
            }
            return tiles;
        }

        public static Tile[,] CopyTiles(Grid org)
        {
            var tiles = new Tile[org.Size, org.Size];
            for (var row = 0; row < org.Size; row++)
            {
                for (var col = 0; col < org.Size; col++)
                {
                    var tile = new Tile(row, col);
                    tiles[row, col] = tile;
                    tile.State = org.Tiles[row, col].State;
                }
            }
            return tiles;
        }

        //Scoots forward if end is greater than col and backwards if end is less than col
        public void Scoot(int row, int col, int end)
        {
            //Start backwards scoot at the beginning on the consecutive tiles
            if (end < col)
            {
                while (Tiles[row, col].State)
                    col--;
                col++;
            }
            var temp = Tiles[row, end].State;
            var placeholder = Tiles[row, col].State;
            while (col != end)
            {
                Tiles[row, col].State = temp;
                temp = placeholder;
                if (end > col)
                {
                    placeholder = Tiles[row, col + 1].State;
                    col++;
                }
                else if (end < col)
                {
                    placeholder = Tiles[row, col - 1].State;
                    col--;
                }
                if (col == end)
                    Tiles[row, col].State = temp;
            }

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
                for (var row = 0; row < Size; row++)
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

        private int GetScore()
        {
            var score = 0;

            for (var i = 0; i < Size; i++)
                score += GetRowScore(i);

            return score;
        }

        public int GetRowScore(int row)
        {
            var score = 0;

            for (var j = 0; j < Size; j++)
                if (Tiles[row, j].State == Solution[row][j] && Tiles[row, j].State)
                    score++;

            return score;
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