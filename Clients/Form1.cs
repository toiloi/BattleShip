using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace BattleshipClientWin
{
    public partial class Form1 : Form
    {
        private TcpClient? _client;
        private NetworkStream? _stream;
        private string _myRole = "";
        private string _currentTurn = "";
        private bool _connected = false;
        private List<Ship> _myShips = new();
        private bool _placingShips = true;
        private int _currentShipIndex = 0;
        private string _shipDir = "H";
        private int[] _shipSizes = new[] { 5, 4, 3, 3, 2 };
        private List<Point> _previewCells = new(); // danh sách ô đang preview
        private Color _lastHoverColor = Color.White;
        private int _lastHoverX = -1;
        private int _lastHoverY = -1;
        private Timer _hitAnimTimer = new Timer();
        private DataGridViewCell? _animCell = null;
        private int _animState = 0;
        private Color _animColor1;
        private Color _animColor2;
        private Color _animFinal;
        private Timer _explosionTimer = new Timer();
        private List<DataGridViewCell> _explosionCells = new();
        private int _explosionState = 0;


        public Form1()
        {
            InitializeComponent();

            InitGrids();

            lblStatus.Text = "Status: waiting for game";

            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;

            dgvMyBoard.CellClick += dgvMyBoard_CellClick;
            dgvMyBoard.MouseDown += dgvMyBoard_MouseDown;
            dgvMyBoard.CellMouseEnter += dgvMyBoard_CellMouseEnter;
            dgvMyBoard.CellMouseLeave += dgvMyBoard_CellMouseLeave;

            dgvTarget.CellMouseEnter += dgvTarget_CellMouseEnter;
            dgvTarget.CellMouseLeave += dgvTarget_CellMouseLeave;

            _hitAnimTimer.Interval = 100;
            _hitAnimTimer.Tick += HitAnimTimer_Tick;

            _explosionTimer.Interval = 80;
            _explosionTimer.Tick += ExplosionTimer_Tick;

            // 🔥 Khi form resize => grid fit lại panel
            this.Resize += (s, e) =>
            {
                FitGridToPanel(dgvMyBoard, panelMyBoard);
                FitGridToPanel(dgvTarget, panelTarget);
            };
        }

        private void FitGridToPanel(DataGridView grid, Panel panel)
        {
            int cols = grid.ColumnCount;
            int rows = grid.RowCount;

            // Mỗi ô phải là hình vuông → lấy size nhỏ nhất
            int cellSize = Math.Min(panel.Width / cols, panel.Height / rows);

            // Cập nhật kích thước cột
            foreach (DataGridViewColumn col in grid.Columns)
                col.Width = cellSize;

            // Cập nhật chiều cao hàng
            grid.RowTemplate.Height = cellSize;
            foreach (DataGridViewRow r in grid.Rows)
                r.Height = cellSize;

            // Resize grid để vừa panel
            grid.Width = cellSize * cols;
            grid.Height = cellSize * rows;

            // Canh giữa panel
            grid.Left = (panel.Width - grid.Width) / 2;
            grid.Top = (panel.Height - grid.Height) / 2;

            // Không scroll
            grid.ScrollBars = ScrollBars.None;
        }

        private void InitGrids()
        {
            dgvMyBoard.ColumnCount = 10;
            dgvTarget.ColumnCount = 10;
            dgvMyBoard.RowCount = 10;
            dgvTarget.RowCount = 10;

            // Fit ngay khi load
            FitGridToPanel(dgvMyBoard, panelMyBoard);
            FitGridToPanel(dgvTarget, panelTarget);

            dgvMyBoard.AllowUserToResizeColumns = false;
            dgvMyBoard.AllowUserToResizeRows = false;
            dgvTarget.AllowUserToResizeColumns = false;
            dgvTarget.AllowUserToResizeRows = false;

            dgvMyBoard.AllowUserToOrderColumns = false;
            dgvTarget.AllowUserToOrderColumns = false;

            dgvMyBoard.ReadOnly = true;
            dgvTarget.ReadOnly = true;

            dgvMyBoard.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvTarget.SelectionMode = DataGridViewSelectionMode.CellSelect;

            dgvMyBoard.RowHeadersVisible = false;
            dgvTarget.RowHeadersVisible = false;
            dgvMyBoard.ColumnHeadersVisible = false;
            dgvTarget.ColumnHeadersVisible = false;

            dgvMyBoard.MultiSelect = false;
            dgvTarget.MultiSelect = false;

            dgvMyBoard.DefaultCellStyle.SelectionBackColor = Color.Transparent;
            dgvMyBoard.DefaultCellStyle.SelectionForeColor = Color.Transparent;

            dgvTarget.DefaultCellStyle.SelectionBackColor = Color.Transparent;
            dgvTarget.DefaultCellStyle.SelectionForeColor = Color.Transparent;
        }




        private void ResetBoards()
        {
            foreach (DataGridViewRow row in dgvMyBoard.Rows)
                foreach (DataGridViewCell cell in row.Cells)
                    cell.Style.BackColor = Color.White;
        }


        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (!_placingShips) return;

            if (e.KeyCode == Keys.R)
            {
                _shipDir = _shipDir == "H" ? "V" : "H";
                lblStatus.Text = $"Xoay hướng: {(_shipDir == "H" ? "Ngang" : "Dọc")}";
            }
        }
        private void dgvMyBoard_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!_placingShips) return;
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            int x = e.ColumnIndex;
            int y = e.RowIndex;
            int len = _shipSizes[_currentShipIndex];

            if (!CanPlaceShip(x, y, len, _shipDir))
            {
                lblStatus.Text = "Không thể đặt ở đây!";
                return;
            }

            // đặt tàu
            // colorIndex = thứ tự tàu đang đặt
            var ship = new Ship(len, x, y, _shipDir, _currentShipIndex);
            _myShips.Add(ship);
            DrawShip(ship);


            _currentShipIndex++;

            if (_currentShipIndex >= _shipSizes.Length)
            {
                lblStatus.Text = "Đã đặt xong tất cả tàu!";
                _placingShips = false;

                // gửi lên server
                _ = SendAsync(new { type = "PLACE_SHIPS", ships = _myShips });
                //btnPlaceShips.Enabled = false;
                return;
            }

            lblStatus.Text = $"Đặt tàu tiếp theo có độ dài {_shipSizes[_currentShipIndex]}";
        }
        private void RemoveShip(Ship s)
        {
            int idx = _myShips.IndexOf(s);
            if (idx < 0) return;

            // Xóa từ tàu vừa click đến tàu cuối cùng
            _myShips.RemoveRange(idx, _myShips.Count - idx);

            // Chỉ reset lại bảng của mình, không đụng bảng target
            foreach (DataGridViewRow row in dgvMyBoard.Rows)
                foreach (DataGridViewCell cell in row.Cells)
                    cell.Style.BackColor = Color.White;
            DrawMyShips();

            _currentShipIndex = idx;
            _placingShips = true;

            lblStatus.Text = "Đã xóa tàu từ vị trí này trở về sau, hãy đặt lại.";
        }


        private void dgvMyBoard_MouseDown(object? sender, MouseEventArgs e)
        {
            if (!_placingShips) return;
            if (e.Button != MouseButtons.Right) return;

            var hit = dgvMyBoard.HitTest(e.X, e.Y);
            int row = hit.RowIndex;
            int col = hit.ColumnIndex;

            if (row < 0 || col < 0) return;

            // tìm tàu chứa ô này
            Ship? targetShip = null;

            foreach (var s in _myShips)
            {
                int dx = s.dir == "H" ? 1 : 0;
                int dy = s.dir == "V" ? 1 : 0;

                for (int i = 0; i < s.len; i++)
                {
                    int xx = s.x + dx * i;
                    int yy = s.y + dy * i;

                    if (xx == col && yy == row)
                    {
                        targetShip = s;
                        break;
                    }
                }
                if (targetShip != null) break;
            }

            if (targetShip == null) return;

            // ❗ XÓA TÀU
            RemoveShip(targetShip);

            lblStatus.Text = "Đã xóa tàu! Bạn có thể đặt lại.";
        }

        private bool CanPlaceShip(int x, int y, int len, string dir)
        {
            int dx = dir == "H" ? 1 : 0;
            int dy = dir == "V" ? 1 : 0;

            int endX = x + dx * (len - 1);
            int endY = y + dy * (len - 1);

            // vượt biên
            if (endX >= 10 || endY >= 10) return false;

            // trùng tàu khác
            foreach (var s in _myShips)
            {
                if (ShipOverlap(s, x, y, len, dir))
                    return false;
            }

            return true;
        }
        private bool ShipOverlap(Ship s, int x, int y, int len, string dir)
        {
            int dx1 = s.dir == "H" ? 1 : 0;
            int dy1 = s.dir == "V" ? 1 : 0;

            int dx2 = dir == "H" ? 1 : 0;
            int dy2 = dir == "V" ? 1 : 0;

            for (int i = 0; i < s.len; i++)
            {
                int sx = s.x + dx1 * i;
                int sy = s.y + dy1 * i;

                for (int j = 0; j < len; j++)
                {
                    int nx = x + dx2 * j;
                    int ny = y + dy2 * j;

                    if (sx == nx && sy == ny)
                        return true;
                }
            }

            return false;
        }
        private Color[] _shipColors = new[]
         {
            Color.LightBlue,   // tàu 5 ô
            Color.LightGreen,  // tàu 4 ô
            Color.Orange,      // tàu 3 ô (1)
            Color.MediumPurple,// tàu 3 ô (2)
            Color.LightPink    // tàu 2 ô
        };
        private void DrawShip(Ship s)
        {
            int idx = s.colorIndex;
            if (idx < 0 || idx >= _shipColors.Length) idx = 0; // fallback an toàn

            Color color = _shipColors[idx];

            int dx = s.dir == "H" ? 1 : 0;
            int dy = s.dir == "V" ? 1 : 0;

            for (int i = 0; i < s.len; i++)
            {
                int xx = s.x + dx * i;
                int yy = s.y + dy * i;
                dgvMyBoard.Rows[yy].Cells[xx].Style.BackColor = color;
            }
        }


        private async void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync("127.0.0.1", 5000);
                _stream = _client.GetStream();
                _connected = true;
                MessageBox.Show("Connected!");

                _ = Task.Run(() => ListenFromServer());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task ListenFromServer()
        {
            if (_stream == null) return;

            using var reader = new StreamReader(_stream, Encoding.UTF8);

            while (true)
            {
                string? line = null;
                try { line = await reader.ReadLineAsync(); }
                catch { break; }

                if (line == null) break;

                Message? msg = null;
                try { msg = JsonSerializer.Deserialize<Message>(line); }
                catch { }

                if (msg == null) continue;

                this.BeginInvoke(new Action(() => HandleServerMessage(msg)));
            }
        }

        private void HandleServerMessage(Message msg)
        {
            switch (msg.type)
            {
                case "ROOM_CREATED":
                    _myRole = msg.role ?? "";
                    txtRoomId.Text = msg.roomId;
                    lblRole.Text = "Role: " + _myRole;
                    lblStatus.Text = "Status: room created, wait for other player";
                    break;

                case "JOIN_OK":
                    _myRole = msg.role ?? "";
                    lblRole.Text = "Role: " + _myRole;
                    lblStatus.Text = "Status: joined room, both place ships";
                    break;

                case "GAME_START":
                    _currentTurn = msg.turn ?? "";
                    lblTurn.Text = "Turn: " + _currentTurn;
                    lblStatus.Text = "Status: GAME STARTED";
                    ResetBoards();
                    DrawMyShips();
                    break;

                case "FIRE_RESULT":
                    PaintFireResult(msg);
                    _currentTurn = msg.nextTurn ?? "";
                    lblTurn.Text = "Turn: " + (_currentTurn == "" ? "-" : _currentTurn);
                    lblStatus.Text = "Status: shot fired";
                    break;

                case "GAME_OVER":
                    lblStatus.Text = "Winner: " + msg.winner;
                    MessageBox.Show("Winner: " + msg.winner);
                    break;
            }
        }

        private void DrawMyShips()
        {
            foreach (var s in _myShips)
            {
                DrawShip(s);
            }
        }



        private Color GetColorByResult(string result)
        {
            return result switch
            {
                "MISS" => Color.LightGray,
                "HIT" => Color.Red,
                "SUNK" => Color.Black,
                _ => Color.White
            };
        }

        private void PaintFireResult(Message msg)
        {
            DataGridViewCell cell;

            bool isMyShot = msg.from == _myRole;
            DataGridView grid = isMyShot ? dgvTarget : dgvMyBoard;  // ✔ thêm dòng này

            cell = grid.Rows[msg.y].Cells[msg.x];

            // Animation HIT/MISS/SUNK
            AnimateCell(cell, msg.result!);

            // 💥 Explosion wave hiệu ứng lan
            if (msg.result == "HIT" || msg.result == "SUNK")
                StartExplosionWave(grid, msg.x, msg.y);

            // 🚢 Nếu SUNK và bị bắn trúng -> highlight full ship
            if (msg.result == "SUNK" && !isMyShot)
            {
                Ship? targetShip = FindShipAt(msg.x, msg.y);
                if (targetShip != null)
                    HighlightSunkShip(targetShip);
            }
        }


        private async void btnCreate_Click(object sender, EventArgs e)
        {
            if (!_connected || _stream == null) return;
            await SendAsync(new { type = "CREATE_ROOM" });
        }

        private async void btnJoin_Click(object sender, EventArgs e)
        {
            if (!_connected || _stream == null) return;
            if (string.IsNullOrWhiteSpace(txtRoomId.Text)) return;

            await SendAsync(new { type = "JOIN_ROOM", roomId = txtRoomId.Text.Trim() });
        }


        private void ClearPreview()
        {
            foreach (var p in _previewCells)
            {
                dgvMyBoard.Rows[p.Y].Cells[p.X].Style.BackColor = Color.White;
            }
            _previewCells.Clear();
            DrawMyShips(); // vẽ lại tàu cũ
        }
        private void ShowPreview(int x, int y)
        {
            ClearPreview();

            int len = _shipSizes[_currentShipIndex];
            int dx = _shipDir == "H" ? 1 : 0;
            int dy = _shipDir == "V" ? 1 : 0;

            bool valid = CanPlaceShip(x, y, len, _shipDir);

            for (int i = 0; i < len; i++)
            {
                int xx = x + dx * i;
                int yy = y + dy * i;

                if (xx < 0 || xx >= 10 || yy < 0 || yy >= 10) continue;

                dgvMyBoard.Rows[yy].Cells[xx].Style.BackColor =
                    valid ? Color.Khaki : Color.LightCoral;

                _previewCells.Add(new Point(xx, yy));
            }
        }
        private void dgvMyBoard_CellMouseEnter(object? sender, DataGridViewCellEventArgs e)
        {
            if (!_placingShips) return;
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            ShowPreview(e.ColumnIndex, e.RowIndex);
        }
        private void dgvMyBoard_CellMouseLeave(object? sender, DataGridViewCellEventArgs e)
        {
            if (!_placingShips) return;

            ClearPreview();
        }

        private async void dgvTarget_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!_connected || _stream == null) return;
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // chỉ bắn khi tới lượt mình
            if (_currentTurn != _myRole)
            {
                lblStatus.Text = "Status: not your turn!";
                return;
            }

            await SendAsync(new { type = "FIRE", x = e.ColumnIndex, y = e.RowIndex });
            ClearPreview();  

        }
        private void dgvTarget_CellMouseEnter(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // lưu vị trí và màu gốc
            _lastHoverX = e.ColumnIndex;
            _lastHoverY = e.RowIndex;

            var cell = dgvTarget.Rows[e.RowIndex].Cells[e.ColumnIndex];
            _lastHoverColor = cell.Style.BackColor;

            // nếu đã bắn rồi -> không preview
            if (_lastHoverColor != Color.White)
            {
                cell.Style.BackColor = Color.LightGray;
                return;
            }

            // nếu chưa tới lượt -> preview màu đỏ cảnh báo
            if (_currentTurn != _myRole)
            {
                cell.Style.BackColor = Color.LightPink;
                lblStatus.Text = "Not your turn!";
                return;
            }

            // preview màu vàng bình thường
            cell.Style.BackColor = Color.Khaki;
        }
        private void dgvTarget_CellMouseLeave(object? sender, DataGridViewCellEventArgs e)
        {
            if (_lastHoverX < 0 || _lastHoverY < 0) return;

            dgvTarget.Rows[_lastHoverY].Cells[_lastHoverX].Style.BackColor = _lastHoverColor;

            _lastHoverX = -1;
            _lastHoverY = -1;
        }

        private async Task SendAsync(object obj)
        {
            if (_stream == null) return;
            string json = JsonSerializer.Serialize(obj);
            byte[] data = Encoding.UTF8.GetBytes(json + "\n");
            await _stream.WriteAsync(data, 0, data.Length);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void HitAnimTimer_Tick(object? sender, EventArgs e)
        {
            if (_animCell == null) return;

            // nhấp nháy qua lại 6 lần
            if (_animState % 2 == 0)
                _animCell.Style.BackColor = _animColor1;
            else
                _animCell.Style.BackColor = _animColor2;

            _animState++;

            if (_animState > 6)
            {
                _hitAnimTimer.Stop();
                _animCell.Style.BackColor = _animFinal;
                _animCell = null;
            }
        }
        private void AnimateCell(DataGridViewCell cell, string result)
        {
            _animCell = cell;
            _animState = 0;

            switch (result)
            {
                case "HIT":
                    _animColor1 = Color.Red;
                    _animColor2 = Color.OrangeRed;
                    _animFinal = Color.Red;
                    break;

                case "MISS":
                    _animColor1 = Color.LightGray;
                    _animColor2 = Color.DarkGray;
                    _animFinal = Color.LightGray;
                    break;

                case "SUNK":
                    _animColor1 = Color.Black;
                    _animColor2 = Color.Gold;
                    _animFinal = Color.Black;
                    break;
            }

            _hitAnimTimer.Start();
        }

        private Ship? FindShipAt(int x, int y)
        {
            foreach (var s in _myShips)
            {
                int dx = s.dir == "H" ? 1 : 0;
                int dy = s.dir == "V" ? 1 : 0;

                for (int i = 0; i < s.len; i++)
                {
                    int xx = s.x + dx * i;
                    int yy = s.y + dy * i;

                    if (xx == x && yy == y)
                        return s;
                }
            }
            return null;
        }
        private void HighlightSunkShip(Ship s)
        {
            int dx = s.dir == "H" ? 1 : 0;
            int dy = s.dir == "V" ? 1 : 0;

            for (int i = 0; i < s.len; i++)
            {
                int xx = s.x + dx * i;
                int yy = s.y + dy * i;

                var cell = dgvMyBoard.Rows[yy].Cells[xx];
                AnimateCell(cell, "SUNK");
            }
        }


        private void StartExplosionWave(DataGridView grid, int x, int y)
        {
            _explosionCells.Clear();
            _explosionState = 0;

            int[] dx = { -1, 0, 1 };
            int[] dy = { -1, 0, 1 };

            // Lấy 8 ô xung quanh
            foreach (int ox in dx)
            {
                foreach (int oy in dy)
                {
                    if (ox == 0 && oy == 0) continue; // bỏ ô tâm

                    int xx = x + ox;
                    int yy = y + oy;

                    if (xx >= 0 && xx < 10 && yy >= 0 && yy < 10)
                    {
                        _explosionCells.Add(grid.Rows[yy].Cells[xx]);
                    }
                }
            }

            _explosionTimer.Start();
        }


        private void ExplosionTimer_Tick(object? sender, EventArgs e)
        {
            _explosionState++;

            Color c1 = Color.Gold;
            Color c2 = Color.OrangeRed;

            // nháy 4 lần
            foreach (var cell in _explosionCells)
            {
                if (_explosionState % 2 == 0)
                    cell.Style.BackColor = c1;
                else
                    cell.Style.BackColor = c2;
            }

            if (_explosionState > 4)
            {
                _explosionTimer.Stop();
                // trả màu về cũ (trắng)
                foreach (var cell in _explosionCells)
                    cell.Style.BackColor = Color.White;
            }
        }

       

    }

    public class Ship
    {
        public int len { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public string dir { get; set; }

        // thêm thuộc tính màu
        public int colorIndex { get; set; }

        public Ship(int len, int x, int y, string dir, int colorIndex)
        {
            this.len = len;
            this.x = x;
            this.y = y;
            this.dir = dir;
            this.colorIndex = colorIndex;
        }
    }


    public class Message
    {
        public string? type { get; set; }
        public string? roomId { get; set; }
        public string? role { get; set; }
        public string? winner { get; set; }
        public string? from { get; set; }
        public string? turn { get; set; }
        public string? nextTurn { get; set; }
        public string? result { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int? shipLen { get; set; }
    }
}
