using UnityEngine;

public static class MouseAPI
{
    public static bool isMouseInBounds(Vector2 mousePosition, RectTransform obj)
    {
        Vector2 min = GetWorldRect(obj).min;
        Vector2 max = GetWorldRect(obj).max;

        if (!(mousePosition.x > min.x && mousePosition.y > min.y)) return false;
        if (!(mousePosition.x < max.x && mousePosition.y < max.y)) return false;

        return true;
    }

    public static Rect GetWorldRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        // Get the bottom left corner.
        Vector3 position = corners[0];

        Vector2 size = new Vector2(
            rectTransform.lossyScale.x * rectTransform.rect.size.x,
            rectTransform.lossyScale.y * rectTransform.rect.size.y);

        return new Rect(position, size);
    }
}