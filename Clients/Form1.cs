using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleshipClientWin
{
    public partial class Form1 : Form
    {
        private TcpClient? _client;
        private NetworkStream? _stream;
        private string _myRole = "";
        private bool _connected = false;

        public Form1()
        {
            InitializeComponent();
            InitGrids();
        }

        private void InitGrids()
        {
            // tạo 10 cột
            dgvMyBoard.ColumnCount = 10;
            dgvTarget.ColumnCount = 10;
            dgvMyBoard.RowCount = 10;
            dgvTarget.RowCount = 10;

            dgvMyBoard.RowHeadersVisible = false;
            dgvTarget.RowHeadersVisible = false;

            foreach (DataGridViewColumn col in dgvMyBoard.Columns)
                col.Width = 30;
            foreach (DataGridViewColumn col in dgvTarget.Columns)
                col.Width = 30;

            dgvMyBoard.ReadOnly = true;
            dgvTarget.ReadOnly = true;
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

                // start listener
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
                try
                {
                    line = await reader.ReadLineAsync();
                }
                catch
                {
                    break;
                }
                if (line == null) break;

                // parse message
                Message? msg = null;
                try
                {
                    msg = JsonSerializer.Deserialize<Message>(line);
                }
                catch
                {
                    // ignore bad format
                }

                if (msg == null) continue;

                // vì đang ở thread khác → dùng Invoke
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
                    break;

                case "JOIN_OK":
                    _myRole = msg.role ?? "";
                    lblRole.Text = "Role: " + _myRole;
                    break;

                case "PLACE_SHIPS_REQUEST":
                    // server bảo hãy đặt tàu (ở đây ta nhấn nút Place Ships)
                    break;

                case "PLACE_SHIPS_OK":
                    // ok
                    break;

                case "GAME_START":
                    lblTurn.Text = "Turn: " + msg.turn;
                    break;

                case "FIRE_RESULT":
                    // tô lên bảng
                    PaintFireResult(msg);
                    lblTurn.Text = "Turn: " + (string.IsNullOrEmpty(msg.nextTurn) ? "-" : msg.nextTurn);
                    break;

                case "GAME_OVER":
                    MessageBox.Show("Winner: " + msg.winner);
                    break;
            }
        }

        private void PaintFireResult(Message msg)
        {
            // nếu mình bắn
            if (msg.from == _myRole)
            {
                // tô trên bảng mục tiêu
                var cell = dgvTarget.Rows[msg.y].Cells[msg.x];
                cell.Style.BackColor = msg.result == "MISS" ? System.Drawing.Color.LightGray : System.Drawing.Color.LightCoral;
            }
            else
            {
                // đối thủ bắn mình → tô trên bảng của tôi
                var cell = dgvMyBoard.Rows[msg.y].Cells[msg.x];
                cell.Style.BackColor = msg.result == "MISS" ? System.Drawing.Color.LightGray : System.Drawing.Color.LightCoral;
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
            if (!_connected || _stream == null) return;

            // gửi bộ tàu mẫu (đã fix biên)
            await SendAsync(new
            {
                type = "PLACE_SHIPS",
                ships = new[]
                {
                    new { len = 5, x = 0, y = 0, dir = "H" },
                    new { len = 4, x = 2, y = 2, dir = "V" },
                    new { len = 3, x = 5, y = 5, dir = "H" },
                    new { len = 3, x = 7, y = 1, dir = "V" },
                    new { len = 2, x = 8, y = 3, dir = "H" },
                }
            });

            // tô tàu của mình lên bảng trái cho dễ nhìn
            dgvMyBoard.Rows[0].Cells[0].Style.BackColor = System.Drawing.Color.LightBlue;
            dgvMyBoard.Rows[0].Cells[1].Style.BackColor = System.Drawing.Color.LightBlue;
            dgvMyBoard.Rows[0].Cells[2].Style.BackColor = System.Drawing.Color.LightBlue;
            dgvMyBoard.Rows[0].Cells[3].Style.BackColor = System.Drawing.Color.LightBlue;
            dgvMyBoard.Rows[0].Cells[4].Style.BackColor = System.Drawing.Color.LightBlue;
            // ... bạn có thể vẽ tiếp theo ships thật nếu muốn
        }

        private async void dgvTarget_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (!_connected || _stream == null) return;

            // gửi FIRE
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

    // giống bên client console, nhưng đây là bản cho WinForms:
    public class Message
    {
        public string? type { get; set; }
        public string? roomId { get; set; }
        public string? message { get; set; }
        public string? role { get; set; }
        public string? status { get; set; }
        public string? from { get; set; }
        public string? winner { get; set; }
        public string? turn { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public string? result { get; set; }
        public int? shipLen { get; set; }
        public string? nextTurn { get; set; }
    }
}
