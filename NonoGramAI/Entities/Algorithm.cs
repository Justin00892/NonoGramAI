using System;
using System.Collections.Generic;
using System.Linq;
using NonoGramAI.Properties;

namespace NonoGramAI.Entities
{
    public static class Algorithm
    {
        private static Grid _grid;
        private static Dictionary<Grid, double> _population;

        public static Grid Random(Grid grid)
        {
            var rnd = new Random();
            var tiles = Grid.GenerateNewTiles(grid.Size);
            var newGrid = new Grid(grid.Size, tiles, grid.TopHints, grid.SideHints, grid.Solution);
            for (var row = 0; row < grid.Size; row++)
                RandomizeRow(newGrid, row, rnd);

            return newGrid;
        }

        private static void RandomizeRow(Grid grid, int row, Random rnd)
        {
            _grid = grid;
            for (var x = 0; x < grid.Size; x++)
                grid.Tiles[row, x].State = false;

            for (var x = 0; x < grid.Shaded(row); x++)
            {
                var col = rnd.Next(_grid.Size);
                if (grid.Tiles[row, col].State)
                    x--;
                else
                    grid.Tiles[row, col].State = true;
            }
        }

        public static Grid Genetic(Grid grid)
        {
            _grid = grid;
            var rnd = new Random();
            _population = _grid.ExistingPop ?? new Dictionary<Grid, double>(Settings.Default.Population);

            if (_grid.Stagnant >= 25)
                NaturalSelection();

            while (_population.Count < Settings.Default.Population)
            {
                Grid newGrid;
                do
                    newGrid = Random(_grid);
                while (_population.ContainsKey(newGrid));
                _population.Add(newGrid, newGrid.Score);
            }
            NaturalSelection();
            var alpha = _population.OrderByDescending(g => g.Value).First().Key;
            _population.Remove(alpha);
            while (_population.Count < Settings.Default.Population)
            {
                var mate = _population.OrderBy(p => rnd.Next()).First().Key;
                Grid child;
                do
                {
                    child = Crossover(alpha, mate);
                    child = Mutator(child);                   
                    //child = child.Score > mutation.Score ? child : mutation;
                } while (_population.ContainsKey(child));
                _population.Add(child, child.Score);               
            }

            var finalGrid = _population.OrderByDescending(p => p.Value).First().Key;
            finalGrid.ExistingPop = _population;

            return finalGrid;
        }

        private static void NaturalSelection()
        {
            var avgScore = _population.Values.Average();
            var temp = _population.Where(p => p.Value > avgScore).ToList();
            _population.Clear();
            foreach (var entry in temp)
            {
                if(!_population.ContainsKey(entry.Key))
                    _population.Add(entry.Key,entry.Value);
            }
        }


        private static Grid Crossover(Grid alpha, Grid mate)
        {
            var rnd = new Random();
            var tiles = Grid.GenerateNewTiles(alpha.Size);
            var method = rnd.Next(3);

            for (var row = 0; row < alpha.Size; row++)
            {
                Grid parent;
                switch (method)
                {
                    case 0:
                        //Randomly pick row from either parent
                        parent = rnd.NextDouble() > 0.5 ? alpha : mate;
                        break;
                    case 1:
                        //Use parent with best row fitness
                        parent = alpha.Score > mate.Score ? alpha : mate;
                        break;
                    case 2:
                        //Use any rows with perfect fitness, else random
                        if (alpha.GetRowScore(row) > _grid.SideHints[row].Hints.Count)
                            parent = alpha;
                        else if (mate.GetRowScore(row) > _grid.SideHints[row].Hints.Count)
                            parent = mate;
                        else
                            parent = rnd.NextDouble() > 0.5 ? alpha : mate;
                        break;
                    default:
                        parent = alpha;
                        break;
                }

                for (var col = 0; col < parent.Size; col++)
                    tiles[row, col] = parent.Tiles[row, col];
            }

            var newGrid = new Grid(alpha.Size, tiles, alpha.TopHints, alpha.SideHints, alpha.Solution);
            if (newGrid.Score == alpha.Score || newGrid.Score == mate.Score)
                newGrid = Random(newGrid);
            return newGrid;
        }

        private static Grid Mutator(Grid original)
        {
            var rnd = new Random();
            var newTiles = Grid.CopyTiles(original);
            var mutation = new Grid(original.Size,newTiles,original.TopHints,original.SideHints, original.Solution);
            var method = rnd.Next(2);

            for (var row = 0; row < original.Size; row++)
            {
                List<int> whiteSpace;
                int col;
                switch (method)
                {
                    case 0:
                        //Scoot: Go through a row using the hint values, and verify that the shaded squares match.
                        //       if there are too many shaded squares in a row, scoot them over to get the correct distribution.
                        //var tiles = mutation.GetConsecutiveList(row, true);
                        var hints = mutation.SideHints[row].Hints;
                        var optimumSwaps = new List<int>();
                        whiteSpace = new List<int>();
                        var position = 0;
                        var counter = 0;
                        int? swapCol = null;
                        for (col = 0; col < mutation.Size; col++)
                        {
                            if (mutation.Tiles[row, col].State)
                            {
                                counter++;
                                if (position < hints.Count)
                                {
                                    if (counter > hints[position] && swapCol == null)
                                        swapCol = col;
                                }
                                else
                                {
                                    if (swapCol == null)
                                        swapCol = col;
                                }
                                    
                            }
                            else if (counter > 0)
                            {
                                if (position >= hints.Count || (position < hints.Count && counter < hints[position]))
                                    optimumSwaps.Add(col);
                                position++;
                                counter = 0;
                            }
                            if (counter == 0)
                                whiteSpace.Add(col);
                        }
                        if (swapCol != null)
                        {
                            var swapEnd = optimumSwaps.Any() 
                                    ? optimumSwaps[rnd.Next(optimumSwaps.Count)] 
                                    : whiteSpace[rnd.Next(whiteSpace.Count)];
                            mutation.Scoot(row, swapCol.Value, swapEnd);
                        }
                        

                    break;
                    case 1:
                        //because we are solving a square, if we had rectangles this would need to be random by #columns.

                        //DO NOT need to swap in row = row, row now only refers to col
                        col = row;
                        whiteSpace = new List<int>();
                        var shadedSpace = new List<int>();
                        for (var r = 0; r < original.Size; r++)
                        {
                            //For each shaded in a col
                            if (mutation.Tiles[r, col].State)
                                shadedSpace.Add(r);
                            else
                                whiteSpace.Add(r);
                        }

                        //Column Too-Many: Look for a column with too many shaded values in it, select a shaded square. 
                        //       Within that shaded square's row, swap the shaded square with a non-shaded square.

                        var sum = _grid.TopHints[col].Hints.Sum();
                        if (shadedSpace.Count > sum)
                            row = shadedSpace[rnd.Next(shadedSpace.Count)];
                        //Column Too-Few: Look for a column with too few shaded values in it, select a non-shaded square. 
                        //       Within that non-shaded square's row, swap the non-shaded square with a shaded square.
                        else if (shadedSpace.Count < sum)
                            row = whiteSpace[rnd.Next(whiteSpace.Count)];
                        if (shadedSpace.Count != sum)
                            TooManyTooFew(col, row, mutation);
                        break;
                    default:
                        RandomizeRow(mutation, row,rnd);
                        break;
               }
            }
            return mutation;
        }

        private static void TooManyTooFew(int colNum, int rowNum, Grid grid)
        {
            var rnd = new Random();
            var state = grid.Tiles[rowNum, colNum].State;
            var possibleSwaps = new List<int>();
            for (var col = 0; col < grid.Size; col++)
            {
                if (grid.Tiles[rowNum, col].State != state)
                    possibleSwaps.Add(col);
            }

            if (!possibleSwaps.Any()) return;
            var rndInt = possibleSwaps[rnd.Next(possibleSwaps.Count)];
            grid.Tiles[rowNum, colNum].State = !state;
            grid.Tiles[rowNum, rndInt].State = state;

        }
    }
}