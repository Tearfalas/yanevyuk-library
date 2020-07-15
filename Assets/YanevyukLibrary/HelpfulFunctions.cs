using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YanevyukLibrary
{
    public static class FuncLib
    {
        #region Delegates
        public delegate void simpleDelegate();

        public delegate bool boolDelegate();
        public delegate void singleInputDelegate(object param);
        public delegate bool singleInputBoolDelegate(object param);

        #endregion

        /// <summary>
        /// Sets the layer of this object and all of its children to the target layer
        /// </summary>
        /// <param name="target"></param>
        public static void SetLayer(this GameObject target, int layer){
            target.layer = layer;
            for(int i = 0;i<target.transform.childCount;i++){
                target.transform.GetChild(i).gameObject.SetLayer(layer);
            }
        }


        /// <summary>
        /// Sets the layer of this object and all of its children to the target layer
        /// </summary>
        /// <param name="target"></param>
        public static void SetLayer(this GameObject target, string layer){
            target.layer = LayerMask.NameToLayer(layer);
            for(int i = 0;i<target.transform.childCount;i++){
                target.transform.GetChild(i).gameObject.SetLayer(layer);
            }
        }
        /// <summary>
        /// Returns the average Vector3 of a list.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Vector3 Average(this IList<Vector3> list){
            Vector3 sum = Vector3.zero;
            foreach (var item in list)
            {
                sum+=item;
            }
            return sum/(list.Count+0.0f);
        }

        /// <summary>
        /// Returns the average float of a list.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static float Average(this IList<float> list){
            float sum = 0;
            foreach (var item in list)
            {
                sum+=item;
            }
            return sum/(list.Count+0.0f);
        }

        /// <summary>
        /// Offsets every Vector3 in a list by said Vector3.
        /// </summary>
        /// <param name="list"></param>
        public static void Offset(this IList<Vector3> list, Vector3 offset){
            for(var i = 0;i < list.Count;i++){
                list[i] = list[i] +offset;
            }
        }
        public static void Shuffle<T>(this IList<T> ts) {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i) {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = ts[i];
                ts[i] = ts[r];
                ts[r] = tmp;
            }
        }
        public static float LinearDistance(this Vector3 v1, Vector3 v2, Vector3? relativeTo = null)
        {
            if (relativeTo == null)
            {
                relativeTo = Vector3.up;
            }
            Plane myplane = new Plane(relativeTo.Value,Vector3.zero);

            v1 = myplane.ClosestPointOnPlane(v1);
            v2 = myplane.ClosestPointOnPlane(v2);
            return Vector3.Distance(v1, v2);
        }

        public static void Call(this simpleDelegate ev)
        {
            if (ev != null)
            {
                ev();
            }
        }
        
        public static void DoParabolicMove(this GameObject target,Vector3 targetPosition,float height, float time){
            Parabola parabola= new Parabola();
            parabola.ConstructParabola(target.transform.position,targetPosition,height);

            target.GetComponent<MonoBehaviour>().StartCoroutine(parabolaLoop(target.transform,time,parabola));

        }

        private static IEnumerator parabolaLoop(Transform target,float time,Parabola parabola){
            float ratio = 0;
            float t = 0;
            while(true){
                t += Time.deltaTime;
                ratio = Mathf.Clamp(t/time,0,1);
                target.position = parabola.GetPositionAt(ratio);
                if(ratio>=1){
                    yield break;
                }
                yield return null;
            }
        }
    }
}