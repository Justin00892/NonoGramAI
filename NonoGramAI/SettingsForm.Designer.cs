namespace NonoGramAI
{
    partial class SettingsForm
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
            this.popTextBox = new System.Windows.Forms.TextBox();
            this.popLabel = new System.Windows.Forms.Label();
            this.genLabel = new System.Windows.Forms.Label();
            this.genTextBox = new System.Windows.Forms.TextBox();
            this.algTextBox = new System.Windows.Forms.TextBox();
            this.algLabel = new System.Windows.Forms.Label();
            this.crossLabel = new System.Windows.Forms.Label();
            this.crossTextBox = new System.Windows.Forms.TextBox();
            this.mutLabel = new System.Windows.Forms.Label();
            this.mutTextBox = new System.Windows.Forms.TextBox();
            this.trivialCheckBox = new System.Windows.Forms.CheckBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // popTextBox
            // 
            this.popTextBox.Location = new System.Drawing.Point(105, 10);
            this.popTextBox.Name = "popTextBox";
            this.popTextBox.Size = new System.Drawing.Size(100, 22);
            this.popTextBox.TabIndex = 0;
            // 
            // popLabel
            // 
            this.popLabel.AutoSize = true;
            this.popLabel.Location = new System.Drawing.Point(12, 13);
            this.popLabel.Name = "popLabel";
            this.popLabel.Size = new System.Drawing.Size(75, 17);
            this.popLabel.TabIndex = 1;
            this.popLabel.Text = "Population";
            // 
            // genLabel
            // 
            this.genLabel.AutoSize = true;
            this.genLabel.Location = new System.Drawing.Point(12, 41);
            this.genLabel.Name = "genLabel";
            this.genLabel.Size = new System.Drawing.Size(86, 17);
            this.genLabel.TabIndex = 2;
            this.genLabel.Text = "Generations";
            // 
            // genTextBox
            // 
            this.genTextBox.Location = new System.Drawing.Point(105, 38);
            this.genTextBox.Name = "genTextBox";
            this.genTextBox.Size = new System.Drawing.Size(100, 22);
            this.genTextBox.TabIndex = 3;
            // 
            // algTextBox
            // 
            this.algTextBox.Location = new System.Drawing.Point(105, 66);
            this.algTextBox.Name = "algTextBox";
            this.algTextBox.Size = new System.Drawing.Size(100, 22);
            this.algTextBox.TabIndex = 4;
            // 
            // algLabel
            // 
            this.algLabel.AutoSize = true;
            this.algLabel.Location = new System.Drawing.Point(12, 69);
            this.algLabel.Name = "algLabel";
            this.algLabel.Size = new System.Drawing.Size(67, 17);
            this.algLabel.TabIndex = 5;
            this.algLabel.Text = "Algorithm";
            // 
            // crossLabel
            // 
            this.crossLabel.AutoSize = true;
            this.crossLabel.Location = new System.Drawing.Point(12, 97);
            this.crossLabel.Name = "crossLabel";
            this.crossLabel.Size = new System.Drawing.Size(72, 17);
            this.crossLabel.TabIndex = 6;
            this.crossLabel.Text = "Crossover";
            // 
            // crossTextBox
            // 
            this.crossTextBox.Location = new System.Drawing.Point(105, 94);
            this.crossTextBox.Name = "crossTextBox";
            this.crossTextBox.Size = new System.Drawing.Size(100, 22);
            this.crossTextBox.TabIndex = 7;
            // 
            // mutLabel
            // 
            this.mutLabel.AutoSize = true;
            this.mutLabel.Location = new System.Drawing.Point(12, 125);
            this.mutLabel.Name = "mutLabel";
            this.mutLabel.Size = new System.Drawing.Size(62, 17);
            this.mutLabel.TabIndex = 8;
            this.mutLabel.Text = "Mutation";
            // 
            // mutTextBox
            // 
            this.mutTextBox.Location = new System.Drawing.Point(105, 122);
            this.mutTextBox.Name = "mutTextBox";
            this.mutTextBox.Size = new System.Drawing.Size(100, 22);
            this.mutTextBox.TabIndex = 9;
            // 
            // trivialCheckBox
            // 
            this.trivialCheckBox.AutoSize = true;
            this.trivialCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.trivialCheckBox.Location = new System.Drawing.Point(12, 150);
            this.trivialCheckBox.Name = "trivialCheckBox";
            this.trivialCheckBox.Size = new System.Drawing.Size(145, 21);
            this.trivialCheckBox.TabIndex = 11;
            this.trivialCheckBox.Text = "Solve Trivial Rows";
            this.trivialCheckBox.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(127, 177);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 12;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(12, 177);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 13;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(214, 210);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.trivialCheckBox);
            this.Controls.Add(this.mutTextBox);
            this.Controls.Add(this.mutLabel);
            this.Controls.Add(this.crossTextBox);
            this.Controls.Add(this.crossLabel);
            this.Controls.Add(this.algLabel);
            this.Controls.Add(this.algTextBox);
            this.Controls.Add(this.genTextBox);
            this.Controls.Add(this.genLabel);
            this.Controls.Add(this.popLabel);
            this.Controls.Add(this.popTextBox);
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox popTextBox;
        private System.Windows.Forms.Label popLabel;
        private System.Windows.Forms.Label genLabel;
        private System.Windows.Forms.TextBox genTextBox;
        private System.Windows.Forms.TextBox algTextBox;
        private System.Windows.Forms.Label algLabel;
        private System.Windows.Forms.Label crossLabel;
        private System.Windows.Forms.TextBox crossTextBox;
        private System.Windows.Forms.Label mutLabel;
        private System.Windows.Forms.TextBox mutTextBox;
        private System.Windows.Forms.CheckBox trivialCheckBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button saveButton;
    }
}