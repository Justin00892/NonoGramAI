using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NonoGramAI.Entities;

namespace NonoGramAI
{
    public partial class MainForm : Form
    {
        private int _size = 20;
        private Tile[,] _tiles;
        private Hint[] _topHints;
        private Hint[] _sideHints;

        public MainForm()
        {
            InitializeComponent();

            //sets up grid
            gridPanel.RowCount = _size;
            gridPanel.ColumnCount = _size;

            _tiles = new Tile[_size, _size];
            for (var i = 0; i < _size; i++)
            {
                for (var j = 0; j < _size; j++)
                {
                    var tile = new Tile(i, j);

                    _tiles[i, j] = tile;
                    gridPanel.Controls.Add(tile);
                    gridPanel.SetRow(tile, i);
                    gridPanel.SetColumn(tile, j);
                }
            }

            //sets up top hints
            topListPanel.ColumnCount = _size;

            _topHints = new Hint[_size];
            for (var i = 0; i < _size; i++)
            {
                var hint = new Hint(new List<int> {1, 1},true);
                hint.Width = 30;
                hint.Height = 100;
                hint.TextAlign = ContentAlignment.BottomCenter;

                _topHints[i] = hint;
                topListPanel.Controls.Add(hint);
                topListPanel.SetColumn(hint, i);
            }

            //sets up side hints
            sideListPanel.RowCount = _size;

            _sideHints = new Hint[_size];
            for (var j = 0; j < _size; j++)
            {
                var hint = new Hint(new List<int> {1, 1},false);
                hint.Width = 100;
                hint.Height = 30;
                hint.TextAlign = ContentAlignment.MiddleRight;

                _sideHints[j] = hint;
                sideListPanel.Controls.Add(hint);
                sideListPanel.SetRow(hint, j);
            }
        }

        private void UpdateDisplay()
        {

        }
    }
}
