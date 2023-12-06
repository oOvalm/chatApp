namespace ChatNetClient
{
    partial class LoginForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            LoginButton = new Button();
            IdLable = new Label();
            PswLable = new Label();
            IdText = new TextBox();
            PswText = new TextBox();
            RegisterButton = new Button();
            IPTextBox = new TextBox();
            label1 = new Label();
            SuspendLayout();
            // 
            // LoginButton
            // 
            LoginButton.Font = new Font("Microsoft YaHei UI", 22F, FontStyle.Regular, GraphicsUnit.Point);
            LoginButton.Location = new Point(173, 299);
            LoginButton.Name = "LoginButton";
            LoginButton.Size = new Size(220, 70);
            LoginButton.TabIndex = 0;
            LoginButton.Text = "登录";
            LoginButton.UseVisualStyleBackColor = true;
            LoginButton.Click += LoginButton_Click;
            // 
            // IdLable
            // 
            IdLable.AutoSize = true;
            IdLable.Font = new Font("Microsoft YaHei UI", 20F, FontStyle.Regular, GraphicsUnit.Point);
            IdLable.Location = new Point(173, 112);
            IdLable.Name = "IdLable";
            IdLable.Size = new Size(88, 45);
            IdLable.TabIndex = 1;
            IdLable.Text = "账号";
            // 
            // PswLable
            // 
            PswLable.AutoSize = true;
            PswLable.Font = new Font("Microsoft YaHei UI", 20F, FontStyle.Regular, GraphicsUnit.Point);
            PswLable.Location = new Point(173, 206);
            PswLable.Name = "PswLable";
            PswLable.Size = new Size(88, 45);
            PswLable.TabIndex = 2;
            PswLable.Text = "密码";
            // 
            // IdText
            // 
            IdText.Font = new Font("Microsoft YaHei UI", 20F, FontStyle.Regular, GraphicsUnit.Point);
            IdText.Location = new Point(325, 109);
            IdText.MaxLength = 20;
            IdText.Name = "IdText";
            IdText.Size = new Size(324, 50);
            IdText.TabIndex = 3;
            IdText.Text = "10000";
            // 
            // PswText
            // 
            PswText.Font = new Font("Microsoft YaHei UI", 20F, FontStyle.Regular, GraphicsUnit.Point);
            PswText.Location = new Point(325, 206);
            PswText.Name = "PswText";
            PswText.PasswordChar = '*';
            PswText.Size = new Size(324, 50);
            PswText.TabIndex = 4;
            PswText.Text = "123456";
            PswText.KeyDown += LoginForm_KeyDown;
            // 
            // RegisterButton
            // 
            RegisterButton.Font = new Font("Microsoft YaHei UI", 22F, FontStyle.Regular, GraphicsUnit.Point);
            RegisterButton.Location = new Point(429, 299);
            RegisterButton.Name = "RegisterButton";
            RegisterButton.Size = new Size(220, 70);
            RegisterButton.TabIndex = 5;
            RegisterButton.Text = "注册";
            RegisterButton.UseVisualStyleBackColor = true;
            RegisterButton.Click += RegisterButton_Click;
            // 
            // IPTextBox
            // 
            IPTextBox.Font = new Font("Microsoft YaHei UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            IPTextBox.Location = new Point(126, 6);
            IPTextBox.MaxLength = 20;
            IPTextBox.Name = "IPTextBox";
            IPTextBox.Size = new Size(135, 37);
            IPTextBox.TabIndex = 7;
            IPTextBox.Text = "127.0.0.1";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft YaHei UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(108, 31);
            label1.TabIndex = 6;
            label1.Text = "服务器IP";
            label1.TextAlign = ContentAlignment.TopCenter;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(IPTextBox);
            Controls.Add(label1);
            Controls.Add(RegisterButton);
            Controls.Add(PswText);
            Controls.Add(IdText);
            Controls.Add(PswLable);
            Controls.Add(IdLable);
            Controls.Add(LoginButton);
            MaximizeBox = false;
            MaximumSize = new Size(818, 497);
            MinimumSize = new Size(818, 497);
            Name = "LoginForm";
            Text = "ChatApp";
            KeyDown += LoginForm_KeyDown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button LoginButton;
        private Label IdLable;
        private Label PswLable;
        private TextBox IdText;
        private TextBox PswText;
        private Button RegisterButton;
        private TextBox IPTextBox;
        private Label label1;
    }
}