using System.Text.RegularExpressions;

namespace ChatNetClient
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            string user_id = IdText.Text;
            string user_psw = PswText.Text;
            if (!Regex.IsMatch(user_id, @"^\d+$"))
            {
                MessageBox.Show("账号不正确", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (user_psw.Contains(' '))
            {
                MessageBox.Show("密码不能含有空格", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!Program.Login(user_id, user_psw))
            {
                PswText.Clear();
            }
        }

        public void LoginFail(Message message)
        {
            PswText.Clear();
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            RegisterForm regForm = new RegisterForm(this);
            this.Hide();
            regForm.ShowDialog();
        }

        private void LoginForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoginButton_Click(sender, e);
            }
        }

        public string GetServerIP() { return IPTextBox.Text; }
    }
}