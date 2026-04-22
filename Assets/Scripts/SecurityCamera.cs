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

    private float currentAngle = 0f;
    private float direction = 1f;
    private LineRenderer leftLine;
    private LineRenderer rightLine;

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
        GameObject leftObj = new GameObject("LeftVisionLine");
        leftObj.transform.parent = transform;
        leftObj.transform.localPosition = Vector3.zero;
        leftLine = leftObj.AddComponent<LineRenderer>();

        GameObject rightObj = new GameObject("RightVisionLine");
        rightObj.transform.parent = transform;
        rightObj.transform.localPosition = Vector3.zero;
        rightLine = rightObj.AddComponent<LineRenderer>();

        SetupLineRenderer(leftLine);
        SetupLineRenderer(rightLine);
    }

    private void SetupLineRenderer(LineRenderer line)
    {
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = visionColor;
        line.endColor = visionColor;
        line.positionCount = 2;
    }

    private void UpdateVisionLines()
    {
        if (leftLine != null)
        {
            Vector3 leftDir = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
            RaycastHit hit;
            float distance = viewRange;

            if (Physics.Raycast(transform.position, leftDir, out hit, viewRange, obstacleMask))
            {
                distance = hit.distance;
            }

            leftLine.SetPosition(0, transform.position);
            leftLine.SetPosition(1, transform.position + leftDir * distance);
        }

        if (rightLine != null)
        {
            Vector3 rightDir = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;
            RaycastHit hit;
            float distance = viewRange;

            if (Physics.Raycast(transform.position, rightDir, out hit, viewRange, obstacleMask))
            {
                distance = hit.distance;
            }

            rightLine.SetPosition(0, transform.position);
            rightLine.SetPosition(1, transform.position + rightDir * distance);
        }
    }

    protected override void OnFullDetection()
    {
        GameOverManager.Instance.GameOver();
    }

    void OnDestroy()
    {
        if (leftLine != null) Destroy(leftLine.gameObject);
        if (rightLine != null) Destroy(rightLine.gameObject);
    }
}