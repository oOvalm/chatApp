namespace ChatNetClient
{
    partial class CallingDialog
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
            CancelButton = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Location = new Point(44, 48);
            label1.Name = "label1";
            label1.Size = new Size(165, 42);
            label1.TabIndex = 0;
            label1.Text = "label1";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // CancelButton
            // 
            CancelButton.Location = new Point(136, 125);
            CancelButton.Name = "CancelButton";
            CancelButton.Size = new Size(101, 41);
            CancelButton.TabIndex = 1;
            CancelButton.Text = "取消";
            CancelButton.UseVisualStyleBackColor = true;
            CancelButton.Click += button1_Click;
            // 
            // CallingDialog
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(249, 178);
            ControlBox = false;
            Controls.Add(CancelButton);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MaximumSize = new Size(267, 225);
            MinimizeBox = false;
            MinimumSize = new Size(267, 225);
            Name = "CallingDialog";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "呼叫";
            FormClosed += CloseForm;
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private Button CancelButton;
    }
}