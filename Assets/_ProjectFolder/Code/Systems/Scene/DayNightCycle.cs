using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Tiempo")]
    [Range(0f, 1f)]
    public float timeOfDay = 0f;

    public float dayDuration = 300f;

    [Header("Sol")]
    public Transform sun;

    public Vector2 startPosition;
    public Vector2 endPosition;

    public float arcHeight = 5f;
    public Camera mainCamera;

    private void Update()
    {
        UpdateTime();
        UpdateSunPosition();
    }

    private void UpdateTime()
    {
        timeOfDay += Time.deltaTime / dayDuration;

        if (timeOfDay >= 1f)
        {
            timeOfDay = 0f;

            EndDay();
        }
    }

    private void UpdateSunPosition()
    {
        float cameraHeight = mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        float left = mainCamera.transform.position.x - cameraWidth;
        float right = mainCamera.transform.position.x + cameraWidth;

        float x = Mathf.Lerp(left, right, timeOfDay);

        float arc = Mathf.Sin(
            timeOfDay * Mathf.PI
        ) * arcHeight;

        float y = mainCamera.transform.position.y + arc;

        sun.position = new Vector3(
            x,
            y,
            sun.position.z
        );
    }

    private void EndDay()
    {
        Debug.Log("El día terminó");
    }
}