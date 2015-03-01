using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphs;

namespace FloydWarshal {
    class Program {
        static void Main(string[] args) {
            Graph g = ReadGraph("g1.txt");
            var result = FloydWarshal(g);
            if (HasNegativecycle(result)) {
                Console.WriteLine("The input graph has a negative cycle.");
            } else {
                int u,v;
                int shortestUVPath = GetShortestPath(result, out u, out v);
                Console.WriteLine("Shortest Path : " + shortestUVPath);
            }
        }


        static int[][] FloydWarshal(Graph g) {
            //base case
            //for all i,j belonging to V, 
            //A[i,j,0] = 0 if i=j
            //A[i,j,0] = Cij if i,j belongs to E
            //A[i,j,0] = +Infinity if i<>j and i,j does not belong to E
            
            //define a 2D array
            int[][] A = Initialize2DArray(g.V);

            for (int i = 1; i <= g.V; i++) {
                for (int j = 1; j <= g.V; j++) {
                    if (i == j) {
                        A[i][j] = 0;
                    } else if (g.Adj(i).ContainsKey(j)) {
                        A[i][j] = g.Adj(i)[j];
                    } else {
                        A[i][j] = int.MaxValue;
                    }
                }
            }


            int progress = 0;
            int step = g.V / 100;

            //inductive step
            int[][] prevA = A;
            int[][] currA = null; 
            for (int k = 1; k <= g.V; k++) {
                if (k % step == 0) {
                    Console.WriteLine("Progress : {0} %", progress++);
                }
                currA = Initialize2DArray(g.V);
                
                for (int i = 1; i <= g.V; i++) {
                    for (int j = 1; j <= g.V; j++) {
                        currA[i][j] = Math.Min(
                            prevA[i][j],
                            Sum(prevA[i][k], prevA[k][j])
                        );
                    }
                }

                prevA = currA;                
            }

            return currA;
        }

        static int Sum(int a, int b) {
            if (a == int.MaxValue || b == int.MaxValue) {
                return int.MaxValue;
            }
            return a + b;
        }

        static bool HasNegativecycle(int[][] A) {
            for (int i = 1; i < A.Length; i++) {
                if (A[i][i] < 0) {
                    return true;
                }
            }
            return false;
        }

        static int GetShortestPath(int[][] A, out int u, out int v) {
            int min = int.MaxValue;
            u = int.MaxValue;
            v = int.MaxValue;
            for (int i = 1; i < A.Length; i++) {
                for (int j = 1; j < A.Length; j++) {
                    if (i != j) {
                        if (A[i][j] < min) {
                            min = A[i][j];
                            u = i; v = j;
                        }
                    }
                }
            }
            return min;
        }

        static int[][] Initialize2DArray(int n) {
            int[][] A = new int[n+1][];
            for (int i = 0; i < n+1; i++) {
                A[i] = new int[n+1];
            }
            return A;
        }

        static Graph ReadGraph(string graphPath) {
            Graph g;
            using (StreamReader sr = new StreamReader(graphPath)) {
                g = new Graph(sr);
            }
            return g;
        }
    }
}
