using MES_ReportForms.API; 
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;
using System.Xml;

namespace MES_ReportForms.API.Extensions
{
    public static class EntityExtensions
    {
        /// <summary>
        /// 设置实体操作默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="currentUser"></param>
        /// <param name="state"></param>
        public static T SetDefaultValues<T>(this T entity, string currentUser, EntityState state) where T : class, new()
        {
            // 获取实体类的类型
            Type entityType = entity.GetType();

            switch (state)
            {
                case EntityState.Added:
                    // 获取 CreatedBy 属性并赋值
                    PropertyInfo createdByProp = entityType.GetProperty("CreateUser");
                    if (createdByProp != null && createdByProp.CanWrite && createdByProp.PropertyType == typeof(string))
                    {
                        createdByProp.SetValue(entity, currentUser);
                    }

                    // 获取 CreatedTime 属性并赋值
                    PropertyInfo createdTimeProp = entityType.GetProperty("CreateDate");
                    if (createdTimeProp != null && createdTimeProp.CanWrite)
                    {
                        createdTimeProp.SetValue(entity, DateTime.Now);
                    }

                    break;
                case EntityState.Modified:
                case EntityState.Deleted:
                    // 获取 modifierUserProp 属性并赋值
                    PropertyInfo modifierUserProp = entityType.GetProperty("ModifierUser");
                    if (modifierUserProp != null && modifierUserProp.CanWrite && modifierUserProp.PropertyType == typeof(string))
                    {
                        modifierUserProp.SetValue(entity, currentUser);
                    }

                    // 获取 ModifierDate 属性并赋值
                    PropertyInfo modifierDateProp = entityType.GetProperty("ModifierDate");
                    if (modifierDateProp != null && modifierDateProp.CanWrite)
                    {
                        modifierDateProp.SetValue(entity, DateTime.Now);
                    }
                    break;
            }

            PropertyInfo isDeleteProp = entityType.GetProperty("IsDelete");
            if (isDeleteProp != null && isDeleteProp.CanWrite)
            {
                isDeleteProp.SetValue(entity, state == EntityState.Deleted ? true : false);
            }

            return entity;
        }
    }
}
