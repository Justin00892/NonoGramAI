using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
        private readonly Settings _settings = Settings.Default;

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
            var solution = new List<List<bool>>();
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
                while (hintString != "Solution" && hintString != null)
                {
                    var hintStringList = hintString.Split(',').ToList();
                    var hintList = new List<int>();
                    foreach (var str in hintStringList)
                        hintList.Add(int.Parse(str));

                    var hint = new Hint(hintList, false);
                    sideHints.Add(hint);
                    hintString = reader.ReadLine();
                }

                hintString = reader.ReadLine();
                while (hintString != null)
                {
                    var rowList = hintString.Split(',').ToList();
                    var row = new List<bool>();
                    foreach (var entry in rowList)
                        row.Add(entry != "0");
                    solution.Add(row);
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
                    var panel = tiles[i, j].getTilePanel();
                    var x = i;
                    var y = j;
                    panel.Click += (o, args) =>
                    {
                        _grid.Tiles[x, y].State = !_grid.Tiles[x, y].State;
                        panel.BackColor = _grid.Tiles[x, y].State ? Color.Black : Color.White;
                    };
                    gridPanel.Controls.Add(panel);
                    gridPanel.SetRow(panel, i);
                    gridPanel.SetColumn(panel, j);
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

            _grid = new Grid(size,tiles,topHints,sideHints,solution);

            chooseFileButton.Dispose();
            runAIButton.Show();
            gridPanel.Show();
        }

        private void UpdateDisplay()
        {
            for (var i = 0; i < _grid.Size; i++)
            {
                for (var j = 0; j < _grid.Size; j++)
                {
                    var panel = gridPanel.GetControlFromPosition(j, i);
                    panel.BackColor = _grid.Tiles[i, j].State ? Color.Black : Color.White;
                }
            }

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
                    _grid = await RunGA();
                    runAIButton.Enabled = true;
                    break;
                case 2:
                    _grid = await RunWoC();
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

        private async Task<Grid> RunGA()
        {
            var timer = new Stopwatch();
            timer.Start();
            for (var i = 0; i < _settings.Generations; i++)
            {
                var grid = await Task<Grid>.Factory.StartNew(() => Algorithm.Genetic(_grid));

                if (grid.Score > _grid.Score)
                {
                    _grid = grid;
                    UpdateDisplay();
                }
                else
                    _grid.Stagnant++;

                genLabel.Text = "Gen: " + i;
            }

            timer.Stop();
            
            return _grid;
        }

        private async Task<Grid> RunWoC()
        {
            var timer = new Stopwatch();
            var results = new List<Grid>(); 
            timer.Start();
            for(var i = 0; i < _settings.Population/2; i++)
            {
                for (var x = 0; x < _settings.Generations; x++)
                {
                    var grid = await Task<Grid>.Factory.StartNew(() => Algorithm.Genetic(_grid));
                    _grid = grid;
                }
                results.Add(_grid);
                genLabel.Text = "Complete: " + results.Count;
                _grid.ExistingPop = null;
            }
            var final = Crowds.Crowd(results);
            timer.Stop();
            timerLabel.Text = timer.Elapsed.ToString();
            UpdateDisplay();
            return final;
        }
    }
}
