namespace BattleshipClientWin
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

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
            ((System.ComponentModel.ISupportInitialize)dgvMyBoard).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvTarget).BeginInit();
            SuspendLayout();
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(12, 12);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(90, 30);
            btnConnect.TabIndex = 9;
            btnConnect.Text = "Connect";
            btnConnect.Click += btnConnect_Click;
            // 
            // btnCreate
            // 
            btnCreate.Location = new Point(108, 12);
            btnCreate.Name = "btnCreate";
            btnCreate.Size = new Size(90, 30);
            btnCreate.TabIndex = 8;
            btnCreate.Text = "Create";
            btnCreate.Click += btnCreate_Click;
            // 
            // btnJoin
            // 
            btnJoin.Location = new Point(370, 12);
            btnJoin.Name = "btnJoin";
            btnJoin.Size = new Size(90, 30);
            btnJoin.TabIndex = 7;
            btnJoin.Text = "Join";
            btnJoin.Click += btnJoin_Click;
            // 
            // txtRoomId
            // 
            txtRoomId.Location = new Point(204, 14);
            txtRoomId.Name = "txtRoomId";
            txtRoomId.Size = new Size(160, 23);
            txtRoomId.TabIndex = 6;
            // 
            // lblRole
            // 
            lblRole.Location = new Point(12, 55);
            lblRole.Name = "lblRole";
            lblRole.Size = new Size(150, 25);
            lblRole.TabIndex = 5;
            lblRole.Text = "Role: ???";
            // 
            // lblTurn
            // 
            lblTurn.Location = new Point(170, 55);
            lblTurn.Name = "lblTurn";
            lblTurn.Size = new Size(150, 25);
            lblTurn.TabIndex = 4;
            lblTurn.Text = "Turn: none";
            // 
            // lblStatus
            // 
            lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblStatus.Location = new Point(330, 55);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(300, 25);
            lblStatus.TabIndex = 3;
            lblStatus.Text = "Status: waiting";
            // 
            // dgvMyBoard
            // 
            dgvMyBoard.Location = new Point(12, 90);
            dgvMyBoard.Name = "dgvMyBoard";
            dgvMyBoard.RowHeadersVisible = false;
            dgvMyBoard.Size = new Size(350, 350);
            dgvMyBoard.TabIndex = 2;
            // 
            // dgvTarget
            // 
            dgvTarget.Location = new Point(400, 90);
            dgvTarget.Name = "dgvTarget";
            dgvTarget.RowHeadersVisible = false;
            dgvTarget.Size = new Size(350, 350);
            dgvTarget.TabIndex = 1;
            dgvTarget.CellClick += dgvTarget_CellClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(41, 460);
            label1.Name = "label1";
            label1.Size = new Size(46, 15);
            label1.TabIndex = 10;
            label1.Text = "Lưu Ý : ";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(41, 475);
            label2.Name = "label2";
            label2.Size = new Size(331, 15);
            label2.TabIndex = 11;
            label2.Text = "Trước khi đặt tàu nhấn R để quy định tàu xoay ngang hay dọc";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(41, 490);
            label3.Name = "label3";
            label3.Size = new Size(295, 15);
            label3.TabIndex = 12;
            label3.Text = "Sau khi đặt đủ 5 tàu sẽ khoá lại và không thể chỉnh sửa";
            label3.Click += label3_Click;
            // 
            // Form1
            // 
            ClientSize = new Size(770, 531);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(dgvTarget);
            Controls.Add(dgvMyBoard);
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
            ResumeLayout(false);
            PerformLayout();
        }

        private Button btnConnect;
        private Button btnCreate;
        private Button btnJoin;
        private TextBox txtRoomId;
        private Label lblRole;
        private Label lblTurn;
        private Label lblStatus;
        private DataGridView dgvMyBoard;
        private DataGridView dgvTarget;
        private Button btnPlaceShips;
        private Label label1;
        private Label label2;
        private Label label3;
    }
}
