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

        public Form1()
        {
            InitializeComponent();
            InitGrids();
            lblStatus.Text = "Status: waiting for game";
        }

        private void InitGrids()
        {
            dgvMyBoard.ColumnCount = 10;
            dgvTarget.ColumnCount = 10;
            dgvMyBoard.RowCount = 10;
            dgvTarget.RowCount = 10;

            dgvMyBoard.RowHeadersVisible = false;
            dgvTarget.RowHeadersVisible = false;

            // Ẩn header để click trúng ô
            dgvTarget.ColumnHeadersVisible = false;
            dgvTarget.RowHeadersVisible = false;

            dgvTarget.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvTarget.MultiSelect = false;

            foreach (DataGridViewColumn col in dgvMyBoard.Columns) col.Width = 30;
            foreach (DataGridViewColumn col in dgvTarget.Columns) col.Width = 30;

            dgvMyBoard.ReadOnly = true;
            dgvTarget.ReadOnly = true;

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
                int dx = s.dir == "H" ? 1 : 0;
                int dy = s.dir == "V" ? 1 : 0;
                for (int i = 0; i < s.len; i++)
                {
                    int xx = s.x + i * dx;
                    int yy = s.y + i * dy;
                    dgvMyBoard.Rows[yy].Cells[xx].Style.BackColor = Color.LightBlue;
                }
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

        private async void btnPlaceShips_Click(object sender, EventArgs e)
        {
            if (_stream == null) return;

            _myShips = new List<Ship>()
            {
                new Ship(5,0,0,"H"),
                new Ship(4,2,2,"V"),
                new Ship(3,5,5,"H"),
                new Ship(3,7,1,"V"),
                new Ship(2,8,3,"H")
            };

            await SendAsync(new { type = "PLACE_SHIPS", ships = _myShips });

            DrawMyShips();
            btnPlaceShips.Enabled = false;
            lblStatus.Text = "Status: ships placed, waiting for enemy";
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
        }

        private async Task SendAsync(object obj)
        {
            if (_stream == null) return;
            string json = JsonSerializer.Serialize(obj);
            byte[] data = Encoding.UTF8.GetBytes(json + "\n");
            await _stream.WriteAsync(data, 0, data.Length);
        }
    }

    public class Ship
    {
        public int len { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public string dir { get; set; }

        public Ship(int len, int x, int y, string dir)
        {
            this.len = len;
            this.x = x;
            this.y = y;
            this.dir = dir;
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
