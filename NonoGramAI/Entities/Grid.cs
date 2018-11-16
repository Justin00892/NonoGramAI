using System;
using System.Collections.Generic;
using System.Linq;

namespace NonoGramAI.Entities
{
    public class Grid
    {
        public Tile[,] Tiles { get; }
        public List<Hint> TopHints { get; }
        public List<Hint> SideHints { get; }
        public int[] ColScores { get; set; }
        public int[] RowScores { get; set; }
        public int Size { get; }
        public int Score { get; set; }
        public int Stagnant { get; set; }
        public Dictionary<Grid, int> ExistingPop { get; set; }

        public Grid(int size, Tile[,] tiles, List<Hint> top, List<Hint> side, int[] colScores = null, int[] rowScores = null)
        {
            Tiles = tiles;
            TopHints = top;
            SideHints = side;
            Size = size;
            ColScores = colScores ?? new int[Size];
            RowScores = rowScores ?? new int[Size];
            WholeScore();
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

        public void InitScore()
        {
            for (int x = 0; x < Size; x++)
                ColScores[x] = RowColScore(x, false);
            
        }

        //Scoots forward if end is greater than col and backwards if end is less than col
        public void Scoot(int row, int col, int end)
        {
            var UpdateRows = new List<int>();
            var UpdateCols = new List<int>();
            //Start backwards scoot at the beginning on the consecutive tiles
            if (end < col)
            {
                while (Tiles[row, col].State)
                    col--;
                col++;
            }
            var temp = Tiles[row, end].State;
            bool placeholder = Tiles[row, col].State;
            while (col != end)
            {

                Tiles[row, col].State = temp;
                temp = placeholder;
                UpdateCols.Add(col);
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
            UpdateCols.Add(end);
            UpdateRows.Add(row);
            UpdateRowColScore(UpdateRows, UpdateCols);
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

        public void WholeScore()
        {
            var score = 0;
            score = +ColScores.Sum();
            score = +RowScores.Sum();

            Score = score;
        }

        public void UpdateRowColScore(List<int> rows, List<int> cols)
        {
            foreach (var row in rows)
                RowScores[row] = RowColScore(row, true);
            foreach (var col in cols)
                ColScores[col] = RowColScore(col, false);

            WholeScore();
        }

        public void RandomizeRow(int row, Random rnd)
        {
            var UpdateRows = new List<int>();
            var UpdateCols = new List<int>();
            for (var x = 0; x < Size; x++)
            {
                Tiles[row, x].State = false;
                UpdateCols.Add(x);
            }
            UpdateRows.Add(row);

            for (var x = 0; x < Shaded(row); x++)
            {
                var col = rnd.Next(Size);
                if (Tiles[row, col].State)
                    x--;
                else
                    Tiles[row, col].State = true;
            }
            UpdateRowColScore(UpdateRows, UpdateCols);
        }

        public void TooManyTooFew(int colNum, int rowNum, Random rnd)
        {
            var UpdateRows = new List<int>();
            var UpdateCols = new List<int>();
            var state = Tiles[rowNum, colNum].State;
            var possibleSwaps = new List<int>();
            for (var col = 0; col < Size; col++)
            {
                if (Tiles[rowNum, col].State != state)
                    possibleSwaps.Add(col);
            }
            if (possibleSwaps.Any())
            {
                var rndInt = possibleSwaps[rnd.Next(possibleSwaps.Count)];
                Tiles[rowNum, colNum].State = !state;
                Tiles[rowNum, rndInt].State = state;
                UpdateRows.Add(rowNum);
                UpdateCols.Add(colNum);
                UpdateCols.Add(rndInt);
                UpdateRowColScore(UpdateRows, UpdateCols);
            }

        }

        public int RowColScore(int pos, bool isRow)
        {
            var consecutiveList = GetConsecutiveList(pos, isRow);
            var hintList = isRow ? SideHints[pos].Hints : TopHints[pos].Hints; 

            var tempScore = 0;
            if (consecutiveList.Count > hintList.Count) return tempScore;
            //Adds one to score for every hint that has the corresponding size
            tempScore = hintList
                .TakeWhile((t, x) => x < consecutiveList.Count)
                .Where((t, x) => consecutiveList[x] == t).Count();
            //If all a row's hints are satisfied, up score
            if (tempScore == hintList.Count) tempScore++;

            return tempScore;
        }

        public override int GetHashCode()
        {
            var id = Tiles.Cast<Tile>()
                .Aggregate("", (current, tile) => current + (tile.State ? 1 : 0));
            return id.GetHashCode();
        }

        //public override bool Equals(object obj)
        //{
        //    if (!(obj is Grid tmp)) return false;
        //    return GetHashCode() == tmp.GetHashCode();
        //}
    }
}