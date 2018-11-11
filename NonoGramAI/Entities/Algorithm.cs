﻿using System;
using System.Collections.Generic;
using System.Linq;
using NonoGramAI.Properties;

namespace NonoGramAI.Entities
{
    public class Algorithm
    {
        private Random rnd;
        private Grid grid;

        public Algorithm(Grid grid)
        {
            rnd = new Random();
            this.grid = grid;

        }
        public Grid Random()
        {
            grid.ClearTiles();
            for (var row = 0; row < grid.Size; row++)
            {
                RandomizeRow(row, grid);
            }

            return grid;
        }

        private void RandomizeRow(int row, Grid grid)
        {
            for (var x = 0; x < grid.Shaded(row); x++)
            {
                var col = rnd.Next(grid.Size);
                if (grid.Tiles[row, col].State)
                    x--;
                else
                    grid.Tiles[row, col].State = true;
            }
        }

        //public static Grid Genetic(Grid grid)
        //{
        //    var population = new Dictionary<Grid,int>(Settings.Default.Population);
        //    while (population.Count < Settings.Default.Population)
        //    {
        //        Grid newGrid;
        //        do
        //            newGrid = Random(grid);
        //        while (population.ContainsKey(newGrid));
        //        population.Add(newGrid,newGrid.Score);
        //    }

        //    //crossover/mutation stuff here


        //    return population.OrderByDescending(g => g.Value).First().Key;
        //}

        public static int CheckScore(LinkedList<Tile> entries, List<int> hints)
        {
            var consecutiveList = new List<int>();
            var current = entries.First;
            var count = 0;
            while (current != null)
            {
                if (current.Value.State)
                    count++;
                else if(count > 0)
                {
                    consecutiveList.Add(count);
                    count = 0;
                }

                current = current.Next;
            }
            if(count > 0) consecutiveList.Add(count);

            var hintList = hints;
            var tempScore = 0;
            if (consecutiveList.Count > hintList.Count) return tempScore;
            tempScore = hintList
                .TakeWhile((t, x) => x < consecutiveList.Count)
                .Where((t, x) => consecutiveList[x] == t).Count();
            if (tempScore == hintList.Count) tempScore++;

            return tempScore;
        }

        public static int CheckWholeScore(Grid grid)
        {
            
            var score = 0;
            var size = grid.TopHints.Count;
            for (var i = 0; i < size; i++)
            {
                var column = new LinkedList<Tile>();
                var row = new LinkedList<Tile>();
                for (var j = 0; j < size; j++)
                {
                    column.AddLast(grid.Tiles[i,j]);
                    row.AddLast(grid.Tiles[j,i]);
                }
                    
                //checks columns
                var hintList = grid.TopHints[i].Hints;
                var tempScore = CheckScore(column, hintList);            
                score += tempScore;

                //checks rows
                hintList = grid.SideHints[i].Hints;
                tempScore = CheckScore(row, hintList);                   
                score += tempScore;
            }

            return score;
        }
    }
}