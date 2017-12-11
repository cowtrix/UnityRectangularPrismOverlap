using UnityEngine;

public static class OverlapUtilites2D
{
    /// <summary>
    /// Does the 2D triangle defined by [p1,p2,p3] intersect the given rect?
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="rect"></param>
    /// <returns></returns>
    public static bool TriangleOverlapsRect(Vector2 p1, Vector2 p2, Vector2 p3, Rect rect)
    {
        if (rect.Contains(p1) || rect.Contains(p2) || rect.Contains(p3))
        {
            return true;    // Simple case
        }
        if (TriangleContainsPoint(p1, p2, p3, new Vector2(rect.xMin, rect.yMin)) &&
            TriangleContainsPoint(p1, p2, p3, new Vector2(rect.xMin, rect.yMax)) &&
            TriangleContainsPoint(p1, p2, p3, new Vector2(rect.xMax, rect.yMin)) &&
            TriangleContainsPoint(p1, p2, p3, new Vector2(rect.xMax, rect.yMax)))
        {
            // The triangle completely envelopes the rect
            return true;
        }
        var rectMin = rect.min;
        var rectMax = rect.max;
        if (LineIntersectsRect(p1, p2, rectMin, rectMax) || LineIntersectsRect(p2, p3, rectMin, rectMax) || LineIntersectsRect(p3, p1, rectMin, rectMax))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Does the 2D triangle defined by [p1,p2,p3] contain the given point?
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="testPoint"></param>
    /// <returns></returns>
    public static bool TriangleContainsPoint(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 testPoint)
    {
        var a = 1 / 2f * (-p1.y * p2.x + p0.y * (-p1.x + p2.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y);
        var sign = a < 0 ? -1 : 1;
        var s = (p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * testPoint.x + (p0.x - p2.x) * testPoint.y) * sign;
        var t = (p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * testPoint.x + (p1.x - p0.x) * testPoint.y) * sign;

        return s > 0 && t > 0 && (s + t) < 2 * a * sign;
    }

    /// <summary>
    /// Does the 2D line defined by [p1,p2] contain the rect defined by the given minimum and maximum points?
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="rectMin"></param>
    /// <param name="rectMax"></param>
    /// <returns></returns>
    public static bool LineIntersectsRect(Vector2 p1, Vector2 p2, Vector2 rectMin, Vector2 rectMax)
    {
        var m = (p1.y - p2.y) / (p1.x - p2.x);
        var c = p1.y - (m * p1.x);

        var xMin = rectMin.x;
        var yMin = rectMin.y;
        var xMax = rectMax.x;
        var yMax = rectMax.y;

        float topIntersection, bottomIntersection;
        if (m > 0)
        {
            topIntersection = (m * xMin + c);
            bottomIntersection = (m * xMax + c);
        }
        else
        {
            topIntersection = (m * xMax + c);
            bottomIntersection = (m * xMin + c);
        }

        float topTrianglePoint, bottomTrianglePoint;
        if (p1.y < p2.y)
        {
            topTrianglePoint = p1.y;
            bottomTrianglePoint = p2.y;
        }
        else
        {
            topTrianglePoint = p2.y;
            bottomTrianglePoint = p1.y;
        }

        var topOverlap = topIntersection > topTrianglePoint ? topIntersection : topTrianglePoint;
        var botOverlap = bottomIntersection < bottomTrianglePoint ? bottomIntersection : bottomTrianglePoint;
        return topOverlap < botOverlap && !(botOverlap < yMin || topOverlap > yMax);
    }
}