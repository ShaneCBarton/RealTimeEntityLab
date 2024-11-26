using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    private Sprite circleTexture;
    private Dictionary<string, GameObject> activeBalloons = new Dictionary<string, GameObject>();

    void Start()
    {
        NetworkClientProcessing.SetGameLogic(this);
        circleTexture = Resources.Load<Sprite>("Circle");
    }

    public void SpawnNewBalloon(float xPercent, float yPercent)
    {
        Vector2 screenPosition = new Vector2(xPercent * Screen.width, yPercent * Screen.height);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0));
        worldPosition.z = 0;

        GameObject balloon = new GameObject("Balloon");

        SpriteRenderer spriteRenderer = balloon.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = circleTexture;

        CircleCollider2D circleCollider = balloon.AddComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;

        CircleClick clickHandler = balloon.AddComponent<CircleClick>();

        balloon.transform.position = worldPosition;

        string balloonKey = $"{xPercent:F4},{yPercent:F4}";
        activeBalloons[balloonKey] = balloon;

        Debug.Log($"Client spawned balloon at screen position: {screenPosition}, world position: {worldPosition}");
    }

    public void HandleBalloonPopped(float xPercent, float yPercent)
    {
        string balloonKey = $"{xPercent:F4},{yPercent:F4}";

        if (activeBalloons.TryGetValue(balloonKey, out GameObject balloon))
        {
            Destroy(balloon);
            activeBalloons.Remove(balloonKey);
            Debug.Log($"Balloon popped and removed at position {balloonKey}");
        }
    }
}