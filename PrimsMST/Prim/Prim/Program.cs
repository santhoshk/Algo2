using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Prim {    

    class Edge {
        public Edge(int u, int v, int weight) {
            this.u = u;
            this.v = v;
            this.weight = weight;
        }
        public int u;
        public int v;
        public int weight;
    }

    class Program {
        static Dictionary<int, List<Edge>> graph = new Dictionary<int, List<Edge>>();
        //static List<Node> nodes = new List<Node>();

        static void Main(string[] args) {
            string graphFile = "edges.txt";
            ReadGraphDetails(graphFile);

            List<int> X = new List<int>();
            X.Add(1);

            List<Edge> T = new List<Edge>();

            while (X.Count < graph.Count) {
                //find the cheapest edge 'e' of G with u belonging to X and v not belonging to X.
                Edge e = null;
                int v = int.MinValue;
                int w1 = int.MaxValue;
                foreach(int u in X) {
                    foreach (Edge e1 in graph[u]) {
                        if (!X.Contains(e1.v) && e1.weight < w1) {
                            w1 = e1.weight;
                            v = e1.v;
                            e = e1;
                        }
                    }
                }

                //Assert
                if (e == null || v == int.MinValue) {
                    throw new Exception("Graph not well formed.");
                }

                //add 'e' to  T
                T.Add(e);

                //add 'v' to X
                X.Add(v);
            }

            int mstCost = 0;
            foreach (Edge e in T) {
                mstCost += e.weight;
            }

            Console.WriteLine("MST Cost : " + mstCost);
        }

        static void ReadGraphDetails(string fName) {
            using (StreamReader sr = new StreamReader(fName)) {
                string line;
                line = sr.ReadLine();
                string[] graphInfo = line.Split(' ');
                int n, e;
                if (
                    graphInfo.Length != 2 ||
                    !(int.TryParse(graphInfo[0], out n)) ||
                    !(int.TryParse(graphInfo[1], out e))
                ) {
                    throw new IOException("Graph header could not be read properly. Header line " + line);
                }

                for (int i = 0; i < e; i++) {
                    line = sr.ReadLine();
                    if (line == null) {
                        throw new IOException("Graph details truncated abruptly.");
                    }
                    int u, v, w;

                    string[] edgeInfo = line.Split(' ');
                    if (
                        edgeInfo == null ||
                        edgeInfo.Length != 3 ||
                        !int.TryParse(edgeInfo[0], out u) ||
                        !int.TryParse(edgeInfo[1], out v) ||
                        !int.TryParse(edgeInfo[2], out w)
                    ) {
                        throw new IOException("Graph details could not be parsed at line : " + line);
                    }

                    if (!graph.ContainsKey(u)) {
                        graph.Add(u, new List<Edge>());
                    }
                    if (!graph.ContainsKey(v)) {
                        graph.Add(v, new List<Edge>());
                    }
                    graph[u].Add(new Edge(u, v, w));
                    graph[v].Add(new Edge(v, u, w));
                    
                }
            }
        }
    }
}
