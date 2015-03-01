//In this programming problem and the next you'll code up the knapsack algorithm from lecture. 
//Let's start with a warm-up. Download the text file here. This file describes a knapsack instance, 
//and it has the following format:
// [knapsack_size][number_of_items]
// [value_1] [weight_1]
// [value_2] [weight_2]
// ...
//For example, the third line of the file is "50074 659", indicating that the second item has value 50074 and size 659, respectively. 
//You can assume that all numbers are positive. You should assume that item weights and the knapsack capacity are integers. 

//In the box below, type in the value of the optimal solution. 


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnapsackSmall
{
    class Program
    {
        private static int[] value;
        private static int[] weight;
        private static int W=0, N=0;
        static void Main(string[] args)
        {
            ReadFromFile("knapsack1.txt");
            
            //The items are 1-indexed in the algorithm and not zero indexed.
            //So A[1][5] - refers to the max value of using only 1st 1 elt and tot cap = 5
            int[][] A = new int[N+1][];
            for (int i = 0; i < N+1; i++)
            {
                A[i] = new int[W];
            }

            for (int x = 0; x < W; x++)
            {
                A[0][x] = 0;
            }

            for (int i = 1; i < N+1; i++)
            {
                for (int x = 0; x < W; x++)
                {
                    int c1=0, c2=0;
                    c1 = A[i - 1][x];
                    if (x >= weight[i])
                    {
                        c2 = A[i - 1][x - weight[i]] + value[i];
                    }
                    A[i][x] = Max(c1, c2);
                }
            }

            Console.WriteLine("Max knapsack capacity : " + A[N][W-1]);
        }

        static int Max(int a, int b)
        {
            return a >= b ? a : b;
        }

        private static void ReadFromFile(string fileName)
        {
            using (StreamReader sr = new StreamReader(fileName))
            {
                string line = "";
                line = sr.ReadLine();
                string[] header = line.Split(' ');
                W = int.Parse(header[0]);
                N = int.Parse(header[1]);
                value = new int[N+1];
                weight = new int[N+1];
                for (int i = 1; i < N+1; i++)
                {
                    line = sr.ReadLine();
                    string[] lineItem = line.Split(' ');
                    value[i] = int.Parse(lineItem[0]);
                    weight[i] = int.Parse(lineItem[1]);
                }
            }
        }
    }
}
