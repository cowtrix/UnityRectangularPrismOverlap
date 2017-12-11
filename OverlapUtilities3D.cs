//#define CUSTOMDEBUG

using System;
using UnityEditor;
using UnityEngine;

public static class OverlapUtilities3D
{
    public static bool Contains(this Bounds b, Vector3 point, out Vector3 outPoint)
    {
        outPoint = point;
        return b.Contains(point);
    }

    /// <summary>
    /// Returns a series of triangles that represent a surface of a RotationalBounds (i.e. a rotated rectangular prism)
    /// </summary>
    /// <param name="b"></param>
    /// <param name="result"></param>
    public static void ReduceToTriangles(this RotationalBounds b, Triangle[] result)
    {
        if (result.Length < 12)
        {
            throw new Exception("Result array must be length >= 16");
        }
        Triangle tri1, tri2, tri3, tri4, tri5, tri6, tri7, tri8, tri9, tri10, tri11, tri12;
        b.ReduceToTriangles(out tri1, out tri2, out tri3, out tri4,out tri5,out tri6,out tri7,out tri8,out tri9,out tri10,out tri11,out tri12);
        result[0] = tri1;
        result[1] = tri2;
        result[2] = tri3;
        result[3] = tri4;
        result[4] = tri5;
        result[5] = tri6;
        result[6] = tri7;
        result[7] = tri8;
        result[8] = tri9;
        result[9] = tri10;
        result[10] = tri11;
        result[11] = tri12;
    }

    /// <summary>
    /// Returns a series of triangles that represent a surface of a RotationalBounds (i.e. a rotated rectangular prism)
    /// </summary>
    /// <param name="b"></param>
    /// <param name="result"></param>
    public static void ReduceToTriangles(this RotationalBounds invRotB,
        out Triangle tri1,
        out Triangle tri2,
        out Triangle tri3,
        out Triangle tri4,
        out Triangle tri5,
        out Triangle tri6,
        out Triangle tri7,
        out Triangle tri8,
        out Triangle tri9,
        out Triangle tri10,
        out Triangle tri11,
        out Triangle tri12
    )
    {
        var p1 = invRotB.center + invRotB.rotation * new Vector3(invRotB.extents.x, invRotB.extents.y, invRotB.extents.z);
        var p2 = invRotB.center + invRotB.rotation * new Vector3(invRotB.extents.x, -invRotB.extents.y, invRotB.extents.z);
        var p3 = invRotB.center + invRotB.rotation * new Vector3(invRotB.extents.x, invRotB.extents.y, -invRotB.extents.z);
        var p4 = invRotB.center + invRotB.rotation * new Vector3(invRotB.extents.x, -invRotB.extents.y, -invRotB.extents.z);
        var p5 = invRotB.center + invRotB.rotation * new Vector3(-invRotB.extents.x, invRotB.extents.y, invRotB.extents.z);
        var p6 = invRotB.center + invRotB.rotation * new Vector3(-invRotB.extents.x, -invRotB.extents.y, invRotB.extents.z);
        var p7 = invRotB.center + invRotB.rotation * new Vector3(-invRotB.extents.x, invRotB.extents.y, -invRotB.extents.z);
        var p8 = invRotB.center + invRotB.rotation * new Vector3(-invRotB.extents.x, -invRotB.extents.y, -invRotB.extents.z);
        
        tri1 = new Triangle(p3, p1, p2);
        tri2 = new Triangle(p2, p4, p3);

        tri3 = new Triangle(p7, p3, p4);
        tri4 = new Triangle(p4, p8, p7);

        tri5 = new Triangle(p5, p7, p8);
        tri6 = new Triangle(p8, p6, p5);

        tri7 = new Triangle(p1, p5, p6);
        tri8 = new Triangle(p6, p2, p1);

        tri9 = new Triangle(p1, p3, p7);
        tri10 = new Triangle(p7, p5, p1);

        tri11 = new Triangle(p2, p6, p8);
        tri12 = new Triangle(p8, p4, p2);
    }

    private static readonly Triangle[] _triangleCache = new Triangle[12];

    /// <summary>
    /// Do the two rectangular prisms (defined by a center, size and rotation) overlap?
    /// </summary>
    /// <param name="r1Center"></param>
    /// <param name="r1Extents"></param>
    /// <param name="r1Rot"></param>
    /// <param name="r2Center"></param>
    /// <param name="r2Extents"></param>
    /// <param name="r2Rot"></param>
    /// <returns></returns>
    public static bool RectangularPrismOverlaps(Vector3 r1Center, Vector3 r1Extents, Quaternion r1Rot, Vector3 r2Center, Vector3 r2Extents, Quaternion r2Rot)
    {
        var invRot = Quaternion.Inverse(r1Rot); 
        var invRot2 = Quaternion.Inverse(r2Rot);

        var axisBounds = new Bounds(invRot * r1Center, r1Extents * 2);
        var invRotB = new RotationalBounds(axisBounds.center + invRot * (r1Center - r2Center), r2Extents * 2, invRot  *  r2Rot);

#if CUSTOMDEBUG
        DebugHelper.DrawCube(axisBounds.center, axisBounds.extents, Quaternion.identity, Color.gray, 0);
        DebugHelper.DrawCube(invRotB.center, invRotB.extents, invRotB.rotation, Color.gray, 0);
#endif

        if (axisBounds.Contains(invRotB.center + invRotB.rotation * new Vector3(invRotB.extents.x, invRotB.extents.y, invRotB.extents.z)) ||
            axisBounds.Contains(invRotB.center + invRotB.rotation * new Vector3(invRotB.extents.x, -invRotB.extents.y, invRotB.extents.z)) ||
            axisBounds.Contains(invRotB.center + invRotB.rotation * new Vector3(invRotB.extents.x, invRotB.extents.y, -invRotB.extents.z)) ||
            axisBounds.Contains(invRotB.center + invRotB.rotation * new Vector3(invRotB.extents.x, -invRotB.extents.y, -invRotB.extents.z)) ||
            axisBounds.Contains(invRotB.center + invRotB.rotation * new Vector3(-invRotB.extents.x, invRotB.extents.y, invRotB.extents.z)) ||
            axisBounds.Contains(invRotB.center + invRotB.rotation * new Vector3(-invRotB.extents.x, -invRotB.extents.y, invRotB.extents.z)) ||
            axisBounds.Contains(invRotB.center + invRotB.rotation * new Vector3(-invRotB.extents.x, invRotB.extents.y, -invRotB.extents.z)) ||
            axisBounds.Contains(invRotB.center + invRotB.rotation * new Vector3(-invRotB.extents.x, -invRotB.extents.y, -invRotB.extents.z)))
        {
            return true;
        }

        invRotB.ReduceToTriangles(_triangleCache);
        
        // Test x-axis aligned cube
        var xzRect = new Rect(axisBounds.min.xz(), axisBounds.size.xz());
#if CUSTOMDEBUG
        DebugHelper.DrawCube(new Vector3(xzRect.center.x, 0, xzRect.center.y), new Vector3(xzRect.width / 2, 0, xzRect.height / 2), Quaternion.identity, Color.black, 0);
#endif

        if (!OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[0].p1.xz(), _triangleCache[0].p2.xz(), _triangleCache[0].p3.xz(), xzRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[1].p1.xz(), _triangleCache[1].p2.xz(), _triangleCache[1].p3.xz(), xzRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[2].p1.xz(), _triangleCache[2].p2.xz(), _triangleCache[2].p3.xz(), xzRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[3].p1.xz(), _triangleCache[3].p2.xz(), _triangleCache[3].p3.xz(), xzRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[4].p1.xz(), _triangleCache[4].p2.xz(), _triangleCache[4].p3.xz(), xzRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[5].p1.xz(), _triangleCache[5].p2.xz(), _triangleCache[5].p3.xz(), xzRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[6].p1.xz(), _triangleCache[6].p2.xz(), _triangleCache[6].p3.xz(), xzRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[7].p1.xz(), _triangleCache[7].p2.xz(), _triangleCache[7].p3.xz(), xzRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[8].p1.xz(), _triangleCache[8].p2.xz(), _triangleCache[8].p3.xz(), xzRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[9].p1.xz(), _triangleCache[0].p2.xz(), _triangleCache[9].p3.xz(), xzRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[10].p1.xz(), _triangleCache[10].p2.xz(), _triangleCache[10].p3.xz(), xzRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[11].p1.xz(), _triangleCache[11].p2.xz(), _triangleCache[11].p3.xz(), xzRect))
        {
#if CUSTOMDEBUG
            foreach (var triangle in _triangleCache)
            {
                Debug.DrawLine(triangle.p1.x0z(), triangle.p2.x0z(), Color.red);
                Debug.DrawLine(triangle.p2.x0z(), triangle.p3.x0z(), Color.red);
                Debug.DrawLine(triangle.p3.x0z(), triangle.p1.x0z(), Color.red);
            }
#endif
            return false;
        }
#if CUSTOMDEBUG
        else
        {
            foreach (var triangle in _triangleCache)
            {
                Debug.DrawLine(triangle.p1.x0z(), triangle.p2.x0z(), Color.green);
                Debug.DrawLine(triangle.p2.x0z(), triangle.p3.x0z(), Color.green);
                Debug.DrawLine(triangle.p3.x0z(), triangle.p1.x0z(), Color.green);
            }
        }
#endif

        var xyRect = new Rect(axisBounds.min.xy(), axisBounds.size.xy());
#if CUSTOMDEBUG
        DebugHelper.DrawCube(new Vector3(xyRect.center.x, xyRect.center.y, 0), new Vector3(xyRect.width / 2, xyRect.height / 2, 0), Quaternion.identity, Color.black, 0);
#endif
        if (!OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[0].p1.xy(), _triangleCache[0].p2.xy(), _triangleCache[0].p3.xy(), xyRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[1].p1.xy(), _triangleCache[1].p2.xy(), _triangleCache[1].p3.xy(), xyRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[2].p1.xy(), _triangleCache[2].p2.xy(), _triangleCache[2].p3.xy(), xyRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[3].p1.xy(), _triangleCache[3].p2.xy(), _triangleCache[3].p3.xy(), xyRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[4].p1.xy(), _triangleCache[4].p2.xy(), _triangleCache[4].p3.xy(), xyRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[5].p1.xy(), _triangleCache[5].p2.xy(), _triangleCache[5].p3.xy(), xyRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[6].p1.xy(), _triangleCache[6].p2.xy(), _triangleCache[6].p3.xy(), xyRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[7].p1.xy(), _triangleCache[7].p2.xy(), _triangleCache[7].p3.xy(), xyRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[8].p1.xy(), _triangleCache[8].p2.xy(), _triangleCache[8].p3.xy(), xyRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[9].p1.xy(), _triangleCache[0].p2.xy(), _triangleCache[9].p3.xy(), xyRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[10].p1.xy(), _triangleCache[10].p2.xy(), _triangleCache[10].p3.xy(), xyRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[11].p1.xy(), _triangleCache[11].p2.xy(), _triangleCache[11].p3.xy(), xyRect))
        {
#if CUSTOMDEBUG
            foreach (var triangle in _triangleCache)
            {
                Debug.DrawLine(triangle.p1.xy0(), triangle.p2.xy0(), Color.red);
                Debug.DrawLine(triangle.p2.xy0(), triangle.p3.xy0(), Color.red);
                Debug.DrawLine(triangle.p3.xy0(), triangle.p1.xy0(), Color.red);
            }
#endif
            return false;
        }
#if CUSTOMDEBUG
        else
        {
            foreach (var triangle in _triangleCache)
            {
                Debug.DrawLine(triangle.p1.xy0(), triangle.p2.xy0(), Color.green);
                Debug.DrawLine(triangle.p2.xy0(), triangle.p3.xy0(), Color.green);
                Debug.DrawLine(triangle.p3.xy0(), triangle.p1.xy0(), Color.green);
            }
        }
#endif

        var yzRect = new Rect(axisBounds.min.yz(), axisBounds.size.yz());
#if CUSTOMDEBUG
        DebugHelper.DrawCube(new Vector3(0, yzRect.center.x, yzRect.center.y), new Vector3(0, yzRect.width / 2, yzRect.height / 2), Quaternion.identity, Color.black, 0);
#endif
        if (!OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[0].p1.yz(), _triangleCache[0].p2.yz(), _triangleCache[0].p3.yz(), yzRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[1].p1.yz(), _triangleCache[1].p2.yz(), _triangleCache[1].p3.yz(), yzRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[2].p1.yz(), _triangleCache[2].p2.yz(), _triangleCache[2].p3.yz(), yzRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[3].p1.yz(), _triangleCache[3].p2.yz(), _triangleCache[3].p3.yz(), yzRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[4].p1.yz(), _triangleCache[4].p2.yz(), _triangleCache[4].p3.yz(), yzRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[5].p1.yz(), _triangleCache[5].p2.yz(), _triangleCache[5].p3.yz(), yzRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[6].p1.yz(), _triangleCache[6].p2.yz(), _triangleCache[6].p3.yz(), yzRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[7].p1.yz(), _triangleCache[7].p2.yz(), _triangleCache[7].p3.yz(), yzRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[8].p1.yz(), _triangleCache[8].p2.yz(), _triangleCache[8].p3.yz(), yzRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[9].p1.yz(), _triangleCache[0].p2.yz(), _triangleCache[9].p3.yz(), yzRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[10].p1.yz(), _triangleCache[10].p2.yz(), _triangleCache[10].p3.yz(), yzRect) &&
            !OverlapUtilites2D.TriangleOverlapsRect(_triangleCache[11].p1.yz(), _triangleCache[11].p2.yz(), _triangleCache[11].p3.yz(), yzRect))
        {
#if CUSTOMDEBUG
            foreach (var triangle in _triangleCache)
            {
                Debug.DrawLine(triangle.p1.x0yz(), triangle.p2.x0yz(), Color.red);
                Debug.DrawLine(triangle.p2.x0yz(), triangle.p3.x0yz(), Color.red);
                Debug.DrawLine(triangle.p3.x0yz(), triangle.p1.x0yz(), Color.red);
            }
#endif
            return false;
        }
#if CUSTOMDEBUG
        else
        {
            foreach (var triangle in _triangleCache)
            {
                Debug.DrawLine(triangle.p1.x0yz(), triangle.p2.x0yz(), Color.green);
                Debug.DrawLine(triangle.p2.x0yz(), triangle.p3.x0yz(), Color.green);
                Debug.DrawLine(triangle.p3.x0yz(), triangle.p1.x0yz(), Color.green);
            }
        }
#endif

        return true;
    }
}