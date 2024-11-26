using UnityEngine;

public class CircleClick : MonoBehaviour
{
    void OnMouseDown()
    {
        Vector3 worldPos = transform.position;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        float xPercent = screenPos.x / Screen.width;
        float yPercent = screenPos.y / Screen.height;

        string popMessage = string.Format("{0},{1},{2}",
            ClientToServerSignifiers.BALLOON_POPPED,
            xPercent.ToString("F4"),
            yPercent.ToString("F4"));

        NetworkClientProcessing.SendMessageToServer(popMessage, TransportPipeline.ReliableAndInOrder);

        Debug.Log("Balloon pop requested at position: " + xPercent + ", " + yPercent);
    }
}