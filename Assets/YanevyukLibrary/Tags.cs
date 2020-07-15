using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YanevyukLibrary
{
    public static class Tags
    {
        private static Dictionary<object,byte> TagList = new Dictionary<object, byte>();

        public static void TagObject(object obj, byte tag)
        {
            if (TagList.ContainsKey(obj))
            {
                TagList[obj] = tag;
            }
            else
            {
                TagList.Add(obj,tag);
            }
        }

        public static byte GetTag(object obj)
        {
            if (TagList.ContainsKey(obj))
            {
                return TagList[obj];
            }
            else
            {
                return 0;
            }
        }

        public static void RemoveTag(object obj)
        {
            if (TagList.ContainsKey(obj))
            {
                TagList.Remove(obj);
            }
        }
    }
}