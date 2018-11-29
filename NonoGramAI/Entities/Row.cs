using System;
using System.Collections.Generic;
using System.Linq;

namespace NonoGramAI.Entities
{
    public class Row
    {
        public List<Tile> Tiles { get; }
        public int Index { get; }
        public int RowScore => GetRowScore();
        private List<int> Hints { get; }
        private List<bool> Solution { get; }

        public Row(List<Tile> tiles, int index, List<int> hints, List<bool> solution)
        {
            Tiles = tiles;
            Index = index;
            Hints = hints;
            Solution = solution;
        }

        private int GetRowScore()
        {
            var score = 0;
            var pos = 0;
            foreach (var hint in Hints)
            {
                var count = 0;
                var broken = false;
                while(pos < Tiles.Count)
                {
                    if (count > hint)
                    {
                        score -= count;
                        pos++;
                        break;
                    }
                    if (!broken)
                    {
                        if (Tiles[pos].State) count++;

                        else if (count > 0)
                        {
                            if (count == hint)
                            {
                                score += count;
                                pos++;
                                break;
                            }

                            broken = true;
                            score--;
                        }
                    }
                    else
                    {
                        if (Tiles[pos].State)
                        {
                            count++;
                            if (count == hint)
                            {
                                pos++;
                                break;
                            }
                        }
                        else score--;
                    }

                    pos++;
                }

            }

            if (score == Hints.Sum())
                score *= 2;

            //Console.WriteLine("Row: " + Index + ", Score: "+score);
            return score;
        }

        public override int GetHashCode()
        {
            var id = Tiles
                .Aggregate("", (current, tile) => current + (tile.State ? 1 : 0));
            id += Index;
            return id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Grid tmp)) return false;
            return GetHashCode() == tmp.GetHashCode();
        }
    }
}