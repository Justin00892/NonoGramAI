using System.Windows.Forms;

namespace NonoGramAI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainPanel = new System.Windows.Forms.TableLayoutPanel();
            this.gridPanel = new System.Windows.Forms.TableLayoutPanel();
            this.sideListPanel = new System.Windows.Forms.TableLayoutPanel();
            this.topListPanel = new System.Windows.Forms.TableLayoutPanel();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.timerLabel = new System.Windows.Forms.Label();
            this.genLabel = new System.Windows.Forms.Label();
            this.chooseFileButton = new System.Windows.Forms.Button();
            this.scoreLabel = new System.Windows.Forms.Label();
            this.runAIButton = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainPanel.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.AutoSize = true;
            this.mainPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mainPanel.ColumnCount = 2;
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainPanel.Controls.Add(this.gridPanel, 1, 1);
            this.mainPanel.Controls.Add(this.sideListPanel, 0, 1);
            this.mainPanel.Controls.Add(this.topListPanel, 1, 0);
            this.mainPanel.Controls.Add(this.buttonPanel, 0, 0);
            this.mainPanel.Location = new System.Drawing.Point(0, 28);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.RowCount = 2;
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.Size = new System.Drawing.Size(100, 100);
            this.mainPanel.TabIndex = 0;
            // 
            // gridPanel
            // 
            this.gridPanel.AutoSize = true;
            this.gridPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gridPanel.ColumnCount = 1;
            this.gridPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.gridPanel.Location = new System.Drawing.Point(100, 100);
            this.gridPanel.Margin = new System.Windows.Forms.Padding(0);
            this.gridPanel.Name = "gridPanel";
            this.gridPanel.RowCount = 1;
            this.gridPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.gridPanel.Size = new System.Drawing.Size(0, 0);
            this.gridPanel.TabIndex = 3;
            this.gridPanel.Visible = false;
            // 
            // sideListPanel
            // 
            this.sideListPanel.AutoSize = true;
            this.sideListPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.sideListPanel.ColumnCount = 1;
            this.sideListPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.sideListPanel.Location = new System.Drawing.Point(0, 100);
            this.sideListPanel.Margin = new System.Windows.Forms.Padding(0);
            this.sideListPanel.Name = "sideListPanel";
            this.sideListPanel.RowCount = 1;
            this.sideListPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.sideListPanel.Size = new System.Drawing.Size(0, 0);
            this.sideListPanel.TabIndex = 2;
            // 
            // topListPanel
            // 
            this.topListPanel.AutoSize = true;
            this.topListPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.topListPanel.ColumnCount = 1;
            this.topListPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.topListPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns;
            this.topListPanel.Location = new System.Drawing.Point(100, 0);
            this.topListPanel.Margin = new System.Windows.Forms.Padding(0);
            this.topListPanel.Name = "topListPanel";
            this.topListPanel.RowCount = 1;
            this.topListPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.topListPanel.Size = new System.Drawing.Size(0, 0);
            this.topListPanel.TabIndex = 1;
            // 
            // buttonPanel
            // 
            this.buttonPanel.Controls.Add(this.timerLabel);
            this.buttonPanel.Controls.Add(this.genLabel);
            this.buttonPanel.Controls.Add(this.chooseFileButton);
            this.buttonPanel.Controls.Add(this.scoreLabel);
            this.buttonPanel.Controls.Add(this.runAIButton);
            this.buttonPanel.Location = new System.Drawing.Point(0, 0);
            this.buttonPanel.Margin = new System.Windows.Forms.Padding(0);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(100, 100);
            this.buttonPanel.TabIndex = 4;
            // 
            // timerLabel
            // 
            this.timerLabel.AutoSize = true;
            this.timerLabel.Location = new System.Drawing.Point(13, 51);
            this.timerLabel.Name = "timerLabel";
            this.timerLabel.Size = new System.Drawing.Size(56, 17);
            this.timerLabel.TabIndex = 3;
            this.timerLabel.Text = "Time: X";
            // 
            // genLabel
            // 
            this.genLabel.AutoSize = true;
            this.genLabel.Location = new System.Drawing.Point(13, 34);
            this.genLabel.Name = "genLabel";
            this.genLabel.Size = new System.Drawing.Size(52, 17);
            this.genLabel.TabIndex = 2;
            this.genLabel.Text = "Gen: X";
            // 
            // chooseFileButton
            // 
            this.chooseFileButton.Location = new System.Drawing.Point(0, 76);
            this.chooseFileButton.Name = "chooseFileButton";
            this.chooseFileButton.Size = new System.Drawing.Size(100, 23);
            this.chooseFileButton.TabIndex = 1;
            this.chooseFileButton.Text = "Choose File";
            this.chooseFileButton.UseVisualStyleBackColor = true;
            this.chooseFileButton.Click += new System.EventHandler(this.chooseFileButton_Click);
            // 
            // scoreLabel
            // 
            this.scoreLabel.AutoSize = true;
            this.scoreLabel.Location = new System.Drawing.Point(13, 13);
            this.scoreLabel.Name = "scoreLabel";
            this.scoreLabel.Size = new System.Drawing.Size(62, 17);
            this.scoreLabel.TabIndex = 1;
            this.scoreLabel.Text = "Score: X";
            // 
            // runAIButton
            // 
            this.runAIButton.Location = new System.Drawing.Point(0, 75);
            this.runAIButton.Margin = new System.Windows.Forms.Padding(0);
            this.runAIButton.Name = "runAIButton";
            this.runAIButton.Size = new System.Drawing.Size(100, 25);
            this.runAIButton.TabIndex = 0;
            this.runAIButton.Text = "Run AI";
            this.runAIButton.UseVisualStyleBackColor = true;
            this.runAIButton.Visible = false;
            this.runAIButton.Click += new System.EventHandler(this.runAIButton_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(782, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.resetToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(74, 24);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.optionsToolStripMenuItem.Text = "Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.options_Click);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.resetToolStripMenuItem.Text = "Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.reset_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(782, 753);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "Nonogram AI";
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.buttonPanel.ResumeLayout(false);
            this.buttonPanel.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel sideListPanel;
        private System.Windows.Forms.TableLayoutPanel topListPanel;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TableLayoutPanel mainPanel;
        private TableLayoutPanel gridPanel;
        private Panel buttonPanel;
        private Button runAIButton;
        private Label scoreLabel;
        private Button chooseFileButton;
        private Label genLabel;
        private Label timerLabel;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem resetToolStripMenuItem;
    }
}

