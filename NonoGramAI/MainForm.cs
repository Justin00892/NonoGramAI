using System;
using System.Collections.Concurrent;
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

        private MainForm(Grid grid)
        {
            InitializeComponent();
            chooseFileButton.Dispose();
            runAIButton.Show();
            runAIButton.Enabled = false;

            _grid = grid;
            ConstructBoard();
        }

        private void ConstructBoard()
        {
            var size = _grid.TopHints.Count;
            gridPanel.RowCount = size;
            gridPanel.ColumnCount = size;
            gridPanel.Controls.Clear();

            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    var panel = _grid.Tiles[i][j].GetTilePanel();
                    var x = i;
                    var y = j;
                    panel.Click += (o, args) =>
                    {
                        _grid.Tiles[x][y].State = !_grid.Tiles[x][y].State;
                        panel.BackColor = _grid.Tiles[x][y].State ? Color.Black : Color.White;
                    };
                    //if(_grid.Tiles[x][y].Set) panel.BackColor = Color.Red;
                    gridPanel.Controls.Add(panel);
                    gridPanel.SetRow(panel, i);
                    gridPanel.SetColumn(panel, j);
                }
            }

            topListPanel.ColumnCount = _grid.Size;
            sideListPanel.RowCount = _grid.Size;

            for (var i = 0; i < _grid.TopHints.Count; i++)
            {
                //sets up top hints
                var hint = _grid.TopHints[i];
                topListPanel.Controls.Add(hint);
                topListPanel.SetColumn(hint, i);

                //sets up side hints
                hint = _grid.SideHints[i];
                sideListPanel.Controls.Add(hint);
                sideListPanel.SetRow(hint, i);
            }

            chooseFileButton.Dispose();
            runAIButton.Show();
            gridPanel.Show();
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
            var tiles = new List<List<Tile>>(size);
            for (var i = 0; i < size; i++)
            {
                var row = new List<Tile>(size);
                for (var j = 0; j < size; j++)
                    row.Add(new Tile(i, j));
                tiles.Add(row);
            }
                
            

            _grid = new Grid(size,tiles,topHints,sideHints,solution);

            ConstructBoard();
        }

        private void UpdateDisplay()
        {
            for (var i = 0; i < _grid.Size; i++)
            {
                for (var j = 0; j < _grid.Size; j++)
                {
                    var panel = gridPanel.GetControlFromPosition(j, i);
                    panel.BackColor = _grid.Tiles[i][j].State ? Color.Black : Color.White;

                    //if (_grid.Tiles[i][j].Set && !_grid.Tiles[i][j].State) panel.BackColor = Color.Red;
                    //else if(_grid.Tiles[i][j].Set && _grid.Tiles[i][j].State) panel.BackColor = Color.Orange;
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
                    _grid = RunWoC();
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
                    timerLabel.Text = timer.Elapsed.ToString();
                }                
                else
                    _grid.Stagnant++; 

                genLabel.Text = "Gen: " + i;
            }
            timer.Stop();

            return _grid;
        }

        private Grid RunWoC()
        {
            var timer = new Stopwatch();
            var results = new ConcurrentBag<Grid>();
            var taskList = new Task[_settings.Population / 2];
            timer.Start();
            var numProcessors = Environment.ProcessorCount;
            for (var i = 0; i < taskList.Length;)
            {
                if (taskList.Count(t =>t != null && (int) t.Status <= 3) >= numProcessors) continue;
                taskList[i] = Task.Factory.StartNew(async () =>
                {
                    var newForm = new MainForm(_grid);
                    results.Add(await newForm.RunGA());
                });
                i++;
            }               
            Task.WaitAll(taskList);

            //var final = Crowds.Crowd(results.ToList());
            timer.Stop();
            timerLabel.Text = timer.Elapsed.ToString();
            UpdateDisplay();
            //return final;
            return _grid;
        }
    }
}
