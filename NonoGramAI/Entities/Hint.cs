using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NonoGramAI.Entities
{
    public sealed class Hint : Label
    {
        public List<int> Hints { get; }

        public Hint(List<int> hints,bool top)
        {
            Hints = hints;

            Text = "";
            Font = new Font("Comic Sans MS", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Margin = new Padding(0);
            Padding = new Padding(0);
            BackColor = Color.White;
            BorderStyle = BorderStyle.FixedSingle;

            foreach (var hint in Hints)
            {
                if (top)
                    Text += "\n" + hint;
                else
                    Text += " " + hint;
            }
        }
    }
}