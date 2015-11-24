using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileRead
{
    public partial class WelcomeForm : Form
    {
        public WelcomeForm()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value < progressBar1.Maximum)
            {
                progressBar1.Value++;
                //MessageBox.Show(progressBar1.Value.ToString());
            }
            else
            {
                timer1.Enabled = false;
                this.Close();
            }
        }

        private void WelcomeForm_Load(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer1.Interval = 50;
        }
    }
}
