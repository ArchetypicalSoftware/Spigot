using Spigot.Samples.EventualConsistency.SynchronizedNodes.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Spigot.Samples.EventualConsistency.SynchronizedNodes
{
    public static class ItemComparer<T>
    {
        public static List<(string name, Type type, Func<T, object> accessor, Action<T, object> setter)> _accessors;

        static ItemComparer()
        {
            var allProperties = typeof(T).GetProperties();
            var entity = Expression.Parameter(typeof(T));
            _accessors = allProperties.
                Select(p =>
                {
                    var getterCall = Expression.Call(entity, p.GetGetMethod());
                    var castToObject = Expression.Convert(getterCall, typeof(object));
                    var objExpr = Expression.Variable(typeof(object));
                    var castFromObject = Expression.Convert(objExpr, p.PropertyType);
                    var setterCall = Expression.Call(entity, p.GetSetMethod(), castFromObject);
                    var lambda = Expression.Lambda(castToObject, entity);
                    var setterLambda = Expression.Lambda(setterCall, entity, objExpr);
                    return (p.Name, p.PropertyType, (Func<T, object>)lambda.Compile(), (Action<T, object>)setterLambda.Compile());
                }).ToList();
        }

        public static List<Property> Compare(T left, T right)
        {
            return _accessors.Where(kvp =>
                {
                    var l = kvp.accessor(left);
                    var r = kvp.accessor(right);
                    if (l == null)
                    {
                        return r != null;
                    }

                    if (r == null)
                    {
                        return true;
                    }

                    return !l.Equals(r);
                }).Select(kvp =>
                    new Property
                    {
                        Name = kvp.name,
                        Type = kvp.type.ToString(),
                        Value = kvp.accessor(right)
                    })
                .ToList();
        }

        public static void ApplyDifferences(T itemToAcceptChanges, List<Property> changedProperties)
        {
            foreach (var changedProperty in changedProperties)
            {
                var item = _accessors.FirstOrDefault(x => x.name.Equals(changedProperty.Name));

                item.setter(itemToAcceptChanges, changedProperty.Value);
            }
        }
    }
}