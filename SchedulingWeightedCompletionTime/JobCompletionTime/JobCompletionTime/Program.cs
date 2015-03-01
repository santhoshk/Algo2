using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace JobCompletionTime {
    class ScoreNode {
        public double Score;
        public int Cj;
        public int Weight;
        public int Length;
        public ScoreNode next;
        public ScoreNode prev;
    }

    class Program {
        static int[] W;
        static int[] L;
        static ScoreNode Root;

        static void Main(string[] args) {
            string jobFile = "jobs.txt";
            int numberOfJobs = ReadJobDetails(jobFile);

            #region calculate order of processing
            for (int i = 0; i < numberOfJobs; i++) {
                ScoreNode node = new ScoreNode() {
                    //Score = W[i] - L[i],
                    Score = W[i]/(double)L[i],
                    Weight = W[i],
                    Length = L[i]
                };

                if (Root == null) {
                    Root = node;
                } else {
                    ScoreNode curr = Root;
                    ScoreNode lastNode = Root;
                    bool inserted = false;
                    while((!inserted) && (curr != null)) {
                        lastNode = curr;
                        if (curr.Score > node.Score) { //new node's score is less, so it comes later in the list
                            curr = curr.next;
                        } else {
                            if (curr.Score < node.Score) { //new node's score is more than current node, so it comes before current node.
                                curr = InsertBefore(node, curr);
                                if (curr.prev == null) {
                                    Root = curr;
                                }
                                inserted = true;
                            } else { //TIE: broken based on weight
                                if (curr.Weight > node.Weight) { //new node has lower weight, so it comes after current node
                                    curr = curr.next;
                                } else { //new node has higher weight and same score as curr node, so it comes before this node
                                    curr = InsertBefore(node, curr);
                                    if (curr.prev == null) {
                                        Root = curr;
                                    }
                                    inserted = true;
                                }
                            }
                        }
                    }

                    if (!inserted) {
                        lastNode.next = node;
                        node.prev = lastNode;
                    }

                    //if (!inserted) {
                    //    if (curr.Score > node.Score) { //new node has lower score so it comes after last
                    //        curr.next = node;
                    //    } else {
                    //        if (curr.Score < node.Score) { //new node has higher score so it comes before last
                    //            InsertBefore(node, curr);
                    //        } else { //equal score; break ties based on weight
                    //            if (curr.Weight > node.Weight) { //new node has lower weight, so it comes after last
                    //                curr.next = node;
                    //            } else { //new node has higher weight, so it comes before last
                    //                InsertBefore(node, curr);
                    //            }
                    //        }
                    //    }
                    //}
                }
            }

            #endregion

            #region calculate Wj.Cj

            ScoreNode cNode = Root;
            int cLen = 0;
            while (cNode != null) {
                cLen += cNode.Length;
                cNode.Cj = cLen;
                cNode = cNode.next;
            }

            cNode = Root;
            long totWeight = 0;
            while (cNode != null) {
                totWeight += cNode.Weight * cNode.Cj;
                cNode = cNode.next;
            }

            Console.WriteLine("Total Weight = " + totWeight);

            #endregion
        }

        static ScoreNode InsertBefore(ScoreNode node, ScoreNode before) {
            node.prev = before.prev;
            node.next = before;

            if (before.prev != null) {
                before.prev.next = node;
            }
            before.prev = node;
            return node;
        }

        static int ReadJobDetails(string fName) {
            int N;
            using (StreamReader sr = new StreamReader(fName)) {
                string line;
                line = sr.ReadLine();
                int numberOfJobs;
                if (!int.TryParse(line, out numberOfJobs)) {
                    throw new IOException("Number of jobs could not be read.");
                }
                W = new int[numberOfJobs];
                L = new int[numberOfJobs];
                N = numberOfJobs;

                for (int i = 0; i < numberOfJobs; i++) {
                    line = sr.ReadLine();
                    if (line == null) {
                        throw new IOException("Job details truncated abruptly at line " + i);
                    }

                    int w, l;
                    string[] jobInfo = line.Split(' ');
                    if (
                        jobInfo.Length != 2 ||
                        !(int.TryParse(jobInfo[0], out w)) ||
                        !(int.TryParse(jobInfo[1], out l))
                    ) {
                        throw new IOException("Job details could not be read properly for the line : " + line);
                    }

                    W[i] = w; //maybe this can be set direcly via the array 'out W[i]'
                    L[i] = l;
                }
            }

            return N;
        }
    }
}
