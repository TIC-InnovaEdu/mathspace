using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class RegistroManager : MonoBehaviour
{
    public TMP_InputField correoInput;
    public TMP_InputField nombreInput;
    public TMP_InputField contrasenaInput;
    public TMP_Text estadoRegistroText; // Para mostrar el estado del registro

    private string serverURL = "http://localhost:3000"; // Cambia según tu servidor

    public void OnRegistroButton()
    {
        // Verificar si los campos no están vacíos
        if (string.IsNullOrEmpty(correoInput.text) || string.IsNullOrEmpty(nombreInput.text) || string.IsNullOrEmpty(contrasenaInput.text))
        {
            estadoRegistroText.text = "❌ Todos los campos deben ser llenados.";
            return;
        }

        StartCoroutine(RegistrarUsuario(correoInput.text, nombreInput.text, contrasenaInput.text));
    }

    IEnumerator RegistrarUsuario(string correo, string nombre, string contrasena)
    {
        string jsonData = "{\"correo\": \"" + correo + "\", \"nombre\": \"" + nombre + "\", \"contraseña\": \"" + contrasena + "\"}";
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(serverURL + "/registro", "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            estadoRegistroText.text = "✅ Registro exitoso!";
        }
        else
        {
            estadoRegistroText.text = "❌ " + request.downloadHandler.text; // Muestra el mensaje de error
        }
    }
}
