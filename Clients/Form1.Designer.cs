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
            this.btnConnect = new Button();
            this.btnCreate = new Button();
            this.btnJoin = new Button();
            this.txtRoomId = new TextBox();
            this.lblRole = new Label();
            this.lblTurn = new Label();
            this.lblStatus = new Label();
            this.dgvMyBoard = new DataGridView();
            this.dgvTarget = new DataGridView();
            this.label1 = new Label();
            this.label2 = new Label();
            this.label3 = new Label();

            // ⭐️ 2 PANEL mới
            this.panelMyBoard = new Panel();
            this.panelTarget = new Panel();

            ((System.ComponentModel.ISupportInitialize)(this.dgvMyBoard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTarget)).BeginInit();
            this.SuspendLayout();

            // ======================
            //  BUTTONS + LABELS
            // ======================

            this.btnConnect.Location = new Point(12, 12);
            this.btnConnect.Size = new Size(90, 30);
            this.btnConnect.Text = "Connect";
            this.btnConnect.Click += btnConnect_Click;

            this.btnCreate.Location = new Point(108, 12);
            this.btnCreate.Size = new Size(90, 30);
            this.btnCreate.Text = "Create";
            this.btnCreate.Click += btnCreate_Click;

            this.txtRoomId.Location = new Point(204, 14);
            this.txtRoomId.Size = new Size(160, 23);

            this.btnJoin.Location = new Point(370, 12);
            this.btnJoin.Size = new Size(90, 30);
            this.btnJoin.Text = "Join";
            this.btnJoin.Click += btnJoin_Click;

            this.lblRole.Location = new Point(12, 55);
            this.lblRole.Size = new Size(150, 25);
            this.lblRole.Text = "Role: ???";

            this.lblTurn.Location = new Point(170, 55);
            this.lblTurn.Size = new Size(150, 25);
            this.lblTurn.Text = "Turn: none";

            this.lblStatus.Location = new Point(330, 55);
            this.lblStatus.Size = new Size(300, 25);
            this.lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblStatus.Text = "Status: waiting for game";

            // ======================
            //  PANELS
            // ======================

            this.panelMyBoard.Location = new Point(12, 90);
            this.panelMyBoard.Size = new Size(350, 350);
            this.panelMyBoard.BorderStyle = BorderStyle.FixedSingle;

            this.panelTarget.Location = new Point(400, 90);
            this.panelTarget.Size = new Size(350, 350);
            this.panelTarget.BorderStyle = BorderStyle.FixedSingle;

            // ======================
            //  GRIDS đặt vào PANEL
            // ======================

            this.dgvMyBoard.Dock = DockStyle.Fill;
            this.dgvMyBoard.RowHeadersVisible = false;
            this.dgvMyBoard.ColumnHeadersVisible = false;

            this.dgvTarget.Dock = DockStyle.Fill;
            this.dgvTarget.RowHeadersVisible = false;
            this.dgvTarget.ColumnHeadersVisible = false;
            this.dgvTarget.CellClick += dgvTarget_CellClick;

            // ⭐️ THÊM GRID VÀO PANEL
            this.panelMyBoard.Controls.Add(this.dgvMyBoard);
            this.panelTarget.Controls.Add(this.dgvTarget);

            // ======================
            //  LƯU Ý
            // ======================

            this.label1.AutoSize = true;
            this.label1.Location = new Point(41, 460);
            this.label1.Text = "Lưu ý:";

            this.label2.AutoSize = true;
            this.label2.Location = new Point(41, 475);
            this.label2.Text = "Nhấn R để xoay hướng tàu (ngang/dọc)";

            this.label3.AutoSize = true;
            this.label3.Location = new Point(41, 490);
            this.label3.Text = "Sau khi đặt đủ 5 tàu sẽ khoá lại và không thể chỉnh sửa";

            // ======================
            //  FORM SETTINGS
            // ======================

            this.ClientSize = new Size(770, 531);
            this.Controls.Add(this.panelMyBoard);
            this.Controls.Add(this.panelTarget);

            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);

            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblTurn);
            this.Controls.Add(this.lblRole);

            this.Controls.Add(this.txtRoomId);
            this.Controls.Add(this.btnJoin);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.btnConnect);

            this.Text = "Battleship Client";

            ((System.ComponentModel.ISupportInitialize)(this.dgvMyBoard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTarget)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Button btnConnect;
        private Button btnCreate;
        private Button btnJoin;
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
    }
}
