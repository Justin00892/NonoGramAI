using System.Collections.Generic;
using System.Linq;

namespace NonoGramAI.Entities
{
    public static class Crowds
    {
        public static Grid Crowd(List<Grid> grids)
        {
            var newGrid = grids.OrderByDescending(g => g.Score).First();
            var size = newGrid.Size;
            var rowDict = new Dictionary<Row,int>();
            foreach (var grid in grids)
            {
                for(var i = 0; i < grid.Size; i ++)
                {
                    var rowList = new Tile[grid.Size];
                    for (var j = 0; j < grid.Size; j++)
                        rowList[j] = grid.Tiles[i, j];
                    var row = new Row(rowList,i);

                    var count = 1;
                    if (rowDict.ContainsKey(row))
                        count = rowDict[row] + 1;
                    rowDict[row] = count;
                }
            }

            var tiles = Grid.GenerateNewTiles(newGrid.Size);
            for (var i = 0; i < size; i++)
            {
                var i1 = i;
                var tempDict = rowDict.Where(d => d.Key.Index == i1);
                var bestRow = tempDict.OrderByDescending(d => d.Value).First().Key;
                for(var j = 0; j < size; j++)
                    tiles[i, j] = bestRow.Tiles[j];
            }
            var finalGrid = new Grid(size, tiles,newGrid.TopHints,newGrid.SideHints);
            return finalGrid.Score > newGrid.Score ? finalGrid : newGrid;
        }
    }
}