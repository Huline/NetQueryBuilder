using System.Linq.Expressions;
using System.Text;

namespace NetQueryBuilder.Operators
{
    public class UpperSeparatorExpressionStringifier : IExpressionStringifier
    {
        public string GetString(ExpressionType expressionType, string name)
        {
            return SeparateByUpperCase(string.IsNullOrEmpty(name) ? expressionType.ToString() : name);
        }

        private static string SeparateByUpperCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var result = new StringBuilder();
            result.Append(char.ToUpper(input[0]));

            for (var i = 1; i < input.Length; i++)
                if (char.IsUpper(input[i]))
                    result.Append(' ').Append(char.ToLower(input[i]));
                else
                    result.Append(input[i]);

            return result.ToString();
        }
    }
}