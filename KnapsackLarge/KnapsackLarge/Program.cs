//This problem also asks you to solve a knapsack instance, but a much bigger one. 
//Download the text file here. This file describes a knapsack instance, and it has the following format:
// [knapsack_size][number_of_items]
// [value_1] [weight_1]
// [value_2] [weight_2]
// ...
// For example, the third line of the file is "50074 834558", indicating that the second item has value 
//50074 and size 834558, respectively. As before, you should assume that item weights and the knapsack capacity are integers. 

//This instance is so big that the straightforward iterative implemetation uses an infeasible amount of time and space. 
//So you will have to be creative to compute an optimal solution. One idea is to go back to a recursive implementation, 
//solving subproblems --- and, of course, caching the results to avoid redundant work --- only on an "as needed" basis. 
//Also, be sure to think about appropriate data structures for storing and looking up solutions to subproblems. 

//In the box below, type in the value of the optimal solution. 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnapsackLarge {
    class Program {
        private static int[] value;
        private static int[] weight;
        private static int W = 0, N = 0;
        static void Main(string[] args) {
            ReadFromFile("knapsack_big.txt");

            //The items are 1-indexed in the algorithm and not zero indexed. So, N+1 elts.
            int[] A = new int[N + 1];

            Console.WriteLine("N = {0}, W = {1}\n\n", N, W);
            int step = N / 100;
            int progress = 0;

            int[] prevIdxInA = new int[W];
            int[] currIdxInA = new int[W];
            for (int i = 1; i < N + 1; i++) {
                //we are only ever interested in A[i] and A[i-1]
                //So no need to maintain the full 2D array A[][] in memory.
                //A[i-1][] is denoted by prevIdxInA[]
                //A[i][] is denoted by currIdxInA[]
                prevIdxInA = currIdxInA;
                currIdxInA = new int[W];
                if (i % step == 0) {
                    Console.WriteLine("Calculation Progress = {0} %", progress++);
                }

                for (int x = 0; x < W; x++) {
                    int c1 = 0, c2 = 0;
                    c1 = prevIdxInA[x];
                    if (x >= weight[i]) {
                        c2 = prevIdxInA[x - weight[i]] + value[i];
                    }
                    currIdxInA[x] = Max(c1, c2);
                }
            }

            Console.WriteLine("Max knapsack capacity : " + currIdxInA[W - 1]);
        }

        static int Max(int a, int b) {
            return a >= b ? a : b;
        }

        private static void ReadFromFile(string fileName) {
            using (StreamReader sr = new StreamReader(fileName)) {
                string line = "";
                line = sr.ReadLine();
                string[] header = line.Split(' ');
                W = int.Parse(header[0]);
                N = int.Parse(header[1]);
                value = new int[N + 1];
                weight = new int[N + 1];
                int step = N / 100;
                int progress = 0;
                for (int i = 1; i < N + 1; i++) {
                    if (i % step == 0) {
                        Console.WriteLine("Reading input Progress : {0} %", progress++);
                    }
                    line = sr.ReadLine();
                    string[] lineItem = line.Split(' ');
                    value[i] = int.Parse(lineItem[0]);
                    weight[i] = int.Parse(lineItem[1]);
                }
            }
        }
    }
}
