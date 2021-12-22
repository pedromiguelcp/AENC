using System;
using System.Collections.Generic;
using Test;


namespace ExtensionMethods
{
    public static class MyExtensions
    {
        public static List<int> AllIndexesOf(this string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        public static int LineCount(this string str)
        {
            return str.Split('\n').Length;
        }
    }
}

namespace Test
{
    using ExtensionMethods;

    public class Methods_t
    {
        public string dataString;
        public string functionName { get; private set; }
        public string returnType { get; private set; }
        public int lineCount { get; private set; }
        public float lineToCommentRatio { get; private set; }
        public string[] parameters { get; private set; }
        public int nestingDegree { get; private set; }
        public Methods_t(string dataString)
        {

            this.dataString = dataString.Remove(0, 1); //Removes the first space from the 

            parseFunctionName();
            lineCount = dataString.LineCount();
            
            Console.WriteLine("------------------------------------");
            Console.WriteLine(dataString);
            Console.WriteLine("Name: " + functionName);
            Console.WriteLine("Return: " + returnType);
            calculateRatio();
            removeComments();
            calculateDegreeOfNesting();

            List<string> toPrint = hasUnusedParameters();

            Console.WriteLine("This is the rati: " + lineToCommentRatio);
            Console.WriteLine("These parameters are unused: ");
            
            for(int index = 0; index != toPrint.Count; index++)
                Console.WriteLine(toPrint[index]);

            if (parameters == null)
                return;

            for (int index = 0; index < parameters.Length; index++)
            {
                parameters[index] = parameters[index].Trim(' ');
                Console.WriteLine("Parameter: " + parameters[index]);
            }
        }

        public void parseFunctionName()
        {
            string aux;
            int index = 0;

            index = dataString.IndexOf("{");
            aux = dataString.Substring(0, index);               //Returns all the text before the start of the function

            returnType = aux.Substring(0, aux.IndexOf(" "));    //Returns the text before the first space. This string is referent to the return type
            returnType = returnType.Trim();
            aux = aux.Remove(0, aux.IndexOf(" "));              //Eliminates the return type


            index = aux.IndexOf("(");                             //Where the parameters start
            if(index <= 0)
            {
                index = aux.IndexOf("\r");
                if (index > 0)
                {
                    aux = aux.Remove(index, 2);
                    functionName = aux.Trim();
                }
                else
                {
                    functionName = returnType;
                    returnType = "";
                }
            }
            else
            {
                functionName = aux.Substring(0, index).Trim(' ');               //Returns all the text before the start of the function

                aux = aux.Remove(0, index + 1);
                aux = aux.Remove(aux.LastIndexOf(")"));

                parameters = aux.Split(',');

                if (parameters != null)
                {
                    for (index = 0; index < parameters.Length; index++)
                        parameters[index] = parameters[index].Trim(' ');
                }
            }
        }

        public void calculateRatio()
        {
            int commentedLines = 0, counter = 0;
            string auxiliar = "";
            List<int> normalComment;
            List<int> commentStart = dataString.AllIndexesOf("/*");
            List<int> commentEnd = dataString.AllIndexesOf("*/");

            for(int index = 0; index < commentStart.Count; index++)
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

        public void removeComments()
        {
            List<int> commentStart = dataString.AllIndexesOf("/*");
            List<int> commentEnd = dataString.AllIndexesOf("*/");

            while(commentStart.Count != 0)
            {
                dataString = dataString.Remove(commentStart[0], commentEnd[0] - commentStart[0] + 2);

                commentStart = dataString.AllIndexesOf("/*");
                commentEnd = dataString.AllIndexesOf("*/");
            }

            List<int> normalComment = dataString.AllIndexesOf("//");
            while (normalComment.Count != 0)
            {
                string auxiliar = dataString.Substring(normalComment[0]);
                int indexOfNL = auxiliar.IndexOf("\n"); 

                dataString = dataString.Remove(normalComment[0], indexOfNL + 1);

                normalComment = dataString.AllIndexesOf("//");
            }

        }

        public void calculateDegreeOfNesting()
        {
            int degree = 0, maxDegree = 0, nFront = 0;
            List<int> frontIndexes = dataString.AllIndexesOf("{");
            List<int> backIndexes = dataString.AllIndexesOf("}");

            frontIndexes.RemoveAt(0);
            backIndexes.RemoveAt(backIndexes.Count - 1);

            while (backIndexes.Count != 0)
            {
                if (frontIndexes.Count > nFront && frontIndexes[nFront] < backIndexes[0] && frontIndexes.Count > 1)       //If front indexs still is smaller it means another nest was encountered
                {
                    nFront++;
                    degree++;                               //Increase degree
                }
                else
                {
                    if (degree > maxDegree)
                        maxDegree = degree;

                    for (int i = 0; i < nFront; i++)
                        frontIndexes.RemoveAt(0);   //Remove the index

                    degree = 0;
                    nFront = 0;
                    backIndexes.RemoveAt(0);
                }
            }

            nestingDegree = maxDegree;
        }

        /// <summary>
        /// Function that checks if the number of entry parameters is good
        /// </summary>
        /// <param name="number">Reference to store the number of parameters used</param>
        /// <returns>True if the number of parameters is above a 4 lines
        /// Flase if the number of parameters is below a 4 lines</returns>
        public bool hasToManyParameters(/*ref int number*/)
        {
            int number;
            if (parameters != null)
            {
                number = parameters.Length;

                if (number >= 4)
                    return true;
            }
            number = 0;
            return false;
        }

        /// <summary>
        /// Function that checks if the line number is good
        /// </summary>
        /// <returns>True if the overall size of the method in number of lines is above a 100 lines
        /// Flase if the overall size of the method in number of lines is below a 100 lines</returns>
        public bool hasToManyLines()
        {
            if (lineCount >= 100)
                return true;
    
            return false;
        }

        /// <summary>
        /// Function that checks if the line to comment ratio is good.
        /// </summary>
        /// <returns>True if the ratio is below 35% which is good
        /// False if the ratio is above 35% which is bad</returns>
        public bool isTheLineToCommentRatioGood()
        {
            if (lineToCommentRatio >= 0.35)
                return false;

            return true;
        }

        /// <summary>
        /// Function that checks if there are any unused parameters inside the function
        /// </summary>
        /// <returns>A list with the unused variables</returns>
        public List<string> hasUnusedParameters()
        {
            List<string> parameteresToReturn = new List<string>();
            string auxiliar = "", auxiliar2 = "";

            auxiliar2 = dataString.Substring(dataString.IndexOf("{"), dataString.LastIndexOf("}") - dataString.IndexOf("{"));

            if (parameters == null)
                return parameteresToReturn;

            for(int index = 0; index != parameters.Length; index++)
            {
                if (parameters[index].IndexOf(" ") <= 0)
                    break;

                auxiliar = parameters[index].Remove(0, parameters[index].IndexOf(" ")).Trim();

                if (!auxiliar2.Contains(auxiliar))
                    parameteresToReturn.Add(parameters[index]);
            }

            return parameteresToReturn;
        }

        /// <summary>
        /// Function that checks if there are to many nestings inside the method
        /// </summary>
        /// <returns>True if the nestingDegree is above 3 which is bad
        /// False if the nestingDegree is below 3 which is good</returns>
        public bool toMuchNesting()
        {
            if (nestingDegree > 3)
                return true;

            return false;
        }

        /// <summary>
        /// Function that checks if there are to many cases inside the method
        /// </summary>
        /// <param name="numberCases">Reference to store the number of cases</param>
        /// <returns>True if the number of cases is above or equal to 4 which is bad
        /// False if the the number of cases is below 4 which is good</returns>
        public bool switchCaseToComplex(/*ref int numberCases*/)
        {
            List<int> nCases = new List<int>();
            if (dataString.Contains("switch"))
                nCases = dataString.AllIndexesOf("case");

            //numberCases = nCases.Count;

            if (nCases.Count >= 4)
                return true;

            return false;
        }
    }
}



