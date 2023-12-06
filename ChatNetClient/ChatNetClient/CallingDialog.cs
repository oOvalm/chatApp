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
    public partial class CallingDialog : Form
    {
        MainForm mainForm;
        public CallingDialog(string userid, string username, MainForm mainForm)
        {
            InitializeComponent();
            label1.Text = $"正在呼叫{username}({userid})...";
            this.mainForm = mainForm;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mainForm.EndCall();
            Close();
        }

        public void Deny(string msg)
        {
            if (msg.Contains("offline"))
                msg = msg.Replace("offline ", "");
            label1.Text = msg;
        }

        private void CloseForm(object sender, FormClosedEventArgs e)
        {
            mainForm.EndCall();
        }
    }
}
