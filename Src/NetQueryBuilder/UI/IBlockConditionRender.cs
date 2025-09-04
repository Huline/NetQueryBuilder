using System.Collections.Generic;
using NetQueryBuilder.Conditions;
using NetQueryBuilder.Properties;

namespace NetQueryBuilder.UI
{
    public interface IBlockConditionRender<out T>
    {
        T Render(BlockCondition condition);
    }

    public interface IConditionRender<out T>
    {
        T Render(SimpleCondition condition);
    }

    public interface ISelectFieldsRender<out T>
    {
        T Render(IEnumerable<SelectPropertyPath> fields);
    }
}