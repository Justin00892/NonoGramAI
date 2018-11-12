using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NonoGramAI.Entities;
using NonoGramAI.Properties;

namespace NonoGramAI
{
    public partial class MainForm : Form
    {
        private Grid _grid;
        private Grid _initial;
        private Settings _settings = Settings.Default;

        public MainForm()
        {
            InitializeComponent();
        }

        private void chooseFileButton_Click(object sender, EventArgs e)
        {
            chooseFileButton.Enabled = false;
            var dr = openFileDialog.ShowDialog();
            if(dr != DialogResult.OK)
            {
                Application.Exit();
                return;
            }
            var topHints = new List<Hint>();
            var sideHints = new List<Hint>();
            using (var reader = new StreamReader(openFileDialog.FileName))
            {
                //skips top line
                reader.ReadLine();
                var hintString = reader.ReadLine();

                var maxSize = 1;
                while (hintString != "Side Hints" && hintString != null)
                {
                    var hintStringList = hintString.Split(',').ToList();
                    var hintList = new List<int>();
                    foreach (var str in hintStringList)
                        hintList.Add(int.Parse(str));

                    if (hintList.Count > maxSize)
                        maxSize = hintList.Count;

                    var hint = new Hint(hintList, true);
                    topHints.Add(hint);
                    hintString = reader.ReadLine();
                }

                hintString = reader.ReadLine();
                while (hintString != null)
                {
                    var hintStringList = hintString.Split(',').ToList();
                    var hintList = new List<int>();
                    foreach (var str in hintStringList)
                        hintList.Add(int.Parse(str));

                    var hint = new Hint(hintList, false);
                    sideHints.Add(hint);
                    hintString = reader.ReadLine();
                }
            }

            if (topHints.Count != sideHints.Count)
            {
                Application.Exit();
                return;
            }
            var size = topHints.Count;
            gridPanel.RowCount = size;
            gridPanel.ColumnCount = size;
            gridPanel.Controls.Clear();

            var tiles = new Tile[size, size];
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    tiles[i, j] = new Tile(i, j);
                    var x = i;
                    var y = j;
                    tiles[i, j].Click += (sender1, args) =>
                    {
                        tiles[x, y].State = !tiles[x, y].State;
                        UpdateDisplay();
                    };
                    gridPanel.Controls.Add(tiles[i, j]);
                    gridPanel.SetRow(tiles[i, j], i);
                    gridPanel.SetColumn(tiles[i, j], j);
                }
            }

            
            topListPanel.ColumnCount = size;
            sideListPanel.RowCount = size;

            for (var i = 0; i < topHints.Count; i++)
            {
                //sets up top hints
                var hint = topHints[i];
                topListPanel.Controls.Add(hint);
                topListPanel.SetColumn(hint, i);

                //sets up side hints
                hint = sideHints[i];
                sideListPanel.Controls.Add(hint);
                sideListPanel.SetRow(hint, i);
            }

            _grid = new Grid(size,tiles,topHints,sideHints);
            _initial = _grid;

            chooseFileButton.Dispose();
            runAIButton.Show();
            gridPanel.Show();
        }

        private void UpdateDisplay()
        {
            foreach (var obj in gridPanel.Controls)
                if (obj is Tile tile)
                    tile.State = _grid.GetTiles()
                        .First(t => t.X == tile.X && t.Y == tile.Y).State;

            scoreLabel.Text = "Score: " + _grid.Score;
        }

        private async void runAIButton_Click(object sender, EventArgs e)
        {
            runAIButton.Enabled = false;
            switch (_settings.Algorithm)
            {
                case 0:
                    _grid = await Task<Grid>.Factory.StartNew(
                        () => Algorithm.Random(_grid));
                    UpdateDisplay();
                    runAIButton.Enabled = true;
                    break;
                case 1:
                    _grid = await Run();
                    runAIButton.Enabled = true;
                    break;
                default:
                    _grid = await Task<Grid>.Factory.StartNew(
                        () => Algorithm.Random(_grid));
                    UpdateDisplay();
                    runAIButton.Enabled = true;
                    break;
            }
        }

        private async Task<Grid> Run()
        {
            var results = new Dictionary<Grid, int>();
            var timer = new Stopwatch();
            timer.Start();
            while (results.Count < 1)
            {
                do
                    for (var i = 0; i < _settings.Generations/2; i++)
                        _grid = await Task<Grid>.Factory.StartNew(() => Algorithm.Genetic(_grid));
                while (results.ContainsKey(_grid));
                results.Add(_grid,_grid.Score);
                //UpdateDisplay();
                _grid = _initial;
            }
            timer.Stop();
            genLabel.Text = timer.Elapsed.ToString();
            return results.OrderByDescending(r => r.Value).First().Key;
        }
    }
}
