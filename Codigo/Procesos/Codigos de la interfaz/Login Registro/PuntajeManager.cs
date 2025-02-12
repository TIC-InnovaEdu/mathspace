using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class PuntajeManager : MonoBehaviour
{
    public TMP_Text nombreJugadorText; // Mostrar el nombre del jugador
    public TMP_InputField puntajeInput; // Input para ingresar el puntaje
    public TMP_Text estadoPuntajeText; // Para mostrar el estado del puntaje

    private string serverURL = "http://localhost:3000"; // Cambia según tu servidor

    void Start()
    {
        // Mostrar el nombre del jugador logueado
        nombreJugadorText.text = LoginManager.jugadorLogueadoNombre;
    }

    public void EnviarPuntaje()
    {
        if (string.IsNullOrEmpty(puntajeInput.text))
        {
            estadoPuntajeText.text = "❌ El puntaje no puede estar vacío.";
            return;
        }

        int puntaje;
        if (int.TryParse(puntajeInput.text, out puntaje))
        {
            StartCoroutine(ActualizarPuntaje(LoginManager.jugadorLogueadoNombre, puntaje));
        }
        else
        {
            estadoPuntajeText.text = "❌ El puntaje debe ser un número.";
        }
    }

    IEnumerator ActualizarPuntaje(string nombre, int puntaje)
    {
        string jsonData = "{\"nombre\": \"" + nombre + "\", \"nuevoPuntaje\": " + puntaje + "}";
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(serverURL + "/actualizar-puntaje", "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            estadoPuntajeText.text = "✅ Puntaje actualizado!";
        }
        else
        {
            estadoPuntajeText.text = "❌ " + request.downloadHandler.text;
        }
    }
}
