namespace StormMeeting
{
    partial class MainScreen
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
            this.SendButton = new System.Windows.Forms.Button();
            this.SendMessageBox = new System.Windows.Forms.TextBox();
            this.ReceivedMessageBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SendButton
            // 
            this.SendButton.Location = new System.Drawing.Point(425, 129);
            this.SendButton.Name = "SendButton";
            this.SendButton.Size = new System.Drawing.Size(92, 98);
            this.SendButton.TabIndex = 0;
            this.SendButton.Text = "Send";
            this.SendButton.UseVisualStyleBackColor = true;
            this.SendButton.Click += new System.EventHandler(this.SendButton_Click);
            this.SendButton.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SendButton_KeyPress);
            // 
            // SendMessageBox
            // 
            this.SendMessageBox.Location = new System.Drawing.Point(12, 129);
            this.SendMessageBox.Multiline = true;
            this.SendMessageBox.Name = "SendMessageBox";
            this.SendMessageBox.Size = new System.Drawing.Size(399, 98);
            this.SendMessageBox.TabIndex = 1;
            // 
            // ReceivedMessageBox
            // 
            this.ReceivedMessageBox.Location = new System.Drawing.Point(12, 12);
            this.ReceivedMessageBox.Multiline = true;
            this.ReceivedMessageBox.Name = "ReceivedMessageBox";
            this.ReceivedMessageBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ReceivedMessageBox.Size = new System.Drawing.Size(505, 111);
            this.ReceivedMessageBox.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(71, 277);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(382, 107);
            this.button1.TabIndex = 3;
            this.button1.Text = "Start Share Screen";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(529, 444);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ReceivedMessageBox);
            this.Controls.Add(this.SendMessageBox);
            this.Controls.Add(this.SendButton);
            this.Name = "MainScreen";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SendButton;
        private System.Windows.Forms.TextBox SendMessageBox;
        volatile public System.Windows.Forms.TextBox ReceivedMessageBox;
        private System.Windows.Forms.Button button1;
    }
}

