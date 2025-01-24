const express = require("express");
const mongoose = require("mongoose");
const cors = require("cors"); 

const app = express();
const PORT = 3000;

// Middleware
app.use(express.json()); // Permitir JSON en las peticiones
app.use(cors()); // Habilitar CORS

// Conexión a MongoDB Atlas
//require("dotenv").config(); // Importar dotenv
//const MONGO_URI = process.env.MONGO_URI;

const MONGO_URI = "mongodb+srv://jsalazarq47:JSQ47@datosusuario.ansre.mongodb.net/?retryWrites=true&w=majority&appName=DatosUsuario";

mongoose.connect(MONGO_URI)
  .then(() => console.log("✅ Conectado a MongoDB Atlas"))
  .catch((error) => console.error("❌ Error al conectar con MongoDB:", error));

// Modelo de usuario
const UsuarioSchema = new mongoose.Schema({
  nombre: { type: String, required: true, unique: true },
  password: { type: String, required: true },
  puntajeMaximo: { type: Number, default: 0 }
});
const Usuario = mongoose.model("Usuario", UsuarioSchema);
app.get("/", (req, res) => {
    res.send("Servidor funcionando correctamente 🚀");
  });
// 📌 Ruta para LOGIN y REGISTRO AUTOMÁTICO
app.post("/login", async (req, res) => {
  const { nombre, password } = req.body;
  try {
    let usuario = await Usuario.findOne({ nombre });
    if (!usuario) {
      usuario = new Usuario({ nombre, password, puntajeMaximo: 0 });
      await usuario.save();
      return res.json({ success: true, message: "Usuario creado", usuario });
    }
    if (usuario.password !== password) {
      return res.status(401).json({ success: false, message: "Contraseña incorrecta" });
    }
    return res.json({ success: true, message: "Login exitoso", usuario });
  } catch (error) {
    return res.status(500).json({ success: false, message: "Error en el servidor", error });
  }
});

// 📌 Ruta para ACTUALIZAR el puntaje máximo
app.post("/actualizar-puntaje", async (req, res) => {
  const { nombre, nuevoPuntaje } = req.body;
  try {
    let usuario = await Usuario.findOne({ nombre });
    if (!usuario) return res.status(404).json({ success: false, message: "Usuario no encontrado" });
    if (nuevoPuntaje > usuario.puntajeMaximo) {
      usuario.puntajeMaximo = nuevoPuntaje;
      await usuario.save();
    }
    return res.json({ success: true, message: "Puntaje actualizado", usuario });
  } catch (error) {
    return res.status(500).json({ success: false, message: "Error en el servidor", error });
  }
});

// 📌 Iniciar servidor
app.listen(PORT, () => {
  console.log(`🚀 Servidor corriendo en http://localhost:${PORT}`);
});
