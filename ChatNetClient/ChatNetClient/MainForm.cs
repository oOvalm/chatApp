using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Sockets;

namespace ChatNetClient
{
    public partial class MainForm : Form
    {
        private TcpClient client;
        private LoginForm loginForm;
        private ClientService server;
        public string current_user_id;
        public string current_user_name;
        private Dialog openingDialog = null;
        public CallingDialog callingDialog { get; set; }
        public CallForm callForm { get; set; }

        public Dictionary<string, CardButton> FriendList = new Dictionary<string, CardButton>(); // ID查对应的btn Key的结构为类型(USER/GROUP)+id
        public MainForm(TcpClient _client, LoginForm _loginForm, string _user_id)
        {
            callingDialog = null;
            this.current_user_id = _user_id;
            loginForm = _loginForm;
            client = _client;
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            Thread thread = new Thread(() => { server = new ClientService(client, this, current_user_id); });
            thread.Start();
        }


        #region 控件事件触发的函数
        private void SendMsgButton_Click(object sender, EventArgs e)
        {
            if (openingDialog == null)
            {
                MessageBox.Show("你还没有打开对话框");
                return;
            }
            string msg = SendTextBox.Text;
            if (msg.Length == 0 || (msg.Length == 1 && (msg[0] == '\r' || msg[0] == '\n'))
                || (msg.Length == 2 && (msg[0] == '\r' || msg[0] == '\n') && msg[1] < 32))
            {
                SendTextBox.Text = string.Empty;
                MessageBox.Show("不能发送空信息！", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            server.SendMsg(msg, openingDialog);
        }
        private void SendTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Control)
            {
                if (SendTextBox.Text.EndsWith('\n'))
                {
                    SendTextBox.Text.Remove(SendTextBox.Text.Length - 1, 1);
                }
                SendMsgButton_Click(sender, e);
            }
        }
        private void CallButton_Click(object sender, EventArgs e)
        {
            Debug.Assert(openingDialog.type == CardType.USER);
            if (callForm != null)
            {
                MessageBox.Show("你已经在通话了！", "error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            callingDialog = new CallingDialog(openingDialog.dest_id, FriendList[openingDialog.type + openingDialog.dest_id].FriendName, this);
            callingDialog.Owner = this;
            server.RequestCall(openingDialog.dest_id);
            callingDialog.ShowDialog();
        }

        private void MessageTextBox_TextChanged(object sender, EventArgs e)
        {
            this.MessageTextBox.SelectionStart = this.MessageTextBox.Text.Length;
            this.MessageTextBox.ScrollToCaret();
        }

        private void SendTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Control)
            {
                SendTextBox.Text = string.Empty;
            }
        }
        #endregion


        public void SendSuccessful(string msg)
        {
            MessageTextBox.Text += msg;
            SendTextBox.Text = "";
        }

        #region 接收到信息
        public void ReceiveMsg(SendMessage msg)
        {
            Debug.Assert(!msg.SrcId.Equals(current_user_id));   // 不可能给自己发消息
            string listKey, msgSrcId;
            if (msg.MsgType == CardType.USER)
            {
                listKey = msg.MsgType + msg.SrcId;
                msgSrcId = msg.SrcId;
            }
            else
            {
                listKey = msg.MsgType + msg.DestId;
                msgSrcId = msg.DestId;
            }

            if (openingDialog != null && msg.MsgType == openingDialog.type && msgSrcId.Equals(openingDialog.dest_id))
            {
                openingDialog.TalkInfo += msg.MsgInfo;
                MessageTextBox.Text += msg.MsgInfo;
                return;
            }
            else if (FriendList[listKey].dialog.TalkInfo == null)
            {
                server.RequestDialogInfo(FriendList[listKey].dialog);
            }
            else
            {
                FriendList[listKey].dialog.TalkInfo += msg.MsgInfo;
                FriendList[listKey].Remind();
            }
        }
        #endregion

        #region 更新用户列表
        async public void UpdateFriendList(string message)
        {
            List<CardInfo> list = JsonConvert.DeserializeObject<List<CardInfo>>(message);
            FriendList.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                CardInfo info = list[i];
                string card_id = info.dest_id, card_name = info.dest_name;
                if (card_id.Equals(current_user_id))   // 好友列表没有自己
                {
                    current_user_name = card_name;
                    this.Text = card_name + "(" + card_id + ")";
                    continue;
                }
                CardButton btn = new CardButton(info, this);
                FriendList.Add(info.type + card_id, btn);
                Invoke(new Action(() =>
                {
                    FriendListPane.Controls.Add(btn);
                }));
            }
        }
        #endregion

        #region 打开对话框
        public void OpenDialog(Dialog dialog)
        {
            if (dialog == openingDialog) return;
            openingDialog = dialog;
            FriendNameTextBox.Text = $"{FriendList[dialog.type + dialog.dest_id].FriendName}({dialog.dest_id})";
            if (dialog.TalkInfo == null)
            {
                server.RequestDialogInfo(dialog);
            }
            else
            {
                MessageTextBox.Text = dialog.TalkInfo;
            }

            if (dialog.type == CardType.USER)
            {
                CallButton.Visible = true;
            }
            else
            {
                CallButton.Visible = false;
            }
        }
        #endregion

        #region 获得对话框信息
        async public void UpdateDialogInfo(string msg)
        {
            Dialog newDialog = Parser.String2Obj<Dialog>(msg);
            if (newDialog.src_id.Equals(current_user_id))
            {
                if (openingDialog != null && openingDialog.type == newDialog.type && newDialog.dest_id.Equals(openingDialog.dest_id))
                {
                    openingDialog.TalkInfo = Parser.String2Obj<TalkMessages>(newDialog.TalkInfo).Format();
                    //MessageBox.Show($"OKO111K ->\n{openingDialog.TalkInfo}");
                    MessageTextBox.Text = openingDialog.TalkInfo;
                }
                else
                {
                    //FriendList[newDialog.dest_id].dialog = newDialog;
                    newDialog.TalkInfo = Parser.String2Obj<TalkMessages>(newDialog.TalkInfo).Format();
                    FriendList[newDialog.type + newDialog.dest_id].UpdateDialog(newDialog);
                }
            }
            else MessageBox.Show("recv useless Dialog infomation", "error", MessageBoxButtons.OK);
        }
        #endregion

        #region 窗口调整
        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            FriendNameTextBox.Width = this.Width - FriendListPane.Width - 48;           // 好友名字的控件宽度
            DialogSplitContainer.Width = FriendNameTextBox.Width;        // 聊天窗口的宽度
            DialogSplitContainer.Height = this.Height - 141;                            // 聊天窗口高度
            FriendListPane.Height = this.Height - 101;
            SendButton.Location = new Point(this.Width - 150, this.Height - 90);
        }

        private void Update_TextBoxSize()
        {
            MessageTextBox.Height = this.DialogSplitContainer.Panel1.Height;
            SendTextBox.Height = this.DialogSplitContainer.Panel2.Height - 3;
            MessageTextBox.Width = this.DialogSplitContainer.Panel1.Width;
            SendTextBox.Width = this.DialogSplitContainer.Panel2.Width;

        }

        private void DialogSplitContainer_SizeChanged(object sender, EventArgs e)
        {
            Update_TextBoxSize();
        }

        private void DialogSplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            Update_TextBoxSize();
        }
        #endregion


        #region 呼叫通话相关
        public void EndCall()
        {
            callingDialog = null;
        }

        private string _dest_id, _dest_ip;
        private int _dest_port, _ty;
        public void ShowCallForm(int ty, string dest_id, string dest_ip, int dest_port)
        {
            this._ty = ty;
            this._dest_id = dest_id;
            this._dest_ip = dest_ip;
            this._dest_port = dest_port;
            Thread thr = new Thread(() =>
            {
                MethodInvoker MethInvo = new MethodInvoker(InnerShowCallForm);
                BeginInvoke(MethInvo);
            });
            thr.Start();
        }
        public void InnerShowCallForm()
        {
            callForm = new CallForm(_ty, current_user_id, _dest_id, _dest_ip, _dest_port, server);
            callForm.Show();
        }

        #endregion

    }
}
