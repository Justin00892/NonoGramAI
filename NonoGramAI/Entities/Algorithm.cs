using System;
using System.Collections.Generic;
using System.Linq;
using NonoGramAI.Properties;

namespace NonoGramAI.Entities
{
    public class Algorithm
    {
        private static Grid _grid;
        private static Dictionary<Tile[,], int> _population;
        private static Random _rnd = new Random();

        public static Grid Random(Grid grid)
        {
            var tiles = Grid.GenerateNewTiles(grid.Size);
            var newGrid = new Grid(grid.Size, tiles, grid.TopHints, grid.SideHints);
            for (var row = 0; row < grid.Size; row++)
                RandomizeRow(newGrid, row);

            return newGrid;
        }

        private static void RandomizeRow(Grid grid, int row)
        {
            
            for (var x = 0; x < grid.Shaded(row); x++)
            {
                var col = _rnd.Next(grid.Size);
                if (grid.Tiles[row, col].State)
                    x--;
                else
                    grid.Tiles[row, col].State = true;
            }
        }

        public static Grid Genetic(Grid grid)
        {
            _grid = grid;
            _population = _grid.ExistingPop ?? new Dictionary<Tile[,], int>(Settings.Default.Population);
            while (_population.Count < Settings.Default.Population)
            {
                Tile[,] newGrid;
                do
                    newGrid = Random(_grid).Tiles;
                while (_population.ContainsKey(newGrid));
                _population.Add(newGrid, CheckWholeScore(newGrid,_grid.TopHints,_grid.SideHints));
            }
            NaturalSelection();
            var alpha = _population.OrderByDescending(g => g.Value).First().Key;
            _population.Remove(alpha);
            while (_population.Count < Settings.Default.Population)
            {
                var mate = _population.ElementAt(_rnd.Next(_population.Count)).Key;
                Tile[,] child;
                do
                {
                    //child = Random();
                    child = Crossover(alpha, mate);
                    //child = Mutator(child,_grid.TopHints);
                }
                while (_population.ContainsKey(child));
                _population.Add(child, CheckWholeScore(child, _grid.TopHints, _grid.SideHints));
            }

            var finalGrid = new Grid(_grid.Size, alpha, _grid.TopHints, _grid.SideHints)
            {
                ExistingPop = _population
            };
            return finalGrid;
        }
        private static void NaturalSelection()
        {
            var casualities = (_population.OrderByDescending(g => g.Value)
                                    .Skip(Settings.Default.Population / 2)
                                    .ToList());

            foreach (var casuality in casualities)
                _population.Remove(casuality.Key);
        }


        private static Tile[,] Crossover(Tile[,] alpha, Tile[,] mate)
        {         
            var size =(int) Math.Sqrt(alpha.Length);
            var tiles = Grid.GenerateNewTiles(size);
            var method = Settings.Default.CrossoverMethod == 3 ? _rnd.Next(3) : Settings.Default.CrossoverMethod;

            for (var row = 0; row < size; row++)
            {
                Tile[,] parent;
                switch (method)
                {
                    case 0:
                        //Randomly pick row from either parent
                        parent = _rnd.NextDouble() > 0.5 ? alpha : mate;
                        break;
                    case 1:
                        //Use parent with best row fitness
                        parent = CheckScore(alpha, row, true, _grid.TopHints,_grid.SideHints) > 
                                 CheckScore(mate, row, true, _grid.TopHints, _grid.SideHints) ? alpha : mate;
                        break;
                    case 2:
                        //Use any rows with perfect fitness, else random
                        if (CheckScore(alpha, row, true, _grid.TopHints, _grid.SideHints) > 
                            _grid.SideHints[row].Hints.Count)
                            parent = alpha;
                        else if (CheckScore(mate, row, true, _grid.TopHints, _grid.SideHints) > 
                                 _grid.SideHints[row].Hints.Count)
                            parent = mate;
                        else
                            parent = _rnd.NextDouble() > 0.5 ? alpha : mate;
                        break;
                    default:
                        parent = alpha;
                        break;
                }

                for (var col = 0; col < size; col++)
                {
                    tiles[row, col] = parent[row, col];
                }
            }

            return tiles;
        }

        private static Tile[,] Mutator(Tile[,] original, List<Hint> topHints)
        {
            var size = (int)Math.Sqrt(original.Length);
            var tiles = Grid.GenerateNewTiles(_grid.Size);
            var method = Settings.Default.MutationMethod == 4 ? _rnd.Next(4) : Settings.Default.MutationMethod;

            for (var row = 0; row < size; row++)
            {
                switch (method)
                {
                    case 0:
                        //Scoot: Go through a row using the hint values, and verify that the shaded squares match.
                        //       if there are too many shaded squares in a row, scoot them over to get the correct distribution.

                        break;
                    case 1:
                        //because we are solving a square, if we had rectangles this would need to be random by #columns.
                        var col = row;
                        var shaded = 0;
                        for (var r = 0; r < size; r++)
                            if (tiles[r, col].State)
                                shaded++;
                        //Column Too-Many: Look for a column with too many shaded values in it, select a shaded square. 
                        //       Within that shaded square's row, swap the shaded square with a non-shaded square.
                        if (shaded > topHints[col].Hints.Sum())
                        {
                            int i;
                            do
                                i = _rnd.Next(size);
                            while (!tiles[row, i].State);
                            TooManyTooFew(col, i, tiles);
                        }
                        //Column Too-Few: Look for a column with too few shaded values in it, select a non-shaded square. 
                        //       Within that non-shaded square's row, swap the non-shaded square with a shaded square.
                        else if (shaded < topHints[col].Hints.Sum())
                        {
                            int i;
                            do
                                i = _rnd.Next(size);
                            while (tiles[row, i].State);
                            TooManyTooFew(col, i, tiles);
                        }
                        
                        break;
                    case 3:
                        //Randomize Row
                        RandomizeRow(_grid, row);
                        break;
                    default:
                        RandomizeRow(_grid, row);
                        break;
                }
                for (var col = 0; col < size; col++)
                {
                    tiles[row, col] = original[row, col];
                }
            }
            return tiles;
        }

        private static void TooManyTooFew(int colNum, int rowNum, Tile[,] tiles)
        {
            var rnd = new Random();
            var state = tiles[rowNum, colNum].State;
            int rndInt;
            do
                rndInt = rnd.Next((int)Math.Sqrt(tiles.Length));
            while (rndInt == rowNum && tiles[rowNum, rndInt].State != state);

            tiles[rowNum, colNum].State = !state;
            tiles[rowNum, rndInt].State = state;
        }

        private static int CheckScore(Tile[,] grid, int position, bool isRow, List<Hint> topHints, List<Hint> sideHints)
        {
            var size = Math.Sqrt(grid.Length);
            var consecutiveList = new List<int>();
            var count = 0;
            List<int> hintList; 
            if (isRow)
            {
                hintList = sideHints[position].Hints;
                for (var col = 0; col < size; col++)
                {
                    if (grid[position, col].State)
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
                hintList = topHints[position].Hints;
                for (var row = 0; row < size; row++)
                {
                    if (grid[row, position].State)
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
        
        public static int CheckWholeScore(Tile[,] grid, List<Hint> topHints, List<Hint> sideHints)
        {
            var score = 0;
            var size = Math.Sqrt(grid.Length);
            for (var i = 0; i < size; i++)
            {    
                //checks columns
                var tempScore = CheckScore(grid, i, false, topHints,sideHints);            
                score += tempScore;

                //checks rows
                tempScore = CheckScore(grid, i, true, topHints, sideHints);                   
                score += tempScore;
            }

            return score;
        }
    }
}