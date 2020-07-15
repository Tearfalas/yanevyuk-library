using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace YanevyukLibrary.ProceduralMesh{
    public struct Triangle{
        public int vertex1;
        public int vertex2;
        public int vertex3;

        public Triangle(int vertex1, int vertex2, int vertex3)
        {
            this.vertex1 = vertex1;
            this.vertex2 = vertex2;
            this.vertex3 = vertex3;
        }

        /// <summary>
        /// returns an inverted version fo the triangle. clockwise -> counterclockwise
        /// </summary>
        /// <returns></returns>
        public Triangle invert{
            get{
                return new Triangle(vertex2,vertex1,vertex3);
            }
        }
    }

    public struct Vertex{
        public float x;
        public float y;
        public float z;


        public Vertex(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator Vector3(Vertex d) => new Vector3(d.x,d.y,d.z);
        public static implicit operator Vertex(Vector3 b) => new Vertex(b.x,b.y,b.z);
    }

    public class MeshData{
        public List<Triangle> triangles;
        public List<Vertex> vertices;
        private Mesh _realMesh;

        public Mesh realMesh{
            get{
                if(_realMesh==null){
                    return ConvertToMesh();
                }
                return _realMesh;
            }
        }

        public MeshData()
        {
            this.triangles = new List<Triangle>();
            this.vertices = new List<Vertex>();
        }
        
        /// <summary>
        /// Adds a new vertex and returns its index
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public int AddVertex(Vertex vertex){
            vertices.Add(vertex);
            return vertices.Count-1;
        }

        public void AddVertices(IEnumerable<Vertex> vertexarr){
            vertices.AddRange(vertexarr);
        }

        public void AddTriangle(Triangle triangle){
            if(!AssertTriangle(triangle)){
                Debug.LogError("Tried adding a triangle with references to nonexistent vertices.");
                return;
            }
            
            triangles.Add(triangle);
        }

        private bool AssertTriangle(Triangle triangle){
            int maxIndex = Mathf.Max(triangle.vertex1,triangle.vertex2,triangle.vertex3);
            if(maxIndex>=vertices.Count){
                return false;
            }
            return true;
        }

        public void AddTriangles(IEnumerable<Triangle> trigarr){
            foreach (var item in trigarr)
            {
                if(!AssertTriangle(item)){
                    Debug.LogError("Tried adding a triangle with references to nonexistent vertices.");
                    return;
                }
                
                triangles.Add(item);
            }
        }

        public Mesh ConvertToMesh(){
            if(_realMesh==null)
                _realMesh = new Mesh();
            _realMesh.vertices = convertVertices().ToArray();
            _realMesh.triangles = convertTriangles().ToArray();
            _realMesh.RecalculateNormals();
            _realMesh.RecalculateTangents();
            return _realMesh;
        }

        private List<Vector3> convertVertices(){
            List<Vector3> vector3List = new List<Vector3>();
            foreach (var item in vertices)
            {
                vector3List.Add(item);
            }
            return vector3List;
        }
        private List<int> convertTriangles(){
            List<int> trigList = new List<int>();
            foreach (var item in triangles)
            {
                trigList.Add(item.vertex1);
                trigList.Add(item.vertex2);
                trigList.Add(item.vertex3);
            }
            return trigList;
        }

        public void InvertTriangles(){
            for(int i = 0; i<triangles.Count;i++){
                Triangle trig = triangles[i];
                trig = trig.invert;
                triangles[i] = trig;
            }
            ConvertToMesh();
        }

        /// <summary>
        /// Subdivides every triangle in this mesh.
        /// </summary>
        public void Subdivide(){
            List<Triangle> prevTriangles = new List<Triangle>();
            prevTriangles.AddRange(triangles);
            List<Vertex> prevVertices = new List<Vertex>();
            prevVertices.AddRange(vertices);
            triangles.Clear();
            vertices.Clear();
            foreach (var item in prevTriangles)
            {
                Vertex vertex1 = prevVertices[item.vertex1];
                Vertex vertex2 = prevVertices[item.vertex2];
                Vertex vertex3 = prevVertices[item.vertex3];

                Vertex vertex12 = ((Vector3)vertex1+vertex2)/2;
                Vertex vertex23 = ((Vector3)vertex2+vertex3)/2;
                Vertex vertex31 = ((Vector3)vertex3+vertex1)/2;

                int v1 = AddVertex(vertex1);
                int v2 = AddVertex(vertex2);
                int v3 = AddVertex(vertex3);
                int v12 = AddVertex(vertex12);
                int v23 = AddVertex(vertex23);
                int v31 = AddVertex(vertex31);
                AddTriangle(new Triangle(v1,v12,v31));
                AddTriangle(new Triangle(v12,v23,v31));
                AddTriangle(new Triangle(v12,v2,v23));
                AddTriangle(new Triangle(v31,v23,v3));
            }
            FixDuplicateVertices();
            ConvertToMesh();
        }

        public void Subdivide(int division){
            List<Triangle> prevTriangles = new List<Triangle>();
            prevTriangles.AddRange(triangles);
            List<Vertex> prevVertices = new List<Vertex>();
            prevVertices.AddRange(vertices);
            triangles.Clear();
            vertices.Clear();
            foreach (var item in prevTriangles)
            {
                List<Vertex> simpleVertices; //simple right triangle data.
                List<Triangle> simpleTriangles;
                SubdivideSimpleTriangle(division,out simpleVertices,out simpleTriangles);
                List<Vertex> targetTriangle = new List<Vertex>();
                targetTriangle.Add(prevVertices[item.vertex1]);
                targetTriangle.Add(prevVertices[item.vertex2]);
                targetTriangle.Add(prevVertices[item.vertex3]);
                TransformSimpleTriangle(targetTriangle,ref simpleVertices); //the positions of the new vertices are now correct.
                //need to fix the triangle indices., we simply have to increment each value by how many vertices there already are.
                for(int i = 0;i < simpleTriangles.Count;i++){
                    Triangle t = simpleTriangles[i];
                    t.vertex1 += vertices.Count;
                    t.vertex2 += vertices.Count;
                    t.vertex3 += vertices.Count;
                    simpleTriangles[i] = t;
                }
                AddVertices(simpleVertices);
                AddTriangles(simpleTriangles);
            }
            FixDuplicateVertices();
            ConvertToMesh();
        }

        /// <summary>
        /// Gives a simple right triangle subdivided by k, does this linearly. Each line in the simple right triangle will be split into k+1 lines.
        /// </summary>
        /// <param name="k"></param>
        /// <param name="newvertices"></param>
        /// <param name="newtrigs"></param>
        public static void SubdivideSimpleTriangle(int k, out List<Vertex> newvertices, out List<Triangle> newtrigs){
            int totalVertex = (2+k)*(2+k+1)/2; //sum to 2+k
            newvertices = new List<Vertex>();
            newtrigs = new List<Triangle>();
            //will use a right triangle for easy algebra, which will later be transformed onto any target.
            int to = k+2;
            for(to = k+2; to>0;to--){
                float length = (to-1.0f)/(k+1f);
                float y = (k+2f-to)/(k+1f);
                for(int i = 0; i<to;i++){
                    float x = to!=1 ? ((i+0.0f)/(to-1f))*length : 0;
                    newvertices.Add(new Vertex(x,y,0));
                }
            }

            int trigCount = (k+1)*(k+1);
            int bottomleft = 0;
            int adder = k+2;
            for(int rowTrigCount = 2*k+1; rowTrigCount>=0;rowTrigCount-=2){
                bool orientation = false; //pointing up
                for(int trig = 0; trig<rowTrigCount;trig++){ //triangle row
                    //runs for each supposed triangle.
                    Triangle triangle;
                    if(orientation==false){
                        orientation = true;
                        int botLeft = bottomleft+trig/2;
                        int botRight = bottomleft+1+trig/2;
                        int tip = bottomleft+trig/2+ adder;
                        triangle = new Triangle(tip,botRight,botLeft);

                    }else{
                        orientation = false;
                        int topLeft = bottomleft+(trig+1)/2+adder-1;
                        int topRight = bottomleft+(trig+1)/2+adder;
                        int tip = bottomleft+(trig+1)/2;
                        triangle = new Triangle(topLeft,topRight,tip);
                    }
                    newtrigs.Add(triangle);
                }
                bottomleft +=adder;
                adder--;
            }
        }

        /// <summary>
        /// Maps a set of vertices from the simple triangle to the target triangle.
        /// </summary>
        /// <param name="targetTriangle"></param>
        /// <param name="vertices"></param>
        public static void TransformSimpleTriangle(List<Vertex> targetTriangle, ref List<Vertex> vertices){
            Vector3 fakeY = (targetTriangle[0] - (Vector3)targetTriangle[2]);
            Vector3 fakeX = (targetTriangle[1] - (Vector3)targetTriangle[2]);
            for(int i = 0; i<vertices.Count;i++){
                Vertex vertex = vertices[i];
                Vector3 res = Vector3.zero;
                float xratio = vertex.x;
                float yratio = vertex.y;
                res = targetTriangle[2]+fakeX*xratio + fakeY*yratio;
                vertices[i] = res;
            }
        }

        public void FixDuplicateVertices(float threshold = 0.001f){
            Dictionary<Vertex,int> existingVertices = new Dictionary<Vertex, int>();
            List<Vertex> oldVertices = new List<Vertex>();
            oldVertices.AddRange(vertices);
            vertices.Clear();
            for(int i = 0; i<triangles.Count;i++)
            {
                Triangle trig = triangles[i];
                trig.vertex1 = FixVertex(threshold, existingVertices, oldVertices, trig.vertex1);
                trig.vertex2 = FixVertex(threshold, existingVertices, oldVertices, trig.vertex2);
                trig.vertex3 = FixVertex(threshold, existingVertices, oldVertices, trig.vertex3);
                triangles[i] = trig;
            }
        }

        private int FixVertex(float threshold, in Dictionary<Vertex, int> existingVertices, in List<Vertex> oldVertices, int vertexindex )
        {
            Vertex v = oldVertices[vertexindex];
            int newIndex = -1;
            foreach (var item in existingVertices)
            {
                if (Vector3.Distance(item.Key, v) < threshold)
                { //this vertex is very close to an existing vertex, will treat them as the same. 
                    newIndex = item.Value;
                    break;
                }
            }
            if (newIndex == -1)
            { //couldn't find an already existing vertex, so add a new one.
                newIndex = AddVertex(v);
                existingVertices.Add(v, newIndex);
            }
            return newIndex;
        }

        /// <summary>
        /// Hard spherizes the mesh, meaning that every vertice will have a distance of 1 from the origin.
        /// </summary>
        public void HardSpherize(){
            SoftSpherize(1);
        }

        /// <summary>
        /// Spherizes the mesh by the strength. The strength is the lerp strength where 1 means it becomes a sphere.
        /// </summary>
        /// <param name="strength"></param>
        public void SoftSpherize(float strength){
            for(int i = 0; i<vertices.Count;i++){
                Vector3 v = vertices[i];
                float dist = v.magnitude;
                float targetDist = Mathf.Lerp(dist,1,strength);
                v = v.normalized*targetDist;
                vertices[i] = v;
            }
            ConvertToMesh();
        }

        
    }
}