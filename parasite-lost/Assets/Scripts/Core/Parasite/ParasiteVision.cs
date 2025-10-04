using UnityEngine;
using UnityEngine.UI;
using ParasiteLost.Core.Player;

[RequireComponent(typeof(Canvas))]
public class ParasiteVision : MonoBehaviour
{
    [Tooltip("Reference to the parasite controller. If empty will find first ParasiteController in scene.")]
    public ParasiteController parasite;

    [Tooltip("Alternative: assign a Transform (e.g., the parasite GameObject) if you prefer.")]
    public Transform targetTransform;

    [Tooltip("Radius of visible area in world units.")]
    public float radius = 3f;

    [Range(0.001f, 1f)]
    public float softness = 0.08f;

    [Tooltip("Base darkness color (alpha controls overall darkness).")]
    public Color darkness = new Color(0f, 0f, 0f, 0.95f);

    Canvas uiCanvas;
    RawImage rawImage;
    Material mat;

    Camera mainCam;

    void Awake()
    {
        uiCanvas = GetComponent<Canvas>();
        uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        mainCam = Camera.main;
    }

    void Start()
    {
        if (parasite == null)
        {
#if UNITY_2023_1_OR_NEWER
            parasite = Object.FindFirstObjectByType<ParasiteController>();
#else
            parasite = FindObjectOfType<ParasiteController>();
#endif
        }

        var go = new GameObject("DarknessOverlay", typeof(RectTransform), typeof(RawImage));
        go.transform.SetParent(transform, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;

        rawImage = go.GetComponent<RawImage>();
        rawImage.texture = Texture2D.whiteTexture;

        var shader = Shader.Find("UI/MaskDarkness");
        if (shader == null)
        {
            Debug.LogError("ParasiteVision: Shader 'UI/MaskDarkness' not found.");
            return;
        }

        mat = new Material(shader);
        rawImage.material = mat;
        rawImage.color = Color.white;

        mat.SetColor("_Color", darkness);
        mat.SetFloat("_Softness", softness);
    }

    void Update()
    {
        if (mat == null || mainCam == null) return;

        Vector3 worldPos;
        if (targetTransform != null)
            worldPos = targetTransform.position;
        else if (parasite != null)
            worldPos = parasite.transform.position;
        else
            return;

        Vector3 vp = mainCam.WorldToViewportPoint(worldPos);
        mat.SetVector("_Center", new Vector4(vp.x, vp.y, 0f, 0f));

        if (mainCam.orthographic)
        {
            float vpHeight = mainCam.orthographicSize * 2f;
            float vpWidth = vpHeight * ((float)Screen.width / Screen.height);
            float rVY = radius / vpHeight;
            float rVX = radius / vpWidth;
            mat.SetFloat("_RadiusX", rVX);
            mat.SetFloat("_RadiusY", rVY);
        }
        else
        {
            Vector3 rightPoint = worldPos + mainCam.transform.right * radius;
            Vector3 upPoint = worldPos + mainCam.transform.up * radius;
            Vector3 vpr = mainCam.WorldToViewportPoint(rightPoint);
            Vector3 vpu = mainCam.WorldToViewportPoint(upPoint);
            float rVX = Mathf.Abs(vpr.x - vp.x);
            float rVY = Mathf.Abs(vpu.y - vp.y);
            mat.SetFloat("_RadiusX", rVX);
            mat.SetFloat("_RadiusY", rVY);
        }
    }
}
