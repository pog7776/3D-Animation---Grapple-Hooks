using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Tool for breaking up polygons into triangles.
/// This ignores any Y values.
/// </summary>
[AddComponentMenu("")]
public class PolygonDecomposer
{
    private List<Vector3> vertices = new List<Vector3>();

    public PolygonDecomposer(Vector3[] vertices)
    {
        this.vertices = new List<Vector3>(vertices);
    }

    public int[] Decompose()
    {
        List<int> result = new List<int>();

        //Already a single triangle. Just return and empty array
        if (vertices.Count < 3)
        {
            return result.ToArray();
        }

        int[] vertIs = new int[vertices.Count];
        if (CalcArea() > 0)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                vertIs[i] = i;
            }
        }
        else
        {
            //Negative area. Must reverse the vertices
            for (int i = 0; i < vertices.Count; i++)
            {
                vertIs[i] = (vertices.Count - 1) - i;
            }
        }

        int vertsLeft = vertices.Count;
        int count = 2 * vertsLeft;
        int i2 = vertsLeft - 1;
        while (vertsLeft > 2)
        {
            count--;
            if (count < 0)
            {
                Debug.LogError("Could not correctly build the roof. Fix the shape of your base and try again");
                return result.ToArray();
            }

            //Shift the three vertices around
            int i1 = i2;
            if (vertsLeft <= i1)
            {
                i1 = 0;
            }
            i2 = i1 + 1;
            if (vertsLeft <= i2)
            {
                i2 = 0;
            }
            int i3 = i2 + 1;
            if (vertsLeft <= i3)
            {
                i3 = 0;
            }

            //Try to clip the current triangle
            if (ClipEar(i1, i2, i3, vertsLeft, vertIs))
            {
                //We have a triangle that can be clipped. Add to the result and remove
                //from our remaining polygon
                result.Add(vertIs[i1]);
                result.Add(vertIs[i2]);
                result.Add(vertIs[i3]);
                for (int i = i2, j = i2 + 1; j < vertsLeft; i++, j++)
                {
                    vertIs[i] = vertIs[j];
                }
                vertsLeft--;
                count = 2 * vertsLeft;
            }
        }
        result.Reverse();
        return result.ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="i1">Index of first vert in V</param>
    /// <param name="i2">Index of second vert in V</param>
    /// <param name="i3">Index of third vert in V</param>
    /// <param name="range">Count of vertices remaining in V that haven't been clipped</param>
    /// <param name="V">All the vertices (really their indices in the original set)</param>
    /// <returns></returns>
    private bool ClipEar(int i1, int i2, int i3, int numLeft, int[] V)
    {
        Vector3 v1 = vertices[V[i1]];
        Vector3 v2 = vertices[V[i2]];
        Vector3 v3 = vertices[V[i3]];
        if (Mathf.Epsilon > (((v2.x - v1.x) * (v3.z - v1.z)) - ((v2.z - v1.z) * (v3.x - v1.x))))
        {
            return false;
        }
        //Makes sure no other vertex lies inside the triangle
        for (int i = 0; i < numLeft; i++)
        {
            //Skip the given vertices
            if ((i == i1) || (i == i2) || (i == i3))
            {
                continue;
            }
            //Cannot clip if any of the vertices are inside
            if (IsInsideTriangle(v1, v2, v3, vertices[V[i]]))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Checks if a given point lies within triangle
    /// </summary>
    /// <param name="v1">First vertex of triangle</param>
    /// <param name="v2">Second vertex of triangle</param>
    /// <param name="v3">Third vertex of triangle</param>
    /// <param name="testPoint">The point that might be inside</param>
    /// <returns></returns>
    private bool IsInsideTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 testPoint)
    {
        float cross1_2 = (v3.x - v2.x) * (testPoint.z - v2.z) - (v3.z - v2.z) * (testPoint.x - v2.x);
        float cross2_3 = (v1.x - v3.x) * (testPoint.z - v3.z) - (v1.z - v3.z) * (testPoint.x - v3.x);
        float cross3_1 = (v2.x - v1.x) * (testPoint.z - v1.z) - (v2.z - v1.z) * (testPoint.x - v1.x);

        return ((cross1_2 >= 0f) && (cross2_3 >= 0f) && (cross3_1 >= 0f));
    }

    private float CalcArea()
    {
        float A = 0f;
        for (int i = vertices.Count - 1, j = 0; j < vertices.Count; i = j++)
        {
            Vector3 vert1 = vertices[i];
            Vector3 vert2 = vertices[j];
            A += (vert1.x * vert2.z) - (vert2.x * vert1.z);
        }
        return (A * 0.5f);
    }
}