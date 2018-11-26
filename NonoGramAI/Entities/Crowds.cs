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
                    var row = new Row(grid.Tiles[i],i);

                    var count = 1;
                    if (rowDict.ContainsKey(row))
                        count = rowDict[row] + 1;
                    rowDict[row] = count;
                }
            }

            var newGrid = best.GenerateNewTiles();
            for (var i = 0; i < size; i++)
            {
                var i1 = i;
                var tempDict = rowDict.Where(d => d.Key.Index == i1);
                var bestRow = tempDict.OrderByDescending(d => d.Value).First().Key;
                for(var j = 0; j < size; j++)
                    newGrid.Tiles[i][j] = bestRow.Tiles[j];
            }
            return newGrid.Score > best.Score ? newGrid : best;
        }
    }
}