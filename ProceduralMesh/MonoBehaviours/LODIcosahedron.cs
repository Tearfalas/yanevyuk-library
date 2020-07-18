using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YanevyukLibrary.ProceduralMesh;
using NaughtyAttributes;
using UnityEngine.UI;
public struct MeshObject{
    public GameObject gameObject;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public IcosahedronChunk chunk;

    public MeshObject(GameObject gameObject, MeshFilter meshFilter, MeshRenderer meshRenderer,IcosahedronChunk chunk)
    {
        this.gameObject = gameObject;
        this.meshFilter = meshFilter;
        this.meshRenderer = meshRenderer;
        this.chunk = chunk;
    }
}
public class LODIcosahedron : MonoBehaviour
{
    private ChunkedIcosahedron chunkedIcosahedron;
    private List<MeshObject> chunkObjects = new List<MeshObject>();
    public LiveResolutionManager resolutionManager;
    public Text trigCountText;
    [OnValueChanged("ChangeMaterial")] public Material material;
    public PerlinNoise perlinNoise;
    public float noiseStrength;
    
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        MakeIcosahedron();
    }
    public int chunkCountDivisor;
    public int minimumResolution;
    private MeshCollider meshCollider;
    [Button] public void MakeIcosahedron(){
        Clear();
        chunkedIcosahedron = new ChunkedIcosahedron(chunkCountDivisor,minimumResolution,perlinNoise);
        resolutionManager.SetIcosahedron(chunkedIcosahedron);
        foreach (var chunk in chunkedIcosahedron.GetChunks())
        {
            GameObject chunkobj = new GameObject();
            chunkobj.name = "ChunkObject"+chunk.GetHashCode();
            chunkobj.transform.SetParent(transform);
            chunkobj.transform.localPosition = Vector3.zero;
            MeshRenderer rend = chunkobj.AddComponent<MeshRenderer>();
            rend.material = material;
            MeshFilter filt = chunkobj.AddComponent<MeshFilter>();
            filt.mesh = chunk.ActiveMesh.realMesh;
            MeshCollider col = chunkobj.AddComponent<MeshCollider>();
            col.sharedMesh = chunk.GetHighestResolutionMesh().realMesh;
            Rigidbody body = chunkobj.AddComponent<Rigidbody>();
            body.useGravity = false;
            body.isKinematic = true;
            chunkObjects.Add(new MeshObject(chunkobj,filt,rend,chunk));
        }
    }



    [Button] public void Clear(){
        foreach (var item in chunkObjects)
        {
            GameObject.Destroy(item.gameObject);
        }
        chunkObjects.Clear();
    }

    public void ChangeMaterial(){
        foreach (var item in chunkObjects)
        {
            item.meshRenderer.material = material;
        }
    }

    public Transform viewTransform;
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        resolutionManager.UpdateResolutions(transform,viewTransform.position,null);
        foreach (var item in chunkObjects)
        {
            item.meshFilter.mesh = item.chunk.ActiveMesh.realMesh;
        }
        trigCountText.text = "Triangle Count: "+chunkedIcosahedron.GetTriangleCount().ToString();
    }

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {
        return;
        if(chunkedIcosahedron==null) return;
        foreach (var chunk in chunkedIcosahedron.GetChunks())
        {
            int k = 0;
            foreach (var vertex in chunk.ActiveMesh.vertices)
            {
                Vector3 normal = chunk.ActiveMesh.normals[k];
                Gizmos.DrawLine(vertex,vertex+normal*0.1f);
                k++;
            }
        }
    }
}
