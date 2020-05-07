using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
// using OpenQA.Selenium;
// using OpenQA.Selenium.PhantomJS;


//ruch: 0 - antagonista, 1 - protagonista

namespace Graf
{

    public class Node
    {
        public int value;
        public Node parent;
        //public List<Node> children;
        public List<Node> children = new List<Node>();
        public bool isEnd;
        public int win;
        public int level;
        public bool ruch;
        
        private int endVal = 21;
        private int[] iter = new int[] {4,5,6};
        private bool startSide = true;

        public Node()
        {
            value = 0;
            level = 0;
            ruch = startSide;
            parent = null;
            GenerateChildren();
        }
        public Node(int valueIn, Node parentIn, bool ruchIn, int levelIn)
        {
            value = valueIn;
            ruch = ruchIn;
            parent = parentIn;
            level = levelIn;

            if (value >= endVal)
            {
                isEnd = true;
                if (value == endVal)
                {
                    win = 0;
                }
                else
                {
                    if (ruch == true)
                    {
                        win = 1;
                    }
                    else
                    {
                        win = -1;
                    }
                }
            }
            else
            {
                GenerateChildren();
            }
        }

        public int MinMax()
        {
            int temp, childValue;
            if (isEnd)
            {
                return win;
            }
            else
            {
                if (ruch)
                {
                    temp = -2;
                    foreach (Node child in children)
                    {
                        childValue = child.MinMax();
                        if (childValue > temp)
                        {
                            temp = childValue;
                        }
                    }
                }
                else
                {
                    temp = 2;
                    foreach (Node child in children)
                    {
                        childValue = child.MinMax();
                        if (childValue < temp)
                        {
                            temp = childValue;
                        }
                    }
                }
            }

            win = temp;
            return win;
        }

        private void GenerateChildren()
        {
            foreach(int i in iter)
            {
                Node node = new Node(value + i, this, !ruch, level+1);
                children.Add(node);
                //children.Add(new Node(value + i, this, !ruch));
            }
        }


    }

    class Program
    {
        
        static List<string> GenerateCode(Node start)
        {
            List<string> result = new List<string>();
            string line = "";
            bool flag = false;
            /*if (start.parent == null)
            {
                foreach (Node child in start.children)
                {
                    int diff = child.value - start.value;
                    result = result + "\"ant;\\n " + start.value + "\" -> \"prot;\\n " + child.value + "\" [label = \"" + diff + "\"];\n";
                }
            }
            else
            {*/
                if (start.ruch == true)
                {
                    foreach (Node child in start.children)
                    {
                        string color = " ";
                        if (child.win == start.win && child.win != -1 && !flag)
                        {
                            color = " color=\"red\"";
                            flag = true;
                        }
                        int diff = child.value - start.value;
                        line = "\"prot;\\n " + start.value + " " + start.win + " " + start.level + "\" -> \"ant;\\n " +
                               child.value + " " + child.win + " " + child.level + "\" [label = \"" + diff + "\"" +
                               color + "];\n";
                        result.Insert(0,line);
                    }
                }
                else
                {
                    foreach (Node child in start.children)
                    {
                        string color = " ";
                        if (child.win == start.win && child.win != -1 && !flag)
                        {
                            color = " color=\"red\"";
                            flag = true;
                        }
                        int diff = child.value - start.value;
                        line = "\"ant;\\n " + start.value + " " + start.win + " " + start.level + "\" -> \"prot;\\n " +
                               child.value + " " + child.win + " " + child.level + "\" [label = \"" + diff + "\"" +
                               color + "];\n";
                        result.Insert(0,line);
                    }
                }
            //}

            foreach (Node child in start.children)
            {
                result.AddRange(GenerateCode(child));
            }
            // foreach (string item in result)
            // {
            //     Console.WriteLine(item);
            //     Console.WriteLine("test");
            // }
            
            return result;
        }

        static void generateFile(string code)
        {

            // Set a variable to the Documents path.
            using(StreamWriter writetext = new StreamWriter("C:\\Users\\piotr\\Documents\\Uczelnia\\Metody Inżynierii Wiedzy\\Graf-gry\\graf.dot"))
            {
                writetext.Write(code);
            }
        }

        static List<string> removeDup(List<string> input)
        {
            List<string> result = new List<string>();
            foreach (string lineIn in input)
            {
                bool flag = false;
                foreach (string lineOut in result)
                {
                    if (lineOut == lineIn)
                    {
                        flag = true;
                    }
                }

                if (flag == false)
                {
                    result.Add(lineIn);
                }
            }

            return result;
        }

        static string ListToString(List<string> input)
        {
            string result = "";
            foreach (string item in input)
            {
                result = result + item;
            }

            return result;
        }

        static void Main(string[] args)
        {
            Node start = new Node();
            start.MinMax();
            List<string> result = new List<string>();
            result = GenerateCode(start);
            string line = "digraph G {\n\n";
            result.Insert(0,line);
            result.Add("\n}");
            result = removeDup(result);
            string resultS = ListToString(result);
            //Console.Write(result);
            generateFile(resultS);
        }
    }
}