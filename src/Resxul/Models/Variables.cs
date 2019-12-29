using System.Collections.Generic;
using System.Linq;
using Resxul.Properties;

namespace Resxul.Models
{
    public class Variables
    {
        public Variables()
        {
            Collection = new List<Variable>
            {
                new Variable(VariableType.AppName, "{AppName}", Resources.Profile_AppName),
                new Variable(VariableType.LangTag, "{LangTag}", Resources.Language_Tag)
            }.ToArray();
        }

        public Variable GetVariable(VariableType type)
        {
            return Collection.First(x => x.Type == type);
        }

        public void SetVariableValue(VariableType type, string value)
        {
            Variable v = GetVariable(type);
            v.Value = value;
        }

        public Variable[] Collection { get; }

        public string ResolveVariables(string inputString)
        {
            string outputString = inputString;

            if (!string.IsNullOrEmpty(outputString))
            {
                foreach (var variable in Collection.Where(x => x.IsSet))
                {
                    outputString = outputString.Replace(variable.Name, variable.Value);
                }
            }

            return outputString;
        }
    }
}
