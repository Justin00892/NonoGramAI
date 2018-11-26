using System;
using System.Collections.Generic;
using System.Linq;
using NonoGramAI.Properties;

namespace NonoGramAI.Entities
{
    public class Grid
    {
        public List<List<Tile>> Tiles { get; }
        public List<Hint> TopHints { get; }
        public List<Hint> SideHints { get; }
        public int Size => Tiles.Count;
        public int Score => GetScore();
        public int Stagnant { get; set; }
        public List<List<bool>> Solution { get; }
        public Dictionary<Grid, int> ExistingPop { get; set; }

        public Grid(List<List<Tile>> tiles, List<Hint> top, List<Hint> side, List<List<bool>> solution)
        {
            Tiles = tiles;
            TopHints = top;
            SideHints = side;
            Solution = solution;

            if(Settings.Default.SolveTrivial)
                SolveTrivial();
        }

        public int Shaded(int row)
        {
            var score = 0;
            var hints = SideHints.Skip(row).FirstOrDefault();
            if (hints == null) return score;
            foreach (var h2 in hints.Hints)
                score += h2;

            var set = Tiles[row].Count(r => r.Set && r.State);

            return score-set;
        }

        public Grid GenerateNewTiles()
        {
            var tiles = new List<List<Tile>>();
            for (var i = 0; i < Size; i++)
            {
                var row = new List<Tile>(Size);
                for (var j = 0; j < Size; j++)
                    row.Add(new Tile());
                tiles.Add(row);
            }

            return new Grid(tiles,TopHints,SideHints,Solution);
        }

        private void SolveTrivial()
        {
            for (var i = 0; i < Size; i++)
            {
                var rowHints = SideHints[i].Hints;

                if (rowHints.Sum() + rowHints.Count - 1 == Size)
                {
                    var x = 1;
                    for (var j = 0; j < Size; j++)
                    {
                        var temp = rowHints.GetRange(0, x);
                        if (j == temp.Sum() + temp.Count - 1)
                        {
                            Tiles[i][j].State = false;
                            Tiles[i][j].Set = true;
                            x++;
                        }
                        else
                        {
                            Tiles[i][j].State = true;
                            Tiles[i][j].Set = true;
                        }

                    }
                }

                var largest = rowHints.OrderByDescending(h => h).First();
                if (largest == Size)
                {
                    for (var j = 0; j < Size; j++)
                    {
                        Tiles[i][j].State = true;
                        Tiles[i][j].Set = true;
                    }
                }
                else if (largest > Size / 2)
                {
                    var offset = Math.Abs(Size / 2 - largest);
                    var start = Size % 2 == 1 ? Size / 2 - offset + 1 : Size / 2 - offset;
                    for (var j = start; j < Size / 2 + offset; j++)
                    {
                        Tiles[i][j].State = true;
                        Tiles[i][j].Set = true;
                    }
                }

                var colHints = TopHints[i].Hints;
                if (colHints.Sum() + colHints.Count - 1 == Size)
                {
                    var x = 1;
                    for (var j = 0; j < Size; j++)
                    {
                        var temp = colHints.GetRange(0, x);
                        if (j == temp.Sum() + temp.Count - 1)
                        {
                            Tiles[j][i].State = false;
                            Tiles[j][i].Set = true;
                            x++;
                        }
                        else
                        {
                            Tiles[j][i].State = true;
                            Tiles[j][i].Set = true;
                        }

                    }
                }

                largest = colHints.OrderByDescending(h => h).First();
                if (largest == Size)
                {
                    for (var j = 0; j < Size; j++)
                    {
                        Tiles[j][i].State = true;
                        Tiles[j][i].Set = true;
                    }
                }
                else if (largest > Size / 2)
                {
                    var offset = Math.Abs(Size / 2 - largest);
                    var start = Size % 2 == 1 ? Size / 2 - offset + 1 : Size / 2 - offset;
                    for (var j = start; j < Size / 2 + offset; j++)
                    {
                        Tiles[j][i].State = true;
                        Tiles[j][i].Set = true;
                    }
                }
            }
        }

        public static List<List<Tile>> CopyTiles(Grid org)
        {
            var tiles = new List<List<Tile>>(org.Size);
            for (var i = 0; i < org.Size; i++)
            {
                var row = new List<Tile>(org.Size);
                for (var j = 0; j < org.Size; j++)
                {
                    var tile = new Tile {State = org.Tiles[i][j].State};
                    row.Add(tile);
                }
                tiles.Add(row);
            }
            return tiles;
        }

        //Scoots forward if end is greater than col and backwards if end is less than col
        public void Scoot(int row, int col, int end)
        {
            //Start backwards scoot at the beginning on the consecutive tiles
            if (end < col)
            {
                while (Tiles[row][col].State)
                    col--;
                col++;
            }
            var temp = Tiles[row][end].State;
            var placeholder = Tiles[row][col].State;
            while (col != end)
            {
                if (!Tiles[row][col].Set)
                    Tiles[row][col].State = temp;
                temp = placeholder;
                if (end > col)
                {
                    if (!Tiles[row][col + 1].Set)
                        placeholder = Tiles[row][col + 1].State;
                    col++;
                }
                else if (end < col)
                {
                    if (!Tiles[row][col - 1].Set)
                        placeholder = Tiles[row][col - 1].State;
                    col--;
                }
                if (col == end)
                    Tiles[row][col].State = temp;
            }

        }

        public List<int> GetConsecutiveList(int position, bool isRow)
        {
            var consecutiveList = new List<int>();
            var count = 0;
            if (isRow)
            {
                for (var col = 0; col < Size; col++)
                {
                    if (Tiles[position][col].State)
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
                    if (Tiles[row][position].State)
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
                if (Tiles[row][j].State == Solution[row][j] && Tiles[row][j].State)
                    score++;
            if (score == SideHints[row].Hints.Sum())
                score++;

            return score;
        }

        public override int GetHashCode()
        {
            var id = 0;
            for (var i = 0; i < Size; i++)
            {
                id ^= GetRowScore(i).GetHashCode();
            }
            return id;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Grid tmp)) return false;
            return GetHashCode() == tmp.GetHashCode();
        }
    }
}