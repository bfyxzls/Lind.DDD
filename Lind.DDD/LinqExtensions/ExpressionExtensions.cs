using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.LinqExtensions
{
    /// <summary>
    /// 表达式树的扩展
    /// </summary>
    public class ExpressionExtensions
    {
        /// <summary>
        /// 构建表达式树
        /// 调用：GenerateExpression(new string[]{"username"},new object[]{"zzl"},new string[]{"="});
        /// </summary>
        /// <typeparam name="T">表类型</typeparam>
        /// <param name="keys">字段名</param>
        /// <param name="values">字段值</param>
        /// <param name="operation">操作符</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GenerateExpression<T>(string[] keys, object[] values, string[] operation)
        {
            var TType = typeof(T);
            Expression expression_return = Expression.Constant(true);
            ParameterExpression expression_param = Expression.Parameter(TType, "p");
            Expression expression;
            for (int i = 0; i < keys.Length; i++)
            {
                switch (operation[i])
                {
                    case "=":
                        expression = Expression.Equal(Expression.Call(Expression.Property(expression_param, TType.GetProperty(keys[i])),
                            TType.GetMethod("ToString")),
                         Expression.Constant(values[i]));
                        expression_return = Expression.And(expression_return, expression);
                        break;
                    case "%":
                        expression = Expression.Call(Expression.Property(expression_param, TType.GetProperty(keys[i])),
                            typeof(string).GetMethod("Contains"),
                            Expression.Constant(values[i], typeof(string)));
                        expression_return = Expression.And(expression_return, expression);
                        break;
                    case ">":
                        expression = Expression.Call(Expression.Property(expression_param, TType.GetProperty(keys[i])),
                            typeof(double).GetType().GetMethod("GreaterThan"), Expression.Constant(values[i]));
                        expression_return = Expression.And(expression_return, expression);
                        break;
                    case "<":
                        expression = Expression.Call(Expression.Property(expression_param, TType.GetProperty(keys[i])),
                            typeof(double).GetType().GetMethod("LessThan"), Expression.Constant(values[i]));
                        expression_return = Expression.And(expression_return, expression);
                        break;
                    case ">=":
                        expression = Expression.Call(Expression.Property(expression_param, TType.GetProperty(keys[i])),
                            typeof(double).GetType().GetMethod("GreaterThanOrEqual"), Expression.Constant(values[i]));
                        expression_return = Expression.And(expression_return, expression);
                        break;
                    case "<=":
                        expression = Expression.Call(Expression.Property(expression_param, TType.GetProperty(keys[i])),
                            TType.GetProperty(keys[i]).GetType().GetMethod("LessThanOrEqual"), Expression.Constant(values[i]));
                        expression_return = Expression.And(expression_return, expression);
                        break;
                    case "in":
                        string[] strarr = values[i].ToString().Split(',');
                        Expression or_return = Expression.Constant(false);
                        for (int k = 0; k < strarr.Length; k++)
                        {
                            expression = Expression.Equal(Expression.Call(Expression.Property(expression_param, TType.GetProperty(keys[i])),
                                TType.GetMethod("ToString")),
                             Expression.Constant(strarr[k]));
                            or_return = Expression.Or(or_return, expression);
                        }

                        expression_return = Expression.And(expression_return, or_return);
                        break;
                    default:
                        throw new ArgumentException("无效的操作符，目前只支持=,%,>,<,>=,<=,in");
                }
            }

            return (Expression<Func<T, bool>>)Expression.Lambda<Func<T, bool>>(expression_return, new ParameterExpression[] { expression_param });
        }

        /// <summary>
        /// 在类型中，过滤表达式树里的属性
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> FilterPropertyInfo<TModel>(params Expression<Func<TModel, object>>[] expression)
        {
            return FilterPropertyInfo(null, expression);
        }
        /// <summary>
        /// 在类型中，过滤表达式树里的属性
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="navigation">返回导航属性</param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> FilterPropertyInfo<TModel>(List<Tuple<Type, string>> navigation, params Expression<Func<TModel, object>>[] expression)
        {
            List<string> fieldArr = new List<string>();
            navigation = navigation ?? new List<Tuple<Type, string>>();
            foreach (var f in expression)
            {
                PropertyInfo pi = Property(f);
                if (!(pi.ReflectedType == typeof(Lind.DDD.Domain.Entity)
                      || pi.ReflectedType == typeof(Lind.DDD.Domain.NoSqlEntity)
                      || pi.ReflectedType == typeof(Lind.DDD.Domain.EntityBase))
                      && pi.ReflectedType != typeof(TModel))
                    navigation.Add(new Tuple<Type, string>(pi.ReflectedType, pi.Name));
                else
                    fieldArr.Add(pi.Name);　//字段类型不一样时出现问题
            }

            var t = typeof(TModel).GetProperties()//过滤了导航属性
                                  .Where(i => (i.PropertyType.IsValueType
                                      || i.PropertyType == typeof(DateTime)
                                      || i.PropertyType == typeof(DateTime?)
                                      || i.PropertyType == typeof(string)));

            if (fieldArr.Count > 0)
            {
                var listType = fieldArr.Where(i => i.Contains("#"));
                t = t.Where(i => fieldArr.Contains(i.Name));
            }
            return t;
        }

        /// <summary>
        /// 从表达式树里拿到对应的属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static PropertyInfo Property<T>(Expression<Func<T, object>> expr)
        {
            var member = Member(expr);
            var prop = member as PropertyInfo;

            if (prop == null)
            {
                throw new InvalidOperationException("Specified member is not a property.");
            }
            if (typeof(T) != prop.ReflectedType)
            {

            }
            return prop;
        }

        /// <summary>
        ///  从表达式树里拿到对应的字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        static MemberInfo Member<T>(Expression<Func<T, object>> expr)
        {
            // This is a tricky case because of the "object" return type.
            // An expression which targets a value type property will
            // have a UnaryExpression body, whereas an expression which
            // targets a reference type property will have a MemberExpression
            // (or, more specifically, PropertyExpression) Body.
            var unaryExpr = expr.Body as UnaryExpression;
            var memberExpr = (MemberExpression)(unaryExpr == null ? expr.Body : unaryExpr.Operand);

            return memberExpr.Member;
        }
    }
}
