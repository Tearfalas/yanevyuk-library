using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parabola
{
    public Vector3 start_position;
    public Vector3 end_position;

    private float distance;

    public float height;

    public Parabola(){}
    
    private AnimationCurve parabola_height_offset = null;
    private float parabola_height_offset_multiplier;


    public void SetHeightOffsetCurve(AnimationCurve offset){
        parabola_height_offset = offset;
    }

    public void SetHeightOffsetMultiplier(float m){
        parabola_height_offset_multiplier = m;
    }

    public void ConstructParabola(Vector3 start_pos, Vector3 end_pos, float h){
        start_position = start_pos;
        end_position = end_pos;
        height = h;
        distance = Vector3.Distance(start_position,end_position);
    }

    public Vector3 GetPositionAt(float t){
        float x;
        float y;
        float z;
        x = Mathf.Lerp(start_position.x,end_position.x,t);
        z = Mathf.Lerp(start_position.z,end_position.z,t);
        y = -t*(t-1)*4*height + Mathf.Lerp(start_position.y,end_position.y,t);
        if(parabola_height_offset != null){
            y += (parabola_height_offset.Evaluate(t)*parabola_height_offset_multiplier);
        }
        return new Vector3(x,y,z);
    }

    public Vector3 GetPositionAtUnclamped(float t){
        float x;
        float y;
        float z;
        x = Mathf.LerpUnclamped(start_position.x,end_position.x,t);
        z = Mathf.LerpUnclamped(start_position.z,end_position.z,t);
        y = -t*(t-1)*4*height + Mathf.LerpUnclamped(start_position.y,end_position.y,t);
        if(parabola_height_offset != null){
            y += parabola_height_offset.Evaluate(t)*parabola_height_offset_multiplier;
        }
        return new Vector3(x,y,z);
    }

    public Vector3 GetDirectionAt(float t){
        Vector3 prev = GetPositionAtUnclamped(t-0.05f);
        Vector3 next = GetPositionAtUnclamped(t+0.05f);

        return (next-prev).normalized;
    }

    public float GetShortestDistance(){
        return Vector3.Distance(start_position,end_position);
    }
}
