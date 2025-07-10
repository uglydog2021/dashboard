using MES_ReportForms.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace MES_ReportForms.Core;
public static class Extensions
{
    /// <summary>
    /// 注入命名空间和类名后缀的实现类
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assembly"></param>
    /// <param name="namespaceBase"></param>
    /// <param name="classSuffix"></param>
    public static void AddScopedAssemblyServices(this IServiceCollection services, Assembly assembly, string namespaceBase, string classSuffix)
    {
        // 获取程序集中所有类型
        var types = assembly.GetTypes();

        // 筛选出符合命名空间和类名后缀的类型
        var serviceTypes = types.Where(t =>
            t.Namespace != null &&
            t.Namespace.StartsWith(namespaceBase) &&
            t.Name.EndsWith(classSuffix, StringComparison.Ordinal) &&
            !t.IsInterface &&
            !t.IsAbstract).ToList();

        // 遍历筛选后的类型，并注册为 Scoped 服务
        foreach (var serviceType in serviceTypes)
        {
            // 直接注册服务类本身
            services.AddScoped(serviceType);
        }
    }

    /// <summary>
    /// 根据属性名字符串设置排序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="propertyName"></param>
    /// <param name="ascending"></param>
    /// <returns></returns>
    public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string propertyName, bool ascending = true)
    {
        // 检查propertyName是否为空
        if (string.IsNullOrWhiteSpace(propertyName))
            return query;

        // 确保propertyName对应的成员在T中存在
        var propertyInfo = typeof(T).GetRuntimeProperty(propertyName);
        if (propertyInfo == null)
        {
            propertyInfo = typeof(T).GetProperties()
            .Where(prop => string.Equals(prop.Name, propertyName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        if (propertyInfo == null)
        {
            return query;
        }

        // 创建参数表达式
        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");

        // 创建属性访问表达式
        var propertyAccess = Expression.MakeMemberAccess(parameter, propertyInfo);

        // 创建排序表达式
        var orderByExpression = Expression.Lambda(propertyAccess, parameter);

        // 根据排序方向创建排序方法调用表达式
        var resultExpression = ascending
            ? Expression.Call(typeof(Queryable), "OrderBy", new[] { typeof(T), propertyInfo.PropertyType }, query.Expression, Expression.Quote(orderByExpression))
            : Expression.Call(typeof(Queryable), "OrderByDescending", new[] { typeof(T), propertyInfo.PropertyType }, query.Expression, Expression.Quote(orderByExpression));

        // 创建排序查询
        return query.Provider.CreateQuery<T>(resultExpression);
    }

    /// <summary>
    /// 将PageFilter应用到Queryable，设置排序与分页
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public static IQueryable<T> ApplyPageFilter<T>(this IQueryable<T> query, PageFilter filter)
    {
        if (filter == null)
            return query;

        // 设置排序参数
        query = query.OrderByDynamic(filter.OrderBy, filter.IsASC());

        if (filter.PageNumber <= 0)
            filter.PageNumber = 1;

        // 设置分页参数
        query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);

        return query;
    }

    /// <summary>
    /// 为BaseEntity设置公共字段声明
    /// </summary>
    /// <param name="builder"></param>
    public static void PropertyBaseEntity(this EntityTypeBuilder builder)
    {
        builder.Property(nameof(BaseEntity.IsDelete))
            .HasComment("是否已删除")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(nameof(BaseEntity.CreateUser))
            .HasComment("创建用户ID")
            .HasMaxLength(50)
            .HasDefaultValue("")
            .IsRequired();

        builder.Property(nameof(BaseEntity.CreateDate))
            .HasComment("创建时间")
            .HasDefaultValueSql("getdate()")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(nameof(BaseEntity.ModifierUser))
            .HasComment("修改用户ID")
            .HasMaxLength(50);

        builder.Property(nameof(BaseEntity.ModifierDate))
            .HasComment("修改时间");
    }

    /// <summary>
    /// 获取枚举值的描述
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumValue"></param>
    /// <returns></returns>
    public static string ToDescription<T>(this T enumValue) where T : Enum
    {
        var enumType = typeof(T);

        string propName = Enum.GetName(enumType, enumValue);
        if (propName != null)
        {
            var field = enumType.GetField(propName);
            var descriptionAttribute = field.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
            return descriptionAttribute == null ? propName : descriptionAttribute.Description;
        }

        return Enum.GetName(enumType, enumValue);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumValue"></param>
    /// <returns></returns>
    public static string ToDescription<T>(this T? enumValue) where T : struct, Enum
    {
        if (enumValue.HasValue)
            return enumValue.Value.ToDescription();

        return string.Empty;
    }
}
