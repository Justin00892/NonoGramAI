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
        public int Size { get; }
        public int Score { get; set; }
        public int Stagnant { get; set; }
        public Dictionary<Grid, int> ExistingPop { get; set; }

        public Grid(int size, Tile[,] tiles, List<Hint> top, List<Hint> side)
        {
            Tiles = tiles;
            TopHints = top;
            SideHints = side;
            Size = size;
            Score = WholeScore();
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

        public void ReloadScore()
        {
            Score = WholeScore();
        }

        public void Scoot(int row, int col, int end)
        {
            var temp = Tiles[row, end].State;
            var placeholder = Tiles[row, col].State;
            for (; col <= end; col++)
            {
                Tiles[row, col].State = temp;
                temp = placeholder;
                placeholder = Tiles[row, col + 1].State;
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

        public int WholeScore()
        {
            var score = 0;
            for (var i = 0; i < Size; i++)
            {    
                //checks columns
                var tempScore = RowColScore(i, false);            
                score += tempScore;

                //checks rows
                tempScore = RowColScore(i, true);                   
                score += tempScore;
            }

            return score;
        }

        public int RowColScore(int pos, bool isRow)
        {
            var consecutiveList = new List<int>();
            var count = 0;
            List<int> hintList; 
            if (isRow)
            {
                hintList = SideHints[pos].Hints;
                for (var col = 0; col < Size; col++)
                {
                    if (Tiles[pos, col].State)
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
                hintList = TopHints[pos].Hints;
                for (var row = 0; row < Size; row++)
                {
                    if (Tiles[row, pos].State)
                        count++;
                    else if (count > 0)
                    {
                        consecutiveList.Add(count);
                        count = 0;
                    }
                }
            }
            if (count > 0) consecutiveList.Add(count);

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

        public override bool Equals(object obj)
        {
            if (!(obj is Grid tmp)) return false;
            return GetHashCode() == tmp.GetHashCode();
        }
    }
}