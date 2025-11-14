using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

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

        public Form1()
        {
            InitializeComponent();
            InitGrids();
            lblStatus.Text = "Status: waiting for game";
            this.KeyPreview = true;        // để nhận phím R
            this.KeyDown += Form1_KeyDown; // nghe nút xoay
            dgvMyBoard.CellClick += dgvMyBoard_CellClick;
            dgvMyBoard.MouseDown += dgvMyBoard_MouseDown;

        }

        private void InitGrids()
        {
            dgvMyBoard.ColumnCount = 10;
            dgvTarget.ColumnCount = 10;
            dgvMyBoard.RowCount = 10;
            dgvTarget.RowCount = 10;

            // ❌ Không cho resize hàng/cột
            dgvMyBoard.AllowUserToResizeColumns = false;
            dgvMyBoard.AllowUserToResizeRows = false;
            dgvTarget.AllowUserToResizeColumns = false;
            dgvTarget.AllowUserToResizeRows = false;

            // ❌ Không cho reorder cột
            dgvMyBoard.AllowUserToOrderColumns = false;
            dgvTarget.AllowUserToOrderColumns = false;

            // ❌ Không cho chọn nguyên dòng/nguyên cột
            dgvMyBoard.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvTarget.SelectionMode = DataGridViewSelectionMode.CellSelect;

            // ❌ Không cho kéo scroll header (header đã ẩn rồi)
            dgvMyBoard.RowHeadersVisible = false;
            dgvTarget.RowHeadersVisible = false;
            dgvMyBoard.ColumnHeadersVisible = false;
            dgvTarget.ColumnHeadersVisible = false;

            // ❌ Không thể sửa ô
            dgvMyBoard.ReadOnly = true;
            dgvTarget.ReadOnly = true;

            // ❌ Không cho multi-select
            dgvMyBoard.MultiSelect = false;
            dgvTarget.MultiSelect = false;

            // Khi click vào ô -> không highlight xanh
            dgvMyBoard.DefaultCellStyle.SelectionBackColor = Color.Transparent;
            dgvMyBoard.DefaultCellStyle.SelectionForeColor = Color.Transparent;
            dgvTarget.DefaultCellStyle.SelectionBackColor = Color.Transparent;
            dgvTarget.DefaultCellStyle.SelectionForeColor = Color.Transparent;

            // Đặt size ô
            foreach (DataGridViewColumn col in dgvMyBoard.Columns) col.Width = 40;
            foreach (DataGridViewColumn col in dgvTarget.Columns) col.Width = 40;

            dgvMyBoard.RowTemplate.Height = 30;
            dgvTarget.RowTemplate.Height = 30;

            ResetBoards();
        }


        private void ResetBoards()
        {
            foreach (DataGridViewRow r in dgvMyBoard.Rows)
                foreach (DataGridViewCell c in r.Cells)
                    c.Style.BackColor = Color.White;

            foreach (DataGridViewRow r in dgvTarget.Rows)
                foreach (DataGridViewCell c in r.Cells)
                    c.Style.BackColor = Color.White;
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

            // vẽ lại toàn bộ bảng
            ResetBoards();
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
            var color = GetColorByResult(msg.result ?? "MISS");

            if (msg.from == _myRole)
            {
                dgvTarget.Rows[msg.y].Cells[msg.x].Style.BackColor = color;
            }
            else
            {
                dgvMyBoard.Rows[msg.y].Cells[msg.x].Style.BackColor = color;
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

        //private async void btnPlaceShips_Click(object sender, EventArgs e)
        //{
        //    if (_stream == null) return;

        //    _myShips = new List<Ship>()
        //    {
        //        new Ship(5,0,0,"H"),
        //        new Ship(4,2,2,"V"),
        //        new Ship(3,5,5,"H"),
        //        new Ship(3,7,1,"V"),
        //        new Ship(2,8,3,"H")
        //    };

        //    await SendAsync(new { type = "PLACE_SHIPS", ships = _myShips });

        //    DrawMyShips();
        //    btnPlaceShips.Enabled = false;
        //    lblStatus.Text = "Status: ships placed, waiting for enemy";
        //}

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
