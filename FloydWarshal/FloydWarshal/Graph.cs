using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphs {
    class Graph {
        private int v; //number of vertices
        private int e; //number of edges
        private Dictionary<int,Dictionary<int,int>> adjList; //adjacency list - contains weight of each u-v edge

        public Graph(int v, int e) {
            Init(v,e);
        }

        private void Init(int v, int e) {
            this.v = v;
            this.e = e;
            adjList = new Dictionary<int, Dictionary<int, int>>();
            for (int i = 1; i <= v; i++) {
                adjList[i] = new Dictionary<int, int>(); //initialize all adj lists to empty
            }
        }

        public Graph(StreamReader stream){

            var header = stream.ReadLine().Split(' ');
            int v = int.Parse(header[0]);
            int e = int.Parse(header[1]);
            Init(v, e);

            for (int i = 0; i < e; i++) {
                string edgeStr = stream.ReadLine();
                string[] edges = edgeStr.Split(' ');
                AddEdge(int.Parse(edges[0]), int.Parse(edges[1]), int.Parse(edges[2]));
            }
        }

        public int E { get { return e; } }
        public int V { get { return v; } }

        public Dictionary<int,int> Adj(int v) {
            return adjList[v];
        }

        public void AddEdge(int v, int w, int weight) {
            adjList[v].Add(w, weight);
        }

        //public override string ToString() {
        //    string s = v + " vertices, " + e + " edges\n";
        //    for (int vertex = 0; vertex < v; vertex++) {
        //        s += vertex + " : ";
        //        foreach (var w in adjList[vertex]) {
        //            s += w + " ";
        //        }
        //        s += "\n";
        //    }
        //    return s;
        //}

    }
}
    