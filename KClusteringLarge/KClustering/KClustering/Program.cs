//In this question your task is again to run the clustering algorithm from lecture, but on a MUCH bigger graph. 
//So big, in fact, that the distances (i.e., edge costs) are only defined implicitly, rather than being provided 
//as an explicit list.
//The data set is here. The format is:
//[# of nodes] [# of bits for each node's label]
//[first bit of node 1] ... [last bit of node 1]
//[first bit of node 2] ... [last bit of node 2]
//...
//For example, the third line of the file "0 1 1 0 0 1 1 0 0 1 0 1 1 1 1 1 1 0 1 0 1 1 0 1" denotes the 24 bits 
//associated with node #2.

//The distance between two nodes u and v in this problem is defined as the Hamming distance--- the number of 
//differing bits --- between the two nodes' labels. For example, the Hamming distance between the 24-bit label 
//of node #2 above and the label "0 1 0 0 0 1 0 0 0 1 0 1 1 1 1 1 1 0 1 0 0 1 0 1" is 3 (since they differ in the 
//3rd, 7th, and 21st bits).

//The question is: what is the largest value of k such that there is a k-clustering with spacing at least 3? 
//That is, how many clusters are needed to ensure that no pair of nodes with all but 2 bits in common get split 
//into different clusters?

//NOTE: The graph implicitly defined by the data file is so big that you probably can't write it out explicitly, 
//let alone sort the edges by cost. So you will have to be a little creative to complete this part of the question. 
//For example, is there some way you can identify the smallest distances without explicitly looking at every pair of nodes?

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace KClustering {

    class UnionFind {
        int[] parent;
        int[] children;
        int totalComponents;
        public UnionFind(int numberOfComponents) {
            totalComponents = numberOfComponents;
            parent = new int[numberOfComponents+1];
            children = new int[numberOfComponents + 1];

            for (int i = 0; i < numberOfComponents + 1; i++) {
                parent[i] = i;
                children[i] = 1;
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

                //default lets say root1 has more children
                int ch1 = children[root1];
                int ch2 = children[root2];

                int largetRoot = root1;
                int smallerRoot = root2;
                //if root2 has more nodes then lets swap
                if (ch1 < ch2) {
                    largetRoot = root2;
                    smallerRoot = root1;
                }

                parent[smallerRoot] = largetRoot;
                children[largetRoot] += children[smallerRoot];

                //for (int j = 0; j < parent.Length; j++) {

                //}

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
        static Dictionary<string, int> labelToNodeMap = new Dictionary<string, int>();
        static void Main(string[] args) {

            List<string> labels = ReadFromFile("clustering_big.txt");
            UnionFind uf = new UnionFind(labels.Count);

            

            for(int i=0;i<labels.Count;i++) {
                string l = labels[i];
                if (labelToNodeMap.ContainsKey(l)) {
                    uf.Union(labelToNodeMap[l], i+1);
                    labelToNodeMap[l] = i+1;
                } else {
                    labelToNodeMap.Add(l, i+1);  
                }
            }

            //while min_dist >= 3 union the smallest nodes
            //after all the unions, there will be no more any 
            //2 nodes whose dist <= 2.
            for (int i = 0; i < labels.Count; i++) {
                if (i % 100 == 0) {
                    Console.WriteLine("In iteration : " + i);
                }
                
                //get all conbinations whose distance is 1 or 2 from this node
                //and there may be duplicates, so only get distinct nodes.
                IEnumerable<string> perms = GetPermutations(labels[i], 1);
                perms = perms.Concat(GetPermutations(labels[i],2));
                perms = perms.Distinct();
                
                foreach (string p in perms) {
                    if (labelToNodeMap.ContainsKey(p)) {
                        uf.Union(labelToNodeMap[p], i+1);
                    }
                }
            }

            Console.WriteLine("Max K-Clustering : " + uf.GetComponents());
        }

        private static List<string> ReadFromFile(string fileName) {
            List<string> labels = new List<string>();
            using (StreamReader sr = new StreamReader(fileName)) {
                string line;
                line = sr.ReadLine();
                string[] header = line.Split(' ');
                if (header.Length != 2) {
                    throw new Exception("Header not well formed.");
                }

                int numberOfNodes = int.Parse(header[0]);
                int bitsPerLabel = int.Parse(header[1]);
                
                while ((line = sr.ReadLine()) != null) {
                    labels.Add(line.Replace(" ","").Trim());
                }
            }

            return labels;
        }

        static IEnumerable<string> GetPermutations(string source, int distance) {
            if (distance == 0) {
                return new List<string> { source };
            }

            List<string> perms = new List<string>();
            char[] bits = source.ToCharArray();
            for (int currPos = 0; currPos < source.Length; currPos++) {
                IEnumerable<string> smallerPerms = GetPermutations(GetSmallerString(bits, currPos), distance - 1);
                foreach (string smallStr in smallerPerms) {
                    perms.Add(GetLargerString(smallStr, currPos, Toggle(bits[currPos])));
                }
            }

            return perms;
            
        }

        static char Toggle(char c) {
            if (c == '0') return '1';
            else return '0';
        }
    

        static string GetLargerString(string str, int idxToInsert, char valToInsert) {
            return str.Insert(idxToInsert, valToInsert.ToString());
        }

        static string GetSmallerString(char[] bits, int idxToIgnore) {
            char[] smallBits = new char[bits.Length-1];
            int currIdx=0;
            for (int i = 0; i < bits.Length; i++) {
                if (i != idxToIgnore) smallBits[currIdx++] = bits[i];
            }
            return new string(smallBits);
        }
    }
}
