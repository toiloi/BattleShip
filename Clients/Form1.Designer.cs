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
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnJoin = new System.Windows.Forms.Button();
            this.txtRoomId = new System.Windows.Forms.TextBox();
            this.lblRole = new System.Windows.Forms.Label();
            this.lblTurn = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.dgvMyBoard = new System.Windows.Forms.DataGridView();
            this.dgvTarget = new System.Windows.Forms.DataGridView();
            this.btnPlaceShips = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.dgvMyBoard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTarget)).BeginInit();
            this.SuspendLayout();

            // btnConnect
            this.btnConnect.Location = new System.Drawing.Point(12, 12);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(94, 29);
            this.btnConnect.Text = "Connect";
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);

            // btnCreate
            this.btnCreate.Location = new System.Drawing.Point(112, 12);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(94, 29);
            this.btnCreate.Text = "Create";
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);

            // txtRoomId
            this.txtRoomId.Location = new System.Drawing.Point(212, 12);
            this.txtRoomId.Name = "txtRoomId";
            this.txtRoomId.Size = new System.Drawing.Size(194, 27);

            // btnJoin
            this.btnJoin.Location = new System.Drawing.Point(412, 12);
            this.btnJoin.Name = "btnJoin";
            this.btnJoin.Size = new System.Drawing.Size(94, 29);
            this.btnJoin.Text = "Join";
            this.btnJoin.Click += new System.EventHandler(this.btnJoin_Click);

            // lblRole
            this.lblRole.AutoSize = true;
            this.lblRole.Location = new System.Drawing.Point(12, 52);
            this.lblRole.Name = "lblRole";
            this.lblRole.Text = "Role: ???";

            // lblTurn
            this.lblTurn.AutoSize = true;
            this.lblTurn.Location = new System.Drawing.Point(212, 52);
            this.lblTurn.Name = "lblTurn";
            this.lblTurn.Text = "Turn: none";

            // lblStatus  ★★★ NEW ★★★
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(412, 52);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Text = "Status: waiting";

            // dgvMyBoard
            this.dgvMyBoard.Location = new System.Drawing.Point(12, 84);
            this.dgvMyBoard.Size = new System.Drawing.Size(350, 350);
            this.dgvMyBoard.RowHeadersWidth = 51;

            // dgvTarget
            this.dgvTarget.Location = new System.Drawing.Point(412, 84);
            this.dgvTarget.Size = new System.Drawing.Size(350, 350);
            this.dgvTarget.RowHeadersWidth = 51;
            this.dgvTarget.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTarget_CellClick);

            // btnPlaceShips
            this.btnPlaceShips.Location = new System.Drawing.Point(512, 12);
            this.btnPlaceShips.Name = "btnPlaceShips";
            this.btnPlaceShips.Size = new System.Drawing.Size(94, 29);
            this.btnPlaceShips.Text = "Place ships";
            this.btnPlaceShips.Click += new System.EventHandler(this.btnPlaceShips_Click);

            // Form1
            this.ClientSize = new System.Drawing.Size(780, 450);
            this.Controls.Add(this.btnPlaceShips);
            this.Controls.Add(this.dgvTarget);
            this.Controls.Add(this.dgvMyBoard);
            this.Controls.Add(this.lblStatus);   // ★ Thêm vào Form
            this.Controls.Add(this.lblTurn);
            this.Controls.Add(this.lblRole);
            this.Controls.Add(this.txtRoomId);
            this.Controls.Add(this.btnJoin);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.btnConnect);
            this.Name = "Form1";
            this.Text = "Battleship Client";

            ((System.ComponentModel.ISupportInitialize)(this.dgvMyBoard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTarget)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }


        private Button btnConnect;
        private Button btnCreate;
        private Button btnJoin;
        private TextBox txtRoomId;
        private Label lblRole;
        private Label lblTurn;
        private DataGridView dgvMyBoard;
        private DataGridView dgvTarget;
        private Button btnPlaceShips;
        private Label lblStatus;
    }
}
