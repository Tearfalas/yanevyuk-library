using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YanevyukLibrary.Grid.HexagonalLinkedGrid{
    public class HexagonalPoint{
        private HexagonalEnvironment myEnvironment;
        public HexagonalPoint[] connections;
        public Vector2Int gridCoordinate;

        public Vector3 position{
            get{
                float x = 0;
                if(gridCoordinate.y%2==0){
                    x = Mathf.Lerp(myEnvironment.XRange.x,myEnvironment.XRange.y,gridCoordinate.x/myEnvironment.XRes);
                }else{
                    x = Mathf.Lerp(myEnvironment.XRange.x,myEnvironment.XRange.y,gridCoordinate.x/myEnvironment.XRes)-
                    myEnvironment.XCell/2f;
                }
                return new Vector3(
                    x+myEnvironment.XCell/2f,
                    0,
                    Mathf.Lerp(myEnvironment.YRange.x,myEnvironment.YRange.y,gridCoordinate.y/(myEnvironment.YRes-1f)));
            }
        }
        public HexagonalPoint(in HexagonalEnvironment myEnvironment, Vector2Int gridCoordinate)
        {
            this.myEnvironment = myEnvironment;
            this.gridCoordinate = gridCoordinate;
        }

        public void SetLinks(ref List<HexagonalPoint> links){
            connections = new HexagonalPoint[links.Count];
            for(int i = 0; i<links.Count;i++){
                connections[i] = links[i];
            }
        }
        
    }

    public class HexagonalEnvironment{
        
        public readonly Dictionary<Vector2Int,HexagonalPoint> points;
        private Vector2 xrange;
        public Vector2 XRange{
            get{
                return xrange;
            }
        }
        private Vector2 yrange;
        public Vector2 YRange{
            get{
                return yrange;
            }
        }
        private int xresolution;
        public float XRes{
            get{
                return xresolution+0.0f;
            }
        }
        private int yresolution;
        public float YRes{
            get{
                return yresolution+0.0f;
            }
        }

        private float xcellsize;
        public float XCell{
            get{
                return xcellsize;
            }
        }

        public HexagonalEnvironment(Vector2 xrange, Vector2 yrange, int xresolution, int yresolution)
        {
            this.points = new Dictionary<Vector2Int, HexagonalPoint>();
            this.xrange = xrange;
            this.xcellsize = Mathf.Abs(xrange.x-xrange.y)/(xresolution+0.0f);
            this.yrange = yrange;
            this.xresolution = xresolution;
            this.yresolution = yresolution;
            GeneratePoints();
            GenerateLinks();
        }

        private void GeneratePoints(){
            for(int y = 0; y<yresolution;y++){
                for(int x = 0;x < (y%2==0 ? xresolution : xresolution+1);x++){
                    HexagonalPoint point = new HexagonalPoint(this,new Vector2Int(x,y));
                    this.points.Add(new Vector2Int(x,y),point);
                }
            }
            
        }

        public HexagonalPoint GetPoint(Vector2Int gridPosition){
            if(isValid(gridPosition)){
                return points[gridPosition];
            }
            return null;
        }

        public bool isValid(Vector2Int point){
            return points.ContainsKey(point);
            if(point.x<0 || point.y<0){
                return false;
            }
            if(point.y%2==0 && point.x>=xresolution){
                return false;
            }
            if(point.y%2!=0 && point.x>=xresolution+1){
                return false;
            }
            return true;
        }
        private void GenerateLinks(){
            foreach (var point in points)
            {
                Vector2Int gc = point.Value.gridCoordinate;
                Vector2Int[] candidates;
                if(gc.y%2==0){
                    candidates = new Vector2Int[6]{
                        new Vector2Int(gc.x+1,gc.y),
                        new Vector2Int(gc.x+1,gc.y+1),
                        new Vector2Int(gc.x,gc.y+1),
                        new Vector2Int(gc.x-1,gc.y),
                        new Vector2Int(gc.x,gc.y-1),
                        new Vector2Int(gc.x+1,gc.y-1)
                    };
                }else{
                    candidates = new Vector2Int[6]{
                        new Vector2Int(gc.x+1,gc.y),
                        new Vector2Int(gc.x,gc.y+1),
                        new Vector2Int(gc.x-1,gc.y+1),
                        new Vector2Int(gc.x-1,gc.y),
                        new Vector2Int(gc.x-1,gc.y-1),
                        new Vector2Int(gc.x,gc.y-1)
                    };
                }
                List<HexagonalPoint> links = new List<HexagonalPoint>();
                foreach (var item in candidates)
                {
                    if(isValid(item)){
                        links.Add(points[item]);
                    }
                }
                point.Value.SetLinks(ref links);
            }
        }

        
    }
}

