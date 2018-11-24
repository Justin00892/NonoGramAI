using System.Drawing;
using System.Windows.Forms;

namespace NonoGramAI.Entities
{
    public sealed class Tile
    {
        public int X { get; }
        public int Y { get; }
        public bool State { get; set; }
        public bool Set { get; set; }

        public Tile(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Panel GetTilePanel()
        {
            var panel = new Panel
            {
                Width = 30, Height = 30, 
                Margin = new Padding(0), 
                Padding = new Padding(0),
                BackColor = State ? Color.Black : Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            //if (Set && !State) panel.BackColor = Color.Red;
            //else if(Set && State) panel.BackColor = Color.Orange;

            return panel;
        }
    }
}