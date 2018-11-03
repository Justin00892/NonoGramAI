using System;
using System.Collections.Generic;
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
        private Settings _settings = Settings.Default;

        public MainForm()
        {
            InitializeComponent();

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
                
                while (hintString != "Side Hints" && hintString != null)
                {
                    var hintStringList = hintString.Split(',').ToList();
                    var hintList = new List<int>();
                    foreach (var str in hintStringList)
                        hintList.Add(int.Parse(str));

                    topHints.Add(new Hint(hintList,true)
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

                    sideHints.Add(new Hint(hintList,false)
                    {
                        Width = 100, Height = 30, 
                        TextAlign = ContentAlignment.MiddleRight
                    });
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
                    var tile = new Tile(i, j);
                    tile.Click += (sender, args) =>
                    {
                        tile.State = !tile.State;
                        UpdateScore();
                    };
                    tiles[i, j] = tile;
                    gridPanel.Controls.Add(tile);
                    gridPanel.SetRow(tile, i);
                    gridPanel.SetColumn(tile, j);
                }
            }

            //sets up top hints
            topListPanel.ColumnCount = size;

            for (var i = 0; i < topHints.Count; i++)
            {
                var hint = topHints[i];
                topListPanel.Controls.Add(hint);
                topListPanel.SetColumn(hint, i);
            }

            //sets up side hints
            sideListPanel.RowCount = size;

            for (var j = 0; j < size; j++)
            {
                var hint = sideHints[j];
                sideListPanel.Controls.Add(hint);
                sideListPanel.SetRow(hint, j);
            }
            _grid = new Grid(size,tiles,topHints,sideHints);
            TopMost = true;
            mainPanel.Show();
        }

        private void UpdateScore()
        {
            var score = 0;
            for (var i = 0; i < _grid.Size; i++)
            {
                var column = new LinkedList<Tile>();
                var row = new LinkedList<Tile>();
                for (var j = 0; j < _grid.Size; j++)
                {
                    column.AddLast((Tile) gridPanel.GetControlFromPosition(i,j));
                    row.AddLast((Tile) gridPanel.GetControlFromPosition(j,i));
                }
                    
                //checks columns
                var hintList = _grid.TopHints[i].Hints;
                var tempScore = Algorithm.CheckScore(column, hintList);
                topListPanel.GetControlFromPosition(i,0)
                        .Enabled = tempScore-1 != hintList.Count;                   
                score += tempScore;

                //checks rows
                hintList = _grid.SideHints[i].Hints;
                tempScore = Algorithm.CheckScore(row, hintList);
                sideListPanel.GetControlFromPosition(0,i)
                        .Enabled = tempScore-1 != hintList.Count;                    
                score += tempScore;
            }

            scoreLabel.Text = "Score: " + score;
        }

        private async void runAIButton_Click(object sender, EventArgs e)
        {
            switch (_settings.Algorithm)
            {
                case 0: _grid = await Task<Grid>.Factory.StartNew(
                        () => Algorithm.Random(_grid));
                    break;
                case 1:
                    _grid = await Task<Grid>.Factory.StartNew(
                        () => Algorithm.Genetic(_grid));
                    break;
                default: _grid = await Task<Grid>.Factory.StartNew(
                        () => Algorithm.Random(_grid));
                    break;
            }
            UpdateScore();
        }
    }
}
