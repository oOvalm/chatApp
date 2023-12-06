using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatNetClient
{
    public partial class RegisterForm : Form
    {
        private LoginForm loginForm;
        public RegisterForm(LoginForm loginForm)
        {
            this.loginForm = loginForm;
            InitializeComponent();
        }

        private void CancenButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            string user_name = UserNameTextBox.Text, psw = PswTextBox.Text, psw_confirm = PswConfirmTextBox.Text;
            if (user_name.Length == 0)
            {
                MessageBox.Show("请输入用户名", "错误");
                PswTextBox.Text = string.Empty;
                PswConfirmTextBox.Text = string.Empty;
                return;
            }
            if (psw.Length == 0)
            {
                MessageBox.Show("请输入密码", "错误");
                PswTextBox.Text = string.Empty;
                PswConfirmTextBox.Text = string.Empty;
                return;
            }
            if (user_name.Contains(' ') || psw.Contains(" "))
            {
                MessageBox.Show("用户名和密码不能带有空格", "错误");
                PswTextBox.Text = string.Empty;
                PswConfirmTextBox.Text = string.Empty;
                return;
            }
            if (!psw.Equals(psw_confirm))
            {
                MessageBox.Show("两次输入的密码不一致", "错误");
                PswTextBox.Text = string.Empty;
                PswConfirmTextBox.Text = string.Empty;
                return;
            }
            if (!Program.Register(user_name, psw))  // 如果注册失败 2个密码框清空
            {
                PswTextBox.Text = string.Empty;
                PswConfirmTextBox.Text = string.Empty;
            }
        }

        private void RegisterForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            loginForm.Show();
        }
    }
}
