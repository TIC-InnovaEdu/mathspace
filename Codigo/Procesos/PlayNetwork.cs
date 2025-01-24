using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;

public class PlayerNetwork : MonoBehaviour
{
    private WebSocket websocket;
    public GameObject playerPrefab;
    private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();

    async void Start()
    {
        websocket = new WebSocket("ws://localhost:3000");

        websocket.OnMessage += (bytes) =>
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            PlayerData data = JsonUtility.FromJson<PlayerData>(message);

            // Si recibimos una actualización de jugadores
            if (data.type == "updatePlayers")
            {
                Dictionary<string, PlayerPosition> updatedPlayers = JsonUtility.FromJson<PlayerDictionary>(data.data).players;

                foreach (var id in updatedPlayers.Keys)
                {
                    if (!players.ContainsKey(id)) // Si el jugador no existe, créalo
                    {
                        GameObject newPlayer = Instantiate(playerPrefab, new Vector3(updatedPlayers[id].x, updatedPlayers[id].y, 0), Quaternion.identity);
                        players[id] = newPlayer;
                    }
                    else // Si ya existe, actualiza su posición
                    {
                        players[id].transform.position = new Vector3(updatedPlayers[id].x, updatedPlayers[id].y, 0);
                    }
                }

                // Eliminar jugadores desconectados
                List<string> toRemove = new List<string>();
                foreach (var id in players.Keys)
                {
                    if (!updatedPlayers.ContainsKey(id))
                        toRemove.Add(id);
                }
                foreach (var id in toRemove)
                {
                    Destroy(players[id]);
                    players.Remove(id);
                }
            }
        };

        await websocket.Connect();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W)) MovePlayer(0, 0.1f);
        if (Input.GetKey(KeyCode.S)) MovePlayer(0, -0.1f);
        if (Input.GetKey(KeyCode.A)) MovePlayer(-0.1f, 0);
        if (Input.GetKey(KeyCode.D)) MovePlayer(0.1f, 0);
    }

    async void MovePlayer(float dx, float dy)
    {
        Vector3 pos = transform.position;
        pos.x += dx;
        pos.y += dy;
        transform.position = pos;

        if (websocket.State == WebSocketState.Open)
        {
            PlayerPosition newPos = new PlayerPosition { x = pos.x, y = pos.y };
            string json = JsonUtility.ToJson(new { type = "move", data = newPos });
            await websocket.SendText(json);
        }
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }
}

// Estructuras para manejar JSON
[System.Serializable]
public class PlayerData
{
    public string type;
    public string data;
}

[System.Serializable]
public class PlayerPosition
{
    public float x;
    public float y;
}

[System.Serializable]
public class PlayerDictionary
{
    public Dictionary<string, PlayerPosition> players;
}
