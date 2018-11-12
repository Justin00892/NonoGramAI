using System;
using System.Collections.Generic;
using System.Linq;
using NonoGramAI.Properties;

namespace NonoGramAI.Entities
{
    public class Algorithm
    {
        private static Grid _grid;
        private static Dictionary<Grid, int> _population;
        private static Random _rnd = new Random();

        public static Grid Random(Grid grid)
        {
            var tiles = Grid.GenerateNewTiles(grid.Size);
            var newGrid = new Grid(grid.Size, tiles, grid.TopHints, grid.SideHints);
            for (var row = 0; row < grid.Size; row++)
                RandomizeRow(newGrid.Tiles, row, grid.Size, grid.Shaded(row));

            return newGrid;
        }

        private static void RandomizeRow(Tile[,] tiles, int row, int size, int shaded)
        {
            for (var x = 0; x < size; x++)
            {
                tiles[row, x].State = false;
            }

            for (var x = 0; x < shaded; x++)
            {
                var col = _rnd.Next(_grid.Size);
                if (tiles[row, col].State)
                    x--;
                else
                    tiles[row, col].State = true;
            }
        }

        public static Grid Genetic(Grid grid)
        {
            _grid = grid;
            _population = _grid.ExistingPop ?? new Dictionary<Grid, int>(Settings.Default.Population);
            while (_population.Count < Settings.Default.Population)
            {
                Grid newGrid;
                do
                    newGrid = Random(_grid);
                while (_population.ContainsKey(newGrid));
                _population.Add(newGrid, newGrid.WholeScore());
            }
            NaturalSelection();
            var alpha = _population.OrderByDescending(g => g.Value).First().Key;
            while (_population.Count < Settings.Default.Population)
            {
                var mate = _population.Where(p => !Equals(p.Key, alpha))
                    .OrderBy(p => _rnd.Next()).First().Key;
                Grid child;
                do
                {
                    child = Crossover(alpha, mate);
                    //var mutation = Mutator(child);                   
                    //child = child.Score > mutation.Score ? child : mutation;
                } while (_population.ContainsKey(child));
                _population.Add(child, child.WholeScore());               
            }

            var finalGrid = _population.OrderByDescending(p => p.Value).First().Key;
            finalGrid.ExistingPop = _population;
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
            var tiles = Grid.GenerateNewTiles(alpha.Size);
            var method = Settings.Default.CrossoverMethod == 3 ? _rnd.Next(3) : Settings.Default.CrossoverMethod;

            for (var row = 0; row < alpha.Size; row++)
            {
                Grid parent;
                switch (method)
                {
                    case 0:
                        //Randomly pick row from either parent
                        parent = _rnd.NextDouble() > 0.5 ? alpha : mate;
                        break;
                    case 1:
                        //Use parent with best row fitness
                        parent = alpha.WholeScore() > mate.WholeScore() 
                            ? alpha : mate;
                        break;
                    case 2:
                        //Use any rows with perfect fitness, else random
                        if (alpha.RowColScore(row,true) > _grid.SideHints[row].Hints.Count)
                            parent = alpha;
                        else if (mate.RowColScore(row,true) > _grid.SideHints[row].Hints.Count)
                            parent = mate;
                        else
                            parent = _rnd.NextDouble() > 0.5 ? alpha : mate;
                        break;
                    default:
                        parent = alpha;
                        break;
                }

                for (var col = 0; col < parent.Size; col++)
                {
                    tiles[row, col] = parent.Tiles[row, col];
                }
            }

            var newGrid = new Grid(alpha.Size, tiles, alpha.TopHints, alpha.SideHints);
            if (newGrid.WholeScore() == alpha.WholeScore() || newGrid.WholeScore() == mate.WholeScore())
                newGrid = Random(newGrid);
            return newGrid;
        }

        private static Grid Mutator(Grid original)
        {
            var tiles = Grid.GenerateNewTiles(_grid.Size);
            var method = Settings.Default.MutationMethod == 2 ? _rnd.Next(2) : Settings.Default.MutationMethod;

            for (var row = 0; row < original.Size; row++)
            {
                switch (method)
                {
                    case 0:
                        //Scoot: Go through a row using the hint values, and verify that the shaded squares match.
                        //       if there are too many shaded squares in a row, scoot them over to get the correct distribution.
                        RandomizeRow(tiles, row, original.Size, original.Shaded(row));
                        break;
                    case 1:
                        //because we are solving a square, if we had rectangles this would need to be random by #columns.
                        var col = row;
                        var shaded = 0;
                        for (var r = 0; r < original.Size; r++)
                            if (tiles[r, col].State)
                                shaded++;
                        //Column Too-Many: Look for a column with too many shaded values in it, select a shaded square. 
                        //       Within that shaded square's row, swap the shaded square with a non-shaded square.
                        if (shaded > _grid.TopHints[col].Hints.Sum())
                        {
                            int i;
                            do
                                i = _rnd.Next(original.Size);
                            while (!tiles[row, i].State);
                            TooManyTooFew(col, i, tiles);
                        }
                        //Column Too-Few: Look for a column with too few shaded values in it, select a non-shaded square. 
                        //       Within that non-shaded square's row, swap the non-shaded square with a shaded square.
                        else if (shaded < _grid.TopHints[col].Hints.Sum())
                        {
                            int i;
                            do
                                i = _rnd.Next(original.Size);
                            while (tiles[row, i].State);
                            TooManyTooFew(col, i, tiles);
                        }
                        
                        break;
                    default:
                        RandomizeRow(tiles, row, original.Size, original.Shaded(row));
                        break;
                }
                for (var col = 0; col < original.Size; col++)
                {
                    tiles[row, col] = original.Tiles[row, col];
                }
            }
            return new Grid(original.Size,tiles,original.TopHints,original.SideHints);
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
    }
}