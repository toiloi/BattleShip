using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

var listener = new TcpListener(IPAddress.Any, 5000);
listener.Start();
Console.WriteLine("Battleship Server started on port 5000");

var roomManager = new RoomManager();

while (true)
{
    var client = await listener.AcceptTcpClientAsync();
    Console.WriteLine("Client connected");
    _ = HandleClient(client, roomManager);
}

static async Task HandleClient(TcpClient client, RoomManager roomManager)
{
    var player = new PlayerConn(client);
    GameRoom? currentRoom = null;

    using var reader = new StreamReader(player.Stream, Encoding.UTF8);
    try
    {
        while (true)
        {
            var line = await reader.ReadLineAsync();
            if (line == null) break;
            Console.WriteLine("Received: " + line);

            var msg = JsonSerializer.Deserialize<Message>(line);
            if (msg == null) continue;

            switch (msg.type)
            {
                case "CREATE_ROOM":
                    {
                        var room = roomManager.CreateRoom();
                        currentRoom = room;
                        room.Player1 = player;
                        await player.SendAsync(new { type = "ROOM_CREATED", roomId = room.RoomId, role = "P1" });
                        await BroadcastRoomStatus(room);
                        break;
                    }


                case "JOIN_ROOM":
                    {
                        if (msg.roomId == null) break;
                        var room = roomManager.GetRoom(msg.roomId);
                        if (room == null || room.Player2 != null)
                        {
                            await player.SendAsync(new { type = "ERROR", message = "Room not found or full" });
                        }
                        else
                        {
                            currentRoom = room;
                            room.Player2 = player;
                            await player.SendAsync(new { type = "JOIN_OK", roomId = room.RoomId, players = 2, role = "P2" });
                            await BroadcastRoomStatus(room);
                            await Broadcast(room, new { type = "PLACE_SHIPS_REQUEST" });
                        }
                        break;
                    }


                case "PLACE_SHIPS":
                    {
                        if (currentRoom == null) break;
                        if (msg.ships == null) break;

                        var board = new Board();
                        bool ok = board.PlaceShips(msg.ships);
                        if (!ok)
                        {
                            await player.SendAsync(new { type = "PLACE_SHIPS_INVALID", reason = "invalid placement" });
                            break;
                        }
                        player.Board = board;
                        player.ShipsPlaced = true;
                        await player.SendAsync(new { type = "PLACE_SHIPS_OK" });

                        // nếu cả 2 đã đặt
                        if (currentRoom.Player1?.ShipsPlaced == true &&
                            currentRoom.Player2?.ShipsPlaced == true)
                        {
                            currentRoom.IsPlaying = true;
                            currentRoom.CurrentTurn = "P1";
                            await Broadcast(currentRoom, new { type = "GAME_START", turn = "P1" });
                        }
                        break;
                    }

                case "FIRE":
                    {
                        if (currentRoom == null) break;
                        if (!currentRoom.IsPlaying) break;

                        // xác định người bắn là P1 hay P2
                        string who = player == currentRoom.Player1 ? "P1" : "P2";
                        if (who != currentRoom.CurrentTurn)
                        {
                            await player.SendAsync(new { type = "ERROR", message = "Not your turn" });
                            break;
                        }

                        var opp = currentRoom.GetOpponent(player);
                        if (opp?.Board == null) break;

                        var (result, sunkShip) = opp.Board.Fire(msg.x, msg.y);

                        // kiểm tra kết thúc
                        bool gameOver = opp.Board.AreAllShipsSunk();

                        string nextTurn = who == "P1" ? "P2" : "P1";
                        currentRoom.CurrentTurn = nextTurn;

                        await Broadcast(currentRoom, new
                        {
                            type = "FIRE_RESULT",
                            from = who,
                            x = msg.x,
                            y = msg.y,
                            result = result,
                            shipLen = sunkShip?.len,
                            nextTurn = gameOver ? "" : nextTurn
                        });

                        if (gameOver)
                        {
                            currentRoom.IsPlaying = false;
                            await Broadcast(currentRoom, new
                            {
                                type = "GAME_OVER",
                                winner = who,
                                reason = "all_ships_sunk"
                            });
                        }

                        break;
                    }
                case "LEAVE_ROOM":
                    {
                        if (currentRoom != null)
                        {
                            try { await Broadcast(currentRoom, new { type = "OPP_LEFT", roomId = currentRoom.RoomId }); } catch { }
                            currentRoom.RemovePlayer(player);
                            await player.SendAsync(new { type = "LEFT_OK", roomId = currentRoom.RoomId });
                            await BroadcastRoomStatus(currentRoom);
                            currentRoom = null;
                        }
                        break;
                    }
                
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Client error: " + ex.Message);
    }
    finally
    {
        if (currentRoom != null)
        {
            try { await Broadcast(currentRoom, new { type = "OPP_LEFT", roomId = currentRoom.RoomId }); } catch { }
            currentRoom.RemovePlayer(player);
            await BroadcastRoomStatus(currentRoom);
            currentRoom = null;
        }
        try { client.Close(); } catch { }
    }
}

static async Task Broadcast(GameRoom room, object obj)
{
    foreach (var p in room.Players())
    {
        await p.SendAsync(obj);
    }
}

static async Task BroadcastRoomStatus(GameRoom room)
{
    string status = (room.Player1 != null && room.Player2 != null) ? "PLAYING" : "WAITING";
    await Broadcast(room, new { type = "ROOM_STATUS", roomId = room.RoomId, status = status });
}
