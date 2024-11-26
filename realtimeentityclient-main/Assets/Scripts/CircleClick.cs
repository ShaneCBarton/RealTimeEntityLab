using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CircleClick : MonoBehaviour
{
    void Start()
    {
        
    }
    void Update()
    {
        
    }
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
    }
}
