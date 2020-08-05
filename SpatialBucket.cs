using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace YanevyukLibrary{
public interface ISpatialPosition
{
    Vector3 GetCenter();
}

public class SpatialBucket<T> where T : ISpatialPosition
{
    private float _cellsize;
    public float CellSize{
        get{
            return _cellsize;
        }
    }
    public SpatialBucket(float cellSize)
    {
        _cellsize = cellSize;
        _hashtable = new Hashtable();
    }
    private Hashtable _hashtable;

    /// <summary>
    /// Returns the grid position the input parameters is inside of.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Vector3Int WorldToGrid(Vector3 pos){
        pos/=CellSize;
        return new Vector3Int((int)Mathf.Floor(pos.x),(int)Mathf.Floor(pos.y),(int)Mathf.Floor(pos.z));
    }

    /// <summary>
    /// Returns the grid position the input parameters is inside of.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Vector3Int WorldToGrid(Transform target){
        return WorldToGrid(target.position);
    }

    /// <summary>
    /// Converts grid to world position.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Vector3 GridToWorld(Vector3Int pos){
        Vector3 p = pos;
        p *= CellSize;
        return p + Vector3.one*CellSize/2f;
    }

    public void GetCubeAt(Vector3Int gridPosition, out Vector3 center, out Vector3 size){
        center = GridToWorld(gridPosition);
        size = Vector3.one*CellSize;
    }

    private ulong HashGridPosition(Vector3Int vertex){
        Vector3 v = vertex;
        v += Vector3.one*1000;
        v += Vector3.up*1000;
        ulong xhash = (ulong)(v.x*73856093);
        ulong yhash = (ulong)(v.y*19349663);
        ulong zhash = (ulong)(v.z*83492791);
        
        ulong res = (ulong)System.Numerics.BigInteger.ModPow(xhash,yhash,ulong.MaxValue);
        res = (ulong)System.Numerics.BigInteger.ModPow(res,zhash,ulong.MaxValue);
        return res;
    }

    public void AddElement(T element){
        Vector3 pos = element.GetCenter();
        Vector3Int gridp = WorldToGrid(pos);
        ulong hash = HashGridPosition(gridp);
        if(!_hashtable.ContainsKey(hash)){
            _hashtable.Add(hash,new List<T>());
        }

        List<T> list = (List<T>)_hashtable[hash];
        list.Add(element);
        
    }

    public List<T> GetElements(Vector3Int gridPosition,int range = 0){
        List<T> list = new List<T>();
        List<Vector3Int> toCatch = new List<Vector3Int>();
        for(int x = -range; x<=range;x++){
            for(int y = -range; y<=range;y++){
                for(int z = -range; z<=range;z++){
                    Vector3Int p = gridPosition + new Vector3Int(x,y,z);
                    toCatch.Add(p);
                }
            }
        }
        foreach (var item in toCatch)
        {
            ulong hash = HashGridPosition(item);
            if(_hashtable.ContainsKey(hash)){
                List<T> retrieved = (List<T>)_hashtable[hash];
                list.AddRange(retrieved);
            }
        }
        return list;
    }

}

}
