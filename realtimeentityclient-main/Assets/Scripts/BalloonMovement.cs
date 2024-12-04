using UnityEngine;

public class BalloonMovement : MonoBehaviour
{
    private float xPercent;
    private float yPercent;
    private float velocityX;
    private float velocityY;
    private Vector3 targetPosition;
    private float interpolationSpeed = 10f;

    public void Initialize(float x, float y, float vx, float vy)
    {
        xPercent = x;
        yPercent = y;
        velocityX = vx;
        velocityY = vy;
        UpdateWorldPosition();
    }

    public void UpdateServerPosition(float newXPercent, float newYPercent)
    {
        xPercent = newXPercent;
        yPercent = newYPercent;
        UpdateWorldPosition();
    }

    private void UpdateWorldPosition()
    {
        Vector2 screenPosition = new Vector2(xPercent * Screen.width, yPercent * Screen.height);
        targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0));
        targetPosition.z = 0;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * interpolationSpeed);
    }
}