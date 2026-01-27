using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;

public class ScreenCapturer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera m_camera;

    [Header("Settings")]
    [SerializeField] private string m_folderPath;
    [SerializeField] private int2 m_captureResolution;
    [SerializeField] private List<GameObject> m_modelsToCaptures;
    
    private void Start()
    {
        StartCoroutine(CaptureSequence());
    }

    private IEnumerator CaptureSequence()
    {
        RenderTexture tempRT = new RenderTexture(m_captureResolution.x, m_captureResolution.y, 24, RenderTextureFormat.ARGB32)
        {
            //antiAliasing = 4
        };
        m_camera.targetTexture = tempRT;

        if (m_modelsToCaptures.Count > 0)
        {
            foreach (GameObject model in m_modelsToCaptures)
            {
                model.SetActive(false);
            }

            foreach (GameObject model in m_modelsToCaptures)
            {
                model.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                Capture(model.name);
                yield return new WaitForSeconds(0.1f);
                model.SetActive(false);
            }
            
            foreach (GameObject model in m_modelsToCaptures)
            {
                model.SetActive(true);
            }
        }
        else
        {
            Capture("Scene");
        }

        m_camera.targetTexture = null;
        Destroy(tempRT);
        
        Debug.Log($"All image captures!");
    }
    
    private void Capture(string targetName)
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = m_camera.targetTexture;

        m_camera.Render();

        Texture2D image = new Texture2D(m_captureResolution.x, m_captureResolution.y, TextureFormat.ARGB32, false, true);
        image.ReadPixels(new Rect(0, 0, m_captureResolution.x, m_captureResolution.y), 0, 0);
        image.Apply();
        RenderTexture.active = currentRT;

        byte[] Bytes = image.EncodeToPNG();
        Destroy(image);

        string folderPath = m_folderPath;
        if (string.IsNullOrWhiteSpace(m_folderPath))
        {
            folderPath = Application.dataPath + "/Captures/";
        }
        else
        {
            if (!folderPath.EndsWith("/"))
            {
                folderPath += "/";
            }
        }
        string path = folderPath + targetName + ".png";
        File.WriteAllBytes(path, Bytes);
        Debug.Log($"Saved image to {path}");
    }
}
