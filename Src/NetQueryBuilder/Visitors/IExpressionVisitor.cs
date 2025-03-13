using System.Linq.Expressions;

namespace NetQueryBuilder.Visitors;

public interface IExpressionVisitor<T> where T : Expression
{
    T Execute();
}