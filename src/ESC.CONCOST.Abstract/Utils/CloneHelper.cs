
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ESC.CONCOST.Abstract;

public static class CloneHelper
{
    private static Dictionary<Type, PropertyInfo[]> _propertyDictionary = new();

    private static PropertyInfo[] GetCachedProperties(Type type)
    {
        PropertyInfo[] properties;
        if (_propertyDictionary.TryGetValue(type, out properties) == false)
        {
            properties = type.GetProperties();
            _propertyDictionary.TryAdd(type, properties);
        }

        return properties;
    }

    private static MemberExpression ExtractMemberExpression(Expression expression)
    {
        if (expression.NodeType == ExpressionType.MemberAccess)
        {
            return ((MemberExpression)expression);
        }

        if (expression.NodeType == ExpressionType.Convert)
        {
            var operand = ((UnaryExpression)expression).Operand;
            return ExtractMemberExpression(operand);
        }

        return null;
    }

    public static PropertyInfo GetPropertyInfo<T>(this T obj, Expression<Func<T, object>> selector)
    {
        if (selector.NodeType != ExpressionType.Lambda)
        {
            throw new ArgumentException("Selector must be lambda expression", "selector");
        }

        var lambda = (LambdaExpression)selector;

        var memberExpression = ExtractMemberExpression(lambda.Body);

        if (memberExpression == null)
        {
            throw new ArgumentException("Selector must be member access expression", "selector");
        }

        if (memberExpression.Member.DeclaringType == null)
        {
            throw new InvalidOperationException("Property does not have declaring type");
        }

        return memberExpression.Member.DeclaringType.GetProperty(memberExpression.Member.Name);
    }

    public static string ExtractKeyValue<T, TAttribute>(T obj)
    {
        PropertyInfo[] props = typeof(T).GetProperties();

        foreach (var prop in props)
        {
            if (prop.GetCustomAttributes(false)
                .Where(p => p.GetType() == typeof(TAttribute))
                .FirstOrDefault() == null)
                continue;

            return Convert.ToString(prop.GetValue(obj, null));
        }

        return string.Empty;
    }

    public static T CloneTo<T>(this object source)
    {
        if (source == null)
            return default(T);

        try
        {
            var target = Activator.CreateInstance(typeof(T));
            var propFrom = GetCachedProperties(source.GetType());
            var propTo = GetCachedProperties(target.GetType());

            foreach (PropertyInfo pFrom in propFrom)
            {
                PropertyInfo pTo = propTo
                    .Where(x => pFrom.Name == x.Name)
                    .FirstOrDefault();

                if (pTo == null)
                {
                    continue;
                }

                object val = pFrom.GetValue(source, null);
                Type t = Nullable.GetUnderlyingType(pTo.PropertyType) ?? pTo.PropertyType;

                if (!IsSimple(t)) continue;

                if (t == typeof(string) && pFrom.PropertyType == typeof(Guid))
                {
                    val = Convert.ToString(val);
                    pTo.SetValue(target, val, null);
                }
                else if (t == typeof(Guid) && pFrom.PropertyType == typeof(string))
                {
                    Guid guid = Guid.Empty;

                    if (Guid.TryParse((string)val, out guid))
                        pTo.SetValue(target, guid, null);
                }
                else
                {
                    val = GetNullValue(t, val);
                    object safeValue = (val == null) ? null : Convert.ChangeType(val, t);
                    pTo.SetValue(target, safeValue, null);
                }
            }

            return (T)target;
        }
        catch (Exception)
        {
            return default(T);
        }
    }

    public static void TransferTo<T>(this object source, T target)
    {
        if (source == null) return;

        try
        {
            var propFrom = GetCachedProperties(source.GetType());
            var propTo = GetCachedProperties(target.GetType());

            foreach (PropertyInfo pFrom in propFrom)
            {
                PropertyInfo pTo = propTo
                    .Where(x => pFrom.Name == x.Name)
                    .FirstOrDefault();

                if (pTo == null)
                {
                    continue;
                }

                object val = pFrom.GetValue(source, null);
                Type t = Nullable.GetUnderlyingType(pTo.PropertyType) ?? pTo.PropertyType;

                if (!IsSimple(t)) continue;

                if (t == typeof(string) && pFrom.PropertyType == typeof(Guid))
                {
                    val = Convert.ToString(val);
                    pTo.SetValue(target, val, null);
                }
                else if (t == typeof(Guid) && pFrom.PropertyType == typeof(string))
                {
                    Guid guid = Guid.Empty;

                    if (Guid.TryParse((string)val, out guid))
                        pTo.SetValue(target, guid, null);
                }
                else
                {
                    val = GetNullValue(t, val);
                    object safeValue = (val == null) ? null : Convert.ChangeType(val, t);
                    pTo.SetValue(target, safeValue, null);
                }
            }
        }
        catch (Exception)
        {
        }
    }

    public static T CloneFrom<T>(this T target, object source)
    {
        if (source == null) return target;

        try
        {
            var propFrom = GetCachedProperties(source.GetType());
            var propTo = GetCachedProperties(target.GetType());

            foreach (PropertyInfo pFrom in propFrom)
            {
                PropertyInfo pTo = propTo
                    .Where(x => pFrom.Name == x.Name)
                    .FirstOrDefault();

                if (pTo == null)
                {
                    continue;
                }

                object val = pFrom.GetValue(source, null);
                Type t = Nullable.GetUnderlyingType(pTo.PropertyType) ?? pTo.PropertyType;

                if (!IsSimple(t)) continue;

                if (t == typeof(string) && pFrom.PropertyType == typeof(Guid))
                {
                    val = Convert.ToString(val);
                    pTo.SetValue(target, val, null);
                }
                else if (t == typeof(Guid) && pFrom.PropertyType == typeof(string))
                {
                    Guid guid = Guid.Empty;

                    if (Guid.TryParse((string)val, out guid))
                        pTo.SetValue(target, guid, null);
                }
                else
                {
                    val = GetNullValue(t, val);
                    object safeValue = (val == null) ? null : Convert.ChangeType(val, t);
                    pTo.SetValue(target, safeValue, null);
                }
            }

            return target;
        }
        catch (Exception)
        {
            return target;
        }
    }

    public static List<TTarget> CloneList<TTarget, TSource>(List<TSource> sources)
    {
        List<TTarget> targets = new List<TTarget>();

        if (sources == null || sources.Count == 0)
            return targets;

        PropertyInfo[] propFrom = typeof(TSource).GetProperties();
        PropertyInfo[] propTo = typeof(TTarget).GetProperties();

        List<Tuple<PropertyInfo, PropertyInfo>> matchedProps = new List<Tuple<PropertyInfo, PropertyInfo>>();

        foreach (PropertyInfo pFrom in propFrom)
        {
            PropertyInfo pTo = propTo.Where(x => x.Name == pFrom.Name).FirstOrDefault();

            if (pTo == null)
            {
                continue;
            }

            matchedProps.Add(new Tuple<PropertyInfo, PropertyInfo>(pTo, pFrom));
        }

        foreach (var source in sources)
        {
            var target = Activator.CreateInstance(typeof(TTarget));

            foreach (var prop in matchedProps)
            {
                var pTo = prop.Item1;
                var pFrom = prop.Item2;

                object val = pFrom.GetValue(source, null);
                Type t = Nullable.GetUnderlyingType(pTo.PropertyType) ?? pTo.PropertyType;

                if (!IsSimple(t)) continue;

                if (t == typeof(string) && pFrom.PropertyType == typeof(Guid))
                {
                    val = Convert.ToString(val);
                    pTo.SetValue(target, val, null);
                }
                else if (t == typeof(Guid) && pFrom.PropertyType == typeof(string))
                {
                    Guid guid = Guid.Empty;

                    if (Guid.TryParse((string)val, out guid))
                        pTo.SetValue(target, guid, null);
                }
                else
                {
                    val = GetNullValue(t, val);
                    object safeValue = (val == null) ? null : Convert.ChangeType(val, t);
                    pTo.SetValue(target, safeValue, null);
                }
            }

            targets.Add((TTarget)target);
        }

        return targets;
    }

    public static TTarget ThenUpdateList<TTarget, TListItem, TSource>(this TTarget target, Expression<Func<TTarget, object>> expression, List<TSource> source)
    {
        PropertyInfo prop = target.GetPropertyInfo<TTarget>(expression);
        prop.SetValue(target, CloneList<TListItem, TSource>(source), null);
        return target;
    }

    public static object GetNullValue(Type type, object input)
    {
        if (input == null
            || input.Equals(System.DBNull.Value)
            || input.Equals(DateTime.MinValue)
            || input.Equals(Guid.Empty)
            || input.Equals(string.Empty))
        {
            return null;
        }

        return input;
    }

    public static bool IsSimple(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
        {
            type = Nullable.GetUnderlyingType(type);
        }

        return type.IsPrimitive
          || type.Equals(typeof(string))
          || type.Equals(typeof(decimal))
          || type.Equals(typeof(DateTime))
          || type.Equals(typeof(Guid));
    }

    public static bool IsNullAbleType(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
    }
}