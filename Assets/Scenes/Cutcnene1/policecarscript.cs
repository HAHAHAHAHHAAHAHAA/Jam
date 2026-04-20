using UnityEngine;

public class PoliceCarCutscene : MonoBehaviour
{
    [Header("Wheels")]
    [Tooltip("Скорость вращения колёс (градусы в секунду)")]
    public float wheelRotationSpeed = 360f;

    [System.Serializable]
    public class WheelData
    {
        public Transform wheelTransform;
        [Tooltip("Смещение от позиции wheelTransform до центра колеса (локальные координаты)")]
        public Vector3 pivotOffset = Vector3.zero;
    }
    public WheelData[] wheels;

    [Header("Flashing Light")]
    public Renderer lightRenderer;
    public float colorChangeInterval = 0.5f;

    private Material lightMaterial;
    private float timer;
    private bool isRed = true;
    private readonly Color redColor = Color.red;
    private readonly Color blueColor = Color.blue;

    void Start()
    {
        if (lightRenderer != null)
        {
            lightMaterial = lightRenderer.material;
            lightMaterial.EnableKeyword("_EMISSION");
            lightMaterial.SetColor("_EmissionColor", redColor);
        }
        else
            Debug.LogWarning("Light Renderer не назначен!", this);

        timer = colorChangeInterval;
    }

    void Update()
    {
        float angle = wheelRotationSpeed * Time.deltaTime;

        foreach (WheelData wheel in wheels)
        {
            if (wheel.wheelTransform == null) continue;

            // Вычисляем мировую точку центра колеса
            Vector3 centerPoint = wheel.wheelTransform.TransformPoint(wheel.pivotOffset);
            // Вращаем колесо вокруг своей локальной оси X (обычно), но относительно центра
            wheel.wheelTransform.RotateAround(centerPoint, wheel.wheelTransform.right, angle);
        }

        // Переключение цвета мигалки
        if (lightMaterial != null)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                isRed = !isRed;
                lightMaterial.SetColor("_EmissionColor", isRed ? redColor : blueColor);
                timer = colorChangeInterval;
            }
        }
    }
}