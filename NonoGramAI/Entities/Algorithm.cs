using System;
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
            var tiles = Grid.GenerateNewTiles(grid.Size);
            var newGrid = new Grid(grid.Size, tiles, grid.TopHints, grid.SideHints);
            for (var row = 0; row < grid.Size; row++)
            {
                RandomizeRow(newGrid, row);
            }

            return newGrid;
        }

        private void RandomizeRow(Grid grid, int row)
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

        public Grid Genetic()
        {
            var population = new Dictionary<Grid, int>(Settings.Default.Population);
            while (population.Count < Settings.Default.Population)
            {
                Grid newGrid;
                do
                    newGrid = Random();
                while (population.ContainsKey(newGrid));
                population.Add(newGrid, newGrid.Score);
            }
            NaturalSelection(population);
            var alpha = population.OrderByDescending(g => g.Value).First();
            Grid child;
            population.Remove(alpha.Key);
            KeyValuePair<Grid, int> mate;
            while (population.Count < Settings.Default.Population)
            {
                mate = population.ElementAt(rnd.Next(population.Count));
                child = Crossover(alpha.Key, mate.Key);
                do
                    child = Random();
                while (population.ContainsKey(child));
                population.Add(child, child.Score);
            }

            return population.OrderByDescending(g => g.Value).First().Key;
        }

        private void NaturalSelection(Dictionary<Grid, int> population)
        {
            var casualities = (population.OrderByDescending(g => g.Value)
                                    .Skip(Settings.Default.Population / 2)
                                    .ToList());

            foreach (var casuality in casualities)
                population.Remove(casuality.Key);
        }

        private Grid Crossover (Grid alpha, Grid mate)
        {
            var tiles = Grid.GenerateNewTiles(alpha.Size);
            var child = new Grid(alpha.Size, tiles, alpha.TopHints, alpha.SideHints);
            var method = rnd.Next(3);

            var parent = alpha;
            for (var row = 0; row < alpha.Size; row++)
            {
                switch (method)
                {
                    case 0:
                        //Randomly pick row from either parent
                        parent = rnd.NextDouble() > 0.5 ? alpha : mate;
                        break;
                    case 1:
                        //Use parent with best row fitness
                        parent = CheckScore(alpha, row, true) > CheckScore(mate, row, true) ? alpha : mate;
                        break;
                    case 2:
                        //Use any rows with perfect fitness, else random
                        if (CheckScore(alpha, row, true) > alpha.SideHints[row].Hints.Count)
                        parent = alpha;
                        else if (CheckScore(mate, row, true) > mate.SideHints[row].Hints.Count)
                            parent = mate;
                        else
                            parent = rnd.NextDouble() > 0.5 ? alpha : mate;
                        break;
                }
                for (var col = 0; col < alpha.Size; col++)
                {
                    tiles[row, col] = parent.Tiles[row, col];
                }
            }
            return child;
            
        }

        public static int CheckScore(Grid grid, int position, bool isRow)
        {
            var consecutiveList = new List<int>();
            var count = 0;
            List<int> hintList; 
            if (isRow)
            {
                hintList = grid.SideHints[position].Hints;
                for (int col = 0; col < grid.Size; col++)
                {
                    if (grid.Tiles[position, col].State)
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
                hintList = grid.TopHints[position].Hints;
                for (int row = 0; row < grid.Size; row++)
                {
                    if (grid.Tiles[row, position].State)
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
            //Adds one to score for every hint that has the coorsponding size
            tempScore = hintList
                .TakeWhile((t, x) => x < consecutiveList.Count)
                .Where((t, x) => consecutiveList[x] == t).Count();
            //If all a row's hints are satisfied, up score
            if (tempScore == hintList.Count) tempScore++;

            return tempScore;
        }


        public static int CheckWholeScore(Grid grid)
        {
            var score = 0;
            for (var i = 0; i < grid.Size; i++)
            {    
                //checks columns
                var tempScore = CheckScore(grid, i, false);            
                score += tempScore;

                //checks rows
                tempScore = CheckScore(grid, i, true);                   
                score += tempScore;
            }

            return score;
        }
    }
}