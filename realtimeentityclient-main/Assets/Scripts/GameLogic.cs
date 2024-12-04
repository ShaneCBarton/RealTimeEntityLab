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

    public void SpawnNewBalloon(float xPercent, float yPercent, float velocityX, float velocityY)
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

        BalloonMovement movement = balloon.AddComponent<BalloonMovement>();
        movement.Initialize(xPercent, yPercent, velocityX, velocityY);

        balloon.transform.position = worldPosition;

        string balloonKey = $"{xPercent:F4},{yPercent:F4}";
        activeBalloons[balloonKey] = balloon;
    }

    public void UpdateBalloonPosition(string oldKey, string newKey)
    {
        if (activeBalloons.TryGetValue(oldKey, out GameObject balloon))
        {
            activeBalloons.Remove(oldKey);
            activeBalloons[newKey] = balloon;

            string[] coordinates = newKey.Split(',');
            if (coordinates.Length == 2)
            {
                float xPercent = float.Parse(coordinates[0]);
                float yPercent = float.Parse(coordinates[1]);

                BalloonMovement movement = balloon.GetComponent<BalloonMovement>();
                if (movement != null)
                {
                    movement.UpdateServerPosition(xPercent, yPercent);
                }
            }
        }
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