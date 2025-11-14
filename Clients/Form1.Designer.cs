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
            this.dgvMyBoard = new System.Windows.Forms.DataGridView();
            this.dgvTarget = new System.Windows.Forms.DataGridView();
            this.btnPlaceShips = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMyBoard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTarget)).BeginInit();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(12, 12);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(94, 29);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(112, 12);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(94, 29);
            this.btnCreate.TabIndex = 1;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // btnJoin
            // 
            this.btnJoin.Location = new System.Drawing.Point(412, 12);
            this.btnJoin.Name = "btnJoin";
            this.btnJoin.Size = new System.Drawing.Size(94, 29);
            this.btnJoin.TabIndex = 2;
            this.btnJoin.Text = "Join";
            this.btnJoin.UseVisualStyleBackColor = true;
            this.btnJoin.Click += new System.EventHandler(this.btnJoin_Click);
            // 
            // txtRoomId
            // 
            this.txtRoomId.Location = new System.Drawing.Point(212, 12);
            this.txtRoomId.Name = "txtRoomId";
            this.txtRoomId.Size = new System.Drawing.Size(194, 27);
            this.txtRoomId.TabIndex = 3;
            // 
            // lblRole
            // 
            this.lblRole.AutoSize = true;
            this.lblRole.Location = new System.Drawing.Point(12, 52);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(62, 20);
            this.lblRole.TabIndex = 4;
            this.lblRole.Text = "Role: ???";
            // 
            // lblTurn
            // 
            this.lblTurn.AutoSize = true;
            this.lblTurn.Location = new System.Drawing.Point(212, 52);
            this.lblTurn.Name = "lblTurn";
            this.lblTurn.Size = new System.Drawing.Size(77, 20);
            this.lblTurn.TabIndex = 5;
            this.lblTurn.Text = "Turn: none";
            // 
            // dgvMyBoard
            // 
            this.dgvMyBoard.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMyBoard.Location = new System.Drawing.Point(12, 84);
            this.dgvMyBoard.Name = "dgvMyBoard";
            this.dgvMyBoard.RowHeadersWidth = 51;
            this.dgvMyBoard.RowTemplate.Height = 29;
            this.dgvMyBoard.Size = new System.Drawing.Size(350, 350);
            this.dgvMyBoard.TabIndex = 6;
            // 
            // dgvTarget
            // 
            this.dgvTarget.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTarget.Location = new System.Drawing.Point(412, 84);
            this.dgvTarget.Name = "dgvTarget";
            this.dgvTarget.RowHeadersWidth = 51;
            this.dgvTarget.RowTemplate.Height = 29;
            this.dgvTarget.Size = new System.Drawing.Size(350, 350);
            this.dgvTarget.TabIndex = 7;
            this.dgvTarget.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTarget_CellClick);
            // 
            // btnPlaceShips
            // 
            this.btnPlaceShips.Location = new System.Drawing.Point(512, 12);
            this.btnPlaceShips.Name = "btnPlaceShips";
            this.btnPlaceShips.Size = new System.Drawing.Size(94, 29);
            this.btnPlaceShips.TabIndex = 8;
            this.btnPlaceShips.Text = "Place ships";
            this.btnPlaceShips.UseVisualStyleBackColor = true;
            this.btnPlaceShips.Click += new System.EventHandler(this.btnPlaceShips_Click);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(774, 450);
            this.Controls.Add(this.btnPlaceShips);
            this.Controls.Add(this.dgvTarget);
            this.Controls.Add(this.dgvMyBoard);
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
    }
}
