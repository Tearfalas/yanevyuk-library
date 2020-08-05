﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YanevyukLibrary.ProceduralMesh{
    public static class Icosahedron
    {
        const float X=.525731112119133606f;
        const float Z=.850650808352039932f;
        const float N=0.0f;
        
        static Vertex[] vertices=
        {
            new Vertex(-X,N,Z), new Vertex(X,N,Z), new Vertex(-X,N,-Z), new Vertex(X,N,-Z),
            new Vertex(N,Z,X), new Vertex(N,Z,-X), new Vertex(N,-Z,X), new Vertex(N,-Z,-X),
            new Vertex(Z,X,N), new Vertex(-Z,X,N), new Vertex(Z,-X,N), new Vertex(-Z,-X,N)
        };

        static Triangle[] triangles=
        {
            new Triangle(0,4,1), new Triangle(0,9,4), new Triangle(9,5,4), new Triangle(4,5,8),new Triangle(4,8,1),
            new Triangle(8,10,1), new Triangle(8,3,10), new Triangle(5,3,8), new Triangle(5,2,3),new Triangle(2,7,3),
            new Triangle(7,10,3), new Triangle(7,6,10), new Triangle(7,11,6), new Triangle(11,0,6),new Triangle(0,1,6),
            new Triangle(6,1,10), new Triangle(9,0,11), new Triangle(9,11,2), new Triangle(9,2,5),new Triangle(7,2,11),
        };
 
        /// <summary>
        /// Generates the most basic icosahedron
        /// </summary>
        /// <returns></returns>
        public static MeshData SimpleIcosahedron(){
            MeshData data = new MeshData();
            data.AddVertices(vertices);
            data.AddTriangles(triangles);
            data.InvertTriangles();
            data.ConvertToMesh();
            return data;
        }

        public static void GenerateIcosahedronUv(ref MeshData data){
            Vector2[] uv = new Vector2[data.vertices.Count];
            int counter = 0;
            foreach (var item in data.vertices)
            {
                float y = (item.y+1)/2f; 

                Vector3 v = item;
                v.y = 0;
                v = v.normalized;
                
                float xAngle = Vector3.Angle(Vector3.right,v);
                if(Vector3.Dot(v,Vector3.forward) < 0){
                    xAngle = 360-xAngle;
                }
                xAngle = xAngle/360f; //goes from 0 to 1 now.
                float x = xAngle;

                uv[counter] = new Vector2(x,y);
                counter++;
            }
            data.realMesh.SetUVs(0,uv);
            //data.realMesh.uv = uv;
        }

        
    }
}