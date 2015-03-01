using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsp {
    class Point {
        public double x;
        public double y;
    }
    class Program {
        static int n;
        static Dictionary<int, double>[] A;
        static void Main(string[] args) {
            List<Point> points = ParseInput("tsp.txt");
            n = points.Count;

            Console.WriteLine("== Initializing subsets of all lengths ==");
            InitSubSetsOfAllLengths(n);

            int totS = (int)Math.Pow(2,n);
            //double[][] A = new double[totS][];
            A = new Dictionary<int, double>[totS];

            int progress = 0;
            int step = totS / 100;

            //for (int i = 1; i < totS; i++) {
            //    if (i % step == 0) {
            //        Console.WriteLine("Progress = {0} %  Memory = {1} MB", progress++, System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64/1024/1024);
            //    }
            //    A[i] = new Dictionary<int, double>();
            //}

            Console.WriteLine("== Creating memory for the A array ==");
            ////instead of A[S,n] we have A[n,S] to control the memory
            //for (int i = 1; i <= n; i++) {
            //    A[i] = new Dictionary<int, double>();
            //}
            InitAllEltsOfLen(1);

            Console.WriteLine("== Base case ==");
            //base case. using only the set S = {1} we can reach j=1 using min len = 0
            A[1][1] = 0;
            //for (int i = 2; i < totS; i++) {
            //    //using the set S = { all possible sets not containing {1}}, we 
            //    //cannot reach j=1 using any min cost length.
            //    A[i][1] = double.MaxValue;
            //}

            Console.WriteLine("== Workhorse ==");
            for (int m = 2; m <= n; m++) {
                if (m >= 3) {
                    DeleteAllEltsOfLen(m - 2);
                }
                InitAllEltsOfLen(m);
                GC.Collect();
                Console.WriteLine("\t Workhorse : m = " + m);
                
                List<int> subsetsOfLenM = GetSubSetsOfLenM(m);
                Console.WriteLine("\t Got subsets of length m = " + subsetsOfLenM.Count);
                progress = 0;
                step = subsetsOfLenM.Count / 100;

                for (int k = 0; k < subsetsOfLenM.Count; k++) {

                    if ((step!= 0) && (k % step == 0)) {
                        Console.WriteLine("\tProgress = {0} %  Memory = {1} MB", progress++, System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64 / 1024 / 1024);
                    }


                //foreach (int S in subsetsOfLenM) {
                    if(Contains(subsetsOfLenM[k],1)) {
                        List<int> allJsBelongingToS = GetAllPointsBelongingToS(subsetsOfLenM[k]);
                        //byte[] allJsBelongingToS = allPsInS[subsetsOfLenM[k]];
                        for (int i = 0; i < allJsBelongingToS.Count; i++) {
                            
                            if (allJsBelongingToS[i] != 1) {
                                A[subsetsOfLenM[k]][allJsBelongingToS[i]] = CalculateMinAsj(A, subsetsOfLenM[k], allJsBelongingToS[i], points);
                            }
                        }
                        //foreach (int j in allJsBelongingToS) {
                        //    if (j != 1) {
                        //        A[j][S] = CalculateMinAsj(A, S, j, points);
                        //    }
                        //}
                    }
                }
            }

            Console.WriteLine("== Final Roundup==");
            //final roundup
            double minCost = double.MaxValue;
            for (int j = 2; j <= n; j++) {
                //double currCost = A[j][MaxS(totS)] + EuclideanDist(points[j - 1], points[0]);
                double currCost = A[MaxS(totS)][j] + distanceOf[points[j - 1]][points[0]];
                if (minCost < currCost) {
                    minCost = currCost;
                }
            }

            Console.WriteLine("Min Cost = " + minCost);
        }

        static void DeleteAllEltsOfLen(int len) {
            List<int> toDel = subSetsOfLength[len];
            foreach (int td in toDel) {
                A[td] = null;
            }
        }

        static void InitAllEltsOfLen(int len) {
            List<int> toCreate = subSetsOfLength[len];
            foreach (int td in toCreate) {
                A[td] = new Dictionary<int, double>();
            }
        }


        private static int MaxS(int totS) {
            return totS - 1;
        }

        private static double CalculateMinAsj(Dictionary<int, double>[] A, int S, int j, List<Point> points) {
            double minAsj = double.MaxValue;
            IEnumerable<int> allKs = GetAllPointsBelongingToS(S);
            //byte[] allKs = allPsInS[S];
            foreach (int k in allKs) {
                if (k != j) {
                    int sMinusj = MinusFromSet(S, j);
                    //double currMin = A[k][sMinusj] + EuclideanDist(points[k - 1], points[j - 1]);
                    double storedA;
                    if (A[sMinusj] == null || (!A[sMinusj].ContainsKey(k))) {
                        storedA = double.MaxValue;
                    } else {
                        storedA = A[sMinusj][k];
                    }
                    //double currMin = A[k][sMinusj] + distanceOf[points[k - 1]][points[j - 1]];
                    if (storedA < double.MaxValue) {
                        double currMin = storedA + distanceOf[points[k - 1]][points[j - 1]];
                        if (currMin < minAsj) {
                            minAsj = currMin;
                        }
                    }
                }
            }
            return minAsj;
        }

        private static int MinusFromSet(int S, int j) {
            //if (j == 0) {
            //    throw new Exception("Cannot minus 0 from the set.");
            //}
            //if ((S & (1 << (j-1))) == 0) {
            //    throw new Exception("The position j is not set in S.");
            //}
            return S - (1 << (j-1));
        }

        private static List<int> GetAllPointsBelongingToS(int S) {
            List<int> points = new List<int>();
            int shift = n-1;
            while (shift >= 0) {
                if ((S & (1 << shift)) != 0) {
                    points.Add(shift+1);
                }
                shift--;
            }
            return points;
        }

        private static bool Contains(int S, int p) {
            return (S & 1) != 0;
        }

        private static List<int> GetSubSetsOfLenM(int m) {
            return subSetsOfLength[m];
        }

        static List<Point> ParseInput(string fName) {
            List<Point> points = new List<Point>();
            using (StreamReader sr = new StreamReader(fName)) {
                int n =  int.Parse(sr.ReadLine());
                string line ;
                while ((line = sr.ReadLine()) != null) {
                    Point p = new Point();
                    p.x = double.Parse(line.Split(' ')[0]);
                    p.y = double.Parse(line.Split(' ')[1]);
                    points.Add(p);
                }
            }

            for (int i = 0; i < points.Count; i++) {
                distanceOf.Add(points[i], new Dictionary<Point, double>());
                for (int j = 0; j < points.Count; j++) {
                    if (i == j) {
                        distanceOf[points[i]][points[j]] = 0;
                    } else {
                        distanceOf[points[i]][points[j]] = EuclideanDist(points[i], points[j]);
                    }
                }
            }

            return points;
        }

        static double EuclideanDist(Point p1, Point p2) {
            return Math.Sqrt(Math.Pow(p1.x - p2.x, 2) + Math.Pow(p1.y - p2.y, 2));
        }

        static Dictionary<Point, Dictionary<Point, double>> distanceOf = new Dictionary<Point, Dictionary<Point, double>>();

        static Dictionary<int, List<int>> subSetsOfLength = new Dictionary<int, List<int>>();
        static Dictionary<int, byte[]> allPsInS = new Dictionary<int, byte[]>();
        static void InitSubSetsOfAllLengths(int len) {
            for (int i = 1; i <= len; i++) {
                subSetsOfLength[i] = new List<int>();
            }
            int totS = (int)Math.Pow(2, n);

            int progress = 0;
            int step = totS / 100;

            for (int i = 1; i < totS; i++) {

                if ((step != 0) && (i % step == 0)) {
                    Console.WriteLine("Progress = {0} %  Memory = {1} MB", progress++, System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64 / 1024 / 1024);
                }
                int bitCard;
                List<byte> allPs = BitCardinality(i, out bitCard);
                subSetsOfLength[bitCard].Add(i);
                //allPsInS[i] = allPs.ToArray();
            }
        }

        static List<byte> BitCardinality(int x, out int bitCardinality) {
            bitCardinality = 0;
            List<byte> allPs = new List<byte>();
            byte shift = (byte)(n-1);
            while (shift != 255) {
                if ((x & (1 << shift)) != 0) {
                    bitCardinality++;
                    allPs.Add((byte)(shift + 1));
                }
                shift--;
            }
            return allPs;
        }
        
    }
}
