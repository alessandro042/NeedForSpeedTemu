using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DefaultExecutionOrder(-1000)]
public class UIDiagnosticsRunner : MonoBehaviour
{
    [Tooltip("Si está activo, el script desactivará raycastTarget en Images que sean posibles bloqueadores (útil para probar).")]
    public bool disableBlockingImages;

    void Awake()
    {
        
#if UNITY_2023_2_OR_NEWER
        if (UnityEngine.Object.FindFirstObjectByType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
            Debug.Log("UIDiagnosticsRunner: Se ha creado un EventSystem (StandaloneInputModule). Si usas el nuevo Input System, añade el InputSystemUIInputModule manualmente.");
        }
#else
        if (FindObjectOfType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
            Debug.Log("UIDiagnosticsRunner: Se ha creado un EventSystem (StandaloneInputModule). Si usas el nuevo Input System, añade el InputSystemUIInputModule manualmente.");
        }
#endif

        
#if UNITY_2023_2_OR_NEWER
        var buttons = UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None);
#else
        var buttons = FindObjectsOfType<Button>(true);
#endif
        Debug.Log($"UIDiagnosticsRunner: Se encontraron {buttons.Length} botones en la escena.");
        foreach (var b in buttons)
        {
            if (b == null) continue;
            if (b.gameObject.GetComponent<UIButtonLogger>() == null)
            {
                b.gameObject.AddComponent<UIButtonLogger>();
            }
            Debug.Log($"Button: '{b.gameObject.name}' active={b.gameObject.activeInHierarchy} interactable={b.interactable}");
        }

        
#if UNITY_2023_2_OR_NEWER
        var imagesAll = UnityEngine.Object.FindObjectsByType<Image>(FindObjectsSortMode.None);
        var images = imagesAll.Where(i => i != null && i.raycastTarget).ToArray();
#else
        var images = FindObjectsOfType<Image>(true).Where(i => i.raycastTarget).ToArray();
#endif
        Debug.Log($"UIDiagnosticsRunner: Se encontraron {images.Length} Image(s) con raycastTarget=true (posibles bloqueadores).");
        foreach (var img in images)
        {
            if (img == null) continue;
            Debug.Log($"Image bloqueadora: '{img.gameObject.name}' en '{GetFullPath(img.transform)}' - enabled={img.enabled} color.a={img.color.a}");
            if (disableBlockingImages)
            {
                img.raycastTarget = false;
                Debug.Log($"UIDiagnosticsRunner: Se ha desactivado raycastTarget en '{img.gameObject.name}' para probar.");
            }
        }
    }

    string GetFullPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }
}
