const http = require("http");
const { Server } = require("socket.io");
const express = require("express");
const cors = require("cors");

const app = express();
const server = http.createServer(app);
const io = new Server(server, {
    cors: {
        origin: "*",
        methods: ["GET", "POST"]
    }
});

app.use(cors());
app.use(express.json());

let players = {}; // Objeto para guardar jugadores conectados

io.on("connection", (socket) => {
    console.log("🔗 Nuevo jugador conectado:", socket.id);

    // Crear un nuevo jugador cuando se conecte
    players[socket.id] = { x: 0, y: 0 }; // Posición inicial

    // Enviar la lista actualizada de jugadores a todos
    io.emit("updatePlayers", players);

    // Cuando un jugador se mueve, actualizamos su posición
    socket.on("move", (data) => {
        if (players[socket.id]) {
            players[socket.id].x = data.x;
            players[socket.id].y = data.y;
            io.emit("updatePlayers", players); // Enviar actualización a todos
        }
    });

    // Cuando un jugador se desconecta, lo eliminamos
    socket.on("disconnect", () => {
        console.log("❌ Jugador desconectado:", socket.id);
        delete players[socket.id];
        io.emit("updatePlayers", players);
    });
});

server.listen(3000, () => {
    console.log("🚀 Servidor WebSocket corriendo en http://localhost:3000");
});
