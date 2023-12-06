namespace ChatNetClient
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(client.Connected)
                NetworkUtil.SendToStream(client.GetStream(), Parser.Obj2String(new Message(MessageType.LOGOUT, "")));
            server.Logout();
            loginForm.Close();
            client.GetStream().Close();
            client.Close();         // 关闭链接
            client.Dispose();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            MessageTextBox = new TextBox();
            SendTextBox = new TextBox();
            SendButton = new Button();
            FriendNameTextBox = new TextBox();
            FriendListPane = new FlowLayoutPanel();
            label1 = new Label();
            DialogSplitContainer = new SplitContainer();
            CallButton = new Button();
            ((System.ComponentModel.ISupportInitialize)DialogSplitContainer).BeginInit();
            DialogSplitContainer.Panel1.SuspendLayout();
            DialogSplitContainer.Panel2.SuspendLayout();
            DialogSplitContainer.SuspendLayout();
            SuspendLayout();
            // 
            // MessageTextBox
            // 
            MessageTextBox.Location = new Point(0, 0);
            MessageTextBox.Margin = new Padding(0);
            MessageTextBox.MaxLength = 65536;
            MessageTextBox.Multiline = true;
            MessageTextBox.Name = "MessageTextBox";
            MessageTextBox.ReadOnly = true;
            MessageTextBox.ScrollBars = ScrollBars.Vertical;
            MessageTextBox.Size = new Size(563, 241);
            MessageTextBox.TabIndex = 1;
            MessageTextBox.TextChanged += MessageTextBox_TextChanged;
            // 
            // SendTextBox
            // 
            SendTextBox.Location = new Point(0, 3);
            SendTextBox.Margin = new Padding(0);
            SendTextBox.Multiline = true;
            SendTextBox.Name = "SendTextBox";
            SendTextBox.Size = new Size(563, 109);
            SendTextBox.TabIndex = 2;
            SendTextBox.KeyDown += SendTextBox_KeyDown;
            SendTextBox.KeyUp += SendTextBox_KeyUp;
            // 
            // SendButton
            // 
            SendButton.Location = new Point(668, 407);
            SendButton.Name = "SendButton";
            SendButton.Size = new Size(120, 31);
            SendButton.TabIndex = 3;
            SendButton.Text = "发送";
            SendButton.UseVisualStyleBackColor = true;
            SendButton.Click += SendMsgButton_Click;
            // 
            // FriendNameTextBox
            // 
            FriendNameTextBox.BackColor = SystemColors.Menu;
            FriendNameTextBox.Location = new Point(225, 12);
            FriendNameTextBox.Name = "FriendNameTextBox";
            FriendNameTextBox.Size = new Size(563, 27);
            FriendNameTextBox.TabIndex = 4;
            FriendNameTextBox.TextAlign = HorizontalAlignment.Center;
            // 
            // FriendListPane
            // 
            FriendListPane.AutoScroll = true;
            FriendListPane.BorderStyle = BorderStyle.FixedSingle;
            FriendListPane.Location = new Point(12, 42);
            FriendListPane.Name = "FriendListPane";
            FriendListPane.Size = new Size(207, 396);
            FriendListPane.TabIndex = 5;
            // 
            // label1
            // 
            label1.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(12, 12);
            label1.Name = "label1";
            label1.Size = new Size(207, 27);
            label1.TabIndex = 6;
            label1.Text = "消息列表";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // DialogSplitContainer
            // 
            DialogSplitContainer.Location = new Point(225, 45);
            DialogSplitContainer.Name = "DialogSplitContainer";
            DialogSplitContainer.Orientation = Orientation.Horizontal;
            // 
            // DialogSplitContainer.Panel1
            // 
            DialogSplitContainer.Panel1.Controls.Add(MessageTextBox);
            // 
            // DialogSplitContainer.Panel2
            // 
            DialogSplitContainer.Panel2.Controls.Add(SendTextBox);
            DialogSplitContainer.Size = new Size(563, 356);
            DialogSplitContainer.SplitterDistance = 241;
            DialogSplitContainer.SplitterWidth = 3;
            DialogSplitContainer.TabIndex = 7;
            DialogSplitContainer.SplitterMoved += DialogSplitContainer_SplitterMoved;
            DialogSplitContainer.SizeChanged += DialogSplitContainer_SizeChanged;
            // 
            // CallButton
            // 
            CallButton.Location = new Point(542, 407);
            CallButton.Name = "CallButton";
            CallButton.Size = new Size(120, 31);
            CallButton.TabIndex = 8;
            CallButton.Text = "呼叫";
            CallButton.UseVisualStyleBackColor = true;
            CallButton.Visible = false;
            CallButton.Click += CallButton_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(CallButton);
            Controls.Add(DialogSplitContainer);
            Controls.Add(label1);
            Controls.Add(FriendListPane);
            Controls.Add(FriendNameTextBox);
            Controls.Add(SendButton);
            MinimumSize = new Size(450, 360);
            Name = "MainForm";
            Text = "MainForm";
            ResizeEnd += MainForm_ResizeEnd;
            SizeChanged += MainForm_ResizeEnd;
            DialogSplitContainer.Panel1.ResumeLayout(false);
            DialogSplitContainer.Panel1.PerformLayout();
            DialogSplitContainer.Panel2.ResumeLayout(false);
            DialogSplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DialogSplitContainer).EndInit();
            DialogSplitContainer.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox MessageTextBox;
        private TextBox SendTextBox;
        private Button SendButton;
        private TextBox FriendNameTextBox;
        private FlowLayoutPanel FriendListPane;

        public FlowLayoutPanel GetFriendListPane() { return FriendListPane; }

        private Label label1;
        private SplitContainer DialogSplitContainer;
        private Button CallButton;
    }
}