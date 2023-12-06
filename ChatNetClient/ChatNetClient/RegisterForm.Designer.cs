namespace ChatNetClient
{
    partial class RegisterForm
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
            label2 = new Label();
            label3 = new Label();
            PswConfirmTextBox = new TextBox();
            PswTextBox = new TextBox();
            UserNameTextBox = new TextBox();
            RegisterButton = new Button();
            CancenButton = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(78, 88);
            label1.Name = "label1";
            label1.Size = new Size(107, 39);
            label1.TabIndex = 2;
            label1.Text = "用户名";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(108, 140);
            label2.Name = "label2";
            label2.Size = new Size(77, 39);
            label2.TabIndex = 3;
            label2.Text = "密码";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(48, 192);
            label3.Name = "label3";
            label3.Size = new Size(137, 39);
            label3.TabIndex = 4;
            label3.Text = "确认密码";
            // 
            // PswConfirmTextBox
            // 
            PswConfirmTextBox.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            PswConfirmTextBox.Location = new Point(215, 189);
            PswConfirmTextBox.Name = "PswConfirmTextBox";
            PswConfirmTextBox.PasswordChar = '*';
            PswConfirmTextBox.Size = new Size(300, 46);
            PswConfirmTextBox.TabIndex = 7;
            // 
            // PswTextBox
            // 
            PswTextBox.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            PswTextBox.Location = new Point(215, 137);
            PswTextBox.Name = "PswTextBox";
            PswTextBox.PasswordChar = '*';
            PswTextBox.Size = new Size(300, 46);
            PswTextBox.TabIndex = 6;
            // 
            // UserNameTextBox
            // 
            UserNameTextBox.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            UserNameTextBox.Location = new Point(215, 85);
            UserNameTextBox.Name = "UserNameTextBox";
            UserNameTextBox.Size = new Size(300, 46);
            UserNameTextBox.TabIndex = 5;
            // 
            // RegisterButton
            // 
            RegisterButton.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            RegisterButton.Location = new Point(108, 261);
            RegisterButton.Name = "RegisterButton";
            RegisterButton.Size = new Size(154, 50);
            RegisterButton.TabIndex = 8;
            RegisterButton.Text = "注册";
            RegisterButton.UseVisualStyleBackColor = true;
            RegisterButton.Click += RegisterButton_Click;
            // 
            // CancenButton
            // 
            CancenButton.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            CancenButton.Location = new Point(334, 261);
            CancenButton.Name = "CancenButton";
            CancenButton.Size = new Size(168, 50);
            CancenButton.TabIndex = 9;
            CancenButton.Text = "取消";
            CancenButton.UseVisualStyleBackColor = true;
            CancenButton.Click += CancenButton_Click;
            // 
            // RegisterForm
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(592, 380);
            Controls.Add(CancenButton);
            Controls.Add(RegisterButton);
            Controls.Add(UserNameTextBox);
            Controls.Add(PswTextBox);
            Controls.Add(PswConfirmTextBox);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            MaximizeBox = false;
            MaximumSize = new Size(610, 427);
            MinimumSize = new Size(610, 427);
            Name = "RegisterForm";
            Text = "注册";
            FormClosed += RegisterForm_FormClosed;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox UserNameTextBox;
        private TextBox PswTextBox;
        private TextBox PswConfirmTextBox;
        private Button RegisterButton;
        private Button CancenButton;
    }
}