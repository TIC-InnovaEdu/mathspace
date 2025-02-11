const express = require("express");
const http = require("http");
const mongoose = require("mongoose");
const cors = require("cors");

const app = express();
const server = http.createServer(app);

// Middleware
app.use(cors());
app.use(express.json());

// Conectar con MongoDB
const MONGO_URI = "mongodb+srv://jsalazarq47:JSQ47@datosusuario.ansre.mongodb.net/?retryWrites=true&w=majority&appName=DatosUsuario";
mongoose.connect(MONGO_URI, { useNewUrlParser: true, useUnifiedTopology: true })
  .then(() => console.log("✅ Conectado a MongoDB"))
  .catch((error) => console.log("❌ Error al conectar con MongoDB:", error));

// Definir esquema y modelo de usuario
const usuarioSchema = new mongoose.Schema({
  correo: String,
  nombre: String,
  contraseña: String,
  puntajeMaximo: { type: Number, default: 0 }
});
const Usuario = mongoose.model("Usuario", usuarioSchema);

// 🔹 RUTA DE REGISTRO
app.post("/registro", async (req, res) => {
  const { correo, nombre, contraseña } = req.body;
  try {
    // Verificar si el usuario ya existe
    let usuario = await Usuario.findOne({ nombre });
    if (usuario) {
      return res.status(400).json({ success: false, message: "El nombre de usuario ya está registrado" });
    }

    // Crear nuevo usuario
    usuario = new Usuario({ nombre, correo, contraseña });
    await usuario.save();
    
    res.json({ success: true, message: "Registro exitoso", usuario });
  } catch (error) {
    res.status(500).json({ success: false, message: "Error en el servidor", error });
  }
});

// 🔹 RUTA DE LOGIN
app.post("/login", async (req, res) => {
  const { nombre, contraseña } = req.body;
  try {
    let usuario = await Usuario.findOne({ nombre });
    if (!usuario) {
      return res.status(404).json({ success: false, message: "Usuario no encontrado" });
    }
    if (usuario.contraseña !== contraseña) {
      return res.status(401).json({ success: false, message: "Contraseña incorrecta" });
    }
    res.json({ success: true, message: "Login exitoso", usuario });
  } catch (error) {
    res.status(500).json({ success: false, message: "Error en el servidor", error });
  }
});

// 🔹 RUTA PARA ACTUALIZAR EL PUNTAJE MÁXIMO
app.post("/actualizar-puntaje", async (req, res) => {
  const { nombre, nuevoPuntaje } = req.body;
  try {
    let usuario = await Usuario.findOne({ nombre });
    if (!usuario) return res.status(404).json({ success: false, message: "Usuario no encontrado" });

    if (nuevoPuntaje > usuario.puntajeMaximo) {
      usuario.puntajeMaximo = nuevoPuntaje;
      await usuario.save();
    }
    res.json({ success: true, message: "Puntaje actualizado", usuario });
  } catch (error) {
    res.status(500).json({ success: false, message: "Error en el servidor", error });
  }
});

// Iniciar servidor
const PORT = 3000;
server.listen(PORT, () => console.log(`🚀 Servidor corriendo en http://localhost:${PORT}`));
