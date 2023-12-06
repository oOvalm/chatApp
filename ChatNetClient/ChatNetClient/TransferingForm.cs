using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatNetClient
{
    public partial class TransferingForm : Form
    {
        public TransferingForm(string head, string msg)
        {
            InitializeComponent();
            Text = head;
            label1.Text = msg;
            progressBar1.Value = 0;
        }
    }
}
