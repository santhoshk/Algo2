//In this programming problem and the next you'll code up the clustering algorithm from lecture for computing a max-spacing k-clustering. 
//Download the text file here. This file describes a distance function (equivalently, a complete graph with edge costs). It has the following format:
//[number_of_nodes]
//[edge 1 node 1] [edge 1 node 2] [edge 1 cost]
//[edge 2 node 1] [edge 2 node 2] [edge 2 cost]
//...
//There is one edge (i,j) for each choice of 1≤i<j≤n, where n is the number of nodes. For example, the third line of the file is "1 3 5250", 
//indicating that the distance between nodes 1 and 3 (equivalently, the cost of the edge (1,3)) is 5250. 
//You can assume that distances are positive, but you should NOT assume that they are distinct.

//Your task in this problem is to run the clustering algorithm from lecture on this data set, where the target number k of clusters is set to 4. 
//What is the maximum spacing of a 4-clustering?


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace KClusteringSmall {

    class EdgeInfo {
        public EdgeInfo(int n1, int n2, int c) {
            Node1 = n1;
            Node2 = n2;
            Cost = c;
        }
        public int Node1;
        public int Node2;
        public int Cost;
    }

    class UnionFind {
        int[] parent;
        int totalComponents;
        public UnionFind(int numberOfComponents) {
            totalComponents = numberOfComponents;
            parent = new int[numberOfComponents+1];

            for (int i = 0; i < numberOfComponents+1; i++) {
                parent[i] = i;
            }

        }

        public int Find(int c) {
            int temp = c;
            while (parent[temp] != temp) {
                temp = parent[temp];
            }
            return temp;
        }

        public bool Connected(int c1, int c2) {
            return Find(c1) == Find(c2);
        }

        public bool Union(int c1, int c2) {
            int root1 = Find(c1);
            int root2 = Find(c2);
            if (root1 != root2) {
                parent[root2] = root1;
                totalComponents--;
                return true;

            } else {
                return false;
            }
        }

        public int GetComponents() {
            return totalComponents;
        }
    }

    class Program {

        static List<EdgeInfo> edges = new List<EdgeInfo>();

        internal class EdgeComparer : IComparer<EdgeInfo> {
            #region IComparer<EdgeInfo> Members

            public int Compare(EdgeInfo x, EdgeInfo y) {
                if (x == null && y == null) return 0;
                if (x == null && y != null) return -1;
                if (y == null && x != null) return 1;
                return x.Cost.CompareTo(y.Cost);
            }

            #endregion
        }

        static void Main(string[] args) {

            int n = ReadFromFile("clustering1.txt");
            
            //store all the nodes in diff clusters initially
            UnionFind uf = new UnionFind(n);

            //target number of k clusters is set to 4
            //find the max spacing of a 4 cluster

            //sort all the edges
            edges.Sort(new EdgeComparer());

            int i = 0;

            //we need to do work till there are only 4 clusters
            while (uf.GetComponents() > 4) {
                //take the smallest edge
                EdgeInfo smallestEdge = edges[i++];

                //if the 2 nodes of the edge are already connected, then nothing to do for this edge
                //else merge the 2 nodes of the edge
                if (!uf.Connected(smallestEdge.Node1, smallestEdge.Node2)) {
                    uf.Union(smallestEdge.Node1, smallestEdge.Node2);
                }
            }

            //currently i points to the next closest edge
            //the nodes of the edge may or may not be connected,
            //however we are now interested in the smallest i for which
            //the nodes are not connected (which is nothing but the max spacing)
            while (i <= edges.Count && uf.Connected(edges[i].Node1, edges[i].Node2)) { i++; }

            Console.WriteLine("Max spacing of a 4 cluster : " + edges[i].Cost);

            
        }

        private static int ReadFromFile(string fileName) {
            int numberOfNodes = 0;
            using (StreamReader sr = new StreamReader(fileName)) {
                string line;
                line = sr.ReadLine();
                numberOfNodes = int.Parse(line);

                while ((line = sr.ReadLine()) != null) {
                    string[] edge = line.Split(' ');
                    if (edge == null || edge.Length != 3) {
                        throw new Exception("Edge not well formed : " + edge);
                    }

                    int n1 = int.Parse(edge[0]);
                    int n2 = int.Parse(edge[1]);
                    int c = int.Parse(edge[2]);

                    edges.Add(new EdgeInfo(n1, n2, c));
                }
            }

            return numberOfNodes;
           
        }
    }
}
