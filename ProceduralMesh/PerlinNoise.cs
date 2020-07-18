using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YanevyukLibrary.ProceduralMesh{
    [System.Serializable]
public class PerlinNoise
{
    public float Frequency;
    public int Octaves;
    public float Scale;
    public float Persistence;
    public float Lacunarity;
    public int Seed;

    public static int xTotal=-1;
    public static int yTotal=-1;

    private OpenSimplexNoise hiddenNoise;

    public PerlinNoise(float frequency,float scale, int octaves, float persistence, float lacunarity, int? seed = null )
    {
        Frequency = frequency;
        Octaves = octaves;
        Persistence = persistence;
        Lacunarity = lacunarity;
        Scale = scale;
        if(seed != null){
            Seed = seed.Value;
        }else{
            Seed = Random.Range(0,10000);
        }
        
    }

    public void RandomizeSeed(){
        Seed = Random.Range(0,10000);
        hiddenNoise = new OpenSimplexNoise(Seed);
    }





    public float Get(float x, float y){

        if(hiddenNoise==null){
            hiddenNoise = new OpenSimplexNoise(Seed);
        }
        float totalHeight = 0;
        float divisor = 0;

        float pers = 1;
        float freq = Frequency;
        float seed = Seed/100f;

        float totalRatio = (yTotal+0.0f)/(xTotal+0.0f);



        float s = 2*Mathf.PI*freq;//Length of the line travelled by x for this frequency. 
        float xmultiplier = Mathf.PI*2f/(xTotal+0.0f);
        float ymultiplier =  Mathf.PI*4f/(yTotal+1.0f);

        float xratio = (x+0.0f)*xmultiplier;
        float yratio = Mathf.Clamp((y+0.0f)*2f/(yTotal-1.0f)-1f,-1,1);
        float magx = Mathf.Sqrt(1-yratio*yratio);
        Vector3 unitPos = new Vector3(Mathf.Cos(xratio)*magx,yratio,Mathf.Sin(xratio)*magx);
      
        for(int k = 0; k<Octaves;k++){

            
            
            totalHeight += (float)((Mathf.PerlinNoise(seed + (x*freq/Scale),seed + (y*freq/Scale))))*pers;
            divisor += pers;


            pers = pers*Persistence;
            freq = freq*Lacunarity;
            seed *= 2;
        }
        divisor = divisor == 0 ? 1 : divisor;
        return totalHeight/divisor;
    }
}
}
