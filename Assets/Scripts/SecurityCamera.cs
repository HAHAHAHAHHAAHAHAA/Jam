using UnityEngine;

public class SecurityCamera : EnemyVision
{
    [Header("Camera Rotation")]
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private float leftAngle = -45f;
    [SerializeField] private float rightAngle = 45f;

    [Header("Visualization")]
    [SerializeField] private bool showVisionLines = true;
    [SerializeField] private Color visionColor = Color.red;
    [SerializeField] private int rayCount = 15;

    private float currentAngle = 0f;
    private float direction = 1f;
    private LineRenderer[] visionLines;

    void Start()
    {
        if (showVisionLines)
        {
            CreateVisionLines();
        }
    }

    void FixedUpdate()
    {
        RotateCamera();
        if (showVisionLines)
        {
            UpdateVisionLines();
        }
    }

    private void RotateCamera()
    {
        float step = direction * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, step, 0);

        currentAngle += step;

        if (currentAngle >= rightAngle)
        {
            currentAngle = rightAngle;
            direction = -1f;
        }
        else if (currentAngle <= leftAngle)
        {
            currentAngle = leftAngle;
            direction = 1f;
        }
    }

    private void CreateVisionLines()
    {
        visionLines = new LineRenderer[rayCount];

        for (int i = 0; i < rayCount; i++)
        {
            GameObject lineObj = new GameObject($"VisionLine_{i}");
            lineObj.transform.parent = transform;
            lineObj.transform.localPosition = Vector3.zero;
            visionLines[i] = lineObj.AddComponent<LineRenderer>();
            SetupLineRenderer(visionLines[i]);
        }
    }

    private void SetupLineRenderer(LineRenderer line)
    {
        line.startWidth = 0.02f;
        line.endWidth = 0.02f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = visionColor;
        line.endColor = visionColor;
        line.positionCount = 2;
    }

    private void UpdateVisionLines()
    {
        float halfAngle = viewAngle / 2f;

        for (int i = 0; i < rayCount; i++)
        {
            float t = (float)i / (rayCount - 1);
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;

            RaycastHit hit;
            float distance = viewRange;

            if (Physics.Raycast(transform.position, dir, out hit, viewRange, obstacleMask))
            {
                distance = hit.distance;
            }

            visionLines[i].SetPosition(0, transform.position);
            visionLines[i].SetPosition(1, transform.position + dir * distance);
        }
    }

    protected override void OnFullDetection()
    {
        GameOverManager.Instance.GameOver();
    }

    void OnDestroy()
    {
        if (visionLines != null)
        {
            foreach (var line in visionLines)
            {
                if (line != null) Destroy(line.gameObject);
            }
        }
    }
}