using System.Net.Sockets;
using System.Text.Json;
using System.Text;

public class Message
{
    public string? type { get; set; }
    public string? roomId { get; set; }
    public string? message { get; set; }
    public List<Ship>? ships { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public string? from { get; set; }
    public string? status { get; set; }
    public string? winner { get; set; }
    public string? turn { get; set; }
    public string? role { get; set; }
}


public class Ship
{
    public int len { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public string dir { get; set; } = "H"; // H hoặc V
}

public class PlayerConn
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public TcpClient Client { get; set; }
    public NetworkStream Stream => Client.GetStream();
    public bool ShipsPlaced { get; set; } = false;
    public Board? Board { get; set; }

    public PlayerConn(TcpClient c)
    {
        Client = c;
    }

    public async Task SendAsync(object obj)
    {
        string json = JsonSerializer.Serialize(obj);
        var data = Encoding.UTF8.GetBytes(json + "\n");
        await Stream.WriteAsync(data, 0, data.Length);
    }
}

public class GameRoom
{
    public string RoomId { get; set; }
    public PlayerConn? Player1 { get; set; }
    public PlayerConn? Player2 { get; set; }
    public string CurrentTurn { get; set; } = "P1";
    public bool IsPlaying { get; set; } = false;

    public GameRoom(string id)
    {
        RoomId = id;
    }

    public IEnumerable<PlayerConn> Players()
    {
        if (Player1 != null) yield return Player1;
        if (Player2 != null) yield return Player2;
    }

    public PlayerConn? GetOpponent(PlayerConn p)
    {
        if (Player1 == null || Player2 == null) return null;
        return p == Player1 ? Player2 : Player1;
    }

    public void RemovePlayer(PlayerConn p)
{
    if (Player1 == p) Player1 = null;
    else if (Player2 == p) Player2 = null;
    IsPlaying = false;
}

    public bool IsEmpty => Player1 == null && Player2 == null;
}

public class Board
{
    public const int Size = 10;
    public int[,] cells = new int[Size, Size]; // 0 empty, 1 ship, 2 hit, 3 miss
    public List<Ship> ships = new();

    public bool PlaceShips(List<Ship> shipsToPlace)
    {
        // TODO: kiểm tra đúng bộ 5,4,3,3,2 nếu muốn
        // reset
        cells = new int[Size, Size];
        ships.Clear();

        foreach (var s in shipsToPlace)
        {
            if (!PlaceShip(s)) return false;
        }
        ships.AddRange(shipsToPlace);
        return true;
    }

    private bool PlaceShip(Ship s)
    {
        int dx = s.dir == "H" ? 1 : 0;
        int dy = s.dir == "V" ? 1 : 0;

        int endX = s.x + dx * (s.len - 1);
        int endY = s.y + dy * (s.len - 1);

        if (endX < 0 || endX >= Size || endY < 0 || endY >= Size)
            return false;

        // check overlap
        int cx = s.x;
        int cy = s.y;
        for (int i = 0; i < s.len; i++)
        {
            if (cells[cy, cx] != 0) return false;
            cx += dx;
            cy += dy;
        }

        // place
        cx = s.x; cy = s.y;
        for (int i = 0; i < s.len; i++)
        {
            cells[cy, cx] = 1;
            cx += dx;
            cy += dy;
        }

        return true;
    }

    public (string result, Ship? sunkShip) Fire(int x, int y)
    {
        if (cells[y, x] == 2 || cells[y, x] == 3)
            return ("MISS", null); // đã bắn rồi, coi như miss

        if (cells[y, x] == 1)
        {
            cells[y, x] = 2; // hit
            // kiểm tra tàu nào bị chìm
            foreach (var ship in ships)
            {
                if (IsShipSunk(ship))
                    return ("SUNK", ship);
            }
            return ("HIT", null);
        }
        else
        {
            cells[y, x] = 3; // miss
            return ("MISS", null);
        }
    }

    private bool IsShipSunk(Ship s)
    {
        int dx = s.dir == "H" ? 1 : 0;
        int dy = s.dir == "V" ? 1 : 0;

        int cx = s.x, cy = s.y;
        for (int i = 0; i < s.len; i++)
        {
            if (cells[cy, cx] != 2) return false;
            cx += dx;
            cy += dy;
        }
        return true;
    }

    public bool AreAllShipsSunk()
    {
        foreach (var ship in ships)
        {
            if (!IsShipSunk(ship)) return false;
        }
        return true;
    }
}
