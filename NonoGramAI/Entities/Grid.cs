using System;
using System.Collections.Generic;
using System.Linq;
using NonoGramAI.Properties;

namespace NonoGramAI.Entities
{
    public class Grid
    {
        public List<Row> Rows { get; }
        public List<Hint> TopHints { get; }
        public List<Hint> SideHints { get; }
        public int Size => Rows.Count;
        public int Score => GetScore();
        public int Stagnant { get; set; }
        public List<List<bool>> Solution { get; }
        public Dictionary<Grid, int> ExistingPop { get; set; }

        public Grid(List<Row> rows, List<Hint> top, List<Hint> side, List<List<bool>> solution)
        {
            Rows = rows;
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

            var set = Rows[row].Tiles.Count(r => r.Set && r.State);

            return score-set;
        }

        public Grid GenerateNewTiles()
        {
            var tiles = new List<Row>();
            for (var i = 0; i < Size; i++)
            {
                var row = new List<Tile>(Size);
                for (var j = 0; j < Size; j++)
                    row.Add(new Tile());
                tiles.Add(new Row(row,i));
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
                            Rows[i].Tiles[j].State = false;
                            Rows[i].Tiles[j].Set = true;
                            x++;
                        }
                        else
                        {
                            Rows[i].Tiles[j].State = true;
                            Rows[i].Tiles[j].Set = true;
                        }

                    }
                }

                var largest = rowHints.OrderByDescending(h => h).First();
                if (largest == Size)
                {
                    for (var j = 0; j < Size; j++)
                    {
                        Rows[i].Tiles[j].State = true;
                        Rows[i].Tiles[j].Set = true;
                    }
                }
                else if (largest > Size / 2)
                {
                    var offset = Math.Abs(Size / 2 - largest);
                    var start = Size % 2 == 1 ? Size / 2 - offset + 1 : Size / 2 - offset;
                    for (var j = start; j < Size / 2 + offset; j++)
                    {
                        Rows[i].Tiles[j].State = true;
                        Rows[i].Tiles[j].Set = true;
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
                            Rows[j].Tiles[i].State = false;
                            Rows[j].Tiles[i].Set = true;
                            x++;
                        }
                        else
                        {
                            Rows[j].Tiles[i].State = true;
                            Rows[j].Tiles[i].Set = true;
                        }

                    }
                }

                largest = colHints.OrderByDescending(h => h).First();
                if (largest == Size)
                {
                    for (var j = 0; j < Size; j++)
                    {
                        Rows[j].Tiles[i].State = true;
                        Rows[j].Tiles[i].Set = true;
                    }
                }
                else if (largest > Size / 2)
                {
                    var offset = Math.Abs(Size / 2 - largest);
                    var start = Size % 2 == 1 ? Size / 2 - offset + 1 : Size / 2 - offset;
                    for (var j = start; j < Size / 2 + offset; j++)
                    {
                        Rows[j].Tiles[i].State = true;
                        Rows[j].Tiles[i].Set = true;
                    }
                }
            }
        }

        public static List<Row> CopyTiles(Grid org)
        {
            var tiles = new List<Row>(org.Size);
            for (var i = 0; i < org.Size; i++)
            {
                var row = new List<Tile>(org.Size);
                for (var j = 0; j < org.Size; j++)
                {
                    var tile = new Tile {State = org.Rows[i].Tiles[j].State};
                    row.Add(tile);
                }
                tiles.Add(new Row(row,i));
            }
            return tiles;
        }

        //Scoots forward if end is greater than col and backwards if end is less than col
        public void Scoot(int row, int col, int end)
        {
            //Start backwards scoot at the beginning on the consecutive rows
            if (end < col)
            {
                while (col >= 0 && Rows[row].Tiles[col].State)
                    col--;
                col++;
            }
            var temp = Rows[row].Tiles[end].State;
            var placeholder = Rows[row].Tiles[col].State;
            while (col != end)
            {
                if (!Rows[row].Tiles[col].Set)
                    Rows[row].Tiles[col].State = temp;
                temp = placeholder;
                if (end > col)
                {
                    if (!Rows[row].Tiles[col + 1].Set)
                        placeholder = Rows[row].Tiles[col + 1].State;
                    col++;
                }
                else if (end < col)
                {
                    if (!Rows[row].Tiles[col - 1].Set)
                        placeholder = Rows[row].Tiles[col - 1].State;
                    col--;
                }
                if (col == end)
                    Rows[row].Tiles[col].State = temp;
            }

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
                if (Rows[row].Tiles[j].State == Solution[row][j] && Rows[row].Tiles[j].State)
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