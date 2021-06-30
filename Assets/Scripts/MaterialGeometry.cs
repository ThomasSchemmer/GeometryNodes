using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialGeometry : Geometry {
    public Material material;

    public MaterialGeometry(Material material, List<Vector3> vertices, List<int> triangles) : base(vertices, triangles) {
        this.material = material;
    }

    public MaterialGeometry(Material material, Vector3[] vertices, int[] triangles) : base(vertices, triangles){
        this.material = material;
    }

    public MaterialGeometry(Material material, Geometry geometry) : base(geometry.vertices, geometry.triangles) {
        this.material = material;
    }
}
