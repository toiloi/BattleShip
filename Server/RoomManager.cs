using System.Collections.Concurrent;

public class RoomManager
{
    private ConcurrentDictionary<string, GameRoom> _rooms = new();

    public GameRoom CreateRoom()
    {
        string id = GenerateCode();
        var room = new GameRoom(id);
        _rooms[id] = room;
        return room;
    }

    public GameRoom? GetRoom(string id)
    {
        _rooms.TryGetValue(id, out var room);
        return room;
    }

    private string GenerateCode()
    {
        return Guid.NewGuid().ToString("N")[..6].ToUpper();
    }
}
