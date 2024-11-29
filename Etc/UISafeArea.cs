using UnityEngine;

public class UISafeArea : MonoBehaviour
{
    private void Awake()
    {
        var Myrect = this.GetComponent<RectTransform>();

        var minAnchor = Screen.safeArea.min;
        var maxAnchor = Screen.safeArea.max;

        minAnchor.x /= Screen.width;
        minAnchor.y /= Screen.height;

        maxAnchor.x /= Screen.width;
        maxAnchor.y /= Screen.height;

        Myrect.anchorMin = minAnchor;
        Myrect.anchorMax = maxAnchor;
    }
}
