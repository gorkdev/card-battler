using UnityEngine;

namespace CardBattler
{
    // RESPONSIVE kamera: verilen yatay dünya genişliği her ekran oranında
    // görünür kalsın diye orthographic size'ı otomatik ayarlar.
    // Dar (dikey) ekranlarda uzaklaşır, geniş ekranlarda yaklaşır.
    [RequireComponent(typeof(Camera))]
    [ExecuteAlways]
    public class CameraFitter : MonoBehaviour
    {
        [Tooltip("Her zaman görünür kalması gereken yatay dünya genişliği.")]
        [SerializeField] float targetWidth = 12f;

        Camera cam;

        void OnEnable() => cam = GetComponent<Camera>();

        void Update()
        {
            if (cam == null || !cam.orthographic) return;
            float aspect = (float)Screen.width / Mathf.Max(1, Screen.height);
            // orthographicSize = yarı yükseklik; yarı genişlik = size * aspect.
            cam.orthographicSize = (targetWidth * 0.5f) / Mathf.Max(0.0001f, aspect);
        }
    }
}
