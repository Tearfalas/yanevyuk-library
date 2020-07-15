using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YanevyukLibrary
{
    public static class YRandom
    {

      public static Vector2 pointInUnitCircle(){
          float r = Random.value;
          float a = Random.value*Mathf.PI*2;

          float x = Mathf.Cos(a)*r;
          float y = Mathf.Sin(a)*r;

          return new Vector2(x,y);

      }
    }

}