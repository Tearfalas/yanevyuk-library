using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YanevyukLibrary
{

    //TESTING GIT STUFF
    public static class LoopPresets
    {
        /// <summary>
        /// Returns a list of angles in radians from 0 to 2*PI, 2*PI is not included, according to resolution.
        /// </summary>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public static float[] AngleLoop(int resolution, bool clockwise = true)
        {
            float[] array = new float[resolution];
            for (int i = 0; i < resolution; i++)
            {
                float angle = ((i + 0.0f) / (resolution + 0.0f))*Mathf.PI*2;
                if (!clockwise)
                {
                    angle = Mathf.PI * 2 - angle;
                }
                array[i] = angle;
            }

            return array;
        }

        /// <summary>
        /// Returns an array of unit vectors tracing the edge of an unit circle around the given axis (Up by default)
        /// The angle can be offset with the offset value.
        /// </summary>
        /// <param name="resolution">How many unit vectors</param>
        /// <param name="axis">World space axis to revolve around</param>
        /// <param name="offset">Angle offset</param>
        /// <returns></returns>
        public static Vector3[] UnitCircleLoop(int resolution, Vector3? axis = null, float offset = 0,bool clockwise = true)
        {
            if (axis == null)
            {
                axis = Vector3.up;
            }else if (Vector3.Dot(Vector3.up, axis.Value) <0)
            {
                axis = -axis;
            }

            Quaternion rotater = Quaternion.FromToRotation(Vector3.up, axis.Value);

            Vector3[] arr = new Vector3[resolution];
            int i = 0;
            foreach (var angle in AngleLoop(resolution,clockwise))
            {
                Vector3 unitVector = rotater*new Vector3(Mathf.Cos(angle+offset),0,Mathf.Sin(angle+offset));
                arr[i] = unitVector;
                i++;
            }

            return arr;
        }

        /// <summary>
        /// Generates an array of positions to list items in world space, the items will spread out from the center.
        /// </summary>
        /// <param name="cellAmount"></param>
        /// <param name="center"></param>
        /// <param name="tangent"></param>
        /// <returns></returns>
        public static Vector3[] CenteredLine(int cellAmount,float margin, Vector3 center, Vector3 tangent){



            float totalLength = margin*(cellAmount-1f);
            tangent = tangent.normalized;
            Vector3 start = center-(totalLength/2f)*tangent;
            Vector3[] arr = new Vector3[cellAmount];
            for(int i = 0; i <cellAmount;i++){
                arr[i] = start + i*(tangent*margin);
            }
            return arr;
        }

        /// <summary>
        /// Generates a grid centered at center, with cellAmount, using maxCellX and maxCellY for column and row maximums.
        /// </summary>
        /// <param name="cellAmount"></param>
        /// <param name="maxCellX"></param>
        /// <param name="maxCellY"></param>
        /// <param name="margin"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        public static Vector3[] CenteredGrid(int cellAmount,int maxCellX, float margin, Vector3 center,bool averaged = false){

            List<Vector3> list = new List<Vector3>();

            int remaining = cellAmount;
            float y_offset = 0;
            while(remaining>maxCellX){
                Vector3[] positions = CenteredLine(maxCellX,margin,center-Vector3.up*y_offset,Vector3.forward);
                list.AddRange(positions);
                y_offset += margin;
                remaining -= maxCellX;
            }
            if(remaining>0){
                Vector3[] positions = CenteredLine(remaining,margin,center-Vector3.up*y_offset,Vector3.forward);
                list.AddRange(positions);
            }

            if(averaged){
                Vector3 average = list.Average();
                Vector3 offset = center-average;
                list.Offset(offset);
            }else{
                Vector3 max = Vector3.one*-10000;
                Vector3 min = Vector3.one*10000;
                foreach (var item in list)
                {
                    if(item.x>max.x){
                        max.x = item.x;
                    }
                    if(item.y>max.y){
                        max.y = item.y;
                    }
                    if(item.z>max.z){
                        max.z = item.z;
                    }

                    if(item.x<min.x){
                        min.x = item.x;
                    }
                    if(item.y<min.y){
                        min.y = item.y;
                    }
                    if(item.z<min.z){
                        min.z = item.z;
                    }
                }
                Vector3 cent = (min+max)/2f;
                Vector3 offset = center-cent;
                list.Offset(offset);
            }

            return list.ToArray();

        }


        public static Vector3[] CenteredGrid(int cellAmount,int maxCellX, float marginx,float marginy, Vector3 center,Vector3? tangent = null, Vector3? up = null,bool averaged = false){
            if(up==null)
                up = Vector3.up;

            if(tangent==null){
                tangent = Vector3.right;
            }
            List<Vector3> list = new List<Vector3>();

            Vector3 back = (-Vector3.Cross(up.Value,tangent.Value)).normalized;


            int remaining = cellAmount;
            float y_offset = 0;
            while(remaining>maxCellX){
                Vector3[] positions = CenteredLine(maxCellX,marginx,center-back*y_offset,tangent.Value);
                list.AddRange(positions);
                y_offset += marginy;
                remaining -= maxCellX;
            }
            if(remaining>0){
                Vector3[] positions = CenteredLine(remaining,marginx,center-back*y_offset,Vector3.right);
                list.AddRange(positions);
            }

           
            if(averaged){
                Vector3 average = list.Average();
                Vector3 offset = center-average;
                list.Offset(offset);
            }else{
                Vector3 max = Vector3.one*-10000;
                Vector3 min = Vector3.one*10000;
                foreach (var item in list)
                {
                    if(item.x>max.x){
                        max.x = item.x;
                    }
                    if(item.y>max.y){
                        max.y = item.y;
                    }
                    if(item.z>max.z){
                        max.z = item.z;
                    }

                    if(item.x<min.x){
                        min.x = item.x;
                    }
                    if(item.y<min.y){
                        min.y = item.y;
                    }
                    if(item.z<min.z){
                        min.z = item.z;
                    }
                }
                Vector3 cent = (min+max)/2f;
                Vector3 offset = center-cent;
                list.Offset(offset);
            }

            return list.ToArray();

        }




    }

}