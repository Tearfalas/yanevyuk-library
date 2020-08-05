using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;


namespace  YanevyukLibrary.Pathfinding.AStar
{
    public  struct AStarNode<T>{
        public T data;

        //Links managed one by one for constant size.
        public int link1;
        public int link2;
        public int link3;
        public int link4;
        public int link5;
        public int link6;
        public int link7;
        public int link8;

        public AStarNode(T data = default(T))
        {
            this.data = data;
            this.link1 = -1;
            this.link2 = -1;
            this.link3 = -1;
            this.link4 = -1;
            this.link5 = -1;
            this.link6 = -1;
            this.link7 = -1;
            this.link8 = -1;
        }

        public int this[int index]
        {
            get{
                switch (index)
                {
                    case 0:
                        return link1;
                    case 1:
                        return link2;
                    case 2:
                        return link3;
                    case 3:
                        return link4;
                    case 4:
                        return link5;
                    case 5:
                        return link6;
                    case 6:
                        return link7;
                    case 7:
                        return link8;
                }
                
                return 0;
            }
            set{
                switch (index)
                {
                    case 0:
                        link1 = value;
                        break;
                    case 1:
                        link2 = value;
                        break;
                    case 2:
                        link3 = value;
                        break;
                    case 3:
                        link4 = value;
                        break;
                    case 4:
                        link5 = value;
                        break;
                    case 5:
                        link6 = value;
                        break;
                    case 6:
                        link7 = value;
                        break;
                    case 7:
                        link8 = value;
                        break;
                }
            }
        }

        public int[] links{
            get{
                return new int[]{link1,link2,link3,link4,link5,link6,link7,link8};
            }
        }

        public void SetNextLink(int value){
            if(this.link1 == -1){ this.link1 = value; return;};
            if(this.link2 == -1){ this.link2 = value; return;};
            if(this.link3 == -1){ this.link3 = value; return;};
            if(this.link4 == -1){ this.link4 = value; return;};
            if(this.link5 == -1){ this.link5 = value; return;};
            if(this.link6 == -1){ this.link6 = value; return;};
            if(this.link7 == -1){ this.link7 = value; return;};
            if(this.link8 == -1){ this.link8 = value; return;};
        }

    }

    public struct TempNode : IEquatable<TempNode>
    {
        public int index;
        public int activeParent;
        public bool made;
        public float H;
        public float G;
        public float F;

        public TempNode(int index)
        {
            this.index = index;
            this.activeParent = -1;
            this.made = false;
            this.H = 0;
            this.G = 0;
            this.F = 0;
        }

        public bool Equals(TempNode other)
        {
            return this.index == other.index;
        }
    }
    public class AStarEnvironment<T>{
        private NativeArray<AStarNode<T>> nodes;
        private HashSet<int> staticBlocks;


        public AStarNode<T> this[int index]
        {
            get{
                return nodes[index];
            }
            set{
                nodes[index] = value;
            }
        }

        /// <summary>
        /// Constructs a new A* environment from points and links, points can be anything, most likely a vector3 or a vector2int for a grid.
        /// links structure is as follows: 
        ///     links = [5,2,1,0,3,2]
        ///     this means that there is a connection between 5 and 2, 1 and 0, 3 and 2 depending on the index in the first list.
        /// </summary>
        /// <param name="possiblePoints"></param>
        /// <param name="links"></param>
        public AStarEnvironment(ICollection<T> possiblePoints,IList<int> links,Distance distanceFunction,ICollection<int> staticBlocks = null){
            if(links.Count%2 != 0){
                Debug.LogError("AStarEnvironment constructor : links collection isn't paired correctly. Odd number of elements.");
            }
            this.staticBlocks = new HashSet<int>();
            nodes = new NativeArray<AStarNode<T>>(possiblePoints.Count,Allocator.Persistent,NativeArrayOptions.UninitializedMemory);
            this.distanceFunction=distanceFunction;
            int index = 0;
            foreach (var item in possiblePoints)
            {
                AStarNode<T> node = new AStarNode<T>(item);

                nodes[index] = node;
                index++;
            }
            Debug.Log("Constructing links.");
            for(int i = 0;i<links.Count;i+=2){
                int first = links[i];
                int second = links[i+1];

                AStarNode<T> node1 = nodes[first];
                node1.SetNextLink(second);
                AStarNode<T> node2 = nodes[second];
                node2.SetNextLink(first);

                nodes[first] = node1;
                nodes[second] = node2;

                Debug.Log("\t "+first+"<->"+second);
                
            }
            /*
            IEnumerator enumerator = links.GetEnumerator();
            //enumerator.Reset();
            while(true){
                enumerator.MoveNext();
                int first = (int)enumerator.Current;
                bool end = !enumerator.MoveNext();
                int second = (int)enumerator.Current;
                //Setting the links
                AStarNode<T> node1 = nodes[first];
                node1.SetNextLink(second);
                AStarNode<T> node2 = nodes[second];
                node1.SetNextLink(first);
                if(end){
                    break;
                }
            }*/

            if(staticBlocks!=null){
                foreach (var item in staticBlocks)
                {
                    this.staticBlocks.Add(item);
                }
            }

            Debug.Log("Constructed AStarEnvironment.");
            int k = 0;
            foreach (var item in nodes)
            {
                Debug.Log("Node {"+k+"}: "+item.data);
                Debug.Log("Links: ");
                foreach (var l in item.links)
                {
                    Debug.Log("\t->"+l);
                }

                k++;
            }
        }

        public delegate int Distance(T first,T second);
        private Distance distanceFunction;

        /// <summary>
        /// Resets the distance values of all nodes.
        /// </summary>
        public void ResetValues(){
            for(int i = 0;i<nodes.Length;i++){
                AStarNode<T> node1 = nodes[i];
                nodes[i] = node1;
            }
        }

        private int GetSmallestF(ref Dictionary<int,TempNode> openList){
            int smallest = 0;
            float smallestF = 10000;
            float smallestG = 10000;
            foreach (var node in openList)
            {
                if(node.Value.F < smallestF){
                    smallestF = node.Value.F;
                    smallestG = node.Value.G;
                    smallest = node.Key;
                }else if(node.Value.F == smallestF){
                    if(node.Value.G < smallestG){
                        smallestF = node.Value.F;
                        smallestG = node.Value.G;
                        smallest = node.Key;
                    }
                }
            }
            return smallest;
        }

      

        

        public LinkedList<int> CalculatePath(int from, int to){
            Dictionary<int,TempNode> openList = new Dictionary<int,TempNode>();
            Dictionary<int,TempNode> closedList = new Dictionary<int,TempNode>();
            HashSet<int> closedListSimple = new HashSet<int>();

            TempNode start = new TempNode(from);
            start.activeParent = -1;
            openList.Add(from,start);

            while(openList.Count>0){
                int currentKey = GetSmallestF(ref openList);

                Debug.Log("Current key: "+currentKey);
                TempNode q = openList[currentKey];
                openList.Remove(currentKey);
                closedList.Add(q.index,q);

                if(q.index == to){
                    Debug.Log("Found end at: "+q.index);
                    break;
                }
                foreach (var linkIndex in nodes[q.index].links)
                {
                    
                    if(linkIndex!=-1 && !staticBlocks.Contains(linkIndex) && !closedList.ContainsKey(linkIndex)){
                        Debug.Log("\tInspecting link: "+linkIndex);

                        TempNode successor = new TempNode(linkIndex);
                        successor.activeParent = q.index;
                        successor.G = q.G + distanceFunction(nodes[linkIndex].data,nodes[q.index].data);
                        successor.H = distanceFunction(nodes[linkIndex].data,nodes[to].data);
                        successor.F = successor.G + successor.H;
                        

                        if(openList.ContainsKey(linkIndex)){ //this index is already in the open list.
                            TempNode other = openList[linkIndex];
                            if(other.F<successor.F){ //the already is existing one is cheaper, so we don't update it.
                                continue;
                            }else{
                                openList[linkIndex] = successor;
                            }
                        }else{
                            openList.Add(linkIndex,successor);
                        }

                    }
                }
            }
            content = new Dictionary<int, GUIContent>();
            foreach (var item in closedList)
            {
                content.Add(item.Key,new GUIContent(item.Value.G+"/"+item.Value.H+"/"+item.Value.F));
            }

            foreach (var item in openList)
            {
                content.Add(item.Key,new GUIContent(item.Value.G+"/"+item.Value.H+"/"+item.Value.F));
            }

            LinkedList<int> path = new LinkedList<int>();
            TempNode node = new TempNode(1);
            node.activeParent = to;
            while(node.activeParent!=-1){
                node = closedList[node.activeParent];
                path.AddFirst(node.index);
            }
           
            return path;
        }

        public Dictionary<int,GUIContent> content;

        ~AStarEnvironment(){
            this.nodes.Dispose();
            Debug.Log("Destroying");
        }
        
    }
    
}
