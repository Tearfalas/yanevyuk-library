using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YanevyukLibrary
{
    [System.Serializable]
    public class AbstractMapping<T,K>
    {
        public T Key;
        public K Value;

        public AbstractMapping(T _key, K _prefab)
        {
            Key = _key;
            Value = _prefab;
        }

        public AbstractMapping()
        {

        }
    }


    [System.Serializable]
    public class PrefabMapping<T> : AbstractMapping<T, GameObject>
    {

        public PrefabMapping(T _key, GameObject _prefab)
        {
            Key = _key;
            Value = _prefab;
        }

        public PrefabMapping()
        {
        }
    }

    [System.Serializable]
    public class PrefabMappingString : PrefabMapping<string>
    {

        public PrefabMappingString(string _key, GameObject _prefab)
        {
            Key = _key;
            Value = _prefab;
        }
    }



}