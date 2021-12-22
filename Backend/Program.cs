using System;
using System.IO;

namespace Test
{
    class CodeSmeller
    {
        public Namespace_t[] namespacesArray = new Namespace_t[32];
        public string[] namespaces;// = CodeSmeller.detectSection(ref aux, "namespace"); //Count and get the number of namespaces


        public CodeSmeller(string filename)
        {

            string aux = File.ReadAllText(filename);
            namespaces = CodeSmeller.detectSection(ref aux, "namespace");

            for (int index = 1; index < namespaces.Length; index++) //The first string is includes so garbage
            {
                CodeSmeller.detectTheActualEndOfSection(ref namespaces[index]);

                namespacesArray[index-1] = new Namespace_t(ref namespaces[index]);
            }

            Console.ReadLine();
        }

        public static string[] detectSection(ref string input, string spliter)   //Splits the string in all ocurrences of spliter
        {

            string[] stringSeparators = new string[] { spliter };
            string[] namespaceSection = input.Split(stringSeparators, StringSplitOptions.None);

            return namespaceSection;
        }

        public static void detectTheActualEndOfSection(ref string input)   //Splits the string in all ocurrences of spliter
        {
            int index = input.LastIndexOf('}');
            if (index + 1 >= input.Length)
                return;

            input = input.Remove(index + 1);
        }
    }

}