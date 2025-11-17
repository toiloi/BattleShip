namespace BattleshipClientWin
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            btnConnect = new Button();
            btnCreate = new Button();
            btnJoin = new Button();
            txtRoomId = new TextBox();
            lblRole = new Label();
            lblTurn = new Label();
            lblStatus = new Label();
            dgvMyBoard = new DataGridView();
            dgvTarget = new DataGridView();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            panelMyBoard = new Panel();
            panelTarget = new Panel();
            btnLeave = new Button();
            label4 = new Label();
            label5 = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvMyBoard).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvTarget).BeginInit();
            panelMyBoard.SuspendLayout();
            panelTarget.SuspendLayout();
            SuspendLayout();
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(12, 12);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(90, 30);
            btnConnect.TabIndex = 12;
            btnConnect.Text = "Connect";
            btnConnect.Click += btnConnect_Click;
            // 
            // btnCreate
            // 
            btnCreate.Location = new Point(108, 12);
            btnCreate.Name = "btnCreate";
            btnCreate.Size = new Size(90, 30);
            btnCreate.TabIndex = 11;
            btnCreate.Text = "Create";
            btnCreate.Click += btnCreate_Click;
            // 
            // btnJoin
            // 
            btnJoin.Location = new Point(370, 12);
            btnJoin.Name = "btnJoin";
            btnJoin.Size = new Size(90, 30);
            btnJoin.TabIndex = 10;
            btnJoin.Text = "Join";
            btnJoin.Click += btnJoin_Click;
            // 
            // txtRoomId
            // 
            txtRoomId.Location = new Point(204, 14);
            txtRoomId.Name = "txtRoomId";
            txtRoomId.Size = new Size(160, 23);
            txtRoomId.TabIndex = 9;
            // 
            // lblRole
            // 
            lblRole.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblRole.Location = new Point(12, 55);
            lblRole.Name = "lblRole";
            lblRole.Size = new Size(150, 25);
            lblRole.TabIndex = 8;
            lblRole.Text = "Role: ???";
            // 
            // lblTurn
            // 
            lblTurn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTurn.Location = new Point(170, 55);
            lblTurn.Name = "lblTurn";
            lblTurn.Size = new Size(150, 25);
            lblTurn.TabIndex = 7;
            lblTurn.Text = "Turn: none";
            lblTurn.UseMnemonic = false;
            // 
            // lblStatus
            // 
            lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblStatus.Location = new Point(330, 55);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(300, 25);
            lblStatus.TabIndex = 6;
            lblStatus.Text = "Status: waiting for game";
            // 
            // dgvMyBoard
            // 
            dgvMyBoard.ColumnHeadersVisible = false;
            dgvMyBoard.Dock = DockStyle.Fill;
            dgvMyBoard.Location = new Point(0, 0);
            dgvMyBoard.Name = "dgvMyBoard";
            dgvMyBoard.RowHeadersVisible = false;
            dgvMyBoard.Size = new Size(348, 348);
            dgvMyBoard.TabIndex = 0;
            // 
            // dgvTarget
            // 
            dgvTarget.ColumnHeadersVisible = false;
            dgvTarget.Dock = DockStyle.Fill;
            dgvTarget.Location = new Point(0, 0);
            dgvTarget.Name = "dgvTarget";
            dgvTarget.RowHeadersVisible = false;
            dgvTarget.Size = new Size(348, 348);
            dgvTarget.TabIndex = 0;
            dgvTarget.CellClick += dgvTarget_CellClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(38, 492);
            label1.Name = "label1";
            label1.Size = new Size(39, 15);
            label1.TabIndex = 5;
            label1.Text = "Lưu ý:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(38, 507);
            label2.Name = "label2";
            label2.Size = new Size(217, 15);
            label2.TabIndex = 4;
            label2.Text = "Nhấn R để xoay hướng tàu (ngang/dọc)";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(261, 507);
            label3.Name = "label3";
            label3.Size = new Size(295, 15);
            label3.TabIndex = 3;
            label3.Text = "Sau khi đặt đủ 5 tàu sẽ khoá lại và không thể chỉnh sửa";
            // 
            // panelMyBoard
            // 
            panelMyBoard.BorderStyle = BorderStyle.FixedSingle;
            panelMyBoard.Controls.Add(dgvMyBoard);
            panelMyBoard.Location = new Point(12, 90);
            panelMyBoard.Name = "panelMyBoard";
            panelMyBoard.Size = new Size(350, 350);
            panelMyBoard.TabIndex = 1;
            // 
            // panelTarget
            // 
            panelTarget.BorderStyle = BorderStyle.FixedSingle;
            panelTarget.Controls.Add(dgvTarget);
            panelTarget.Location = new Point(400, 90);
            panelTarget.Name = "panelTarget";
            panelTarget.Size = new Size(350, 350);
            panelTarget.TabIndex = 2;
            // 
            // btnLeave
            // 
            btnLeave.Location = new Point(466, 12);
            btnLeave.Name = "btnLeave";
            btnLeave.Size = new Size(90, 30);
            btnLeave.TabIndex = 0;
            btnLeave.Text = "Leave";
            btnLeave.UseVisualStyleBackColor = true;
            btnLeave.Click += btnLeave_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(135, 443);
            label4.Name = "label4";
            label4.Size = new Size(51, 15);
            label4.TabIndex = 13;
            label4.Text = "My Map";
            label4.Click += label4_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(528, 443);
            label5.Name = "label5";
            label5.Size = new Size(70, 15);
            label5.TabIndex = 14;
            label5.Text = "Enemy Map";
            label5.Click += label5_Click;
            // 
            // Form1
            // 
            ClientSize = new Size(770, 531);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(btnLeave);
            Controls.Add(panelMyBoard);
            Controls.Add(panelTarget);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(lblStatus);
            Controls.Add(lblTurn);
            Controls.Add(lblRole);
            Controls.Add(txtRoomId);
            Controls.Add(btnJoin);
            Controls.Add(btnCreate);
            Controls.Add(btnConnect);
            Name = "Form1";
            Text = "Battleship Client";
            ((System.ComponentModel.ISupportInitialize)dgvMyBoard).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvTarget).EndInit();
            panelMyBoard.ResumeLayout(false);
            panelTarget.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnConnect;
        private Button btnCreate;
        private Button btnJoin;
        private Button btnLeave;
        private TextBox txtRoomId;
        private Label lblRole;
        private Label lblTurn;
        private Label lblStatus;

        private Panel panelMyBoard;
        private Panel panelTarget;

        private DataGridView dgvMyBoard;
        private DataGridView dgvTarget;

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
    }
}
