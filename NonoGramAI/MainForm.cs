using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NonoGramAI.Entities;

namespace NonoGramAI
{
    public partial class MainForm : Form
    {
        private int _size;
        private Tile[,] _tiles;
        private List<Hint> _topHints = new List<Hint>();
        private List<Hint> _sideHints = new List<Hint>();

        public MainForm()
        {
            InitializeComponent();

            var dr = openFileDialog.ShowDialog();
            if(dr != DialogResult.OK)
            {
                Application.Exit();
                return;
            }

            using (var reader = new StreamReader(openFileDialog.FileName))
            {
                //skips top line
                reader.ReadLine();
                var hintString = reader.ReadLine();
                while (hintString != "Side Hints" && hintString != null)
                {
                    var hintStringList = hintString.Split(',').ToList();
                    var hintList = new List<int>();
                    foreach (var str in hintStringList)
                        hintList.Add(int.Parse(str));

                    _topHints.Add(new Hint(hintList,true)
                    {
                        Width = 30, Height = 100, 
                        TextAlign = ContentAlignment.BottomCenter
                    });
                    hintString = reader.ReadLine();
                }

                hintString = reader.ReadLine();
                while (hintString != null)
                {
                    var hintStringList = hintString.Split(',').ToList();
                    var hintList = new List<int>();
                    foreach (var str in hintStringList)
                        hintList.Add(int.Parse(str));

                    _sideHints.Add(new Hint(hintList,false)
                    {
                        Width = 100, Height = 30, 
                        TextAlign = ContentAlignment.MiddleRight
                    });
                    hintString = reader.ReadLine();
                }
            }

            if (_topHints.Count != _sideHints.Count)
            {
                Application.Exit();
                return;
            }
            _size = _topHints.Count;
            SetUpGrid();
        }

        //sets up grid
        private void SetUpGrid()
        {
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

            for (var i = 0; i < _topHints.Count; i++)
            {
                var hint = _topHints[i];
                topListPanel.Controls.Add(hint);
                topListPanel.SetColumn(hint, i);
            }

            //sets up side hints
            sideListPanel.RowCount = _size;

            for (var j = 0; j < _size; j++)
            {
                var hint = _sideHints[j];
                sideListPanel.Controls.Add(hint);
                sideListPanel.SetRow(hint, j);
            }

            mainPanel.Show();
        }

        private void UpdateDisplay()
        {

        }
    }
}
