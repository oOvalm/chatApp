using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ChatNetClient
{
    public partial class CallForm : Form
    {
        TcpClient client;
        Stream stream;
        ClientService service;
        string src_id, dest_id;

        #region 发送接收文件相关
        //private string SendFilePath = null;
        private delegate void Delegate_SetVisible(bool vis, string msg);
        private Delegate_SetVisible set_Visible;
        #endregion
        public CallForm(int CallType, string src_id, string dest_id, string dest_ip, int port, ClientService service)
        {
            InitializeComponent();
            this.service = service;
            this.src_id = src_id;
            this.dest_id = dest_id;
            label1.Text = $"{service.mainForm.FriendList["USER" + dest_id].FriendName}({dest_id})";
            this.Text = $"{service.mainForm.current_user_name}({src_id})";
            if (CallType == 1)      // 被呼叫方，打开port等待对方tcpclient
            {
                TcpListener listener = new TcpListener(IPAddress.Parse(dest_ip), port);
                listener.Start();
                client = listener.AcceptTcpClient();
            }
            else if (CallType == 2)  // 呼叫方直接链接对方
            {
                Debug.Assert(port != -1);
                client = new TcpClient(dest_ip, port);
                service.mainForm.EndCall();
            }
            set_Visible = new Delegate_SetVisible(SetVisible);
            stream = client.GetStream();
            Thread th = new Thread(() => ListenMsg());
            th.Start();
        }


        #region 控件触发的函数
        private void SendButton_Click(object sender, EventArgs e)
        {
            string sendText = SendTextBox.Text;
            if (sendText.Length == 0 || (sendText.Length == 1 && (sendText[0] == '\r' || sendText[0] == '\n'))
                || (sendText.Length == 2 && (sendText[0] == '\r' || sendText[0] == '\n') && sendText[1] < 32))
            {
                SendTextBox.Text = string.Empty;
                MessageBox.Show("不能发送空信息！", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DateTime now = DateTime.Now;
            string dest_name = service.mainForm.FriendList["USER" + dest_id].FriendName;
            sendText = $"{dest_name}  {now}\r\n{sendText}\r\n\r\n";
            MessageTextBox.Text += sendText;
            SendMessage SendMsg = new SendMessage(CardType.USER, src_id, dest_id, sendText);
            NetworkUtil.SendToStream(stream, Parser.Obj2String(new Message(MessageType.SEND, Parser.Obj2String(SendMsg))));
            SendTextBox.Text = string.Empty;

        }

        private void SendFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string SendFilePath = dialog.FileName;
                //MessageBox.Show(SendFilePath);
                SendFile(SendFilePath);
            }
        }

        private void SendTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter && !e.Control)
            //{
            //    if (SendTextBox.Text.EndsWith('\n'))
            //    {
            //        SendTextBox.Text.Remove(SendTextBox.Text.Length - 1, 1);
            //    }
            //    SendButton_Click(sender, e);
            //    SendTextBox.Text = string.Empty;
            //}
        }
        private void SendTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == '\r' || e.KeyChar == '\n'))
            {
                if (SendTextBox.Text.EndsWith('\n'))
                {
                    SendTextBox.Text.Remove(SendTextBox.Text.Length - 1, 1);
                }
                SendButton_Click(sender, e);
                SendTextBox.Text = string.Empty;
            }
        }
        private void SendTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Control)
            {
                SendTextBox.Text = string.Empty;
            }
        }
        private void MessageTextBox_TextChanged(object sender, EventArgs e)
        {
            this.MessageTextBox.SelectionStart = this.MessageTextBox.Text.Length;
            this.MessageTextBox.ScrollToCaret();
        }

        private void CallForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (client.Connected)
                NetworkUtil.SendToStream(stream, Parser.Obj2String(new Message(MessageType.CALL_DENY, "")));
            stream.Close();
            client.Close();
        }
        #endregion

        private void SetVisible(bool vis, string fileName)
        {
            TransferLabel.Visible = vis;
            TransferProgressBar.Visible = vis;
            if (vis)
            {
                TransferLabel.Text = $"正在发送文件: {fileName}";
                TransferProgressBar.Value = 0;
            }
        }

        #region 与对方通信服务
        byte[] buffer = new byte[1024 * 1024 * 4];
        async private void ListenMsg()
        {
            int bytesRead;
            try
            {
                while (client.Connected)
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    //string msg = NetworkUtil.ReadFromStream(stream);
                    if (bytesRead > 0)
                    {
                        string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"Received: {msg}");
                        HandleMsg(Parser.String2Obj<Message>(msg));
                    }
                }
            }
            catch (IOException e)
            {
                MessageBox.Show(this, "对方关闭了链接", "通话中断");
            }
            catch (Exception e)
            {
                MessageBox.Show(this, e.Message, "CallForm catch");
            }
            Close();
        }

        private void HandleMsg(Message msg)
        {
            switch (msg.MsgType)
            {
                case MessageType.SEND:
                    RecvMessage(msg.MsgInfo);
                    break;
                case MessageType.RECEIVE:
                    SendMessageOK(msg.MsgInfo);
                    break;
                case MessageType.CALL_DENY:
                    CloseCall();
                    break;
                case MessageType.SENDFILE:
                    RecvFile(msg.MsgInfo);
                    break;
                default:
                    MessageBox.Show(msg.ToString(), "unkown type error", MessageBoxButtons.OK);
                    break;
            }
        }

        private void RecvMessage(string message)
        {
            SendMessage msg = Parser.String2Obj<SendMessage>(message);
            MessageTextBox.Text += msg.MsgInfo;
            NetworkUtil.SendToStream(stream, Parser.Obj2String(new Message(MessageType.RECEIVE, "")));
        }

        private void SendMessageOK(string message)
        {
            SendTextBox.Text = string.Empty;
        }

        private void CloseCall()
        {
            stream.Close();
            client.Close();
            MessageBox.Show(this, "对方关闭了链接", "通话中断");
        }

        #region 发送接收文件
        private void SendFile(string filePath)
        {
            NetworkUtil.SendToStream(stream, Parser.Obj2String(new Message(MessageType.SENDFILE, "")));
            // 发送文件名长度和文件名
            string fileName = filePath.Split('\\').Last();
            TransferingForm transForm = new TransferingForm("发送文件", $"正在发送文件: {fileName}");
            new Thread(() => transForm.ShowDialog()).Start();
            //set_Visible(true, fileName);
            //MessageBox.Show(fileName + "  " + filePath);
            byte[] fileNameByte = Encoding.Unicode.GetBytes(fileName);

            byte[] fileNameLengthForValueByte = Encoding.Unicode.GetBytes(fileNameByte.Length.ToString("D15"));
            byte[] fileAttributeByte = new byte[fileNameByte.Length + fileNameLengthForValueByte.Length];

            fileNameLengthForValueByte.CopyTo(fileAttributeByte, 0);  // 文件名字符流的长度的字符流排在前面。

            fileNameByte.CopyTo(fileAttributeByte, fileNameLengthForValueByte.Length);  //紧接着文件名的字符流

            stream.Write(fileAttributeByte, 0, fileAttributeByte.Length);

            // 发送文件长度和文件内容
            FileStream fileStrem = new FileStream(filePath, FileMode.Open);
            //string TotFileLength = $"{fileStrem.Length}";
            byte[] TotFileLengthByte = Encoding.Unicode.GetBytes(fileStrem.Length.ToString("D15"));
            stream.Write(TotFileLengthByte, 0, TotFileLengthByte.Length);       // 文件大小字符流

            int fileReadSize = 0;
            long fileLength = 0;
            while (fileLength < fileStrem.Length)
            {
                fileReadSize = fileStrem.Read(buffer, 0, buffer.Length);
                stream.Write(buffer, 0, fileReadSize);
                fileLength += fileReadSize;
                transForm.progressBar1.Value = (int)(100 * fileLength / fileStrem.Length);
            }
            fileStrem.Flush();
            fileStrem.Close();
            stream.Flush();
            transForm.progressBar1.Visible = false;
            transForm.label1.Text = "发送完成";
            //set_Visible(false, "");
        }

        private void RecvFile(string msg)
        {
            byte[] fileNameLengthForValueByte = Encoding.Unicode.GetBytes((256).ToString("D15"));
            byte[] fileNameLengByte = new byte[1024];
            int fileNameLengthSize = stream.Read(fileNameLengByte, 0, fileNameLengthForValueByte.Length);
            string fileNameLength = Encoding.Unicode.GetString(fileNameLengByte, 0, fileNameLengthSize);
            //TxtReceiveAddContent("文件名字符流的长度为：" + fileNameLength);

            int fileNameLengthNum = Convert.ToInt32(fileNameLength);
            byte[] fileNameByte = new byte[fileNameLengthNum];

            int fileNameSize = stream.Read(fileNameByte, 0, fileNameLengthNum);
            string fileName = Encoding.Unicode.GetString(fileNameByte, 0, fileNameSize);

            int fileLengthSize = stream.Read(buffer, 0, fileNameLengthForValueByte.Length);
            string tmp = Encoding.Unicode.GetString(buffer, 0, fileLengthSize);
            long fileLength = Convert.ToInt64(tmp);
            //TxtReceiveAddContent("文件名为：" + fileName);
            //set_Visible(true, fileName);
            TransferingForm transForm = new TransferingForm("接收文件", $"正在接收文件: {fileName}");
            new Thread(() => { transForm.ShowDialog(); }).Start();

            string dirPath = $"D:\\coding\\VsualStudioCode\\Files\\{src_id}";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            FileStream fileStream = new FileStream(dirPath + "\\" + fileName, FileMode.Create, FileAccess.Write);
            int fileReadSize = 0;
            long totReadSize = 0;
            while (totReadSize < fileLength && (fileReadSize = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                fileStream.Write(buffer, 0, fileReadSize);
                totReadSize += fileReadSize;
                transForm.progressBar1.Value = (int)(100 * totReadSize / fileLength);
            }
            fileStream.Flush();
            fileStream.Close();
            transForm.label1.Text = $"接收成功\n文件存放在: " + dirPath + "\n文件夹下";
            //set_Visible(false, "");
        }
        #endregion
        #endregion
    }
}
