using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Text.Json;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ChatNetClient
{
    public class ClientService
    {
        private string current_user_id;
        private TcpClient client;
        private Stream stream;
        private Thread talkThread;
        public MainForm mainForm;
        //public CallForm callForm { get; set; }
        bool logout = false;

        #region 系统API
        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        public const int WM_CLOSE = 0x10;
        #endregion

        #region 委托声明
        public delegate void Delegate_UpdateFriendList(string msg);
        public delegate void Delegate_UpdateDialogInfo(string msg);
        public delegate void Delegate_SendSuccessful(string msg);
        public delegate void Delegate_ReceiveMsg(SendMessage msg);
        private Delegate_UpdateFriendList upd_friendlist;
        private Delegate_UpdateDialogInfo upd_dialoginfo;
        private Delegate_SendSuccessful send_successful;
        private Delegate_ReceiveMsg upd_recvMsg;
        #endregion

        public ClientService(TcpClient client, MainForm mainForm, string _user_id)
        {
            this.client = client;
            stream = client.GetStream();
            this.mainForm = mainForm;
            talkThread = new Thread(ListenMsg);
            talkThread.IsBackground = true;
            talkThread.Start();
            this.current_user_id = _user_id;
            upd_friendlist = new Delegate_UpdateFriendList(mainForm.UpdateFriendList);
            upd_dialoginfo = new Delegate_UpdateDialogInfo(mainForm.UpdateDialogInfo);
            upd_recvMsg = new Delegate_ReceiveMsg(mainForm.ReceiveMsg);
            send_successful = new Delegate_SendSuccessful(mainForm.SendSuccessful);
        }

        private void ListenMsg()
        {
            //StreamReader reader = new StreamReader(stream);
            byte[] buffer = new byte[1024 * 1024];
            int bytesRead;
            try
            {
                while (!logout)
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
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                MessageBox.Show(e.Message, "catch");
            }
            //MessageBox.Show("logout!!");
        }

        private void HandleMsg(Message msg)
        {
            switch(msg.MsgType)
            {
                case MessageType.ALL_CARD:          // 获取名片列表
                    upd_friendlist(msg.MsgInfo);
                    break;
                case MessageType.RECEIVE:
                    upd_recvMsg(Parser.String2Obj<SendMessage>(msg.MsgInfo));
                    break;
                case MessageType.SEND_OK:
                    send_successful(msg.MsgInfo);   // mainForm委托
                    break;
                case MessageType.DIALOG_INFO:
                    upd_dialoginfo(msg.MsgInfo);    // mainForm委托
                    break;
                case MessageType.CALL_REQUEST:
                    BeRequestCall(msg.MsgInfo);
                    break;
                case MessageType.CALL_ACCEPT:
                    CallAccept(msg.MsgInfo);
                    break;
                case MessageType.CALL_DENY:
                    CallDeny(msg.MsgInfo);
                    break;
                case MessageType.ERROR:
                    ResolveError(msg.MsgInfo);
                    break;
                default:
                    MessageBox.Show(msg.ToString(), "unkown type error", MessageBoxButtons.OK);
                    break;
            }
        }

        // 处理错误信息
        public void ResolveError(string message)
        {
            string[] strs = message.Split(' ');
            if (strs[0].Equals("offline"))
            {
                string msg = "";
                for (int i = 1; i < strs.Length; i++) msg += strs[i];
                IntPtr ptr = FindWindow(null, "呼叫");
                if (ptr != IntPtr.Zero)
                {
                    PostMessage(ptr, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                }
                MessageBox.Show(mainForm, message, "error", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show(mainForm, message, "error", MessageBoxButtons.OK);
            }
        }

        // 向别人发送信息
        public void SendMsg(string message, Dialog dialog)
        {
            SendMessage send_msg = new SendMessage(dialog.type, current_user_id, dialog.dest_id, message);
            NetworkUtil.SendToStream(stream, Parser.Obj2String(new Message(MessageType.SEND, Parser.Obj2String(send_msg))));
        }

        // 请求聊天记录
        public void RequestDialogInfo(Dialog dialog)
        {
            Debug.Assert(dialog.TalkInfo == null);
            NetworkUtil.SendToStream(stream, Parser.Obj2String(new Message(MessageType.REQUEST_DIALOG, Parser.Obj2String(dialog))));
        }

        public void Logout() { logout = true; }


        #region 通话相关
        // 请求通话
        public void RequestCall(string dest_id)
        {
            NetworkUtil.SendToStream(stream, Parser.Obj2String(new Message(MessageType.CALL_REQUEST, dest_id)));
        }

        // 接受通话请求
        public void BeRequestCall(string msg)
        {
            string[] str = msg.Split(' ');
            string dest_id = str[0];
            string dest_ip = str[1];
            if(mainForm.callForm != null)
            {
                NetworkUtil.SendToStream(stream, Parser.Obj2String(new Message(MessageType.CALL_DENY, $"{dest_id} {current_user_id} 正在通话！")));
                return;
            }
            DialogResult res = MessageBox.Show($"({dest_id} 向你发出通话请求", "通话请求", MessageBoxButtons.YesNo);
            if(res != DialogResult.Yes)
            {
                NetworkUtil.SendToStream(stream, Parser.Obj2String(new Message(MessageType.CALL_DENY, $"{dest_id} {current_user_id} 拒绝你的通话请求！")));
                return;
            }
            int port = NetworkUtil.GetAvailablePort();
            NetworkUtil.SendToStream(stream, Parser.Obj2String(new Message(MessageType.CALL_ACCEPT, $"{dest_id} {port}")));
            mainForm.ShowCallForm(1, dest_id, dest_ip, port);
            //Thread tmp = new Thread(() => callForm = new CallForm(1, current_user_id, dest_id, dest_ip, port, this));
        }

        // 接收通话
        public void CallAccept(string msg)
        {
            string[] str = msg.Split(' ');
            string dest_id = str[0];
            string dest_ip = str[1];
            int dest_port = int.Parse(str[2]);
            mainForm.callingDialog.Close();
            mainForm.callingDialog = null;
            mainForm.ShowCallForm(2, dest_id, dest_ip, dest_port);
            //Thread tmp = new Thread(()=> {
            //    BeginInvoke(new MethodInvoker(ShowCallForm, dest_id, dest_ip, dest_port));
            //    //callForm = new CallForm(2, current_user_id, dest_id, dest_ip, dest_port, this);
            //});
        }

        // 拒绝通话
        public void CallDeny(string msg)
        {
            if (mainForm.callingDialog == null) return;
            mainForm.callingDialog.Deny(msg);
        }

        public void CallEnd()
        {
            mainForm.callForm = null;
        }
        #endregion
    }
}
