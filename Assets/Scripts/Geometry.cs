using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geometry : Result
{
    public enum Type { Cube, Plane, Custom}

    public List<Vector3> vertices;
    public List<int> triangles;

    public Geometry(List<Vector3> vertices, List<int> triangles) {
        this.vertices = vertices;
        this.triangles = triangles;
    }

    public Geometry(Vector3[] vertices, int[] triangles) {
        this.vertices = new List<Vector3>(vertices);
        this.triangles = new List<int>(triangles);
    }

    public static Geometry Create(Type type) {
        switch (type) {
            case Type.Plane:
                return CreatePlane();
            case Type.Cube:
                return CreateCube();
            default:
                return CreateEmpty();
        }

        
    }

    public static Geometry CreateEmpty() {
        return new Geometry(new List<Vector3>(), new List<int>());
    }

    private static Geometry CreatePlane() {
        List<Vector3> points = new List<Vector3>();
        points.AddRange(new Vector3[4] {
            new Vector3(0.5f, 0, 0.5f),
            new Vector3(0.5f, 0, -0.5f),
            new Vector3(-0.5f, 0, 0.5f),
            new Vector3(-0.5f, 0, -0.5f)
        });
        List<int> triangles = new List<int>();
        triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 2 });
        return new Geometry(points, triangles);
    }

    private static Geometry CreateCube() {
        List<Vector3> points = new List<Vector3>();
        points.AddRange(new Vector3[8] {
            new Vector3(+0.5f, +0.5f, +0.5f),
            new Vector3(+0.5f, +0.5f, -0.5f),
            new Vector3(-0.5f, +0.5f, +0.5f),
            new Vector3(-0.5f, +0.5f, -0.5f),
            new Vector3(+0.5f, -0.5f, +0.5f),
            new Vector3(+0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, +0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f)
        });
        List<int> triangles = new List<int>();

        triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 2 });//bottom
        triangles.AddRange(new int[] { 4, 6, 5, 5, 6, 7 });//top
        triangles.AddRange(new int[] { 1, 0, 4, 4, 5, 1 });//rightside
        triangles.AddRange(new int[] { 3, 6, 2, 6, 3, 7 });//leftside
        triangles.AddRange(new int[] { 1, 5, 3, 3, 5, 7 });//frontside
        triangles.AddRange(new int[] { 0, 2, 4, 2, 6, 4 });//backside

        return new Geometry(points, triangles);

    }
}
