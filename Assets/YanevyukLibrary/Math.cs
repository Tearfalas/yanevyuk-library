using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YanevyukLibrary{
    public struct SphereLineIntersectionData{
            public Vector3 point;
            public float ratio;
            public int pathCount;

            public SphereLineIntersectionData(Vector3 point, float ratio)
            {
                this.point = point;
                this.ratio = ratio;
                pathCount = -1;
            }
        }
    public static class Math
    {
        

        private static int counter = 0;
        public static SphereLineIntersectionData[]  SphereLineSegmentIntersection(Vector3 sphereCenter, float sphereRadius, Vector3 line1, Vector3 line2){
            
            float a = Mathf.Pow(line2.x-line1.x,2) + Mathf.Pow(line2.y-line1.y,2) + Mathf.Pow(line2.z-line1.z,2);
            float b = 2*( (line2.x - line1.x)*(line1.x - sphereCenter.x) +
                (line2.y - line1.y)*(line1.y - sphereCenter.y) +
                (line2.z - line1.z)*(line1.z - sphereCenter.z));
            
            float c = sphereCenter.x*sphereCenter.x + sphereCenter.y*sphereCenter.y + sphereCenter.z*sphereCenter.z +
            line1.x*line1.x + line1.y*line1.y + line1.z*line1.z
            - 2*(sphereCenter.x*line1.x + sphereCenter.y*line1.y + sphereCenter.z*line1.z)  - sphereRadius*sphereRadius;

            float determinant = Mathf.Sqrt(b*b - 4*a*c);
            float sol1 = (-b - determinant)/(2*a);
            float sol2 = (-b + determinant)/(2*a);

            if(determinant<0){
                //no intersection
                return null;
            }
            if(determinant<Mathf.Epsilon){
                //tangent
                return null;
            }
            counter = 0;
            if(!(sol1 < 0 || sol1>1)){ //inside the bounds of the line
                counter++;
            }

            if(!(sol2 < 0 || sol2 >1)){ //inside the bounds of the line
                counter++;
            }

            if(counter==0){
                return null;
            }
            SphereLineIntersectionData[] arr = new SphereLineIntersectionData[counter];

            counter = 0;
            // two intersections

            //sol1 and sol2 are for u, u is [0,1] from line1 to line2 
            if(!(sol1 < 0 || sol1>1)){ //inside the bounds of the line
                arr[counter] = new SphereLineIntersectionData(line2*sol1 + line1*(1-sol1),sol1);
                counter++;
            }

            if(!(sol2 < 0 || sol2 >1)){ //inside the bounds of the line
                arr[counter] = new SphereLineIntersectionData(line2*sol2 + line1*(1-sol2),sol2);
            }
            return arr;
        }
    }

}
