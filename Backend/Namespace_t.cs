using System;
using Test;

namespace Test
{
    public class Namespace_t
    {
        public string dataString;
        public Class_t[] classArray = new Class_t[32];

        public Namespace_t(ref string dataString)
        {
            this.dataString = dataString;
            CodeSmeller.detectTheActualEndOfSection(ref dataString);

            string[] classes = CodeSmeller.detectSection(ref dataString, "class");

            for (int index = 1; index < classes.Length; index++)
            {
                CodeSmeller.detectTheActualEndOfSection(ref classes[index]);       //Cleanup

                classArray[index - 1] = new Class_t(classes[index]) ;
            }
        }
    }
}
