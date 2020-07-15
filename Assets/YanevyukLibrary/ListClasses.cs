using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YanevyukLibrary{

    
    public class IntervalMapping<T>
    {
        List<List<T>> mapping = new List<List<T>>();
        private List<float> intervalMaximals = new List<float>();
        private float minimum;
        private float maximum;

        /// <summary>
        /// The minimum and maximum values of the interval are required.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public IntervalMapping(float min,float max)
        {
            minimum = min;
            maximum = max;
            intervalMaximals.Add(maximum);
            mapping.Add(new List<T>());
            //Initially has the whole interval
        }
        
        /// <summary>
        /// Adding intervals works incrementally, ex:
        ///
        /// minimum = 5;
        /// AddInterval(10) //creates new interval [5,10)
        /// AddInterval(12) //new interval [10,12)
        ///
        /// Adding the last interval is not required and not recommended!
        /// </summary>
        /// <param name="f"></param>
        public void AddInterval(float f)
        {
            if (f == maximum)
            {
                return;
            }
            intervalMaximals.RemoveAt(intervalMaximals.Count-1);
            intervalMaximals.Add(f);
            intervalMaximals.Add(maximum);
            
            List<T> lastInterval = mapping[mapping.Count - 1];
            mapping.RemoveAt(mapping.Count - 1);
            mapping.Add(new List<T>());
            mapping.Add(lastInterval);
        }
        
        /// <summary>
        /// Adds the element to the interval valueInInterval is contained in.
        /// </summary>
        /// <param name="valueInInterval"></param>
        /// <param name="element"></param>
        public void Add(float valueInInterval, T element)
        {
            int index = GetIntervalIndex(valueInInterval);
            mapping[index].Add(element);
        }
        
        private int GetIntervalIndex(float value)
        {
            if (value < minimum)
            {
                value = minimum;
            }

            if (value > maximum)
            {
                value = maximum;
            }

            int t = 0;
            while (t<intervalMaximals.Count)
            {
                if (intervalMaximals[t] > value)
                {
                    break;
                }

                t++;
            }
            

            return t;
        }
    
        public List<T> Get(float value)
        {
            if (value < minimum)
            {
                value = minimum;
            }

            if (value > maximum)
            {
                value = maximum;
            }

            int t = 0;
            while (t<intervalMaximals.Count)
            {
                if (intervalMaximals[t] > value)
                {
                    break;
                }

                t++;
            }

            return mapping[t];

        }

        public T GetFirst(float value)
        {
            if (value < minimum)
            {
                value = minimum;
            }

            if (value > maximum)
            {
                value = maximum;
            }

            int t = 0;
            while (t<intervalMaximals.Count)
            {
                if (intervalMaximals[t] > value)
                {
                    break;
                }

                t++;
            }

            if (mapping[t].Count > 0)
            {
                return mapping[t][0];
            }

            return default;
        }
    }

    public class PrefabPool
    {
        private Dictionary<string,Queue<GameObject>> pool = new Dictionary<string, Queue<GameObject>>();

        public void GenerateFromList(List<PrefabMappingString> list, int amount)
        {
            pool.Clear();
            foreach (var prefabMap in list)
            {
                Queue<GameObject> newqueue = new Queue<GameObject>();
                pool.Add(prefabMap.Key,newqueue);

                for (int i = 0; i < amount; i++)
                {
                    GameObject obj = GameObject.Instantiate(prefabMap.Value);
                    obj.gameObject.SetActive(false);
                    newqueue.Enqueue(obj);
                }
            }
        }

        public PrefabPool(List<PrefabMappingString> list,int amount)
        {
            GenerateFromList(list,amount);
        }

        public GameObject GetNext(string key)
        {
            if (pool.ContainsKey(key) && pool[key].Count>0)
            {
                GameObject pulled = pool[key].Dequeue();
                pulled.SetActive(true);
                pool[key].Enqueue(pulled);
                return pulled;
            }
            else
            {
                Debug.LogWarning("PrefabPool : GetNext : this key is invalid");
                return null;
            }
        }
    }
}