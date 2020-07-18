using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace YanevyukLibrary.ProceduralMesh{
    /// <summary>
    /// Automatically chunks an icosahedron and updates it on command with increasing resolution at a point.
    /// </summary>
    public class ChunkedIcosahedron
    {
        private List<IcosahedronChunk> chunks = new List<IcosahedronChunk>();
        private PerlinNoise noise;
        public ChunkedIcosahedron(int chunkCountDivisor,int minimumResolution,PerlinNoise noise = null)
        {
            this.noise = noise;
            MeshData baseData = Icosahedron.SimpleIcosahedron();
            baseData.Subdivide(chunkCountDivisor);
            baseData.HardSpherize();
            baseData.ConvertToMesh();
            //Each triangle in the base icosahedron is a chunk.
            foreach (var item in baseData.triangles)
            {
                IcosahedronChunk chunk = new IcosahedronChunk(baseData,item,minimumResolution);
                chunks.Add(chunk);
                if(noise!=null)
                foreach (var meshdata in chunk.GetMeshDatas())
                {
                    meshdata.ApplyNoise(noise,0.6f);
                }
            }
        }

        public List<IcosahedronChunk> GetChunks(){
            return chunks;
        }

        public void UpdateAllResolutions(int newResolution){
            foreach (var item in chunks)
            {
                item.UpdateDetail(newResolution);
            }
        }

        public int GetTriangleCount(){
            int val = 0;
            foreach (var item in chunks)
            {
                if(item.ActiveMesh!=null)
                val += item.ActiveMesh.triangles.Count;
            }
            return val;
        }

        public Mesh getHighResolutionTotalMesh(){
            
            MeshData temp = Icosahedron.SimpleIcosahedron();
            temp.Subdivide(IcosahedronChunk.RESLIMIT);
            temp.HardSpherize();
            if(noise!=null)
            temp.ApplyNoise(noise,0.6f);
            temp.ConvertToMesh();
            return temp.realMesh;
        }
    }

    public class IcosahedronChunk{
        public const int RESLIMIT = 12;
        private List<Vertex> baseVertices;
        private Triangle baseTriangle;
        private int currentResolution = -1;
        private int minimumResolution = 0;
        private Vector3 _localCenter;
        public Vector3 localCenter{
            get{
                return _localCenter;
            }
        }

        private List<MeshData> allMeshDatas = new List<MeshData>();
        private MeshData activeMesh = null;
        public MeshData ActiveMesh{
            get{
                return activeMesh;
            }
        }

        public List<MeshData> GetMeshDatas(){
            return allMeshDatas;
        }

        public MeshData GetHighestResolutionMesh(){
            return allMeshDatas[allMeshDatas.Count-1];
        }
      
        public IcosahedronChunk(in MeshData referenceMesh, Triangle targetTriangle, int minimumResolution = 0,int startResolution = 0)
        {
            this.minimumResolution = minimumResolution;
            baseVertices = new List<Vertex>();
            baseTriangle = new Triangle(0,1,2);
            for(int i = 0; i<=RESLIMIT;i++){
                MeshData newmesh =  new MeshData();
                Vertex v1 = referenceMesh.vertices[targetTriangle.vertex1];
                Vertex v2 = referenceMesh.vertices[targetTriangle.vertex2];
                Vertex v3 = referenceMesh.vertices[targetTriangle.vertex3];
                _localCenter = ((Vector3)v1+v2+v3)/3;
                newmesh.AddVertex(v1);
                newmesh.AddVertex(v2);
                newmesh.AddVertex(v3);
                newmesh.AddTriangle(new Triangle(0,1,2));
                newmesh.Subdivide(i);
                newmesh.HardSpherize();
                newmesh.DoSphericalNormals();
                newmesh.ConvertToMesh();
                allMeshDatas.Add(newmesh);
            }

            {
                MeshData newmesh =  new MeshData();
                Vertex v1 = referenceMesh.vertices[targetTriangle.vertex1];
                Vertex v2 = referenceMesh.vertices[targetTriangle.vertex2];
                Vertex v3 = referenceMesh.vertices[targetTriangle.vertex3];
                _localCenter = ((Vector3)v1+v2+v3)/3;
                newmesh.AddVertex(v1);
                newmesh.AddVertex(v2);
                newmesh.AddVertex(v3);
                newmesh.AddTriangle(new Triangle(0,1,2));
                newmesh.Subdivide(35);
                newmesh.HardSpherize();
                newmesh.DoSphericalNormals();
                newmesh.ConvertToMesh();
                allMeshDatas.Add(newmesh);
            }
            
            if(startResolution<minimumResolution){
                startResolution = minimumResolution;
            }
            UpdateDetail(startResolution);
        }

        public void UpdateDetail(int newResolution){
            if(newResolution==currentResolution){
                return;
            }
            if(newResolution == -1){
                activeMesh = allMeshDatas[allMeshDatas.Count-1];
            }
            if(newResolution<minimumResolution){
                newResolution = minimumResolution;
            }
            if(newResolution>RESLIMIT){
                newResolution = RESLIMIT;
            }
            currentResolution = newResolution;
            activeMesh = allMeshDatas[currentResolution];
        }

        
    }

    [System.Serializable]
    public class LiveResolutionManager{
        public int minimumResolution;
        public int maximumResolution;
        public AnimationCurve lodCurve;
        public float maxResolutionDistance; 
        public float minResolutionDistance;
        private ChunkedIcosahedron chunkedIcosahedron;

        public void SetIcosahedron(ChunkedIcosahedron target){
            chunkedIcosahedron = target;
        }

        public void UpdateResolutions(Vector3 icosahedronPosition,Quaternion icosahedronRotation, Vector3 viewPosition, Vector3? viewDir = null,float noiseStrength = 1){
            Transform tr = new GameObject().transform;
            tr.position = icosahedronPosition;
            tr.rotation = icosahedronRotation;
            UpdateResolutions(tr,viewPosition,viewDir,noiseStrength);
            GameObject.Destroy(tr.gameObject);
        }
        public void UpdateResolutions(Transform icosahedronTransform, Vector3 viewPosition, Vector3? viewDir = null,float noiseStrength = 1){
            foreach (var chunk in chunkedIcosahedron.GetChunks())
            {
                Vector3 worldPosOfChunk = icosahedronTransform.TransformPoint(chunk.localCenter);
                float distance = Vector3.Distance(worldPosOfChunk,viewPosition);
                if(distance<0.05f){
                    chunk.UpdateDetail(-1);
                    continue;
                }

                float normalized = (distance-minResolutionDistance)/(maxResolutionDistance-minResolutionDistance);
                normalized = Mathf.Clamp(normalized,0,1); //0 means closest, 1 means furthest.
                float result = lodCurve.Evaluate(normalized);
                int targetResolution = minimumResolution + (int)((maximumResolution-minimumResolution + 0.0f)*result);
                chunk.UpdateDetail(targetResolution);
            }
        }

        
    }
}
