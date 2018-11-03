using System.Drawing;
using System.Windows.Forms;

namespace NonoGramAI.Entities
{
    public sealed class Tile : Panel
    {
        public int X { get; }
        public int Y { get; }

        private bool _state;
        public bool State
        {
            get => _state;
            set
            {
                _state = value;
                BackColor = _state ? Color.Black : Color.White;
            }
        }

        public Tile(int x, int y)
        {
            X = x;
            Y = y;

            Width = 30;
            Height = 30;
            Margin = new Padding(0);
            Padding = new Padding(0);
            BackColor = Color.White;
            BorderStyle = BorderStyle.FixedSingle;

        }
    }
}