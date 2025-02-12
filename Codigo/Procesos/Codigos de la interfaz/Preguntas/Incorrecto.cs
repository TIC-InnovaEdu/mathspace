using UnityEngine;
using TMPro;
using System.Collections;
using System.Net;
using UnityEngine.Networking;
using System.IO;

public class Incorrecto : MonoBehaviour
{
    public TextMeshPro textoTMP;
    private string[] palabras;
    private string googleSheetsURL = "https://docs.google.com/spreadsheets/d/e/2PACX-1vTXOPRQ-7KFBaq52zq4cake-ug4i2xaHCX390nRo5TUSNcepTs3uG8hVohfOzrT6k4GDpLGMJp2noC3/pub?gid=0&single=true&output=csv";
    
    void Start()
    {
        StartCoroutine(DescargarPalabras());
    }

    IEnumerator DescargarPalabras()
    {
        UnityWebRequest request = UnityWebRequest.Get(googleSheetsURL);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ProcesarCSV(request.downloadHandler.text);
            SeleccionarPalabraAleatoria();
        }
        else
        {
            Debug.LogError("Error al descargar el archivo CSV: " + request.error);
        }
    }

    void ProcesarCSV(string csv)
    {
        palabras = csv.Split('\n'); // Dividir las palabras por líneas
        Debug.Log("Palabras cargadas correctamente: " + palabras.Length);
    }

    void SeleccionarPalabraAleatoria()
    {
        if (palabras != null && palabras.Length > 0)
        {
            string palabraSeleccionada = palabras[Random.Range(0, palabras.Length)].Trim();
            textoTMP.text = palabraSeleccionada;
            Debug.Log($"Palabra seleccionada: {palabraSeleccionada}");
        }
        else
        {
            Debug.LogError("No hay palabras en la lista.");
        }
    }
}
