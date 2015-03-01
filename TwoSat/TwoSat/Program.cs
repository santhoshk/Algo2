//In this assignment you will implement one or more algorithms for the 2SAT problem. Here are 6 different 2SAT instances: #1 #2 #3 #4 #5 #6.
//The file format is as follows. In each instance, the number of variables and the number of clauses is the same, and this number is specified on the first line of the file. 
//Each subsequent line specifies a clause via its two literals, with a number denoting the variable and a "-" sign denoting logical "not". 
//For example, the second line of the first data file is "-16808 75250", which indicates the clause ¬x16808∨x75250.

//Your task is to determine which of the 6 instances are satisfiable, and which are unsatisfiable. 
//In the box below, enter a 6-bit string, where the ith bit should be 1 if the ith instance is satisfiable, and 0 otherwise. 
//For example, if you think that the first 3 instances are satisfiable and the last 3 are not, then you should enter the string 111000 in the box below.

//DISCUSSION: This assignment is deliberately open-ended, and you can implement whichever 2SAT algorithm you want. 
//For example, 2SAT reduces to computing the strongly connected components of a suitable graph 
//(with two vertices per variable and two directed edges per clause, you should think through the details). 
//This might be an especially attractive option for those of you who coded up an SCC algorithm for Part I of this course. 
//Alternatively, you can use Papadimitriou's randomized local search algorithm. 
//(The algorithm from lecture might be too slow, so you might want to make one or more simple modifications to it to ensure that it runs in a reasonable amount of time.) 
//A third approach is via backtracking. In lecture we mentioned this approach only in passing; see the DPV book, for example, for more details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoSat {
    class Clause {
        public Clause(int v1, int v2) {
            var1 = v1; var2 = v2;
        }
        public int var1;
        public int var2;
    }
    class Program {
        static void Main(string[] args) {
            string[] inputFiles = new string[] { "2sat1.txt", "2sat2.txt", "2sat3.txt", "2sat4.txt", "2sat5.txt", "2sat6.txt" };

            string finalOutput = "";
            foreach (string file in inputFiles) {
                Console.WriteLine("Processing File : " + file);
                //Dictionary<int, HashSet<Clause>> clausesFor = new Dictionary<int, HashSet<Clause>>();
                
                Clause[] clauses = ReadSAT(file/*, clausesFor*/);
                Console.WriteLine("Initial Length = " + clauses.Length);

                var newClauses = PreProcess2(clauses);
                Console.WriteLine("After pre processing; Length = " + newClauses.Length);

                if (IsSatisfiable(newClauses)) {
                    Console.WriteLine(" 1 ");
                    finalOutput += 1;
                } else {
                    Console.WriteLine(" 0");
                    finalOutput += 0;
                }                
            }

            Console.WriteLine("== Final output : {0} ==", finalOutput);
        }

        private static Clause[] GetClauses(Dictionary<int, HashSet<Clause>> clausesFor) {
            List<Clause> clauses = new List<Clause>();
            foreach (var cf in clausesFor) {
                if (cf.Value != null && cf.Value.Count > 0) {
                    clauses.AddRange(cf.Value);
                }
            }

            return clauses.ToArray();
        }

        private static Clause[] GetClauses2(Dictionary<int, HashSet<Clause>> clausesFor) {
            HashSet<Clause> clauses = new HashSet<Clause>();
            foreach (var cf in clausesFor) {
                if (cf.Value != null && cf.Value.Count > 0) {
                    foreach (var cl in cf.Value) {
                        if (!clauses.Contains(cl)) {
                            clauses.Add(cl);
                        } 
                    }
                }
            }

            return clauses.ToArray();
        }
        
        static Clause[] PreProcess2(Clause[] clauses) {

            Clause[] c = clauses;
            bool removed = true;

            while (removed == true) {
                Dictionary<int, bool> sameValue;
                var clausesFor = GetClausesFor(c, out sameValue);
                removed = false;
                foreach (var svp in sameValue) {
                    if (svp.Value) {

                        if (clausesFor.ContainsKey(svp.Key)) {
                            var toRemove = clausesFor[svp.Key].ToList();
                            foreach (var cl in toRemove) {
                                int otherIdx = OtherValue(cl, svp.Key);
                                clausesFor[otherIdx].Remove(cl);
                                if (clausesFor[otherIdx].Count == 0) {
                                    clausesFor.Remove(otherIdx);
                                }
                            }

                            clausesFor.Remove(svp.Key);
                            removed = true;
                        }
                    }
                }
                c = GetClauses2(clausesFor);
            }

            return c;

        }

        private static int OtherValue(Clause cl, int val1) {
            if (Math.Abs(cl.var1) == val1) {
                return Math.Abs(cl.var2);
            } else {
                return Math.Abs(cl.var1);
            }
        }

        static Dictionary<int, HashSet<Clause>> GetClausesFor(Clause[] clauses, out Dictionary<int, bool> sameValue) {
            sameValue = new Dictionary<int, bool>();
            Dictionary<int, int> value = new Dictionary<int, int>();
            Dictionary<int, HashSet<Clause>> clausesFor = new Dictionary<int, HashSet<Clause>>();


            if (clauses != null && clauses.Length > 0) {
                for (int i = 0; i < clauses.Length; i++) {
                    int absv1 = Math.Abs(clauses[i].var1);
                    int absv2 = Math.Abs(clauses[i].var2);
                    
                    if (!clausesFor.ContainsKey(absv1)) {
                        clausesFor[absv1] = new HashSet<Clause>();
                    }

                    if (!clausesFor.ContainsKey(absv2)) {
                        clausesFor[absv2] = new HashSet<Clause>();
                    }

                    clausesFor[absv1].Add(clauses[i]);
                    clausesFor[absv2].Add(clauses[i]);

                    if (!value.ContainsKey(absv1)){
                        value[absv1] = clauses[i].var1;
                        sameValue[absv1] = true;
                    }
                    if (value[absv1] != clauses[i].var1) {
                        sameValue[absv1] = false;
                    } 


                    if (!value.ContainsKey(absv2)) {
                        value[absv2] = clauses[i].var2;
                        sameValue[absv2] = true;
                    }
                    if (value[absv2] != clauses[i].var2) {
                        sameValue[absv2] = false;
                    }                    
                }
            }


            return clausesFor;
        }

        static bool IsSatisfiable(Clause[] clauses) {
            Dictionary<int, bool> vals = new Dictionary<int, bool>();            

            long max = (long) 2 * clauses.Length * clauses.Length;

            int outer = 0;
            int logN = (int)Math.Log((double)clauses.Length, 2);
            while (outer++ <= logN) {
                Console.WriteLine("\n===== Outer : {0} out of {1} ===== ", outer, logN);
                Random rand = new Random();
                Randomize(vals, clauses);

                long step = max / 100;
                int prog = 0;
                Console.WriteLine("\t==Inner==");
                long i = 0;
                while (i++ < max) {

                    if (step != 0 && i % step == 0) {
                        Console.WriteLine("\t   Progress : {0} %", prog++);
                    }
                    //get an unsatisfied clause
                    var cl = GetRandomUnsatisfiedClause(clauses, vals);

                    //randomly flip one of the vars in the unsatisfied clause
                    if (GetRandomBool(rand)) {
                        vals[Math.Abs(cl.var1)] = !vals[Math.Abs(cl.var1)];
                    } else {
                        vals[Math.Abs(cl.var2)] = !vals[Math.Abs(cl.var2)];
                    }

                    if (Satisfied(clauses, vals)) {
                        return true;
                    }
                }
            }

            return false;
        }

        

        private static void Randomize(Dictionary<int, bool> vals, Clause[] clauses) {
            Random rand = new Random();
            for (int i = 0; i < clauses.Length; i++) {
                vals[Math.Abs(clauses[i].var1)] = GetRandomBool(rand);
                vals[Math.Abs(clauses[i].var2)] = GetRandomBool(rand);
            }
        }

        private static bool GetRandomBool(Random rand) {
            return rand.Next(2) == 0;
        }

        private static Clause GetRandomUnsatisfiedClause(Clause[] clauses, Dictionary<int, bool> vals){
            Random rand = new Random();
            while(true){
                int r = rand.Next(clauses.Length);
                if (!Satisfied(clauses[r], vals)) {
                    return clauses[r];
                }
            }
        }

        private static bool Satisfied(Clause[] clauses, Dictionary<int, bool> vals) {
            foreach (var clause in clauses) {
                if (!Satisfied(clause, vals)) {
                    return false;
                }
            }
            return true;
        }

        private static bool Satisfied(Clause clause, Dictionary<int, bool> vals) {
            return
                (clause.var1 > 0 && vals[clause.var1]) ||
                (clause.var2 > 0 && vals[clause.var2]) ||
                (clause.var1 < 0 && !vals[-clause.var1]) ||
                (clause.var2 < 0 && !vals[-clause.var2]);
        }


        static Clause[] ReadSAT(string fName/*, Dictionary<int, HashSet<Clause>> clausesFor*/) {
            List<Clause> constraints = new List<Clause>();
            using (StreamReader sr = new StreamReader(fName)) {
                string line = sr.ReadLine();
                int nVars = int.Parse(line);

                //for (int i = 1; i <= nVars ; i++) {
                //    clausesFor[i] = new HashSet<Clause>();
                //}

                for (int i = 0; i < nVars; i++) {
                    var fields = sr.ReadLine().Split(' ');
                    int v1 = int.Parse(fields[0]);
                    int v2 = int.Parse(fields[1]);
                    Clause c = new Clause(v1, v2);
                    constraints.Add(c);

                    //if(!clausesFor.ContainsKey(Math.Abs(v1))) {
                    //    clausesFor[Math.Abs(v1)] = new HashSet<Clause>();
                    //}
                    //if (!clausesFor.ContainsKey(Math.Abs(v2))) {
                    //    clausesFor[Math.Abs(v2)] = new HashSet<Clause>();
                    //}
                    //clausesFor[Math.Abs(v1)].Add(c);
                    //clausesFor[Math.Abs(v2)].Add(c);
                }
            }

            return constraints.ToArray();
        }

    }
}
