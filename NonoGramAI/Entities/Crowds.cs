using System.Collections.Generic;
using System.Linq;

namespace NonoGramAI.Entities
{
    public static class Crowds
    {
        public static Grid Crowd(List<Grid> grids)
        {
            var best = grids.OrderByDescending(g => g.Score).First();
            var size = best.Size;
            var rowDict = new Dictionary<Row,int>();
            foreach (var grid in grids)
            {
                for(var i = 0; i < grid.Size; i ++)
                {
                    var row = grid.Rows[i];

                    var count = 1;
                    if (rowDict.ContainsKey(row))
                        count = rowDict[row] + 1;
                    rowDict[row] = count;
                }
            }

            var newTiles = new List<Row>(size);
            for (var i = 0; i < size; i++)
            {
                var tempDict = rowDict.Where(d => d.Key.Index == i).ToList();
                var bestRow = tempDict.OrderByDescending(d => d.Value).First();

                if (bestRow.Value == 1)
                    bestRow = tempDict.OrderByDescending(d => d.Key.RowScore).First();

                newTiles.Add(bestRow.Key);
            }
            var newGrid = new Grid(newTiles,best.TopHints, best.SideHints, best.Solution);
            return newGrid.Score > best.Score ? newGrid : best;
        }
    }
}