using System;
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
            gridPanel.Controls.Clear();

            _tiles = new Tile[_size, _size];
            for (var i = 0; i < _size; i++)
            {
                for (var j = 0; j < _size; j++)
                {
                    var tile = new Tile(i, j);
                    tile.Click += (sender, args) =>
                    {
                        tile.State = !tile.State;
                        CheckScore();
                    };
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

        private void CheckScore()
        {
            var score = 0;
            //checks columns
            for (var i = 0; i < _size; i++)
            {
                var column = new LinkedList<Tile>();
                for(var j = 0; j <_size; j++)
                    column.AddLast((Tile) gridPanel.GetControlFromPosition(i,j));

                var consecutiveList = new List<int>();
                var current = column.First;
                var count = 0;
                while (current != null)
                {
                    if (current.Value.State)
                        count++;
                    else if(count > 0)
                    {
                        consecutiveList.Add(count);
                        count = 0;
                    }

                    current = current.Next;
                }
                if(count > 0) consecutiveList.Add(count);

                var hintList = _topHints[i].Hints;
                var tempScore = 0;
                if (consecutiveList.Count <= hintList.Count)
                {
                    for (var x = 0; x < hintList.Count; x++)
                    {
                        if (x >= consecutiveList.Count) break;
                        if (consecutiveList[x] == hintList[x]) tempScore++;
                    }
                    topListPanel.GetControlFromPosition(i, 0)
                            .Enabled = tempScore != hintList.Count;
                    if (tempScore == hintList.Count) tempScore++;
                }
                else topListPanel.GetControlFromPosition(i, 0)
                        .Enabled = true;
                    
                score += tempScore;
            }

            //checks rows
            for (var i = 0; i < _size; i++)
            {
                var row = new LinkedList<Tile>();
                for(var j = 0; j <_size; j++)
                    row.AddLast((Tile) gridPanel.GetControlFromPosition(j,i));

                var consecutiveList = new List<int>();
                var current = row.First;
                var count = 0;
                while (current != null)
                {
                    if (current.Value.State)
                        count++;
                    else if(count > 0)
                    {
                        consecutiveList.Add(count);
                        count = 0;
                    }

                    current = current.Next;
                }
                if(count > 0) consecutiveList.Add(count);

                var hintList = _sideHints[i].Hints;
                var tempScore = 0;
                if (consecutiveList.Count <= hintList.Count)
                {
                    for (var x = 0; x < hintList.Count; x++)
                    {
                        if (x >= consecutiveList.Count) break;
                        if (consecutiveList[x] == hintList[x]) tempScore++;
                    }
                    sideListPanel.GetControlFromPosition(0,i)
                            .Enabled = tempScore != hintList.Count;
                    if (tempScore == hintList.Count) tempScore++;
                }
                else sideListPanel.GetControlFromPosition(0,i)
                    .Enabled = true;
                    
                score += tempScore;
            }

            scoreLabel.Text = "Score: " + score;
        }

        private void runAIButton_Click(object sender, EventArgs e)
        {
            var rnd = new Random();
            foreach (var tile in _tiles)
            {
                tile.State = rnd.NextDouble() >= .5;
            }
            CheckScore();
        }
    }
}
