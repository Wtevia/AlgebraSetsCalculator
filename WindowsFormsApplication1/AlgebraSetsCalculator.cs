using System;
using System.Collections.Generic;
using System.Linq;

namespace WindowsFormsApplication1
{
    public class AlgebraSetsCalculator
    {
        public List<string> setU = new List<string>();
        public List<string> setA = new List<string>();
        public List<string> setB = new List<string>();
        public List<string> setC = new List<string>();

        public AlgebraSetsCalculator() { }
        public AlgebraSetsCalculator(List<string> setA, List<string> setB, List<string> setC)
        {
            this.setA.AddRange(setA);
            this.setB.AddRange(setB);
            this.setC.AddRange(setC);

            for (int i = 0; i < setA.Count; i++)
            {
                if (!setU.Contains(setA[i]))
                    setU.Add(setA[i]);
            }
            for (int i = 0; i < setB.Count; i++)
            {
                if (!setU.Contains(setB[i]))
                    setU.Add(setB[i]);
            }
            for (int i = 0; i < setC.Count; i++)
            {
                if (!setU.Contains(setC[i]))
                    setU.Add(setC[i]);
            }
        }

        //Next 4 functions make operation with sets
        private List<string> getCompliment(List<string> set)
        {
            List<string> resultSet = new List<string>();
            for (int i = 0; i < setU.Count; i++)
            {
                if (!set.Contains(setU[i]) && !setU[i].Equals("null"))
                    resultSet.Add(setU[i]);
            }
            return resultSet;
        }

        private List<string> getUnion(List<string> setA, List<string> setB)
        {
            List<string> resultSet = new List<string>();
            for (int i = 0; i < setA.Count; i++)
            {
                if (!resultSet.Contains(setA[i]))
                    resultSet.Add(setA[i]);
            }
            for (int i = 0; i < setB.Count; i++)
            {
                if (!resultSet.Contains(setB[i]))
                    resultSet.Add(setB[i]);
            }
            return resultSet;
        }

        private List<string> getIntersection(List<string> setA, List<string> setB)
        {
            List<string> resultSet = new List<string>();
            for (int i = 0; i < setA.Count; i++)
            {
                if (setB.Contains(setA[i]))
                    resultSet.Add(setA[i]);
            }
            return resultSet;
        }

        private List<string> getDifference(List<string> setA, List<string> setB)
        {
            List<string> resultSet = new List<string>();
            for (int i = 0; i < setA.Count; i++)
            {
                if (!setB.Contains(setA[i]))
                    resultSet.Add(setA[i]);
            }
            return resultSet;
        }

        //This function returns result set calculated using expr as formula 
        public List<string> getSetByExpression(string expr)
        {
            //+ is union, - is difference, / is intersection, ^ is compliment
            string OPERATORS = "+-/^";
            List<char> stack = expr.ToCharArray().ToList();
            List<string> stackOperations = new List<string>();
            List<string> stackRPN = new List<string>();
            List<string> resultStack = new List<string>();
            expr.Substring(1, expr.Length - 2);
            for (int i = 0; i < stack.Count; i++)
            {
                if (stack[i].Equals('('))
                {
                    stackOperations.Add(stack[i].ToString());
                }
                else if (stack[i].Equals(')'))
                {
                    while (!stackOperations[stackOperations.Count - 1].Equals("("))
                    {
                        stackRPN.Add(stackOperations[stackOperations.Count - 1]);
                        stackOperations.RemoveAt(stackOperations.Count - 1);
                    }
                    stackOperations.RemoveAt(stackOperations.Count - 1);
                }
                else if (OPERATORS.Contains(stack[i]))
                {
                    while (stackOperations.Count != 0 &&
                        OPERATORS.Contains(stackOperations[stackOperations.Count - 1]) &&
                        getPriority(stack[i].ToString()) < getPriority(stackOperations[stackOperations.Count - 1]))
                    {
                        stackRPN.Add(stackOperations[stackOperations.Count - 1]);
                        stackOperations.RemoveAt(stackOperations.Count - 1);
                    }
                    stackOperations.Add(stack[i].ToString());
                }
                else if (stack[i].Equals('A') || stack[i].Equals('B') || stack[i].Equals('C'))
                {
                    stackRPN.Add(stack[i].ToString());
                }
            }
            stackOperations.Reverse();
            stackRPN.AddRange(stackOperations);
            try
            {
                for (int i = 0; i < stackRPN.Count; i++)
                {
                    string param1 = "";
                    string param2 = "";
                    switch (stackRPN[i])
                    {
                        case "A":
                            resultStack.Add(string.Join(" ", setA));
                            break;
                        case "B":
                            resultStack.Add(string.Join(" ", setB));
                            break;
                        case "C":
                            resultStack.Add(string.Join(" ", setC));
                            break;
                        case "+":
                            param1 = resultStack[resultStack.Count - 1];
                            resultStack.RemoveAt(resultStack.Count - 1);
                            param2 = resultStack[resultStack.Count - 1];
                            resultStack.RemoveAt(resultStack.Count - 1);
                            resultStack.Add(string.Join(" ", getUnion(param2.Split(' ').ToList(), param1.Split(' ').ToList())));
                            break;
                        case "-":
                            param1 = resultStack[resultStack.Count - 1];
                            resultStack.RemoveAt(resultStack.Count - 1);
                            param2 = resultStack[resultStack.Count - 1];
                            resultStack.RemoveAt(resultStack.Count - 1);
                            resultStack.Add(string.Join(" ", getDifference(param2.Split(' ').ToList(), param1.Split(' ').ToList())));
                            break;
                        case "/":
                            param1 = resultStack[resultStack.Count - 1];
                            resultStack.RemoveAt(resultStack.Count - 1);
                            param2 = resultStack[resultStack.Count - 1];
                            resultStack.RemoveAt(resultStack.Count - 1);
                            resultStack.Add(string.Join(" ", getIntersection(param2.Split(' ').ToList(), param1.Split(' ').ToList())));
                            break;
                        case "^":
                            param1 = resultStack[resultStack.Count - 1];
                            resultStack.RemoveAt(resultStack.Count - 1);
                            resultStack.Add(string.Join(" ", getCompliment(param1.Split(' ').ToList())));
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return resultStack;
        }

        private int getPriority(string s)
        {
            if (s.Equals("^"))
                return 3;
            if (s.Equals("/"))
                return 2;
            if (s.Equals("+"))
                return 1;
            if (s.Equals("(") || s.Equals(")"))
                return 0;
            return -1;
        }
    }
}
