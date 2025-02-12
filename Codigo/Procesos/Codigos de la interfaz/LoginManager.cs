using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField nombreInput;
    public TMP_InputField contrasenaInput;
    public TMP_Text mensajeLoginText;
    public TMP_Text nombreJugadorText; // Para mostrar el nombre del jugador logueado
    public TMP_Text nombreJugadorText2; // El nuevo TMP_Text donde también mostraremos el nombre
    public GameObject botonJugar; // El botón de jugar que se activará después del login

    public static string jugadorLogueadoNombre; // Variable estática para el nombre del jugador
    public GameObject canvasLogin; // El canvas de login
    public GameObject canvasJuego1; // El primer canvas del juego que se activará
    public GameObject canvasJuego2; // El segundo canvas del juego que se activará

    private string serverURL = "http://localhost:3000"; 

    public void OnLoginButton()
    {
        if (string.IsNullOrEmpty(nombreInput.text) || string.IsNullOrEmpty(contrasenaInput.text))
        {
            mensajeLoginText.text = "❌ Todos los campos deben ser llenados.";
            return;
        }

        StartCoroutine(LoginUsuario(nombreInput.text, contrasenaInput.text));
    }

    IEnumerator LoginUsuario(string nombre, string contrasena)
    {
        string jsonData = "{\"nombre\": \"" + nombre + "\", \"contraseña\": \"" + contrasena + "\"}";
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(serverURL + "/login", "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
            jugadorLogueadoNombre = response.usuario.nombre; // Guardamos el nombre del jugador logueado
            nombreJugadorText.text = "Bienvenido, " + jugadorLogueadoNombre;
            nombreJugadorText2.text = "Jugador: " + jugadorLogueadoNombre; // Actualizamos el segundo TMP_Text
            mensajeLoginText.text = "✅ Login exitoso!";

            // Activamos el botón de Jugar
            botonJugar.SetActive(true); 
        }
        else
        {
            mensajeLoginText.text = "❌ " + request.downloadHandler.text;
            botonJugar.SetActive(false); // Desactivamos el botón de Jugar si el login falla
        }
    }

    public void OnJugarButton()
    {
        // Cerrar el canvas de login
        canvasLogin.SetActive(false);

        // Activar los dos canvas necesarios para el juego
        canvasJuego1.SetActive(true);
        canvasJuego2.SetActive(true);
    }

    [System.Serializable]
    public class LoginResponse
    {
        public bool success;
        public string message;
        public Usuario usuario;
    }

    [System.Serializable]
    public class Usuario
    {
        public string nombre;
        public string contraseña;
    }
}
