namespace ComparadorWebRequests
{
    partial class ComparadorInteligente
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
            txtPortal = new TextBox();
            label1 = new Label();
            label2 = new Label();
            txtRobo = new TextBox();
            groupBox1 = new GroupBox();
            rbdResponse = new RadioButton();
            rbdRequest = new RadioButton();
            btnComparar = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // txtPortal
            // 
            txtPortal.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            txtPortal.Location = new Point(17, 49);
            txtPortal.Multiline = true;
            txtPortal.Name = "txtPortal";
            txtPortal.Size = new Size(498, 497);
            txtPortal.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(17, 26);
            label1.Name = "label1";
            label1.Size = new Size(50, 20);
            label1.TabIndex = 1;
            label1.Text = "Portal:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(549, 26);
            label2.Name = "label2";
            label2.Size = new Size(48, 20);
            label2.TabIndex = 3;
            label2.Text = "Robo:";
            // 
            // txtRobo
            // 
            txtRobo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtRobo.Location = new Point(549, 49);
            txtRobo.Multiline = true;
            txtRobo.Name = "txtRobo";
            txtRobo.Size = new Size(498, 497);
            txtRobo.TabIndex = 2;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(rbdResponse);
            groupBox1.Controls.Add(rbdRequest);
            groupBox1.Location = new Point(25, 552);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(192, 89);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "Tipo do Conteúdo";
            // 
            // rbdResponse
            // 
            rbdResponse.AutoSize = true;
            rbdResponse.Location = new Point(12, 56);
            rbdResponse.Name = "rbdResponse";
            rbdResponse.Size = new Size(93, 24);
            rbdResponse.TabIndex = 1;
            rbdResponse.TabStop = true;
            rbdResponse.Text = "Response";
            rbdResponse.UseVisualStyleBackColor = true;
            // 
            // rbdRequest
            // 
            rbdRequest.AutoSize = true;
            rbdRequest.Checked = true;
            rbdRequest.Location = new Point(12, 26);
            rbdRequest.Name = "rbdRequest";
            rbdRequest.Size = new Size(89, 24);
            rbdRequest.TabIndex = 0;
            rbdRequest.TabStop = true;
            rbdRequest.Text = "Requests";
            rbdRequest.UseVisualStyleBackColor = true;
            // 
            // btnComparar
            // 
            btnComparar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnComparar.Location = new Point(753, 576);
            btnComparar.Name = "btnComparar";
            btnComparar.Size = new Size(294, 56);
            btnComparar.TabIndex = 5;
            btnComparar.Text = "Comparar";
            btnComparar.UseVisualStyleBackColor = true;
            btnComparar.Click += btnComparar_Click;
            // 
            // ComparadorInteligente
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1084, 674);
            Controls.Add(btnComparar);
            Controls.Add(groupBox1);
            Controls.Add(label2);
            Controls.Add(txtRobo);
            Controls.Add(label1);
            Controls.Add(txtPortal);
            Name = "ComparadorInteligente";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Comparador Inteligente";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtPortal;
        private Label label1;
        private Label label2;
        private TextBox txtRobo;
        private GroupBox groupBox1;
        private RadioButton rbdResponse;
        private RadioButton rbdRequest;
        private Button btnComparar;
    }
}
