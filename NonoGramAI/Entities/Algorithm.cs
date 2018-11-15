using System;
using System.Collections.Generic;
using System.Linq;
using NonoGramAI.Properties;

namespace NonoGramAI.Entities
{
    public static class Algorithm
    {
        private static Grid _grid;
        private static Dictionary<Grid, int> _population;

        public static Grid Random(Grid grid)
        {
            var rnd = new Random();
            var tiles = Grid.GenerateNewTiles(grid.Size);
            var newGrid = new Grid(grid.Size, tiles, grid.TopHints, grid.SideHints);
            for (var row = 0; row < grid.Size; row++)
                RandomizeRow(newGrid, row, rnd);

            newGrid.ReloadScore();
            return newGrid;
        }

        private static void RandomizeRow(Grid grid, int row, Random rnd)
        {
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
            _population = _grid.ExistingPop ?? new Dictionary<Grid, int>(Settings.Default.Population);

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
            while (_population.Count < Settings.Default.Population)
            {
                var mate = _population.Where(p => !Equals(p.Key, alpha))
                    .OrderBy(p => rnd.Next()).First().Key;
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
            finalGrid.Stagnant++;
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
                        if (alpha.RowColScore(row,true) > _grid.SideHints[row].Hints.Count)
                            parent = alpha;
                        else if (mate.RowColScore(row,true) > _grid.SideHints[row].Hints.Count)
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

            var newGrid = new Grid(alpha.Size, tiles, alpha.TopHints, alpha.SideHints);
            if (newGrid.Score == alpha.Score || newGrid.Score == mate.Score)
                newGrid = Random(newGrid);
            return newGrid;
        }

        private static Grid Mutator(Grid original)
        {
            var rnd = new Random();
            var newGrid = new Grid(original.Size,original.Tiles,original.TopHints,original.SideHints);
            var method = rnd.Next(2);

            for (var row = 0; row < original.Size; row++)
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
                        for (var r = 0; r < original.Size; r++)
                            if (newGrid.Tiles[r, col].State)
                                shaded++;
                        //Column Too-Many: Look for a column with too many shaded values in it, select a shaded square. 
                        //       Within that shaded square's row, swap the shaded square with a non-shaded square.
                        if (shaded > _grid.TopHints[col].Hints.Sum())
                        {
                            int i;
                            do
                                i = rnd.Next(original.Size);
                            while (!newGrid.Tiles[row, i].State);
                            TooManyTooFew(col, i, newGrid);
                        }
                        //Column Too-Few: Look for a column with too few shaded values in it, select a non-shaded square. 
                        //       Within that non-shaded square's row, swap the non-shaded square with a shaded square.
                        else if (shaded < _grid.TopHints[col].Hints.Sum())
                        {
                            int i;
                            do
                                i = rnd.Next(original.Size);
                            while (newGrid.Tiles[row, i].State);
                            TooManyTooFew(col, i, newGrid);
                        }
                        
                        break;
                    default:
                        RandomizeRow(newGrid, row,rnd);
                        break;
                }
            }
            newGrid.ReloadScore();
            return newGrid;
        }

        private static void TooManyTooFew(int colNum, int rowNum, Grid grid)
        {
            var rnd = new Random();
            var state = grid.Tiles[rowNum, colNum].State;
            int rndInt;
            do
                rndInt = rnd.Next(grid.Size);
            while (rndInt == rowNum && grid.Tiles[rowNum, rndInt].State != state);

            grid.Tiles[rowNum, colNum].State = !state;
            grid.Tiles[rowNum, rndInt].State = state;
        }
    }
}