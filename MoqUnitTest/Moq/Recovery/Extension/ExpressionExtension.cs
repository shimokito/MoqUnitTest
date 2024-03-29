﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using MoqUnitTest.Moq.Recovery.Attirbute;

namespace MoqUnitTest.Moq.Recovery.Extension
{
    public static class ExpressionExtension
    {
        private static Expression<Func<TSource, TResult>> PullByLimit<TSource, TResult>(PropertyInfo[] targetProps)
        {
            var sourceProps = typeof(TSource).GetProperties();

            var sourceParam = Expression.Parameter(typeof(TSource), "source");
            var newInstance = Expression.New(typeof(TResult));
            var binds = new List<MemberAssignment>();

            foreach (var sourceProp in sourceProps)
            {
                foreach (var targetProp in targetProps)
                {
                    if (!Attribute.IsDefined(targetProp, typeof(NonPull)))
                        if (sourceProp.Name == targetProp.Name)
                        {
                            var bindProp = Expression.Property(sourceParam, sourceProp.Name);
                            binds.Add(Expression.Bind(typeof(TResult).GetProperty(targetProp.Name), bindProp));
                            break;
                        }
                }
            }

            var body = Expression.MemberInit(newInstance, binds);
            var lambda = Expression.Lambda<Func<TSource, TResult>>(body, sourceParam);
            return lambda;
        }
        public static Expression<Func<TSource, TResult>> PullThisByLimit<TResult, TSource, TLimit>()
            => PullByLimit<TSource, TResult>(typeof(TLimit).GetProperties());
        public static Expression<Func<TSource, TResult>> PullByLimit<TSource, TResult>()
            => PullByLimit<TSource, TResult>(typeof(TResult).GetProperties());

        //public static void Update<T>(this T model, Action<T> action)
        //    where T : IRecoveryMoqModel<TModel>
        //    where TModel : class
        //{
        //    model.IsUpdated = true;
        //    action(model);
        //}
    }
}
