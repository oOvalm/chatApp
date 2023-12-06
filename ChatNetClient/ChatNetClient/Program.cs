using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;

namespace ChatNetClient
{
    public static class Program
    {
        static LoginForm loginForm;
        static MainForm mainForm;
        private static TcpClient client;
        private static NetworkStream stream;
        static int ServerPort = 10110;
        static bool isLogin = false;
        static string user_id;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(loginForm = new LoginForm());
        }
        private static bool ConnetServer()
        {
            string serverIP = loginForm.GetServerIP();
            try
            {
                client = new TcpClient(serverIP, ServerPort);
                stream = client.GetStream();
            }
            catch(Exception ex)
            {
                MessageBox.Show("�޷����ӷ�����\n" + ex.Message, "error");
                client = null;
                stream = null;
                return false;
            }
            return true;
        }
        public static bool Login(string _user_id, string _user_psw) 
        {
            if (client == null)     // ��û���ӷ����� ��һ��
                if (!ConnetServer()) return false;
           // ������������¼
            Message LoginRequire = new Message(MessageType.LOGIN, _user_id + " " + _user_psw);
            NetworkUtil.SendToStream(stream, Parser.Obj2String(LoginRequire));
            Message result = Parser.String2Obj<Message>(NetworkUtil.ReadFromStream(stream));
            if(result.MsgType != MessageType.OK)
            {
                MessageBox.Show($"{result.MsgType}, {result.MsgInfo}", "��¼ʧ��");
                return false;
            }
            // ��½�ɹ�
            loginForm.Hide();
            user_id = _user_id;
            mainForm = new MainForm(client, loginForm, user_id);
            mainForm.Show();
            return true;
        }
        public static bool Register(string user_name, string user_psw)
        {
            if (client == null)
                if (!ConnetServer()) return false;
            // �����������ע��
            Message RegisterRequire = new Message(MessageType.REGISTER, user_name + " " + user_psw);
            NetworkUtil.SendToStream(stream, Parser.Obj2String(RegisterRequire));
            Message result = Parser.String2Obj<Message>(NetworkUtil.ReadFromStream(stream));
            if (result.MsgType != MessageType.OK)
            {
                MessageBox.Show($"{result.MsgType}, {result.MsgInfo}", "ע��ʧ��");
                return false;
            }
            MessageBox.Show($"{user_name}, �����˺�Ϊ: {result.MsgInfo}", "ע��ɹ�", MessageBoxButtons.OK);
            return true;
        }
    }
}