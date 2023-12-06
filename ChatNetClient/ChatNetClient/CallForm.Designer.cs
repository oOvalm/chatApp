namespace ChatNetClient
{
    partial class CallForm
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
            client.Close();
            service.CallEnd();
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
            label1 = new Label();
            DialogSplitContainer = new SplitContainer();
            MessageTextBox = new TextBox();
            SendTextBox = new TextBox();
            SendButton = new Button();
            SendFileButton = new Button();
            TransferProgressBar = new ProgressBar();
            TransferLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)DialogSplitContainer).BeginInit();
            DialogSplitContainer.Panel1.SuspendLayout();
            DialogSplitContainer.Panel2.SuspendLayout();
            DialogSplitContainer.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(776, 20);
            label1.TabIndex = 0;
            label1.Text = "label1";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // DialogSplitContainer
            // 
            DialogSplitContainer.Location = new Point(12, 32);
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
            DialogSplitContainer.Size = new Size(776, 369);
            DialogSplitContainer.SplitterDistance = 221;
            DialogSplitContainer.TabIndex = 1;
            // 
            // MessageTextBox
            // 
            MessageTextBox.Location = new Point(0, 0);
            MessageTextBox.Multiline = true;
            MessageTextBox.Name = "MessageTextBox";
            MessageTextBox.ReadOnly = true;
            MessageTextBox.ScrollBars = ScrollBars.Vertical;
            MessageTextBox.Size = new Size(776, 222);
            MessageTextBox.TabIndex = 0;
            MessageTextBox.TextChanged += MessageTextBox_TextChanged;
            // 
            // SendTextBox
            // 
            SendTextBox.Location = new Point(0, 3);
            SendTextBox.Multiline = true;
            SendTextBox.Name = "SendTextBox";
            SendTextBox.Size = new Size(776, 141);
            SendTextBox.TabIndex = 1;
            SendTextBox.KeyDown += SendTextBox_KeyDown;
            SendTextBox.KeyPress += SendTextBox_KeyPress;
            SendTextBox.KeyUp += SendTextBox_KeyUp;
            // 
            // SendButton
            // 
            SendButton.Location = new Point(668, 407);
            SendButton.Name = "SendButton";
            SendButton.Size = new Size(120, 31);
            SendButton.TabIndex = 2;
            SendButton.Text = "发送";
            SendButton.UseVisualStyleBackColor = true;
            SendButton.Click += SendButton_Click;
            // 
            // SendFileButton
            // 
            SendFileButton.Location = new Point(542, 407);
            SendFileButton.Name = "SendFileButton";
            SendFileButton.Size = new Size(120, 31);
            SendFileButton.TabIndex = 3;
            SendFileButton.Text = "发送文件";
            SendFileButton.UseVisualStyleBackColor = true;
            SendFileButton.Click += SendFileButton_Click;
            // 
            // TransferProgressBar
            // 
            TransferProgressBar.Location = new Point(140, 407);
            TransferProgressBar.Name = "TransferProgressBar";
            TransferProgressBar.Size = new Size(156, 29);
            TransferProgressBar.TabIndex = 4;
            TransferProgressBar.Visible = false;
            // 
            // TransferLabel
            // 
            TransferLabel.Location = new Point(12, 410);
            TransferLabel.Name = "TransferLabel";
            TransferLabel.Size = new Size(122, 25);
            TransferLabel.TabIndex = 5;
            TransferLabel.Text = "label2";
            TransferLabel.TextAlign = ContentAlignment.MiddleCenter;
            TransferLabel.Visible = false;
            // 
            // CallForm
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(TransferLabel);
            Controls.Add(TransferProgressBar);
            Controls.Add(SendFileButton);
            Controls.Add(SendButton);
            Controls.Add(DialogSplitContainer);
            Controls.Add(label1);
            MaximumSize = new Size(818, 497);
            MinimumSize = new Size(818, 497);
            Name = "CallForm";
            Text = "CallForm";
            FormClosed += CallForm_FormClosed;
            DialogSplitContainer.Panel1.ResumeLayout(false);
            DialogSplitContainer.Panel1.PerformLayout();
            DialogSplitContainer.Panel2.ResumeLayout(false);
            DialogSplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DialogSplitContainer).EndInit();
            DialogSplitContainer.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private SplitContainer DialogSplitContainer;
        private Button SendButton;
        private TextBox MessageTextBox;
        private TextBox SendTextBox;
        private Button SendFileButton;
        private ProgressBar TransferProgressBar;
        private Label TransferLabel;
    }
}