using System;
using System.Collections.Generic;
using System.Linq;
using NonoGramAI.Properties;

namespace NonoGramAI.Entities
{
    public static class Algorithm
    {
        //private static Grid _grid;
        //private static Dictionary<Grid, int> _population;

        public static Grid Random(Grid grid)
        {
            var rnd = new Random();
            var newGrid = grid.GenerateNewTiles();
            for(var x = 0; x < grid.Size;x++)
                newGrid.Rows[x] = RandomizeRow(newGrid.Rows[x],newGrid.Shaded(x), rnd);

            return newGrid;
        }

        private static Row RandomizeRow(Row row, int shaded, Random rnd)
        {
            var potentialTiles = row.Tiles.Where(t => !t.Set).OrderBy(t => rnd.Next()).ToList();
            if (row.Tiles.All(r => r.Set && r.State)) 
                return row;

            foreach (var tile in potentialTiles)
                tile.State = false;

            for (var x = 0; x < shaded; x++)
            {
                potentialTiles.First().State = true;
                potentialTiles.Remove(potentialTiles.First());
            }

            return row;
        }

        public static Grid Genetic(Grid grid)
        {
            //_grid = grid;
            var rnd = new Random();
            var population = grid.ExistingPop ?? new Dictionary<Grid, int>();

            if (grid.Stagnant >= 25)
                NaturalSelection(population);

            while (population.Count < Settings.Default.Population)
            {
                Grid newGrid;
                do
                {
                    //Console.WriteLine(population.Count);
                    newGrid = Random(grid);
                }                  
                while (population.ContainsKey(newGrid));

                population.Add(newGrid, newGrid.Score);
            }
            NaturalSelection(population);
            var alpha = population.OrderByDescending(g => g.Value).First().Key;
            population.Remove(alpha);
            while (population.Count < Settings.Default.Population)
            {
                var mate = population.OrderBy(p => rnd.Next()).First().Key;
                Grid child;
                do
                {
                    child = Crossover(alpha, mate);
                    child = Mutator(child);                   
                    //child = child.Score > mutation.Score ? child : mutation;
                } while (population.ContainsKey(child));
                population.Add(child, child.Score);               
            }

            var finalGrid = population.OrderByDescending(p => p.Value).First().Key;
            finalGrid.ExistingPop = population;
            return finalGrid;
        }

        private static void NaturalSelection(Dictionary<Grid, int> population)
        {
            var avgScore = population.Values.Average();
            var temp = population.Where(p => p.Value > avgScore).ToList();
            population.Clear();
            foreach (var entry in temp)
            {
                if(!population.ContainsKey(entry.Key))
                    population.Add(entry.Key,entry.Value);
            }
        }

        private static Grid Crossover(Grid alpha, Grid mate)
        {
            var rnd = new Random();
            var newGrid = alpha.GenerateNewTiles();
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
                        parent = alpha.GetRowScore(row) > mate.GetRowScore(row) ? alpha : mate;
                        break;
                    case 2:
                        //Use any rows with perfect fitness, else random
                        if (alpha.GetRowScore(row) > alpha.SideHints[row].Hints.Count)
                            parent = alpha;
                        else if (mate.GetRowScore(row) > alpha.SideHints[row].Hints.Count)
                            parent = mate;
                        else
                            parent = rnd.NextDouble() > 0.5 ? alpha : mate;
                        break;
                    default:
                        parent = alpha;
                        break;
                }

                newGrid.Rows[row] = parent.Rows[row];

            }

            if (newGrid.Score == alpha.Score || newGrid.Score == mate.Score)
                newGrid = Random(newGrid);
            return newGrid;
        }

        private static Grid Mutator(Grid original)
        {
            var rnd = new Random();
            var newTiles = Grid.CopyTiles(original);
            var mutation = new Grid(newTiles,original.TopHints,original.SideHints, original.Solution);
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
                            if (mutation.Rows[row].Tiles[col].State && !mutation.Rows[row].Tiles[col].Set)
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
                            else if (counter > 0 && !mutation.Rows[row].Tiles[col].State && !mutation.Rows[row].Tiles[col].Set)
                            {
                                if (position >= hints.Count || position < hints.Count && counter < hints[position])
                                    optimumSwaps.Add(col);
                                position++;
                                counter = 0;
                            }
                            if (counter == 0 && !mutation.Rows[row].Tiles[col].State && !mutation.Rows[row].Tiles[col].Set)
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
                            if (mutation.Rows[r].Tiles[col].State)
                                shadedSpace.Add(r);
                            else
                                whiteSpace.Add(r);
                        }

                        //Column Too-Many: Look for a column with too many shaded values in it, select a shaded square. 
                        //       Within that shaded square's row, swap the shaded square with a non-shaded square.

                        var sum = original.TopHints[col].Hints.Sum();
                        if (shadedSpace.Count > sum)
                            do
                                row = shadedSpace[rnd.Next(shadedSpace.Count)];
                            while (mutation.Rows[row].Tiles[col].Set);
                        //Column Too-Few: Look for a column with too few shaded values in it, select a non-shaded square. 
                        //       Within that non-shaded square's row, swap the non-shaded square with a shaded square.
                        else if (shadedSpace.Count < sum)
                            row = whiteSpace[rnd.Next(whiteSpace.Count)];
                        if (shadedSpace.Count != sum)
                            TooManyTooFew(col, row, mutation);
                        break;
                    default:
                        RandomizeRow(mutation.Rows[row], mutation.Shaded(row),rnd);
                        break;
               }
            }
            return mutation;
        }

        private static void TooManyTooFew(int colNum, int rowNum, Grid grid)
        {
            var rnd = new Random();
            var state = grid.Rows[rowNum].Tiles[colNum].State;
            var possibleSwaps = new List<int>();
            for (var col = 0; col < grid.Size; col++)
            {
                if (grid.Rows[rowNum].Tiles[col].State != state && !grid.Rows[rowNum].Tiles[col].Set)
                    possibleSwaps.Add(col);
            }

            if (!possibleSwaps.Any()) return;
            var rndInt = possibleSwaps[rnd.Next(possibleSwaps.Count)];
            grid.Rows[rowNum].Tiles[colNum].State = !state;
            grid.Rows[rowNum].Tiles[rndInt].State = state;

        }
    }
}