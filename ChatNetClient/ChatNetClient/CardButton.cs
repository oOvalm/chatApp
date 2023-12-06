using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ChatNetClient
{
    /**
     * 好友明信片按钮
     */
    public class CardButton:Button
    {
        public CardInfo info { get; set; }
        private string FriendID;
        public string FriendName { get; }
        public Dialog dialog;
        MainForm mainForm;
        public CardButton(CardInfo Info, MainForm mainForm):base()
        {
            this.info = Info;
            this.FriendID = Info.dest_id;
            this.FriendName = info.dest_name;
            this.mainForm = mainForm;
            if(info.type == CardType.GROUP)
            {
                this.Text = $"群聊：{info.dest_name}";
            }
            else
            {
                this.Text = $"{info.dest_name}";
            }
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            //btn宽、高
            this.Width = mainForm.GetFriendListPane().Width - 25;
            this.Height = 40;
            //设置button间的margin为0
            Padding pd = new Padding();
            pd.All = 0;
            pd.Top = 5;
            this.Margin = pd;
            this.MouseClick += Btn_MouseClick;

            dialog = new Dialog(info.type, mainForm.current_user_id, FriendID, null);
        }
        private void Btn_MouseClick(object? sender, MouseEventArgs e)
        {
            mainForm.OpenDialog(dialog);
            this.BackColor = SystemColors.Control;
        }
        public void UpdateDialog(Dialog dialog)
        {
            this.dialog = dialog;
            Remind();
        }
        public void Remind()
        {
            // TODO: 做收到消息提示
            //MessageBox.Show($"recv message from {dialog.dest_id}", $"你好 {mainForm.current_user_id}");
            this.BackColor = Color.Orange;
        }
    }
}
