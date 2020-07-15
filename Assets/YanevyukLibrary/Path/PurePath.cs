using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace YanevyukLibrary{

    public struct ClosestPoint{
        public PurePath belongsTo;
        public Vector3 point;
        public float trueDistance;
        public float roadDistance;

        public ClosestPoint(PurePath belongsTo, Vector3 point, float trueDistance, float roadDistance)
        {
            this.belongsTo = belongsTo;
            this.point = point;
            this.trueDistance = trueDistance;
            this.roadDistance = roadDistance;
        }
    }
    /// <summary>
    /// The PurePath class is the simplest type of path that all other paths are converted to. It returns linear movement, with only
    /// directional smoothing. Every point in this path is equidistance. 
    /// </summary>
    public class PurePath : IEnumerable<Vector3>
    {
        public static double DefaultMargin = 0.1;
        private Vector3[] rawData;
        private Vector3[] originalData;
        private double _margin;
        private double _length = 0;
        private double lastDistance = 1;

        public double Length{
            get{
                return _length;
            }
        }

        public double Margin{
            get{
                return _margin;
            }
        }

        public Vector3[] GetOriginalData(){
            return originalData;
        }

    
        public PurePath(IEnumerable<Vector3> points, double margin = -1) : this(points.ToArray(),margin) { }
        public PurePath(IList<Vector3> points, double margin = -1) : this(points.ToArray(),margin) { }
        public PurePath(Vector3[] points, double margin = -1){
            originalData = points;
            margin = margin < 0 ? DefaultMargin : margin;
            _margin = margin;
            LinkedList<Vector3> linked = new LinkedList<Vector3>();
            foreach (var item in points)
            {
                linked.AddLast(item);
            }

            LinkedList<SphereLineIntersectionData> data = new LinkedList<SphereLineIntersectionData>();

            SphereLineIntersectionData first = new SphereLineIntersectionData(linked.First.Value,0);
            first.pathCount = 0;
            data.AddLast(
                first
            );
            LinkedListNode<Vector3> pointsLinkNode = linked.First;
            Vector3 currentPoint = pointsLinkNode.Value;
            LinkedListNode<Vector3> l0;
            LinkedListNode<Vector3> l1;

            int counter = 0;
            int k = 0;
            bool exit = false;
            while(true){
                k++;
                if(k>10000000){
                    Debug.LogWarning("INFINITE LOOP?");
                    break;
                }
                l0 = pointsLinkNode;
                l1 = pointsLinkNode.Next;
                if(l1 == null){
                    break;
                }
                SphereLineIntersectionData[] arr = 
                Math.SphereLineSegmentIntersection(currentPoint,(float)margin,l0.Value,l1.Value);
                while(arr==null || arr.Length == 0){
                    //Debug.Log("Moved line");
                    l0 = l1;
                    l1 = l0.Next;
                    if(l1==null){
                        exit = true;
                        break;
                    }
                    arr = Math.SphereLineSegmentIntersection(currentPoint,(float)margin,l0.Value,l1.Value);
                    counter++;
                    if(counter>10000000){
                        Debug.LogWarning("INFINITE LOOP?22");
                        break;
                    }
                }
                if(exit){
                    break;
                }
                if(arr.Length == 1){
                    SphereLineIntersectionData  d = arr[0];
                    //Debug.Log("pathCounters: "+d.ratio+" and "+data.Last.Value.ratio);
                    if(data.Last.Value.pathCount == counter){ //the found point is on the same line as the previous point, make sure it is not before
                        //Debug.Log("Ratios: "+d.ratio+" and "+data.Last.Value.ratio);
                        if(d.ratio<data.Last.Value.ratio){
                            //Debug.Log("Ratios: "+d.ratio+" and "+data.Last.Value.ratio);
                            //the only point found is behind the latest one, this must be nearing the end of the line.
                            //switch to next line segment
                            counter++;
                            pointsLinkNode = l1;
                            continue;
                        }else{
                            currentPoint = d.point;
                            d.pathCount = counter;
                            data.AddLast(d);
                        }
                    }else{
                        currentPoint = d.point;
                        d.pathCount = counter;
                        data.AddLast(d);
                    }
                }else if(arr.Length == 2){
                    SphereLineIntersectionData closest;
                    SphereLineIntersectionData farthest;
                    if(arr[0].ratio < arr[1].ratio){
                        closest = arr[0];
                        farthest = arr[1];
                    }else{
                        closest = arr[1];
                        farthest = arr[0];
                    }

                    if(counter == data.Last.Value.pathCount){ //these points are both on the same line as the previous point. 
                        if(closest.ratio < data.Last.Value.ratio){ //the closest one comes before the last value so we need to ignore it. 
                            if(farthest.ratio< data.Last.Value.ratio){
                                //this should NOT happen. 
                                //Debug.LogError("PurePath: the farthest point is behind the last point.");
                                break;
                            }else{
                                currentPoint = farthest.point;
                                farthest.pathCount = counter;
                                data.AddLast(farthest);
                            }

                        }else{
                            currentPoint = closest.point;
                            closest.pathCount = counter;
                            data.AddLast(closest);
                        }
                    }else{ //they're on a different line, simply pick the closest
                        currentPoint = closest.point;
                        closest.pathCount = counter;
                        data.AddLast(closest);
                    }

                    
                }
                pointsLinkNode = l0;
                //Debug.Log("This ran");
            }

            rawData = new Vector3[data.Count+1];
            int i = 0;
            foreach (var item in data)
            {
                rawData[i] = item.point;
                _length += _margin;
                i++;
            }
            _length -= _margin;

            rawData[i] = linked.Last.Value;
            lastDistance = Vector3.Distance(rawData[i-1],rawData[i]);
            _length += lastDistance;
        }

        public IEnumerator<Vector3> GetEnumerator()
        {
            return ((IEnumerable<Vector3>)rawData).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return rawData.GetEnumerator();
        }


        /// <summary>
        /// Returns the points on the path when travelled by distance.
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public Vector3 getPoint(double distance){
            if(distance<0) distance = 0;
            if(distance>Length) distance = Length;
            int counter = (int)(distance/_margin);
            if(counter<0){
                return rawData[0];
            }

            if(counter>=rawData.Length){
                return rawData[rawData.Length-1];
            }

            double ratio = (distance - _margin*(counter+0.0f))/( counter != rawData.Length-2 ? _margin : lastDistance);
            return Vector3.Lerp(rawData[counter],rawData[counter+1],(float)ratio);
        }

        public Vector3 getDirection(double distance, float smoothness = 0.2f){
            return (getPoint(distance+smoothness) - getPoint(distance-smoothness)).normalized;
        }


        public ClosestPoint GetClosestPoint(Vector3 p){
            return GetClosestPointInSection(p, 0, (float)Length);
        }

        private int resolution = 6;
        public ClosestPoint GetClosestPointInSection(Vector3 p, float start, float end, float depth = 0){
            float ratio;
            float dist;
            int closestK = 1;
            float closestDist=10000000;
            float temp;
            for(int k = 0;k<=resolution;k++){
                ratio = (k+0.0f)/(resolution+0.0f);
                dist = start+ratio*(end-start);
                Vector3 point = getPoint(dist);
                temp =Vector3.Distance(point,p);
                if(temp<closestDist){
                    closestDist = temp;
                    closestK = k;
                }
            }
            ratio = (closestK+0.0f)/(resolution+0.0f);
            dist = start+ratio*(end-start);
            if(depth >= 18){
                return new ClosestPoint(this,getPoint(dist),closestDist,dist);
            }

            float step = (float)(end-start)/(resolution-1f);
            float min = dist-step >= 0 ? dist-step : 0;
            float max = dist+step <=Length ? dist+step : (float)Length;
            depth++;
            return GetClosestPointInSection(p,min,max,depth);
        }
    }
}
