using System;
using Test;

namespace Test
{
    using ExtensionMethods;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class Class_t
    {
        public string dataString;
        public string classname;
        public Methods_t[] methodArray = new Methods_t[32];
        private string[] variableArray = new string[32];
        public int lineCount { get; private set; }
        public float lineToCommentRatio { get; private set; }

        public Class_t(string dataString)
        {
            string methods, variables;
            int tilIndex = 0;
            CodeSmeller.detectTheActualEndOfSection(ref dataString);
            this.dataString = dataString;
            lineCount = dataString.LineCount();
            calculateRatio();
            /*
                In c# functions and variables must start with either public or private, so change all the instances to a single char -> ~
            */
            dataString = dataString.Replace("public", "~");
            dataString = dataString.Replace("private", "~");

            /*
                Get all the indexes of the ~ and all the indexes of {
            */
            List<int> tilIndexes = dataString.AllIndexesOf("~");
            List<int> frontIndexes = dataString.AllIndexesOf("{");

            /*
                Considering all the variables in a class are declared before the functions, and the first instance of { is from the start of the class,
                we can determin exactly where the first function starts -> it starts between the first { and an ocurrence of ~
            */
            while (frontIndexes[1] > tilIndexes[tilIndex]) { tilIndex++; }

            //Divide the strings into variables and functions
            methods = dataString.Substring(tilIndexes[tilIndex-1]);
            variables = dataString.Substring(tilIndexes[0], frontIndexes[1] - tilIndexes[0]);

            //Split by the ~
            string[] methodsList = Regex.Split(methods, @"(?<=[~])");
            string[] variablesList = Regex.Split(variables, @"(?<=[~])");

            //Create the methods list
            for (int index = 1; index < (methodsList.Length - 1); index++)
            {
                CodeSmeller.detectTheActualEndOfSection(ref methodsList[index]);       //Cleanup

                methodArray[index-1] = new Methods_t(methodsList[index]);
                if (methodArray[index - 1].returnType == "")
                    this.classname = methodArray[index - 1].functionName;
            }

            //Create the functions list
            for (int index = 1; index < (variablesList.Length - 1); index++)
            {
                variablesList[index] = variablesList[index].Remove(variablesList[index].LastIndexOf(';') + 1);

                variableArray[index - 1] = variablesList[index];
            }
        }


        public void calculateRatio()
        {
            int commentedLines = 0, counter = 0;
            string auxiliar = "";
            List<int> normalComment;
            List<int> commentStart = dataString.AllIndexesOf("/*");
            List<int> commentEnd = dataString.AllIndexesOf("*/");

            for (int index = 0; index < commentStart.Count; index++)
            {
                auxiliar = dataString.Substring(commentStart[index], commentEnd[index] - commentStart[index]);

                normalComment = auxiliar.AllIndexesOf("//");

                commentedLines += auxiliar.LineCount();

                counter += normalComment.Count;
            }

            normalComment = dataString.AllIndexesOf("//");

            commentedLines += normalComment.Count - counter;
            lineToCommentRatio = ((float)commentedLines / (float)lineCount);
        }

        public bool hasToManyLines()
        {
            if (methodArray != null)
                if (lineCount >= methodArray.Length * 40)
                    return true;

            return false;
        }
        public bool hasToManyMethods()
        {
            if (methodArray != null)
            {
                if (methodArray.Length >= 8)
                    return true;
            }

            return false;
        }
    }
}