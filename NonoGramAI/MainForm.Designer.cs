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
            this.mainPanel = new System.Windows.Forms.Panel();
            this.sideListPanel = new System.Windows.Forms.TableLayoutPanel();
            this.topListPanel = new System.Windows.Forms.TableLayoutPanel();
            this.gridPanel = new System.Windows.Forms.TableLayoutPanel();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.AutoSize = true;
            this.mainPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mainPanel.Controls.Add(this.sideListPanel);
            this.mainPanel.Controls.Add(this.topListPanel);
            this.mainPanel.Controls.Add(this.gridPanel);
            this.mainPanel.Location = new System.Drawing.Point(13, 13);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(103, 103);
            this.mainPanel.TabIndex = 0;
            // 
            // sideListPanel
            // 
            this.sideListPanel.AutoSize = true;
            this.sideListPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.sideListPanel.ColumnCount = 1;
            this.sideListPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.sideListPanel.Location = new System.Drawing.Point(0, 100);
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
            this.topListPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.topListPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns;
            this.topListPanel.Location = new System.Drawing.Point(100, 0);
            this.topListPanel.Name = "topListPanel";
            this.topListPanel.RowCount = 1;
            this.topListPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.topListPanel.Size = new System.Drawing.Size(0, 0);
            this.topListPanel.TabIndex = 1;
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
            this.gridPanel.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(631, 632);
            this.Controls.Add(this.mainPanel);
            this.Name = "MainForm";
            this.Text = "Nonogram AI";
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.TableLayoutPanel gridPanel;
        private System.Windows.Forms.TableLayoutPanel sideListPanel;
        private System.Windows.Forms.TableLayoutPanel topListPanel;
    }
}

