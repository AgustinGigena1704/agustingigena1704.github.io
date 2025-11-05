using CCC.Api.Data;
using CCC.Api.Interfaces;
using CCC.Api.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CCC.Api.Data.Entities.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly CCCDbContext context;
        protected readonly DbSet<T> dbSet;
        public GenericRepository(CCCDbContext _context)
        {
            context = _context;
            dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbSet.Where(e => !EF.Property<bool>(e, "IsDeleted")).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await dbSet.Where(e => !EF.Property<bool>(e, "IsDeleted") && EF.Property<int>(e, "Id") == id).FirstOrDefaultAsync();
        }

        public async Task<T?> GetOneBy(Dictionary<string, object> parameters)
        {
            if (parameters == null || !parameters.Any())
            {
                return null;
            }

            var query = dbSet.Where(e => !EF.Property<bool>(e, "IsDeleted")).AsQueryable();

            var entityType = typeof(T);

            foreach (var param in parameters)
            {
                string propertyName = param.Key;
                object? value = param.Value;

                PropertyInfo? propInfo = entityType.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propInfo == null)
                {
                    Console.WriteLine($"Advertencia: La propiedad '{propertyName}' no existe en la entidad '{entityType.Name}'. Se ignorará.");
                    continue;
                }

                var parameter = Expression.Parameter(entityType, "e");
                var property = Expression.Property(parameter, propInfo);

                Expression constantExpression;

                if (value == null)
                {
                    constantExpression = Expression.Constant(null, propInfo.PropertyType);
                }
                else
                {
                    try
                    {
                        object? convertedValue = Convert.ChangeType(value, propInfo.PropertyType);
                        constantExpression = Expression.Constant(convertedValue, propInfo.PropertyType);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al convertir el valor '{value}' a tipo '{propInfo.PropertyType.Name}' para la propiedad '{propertyName}': {ex.Message}. Se ignorará este filtro.");
                        continue;
                    }
                }

                BinaryExpression equalExpression;

                if (propInfo.PropertyType == typeof(string) && value is string stringValue && value != null)
                {
                    var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                    var propertyToLower = Expression.Call(property, toLowerMethod!);
                    var constantToLower = Expression.Call(constantExpression, toLowerMethod!);
                    equalExpression = Expression.Equal(propertyToLower, constantToLower);
                }
                else
                {
                    equalExpression = Expression.Equal(property, constantExpression);
                }

                var lambda = Expression.Lambda<Func<T, bool>>(equalExpression, parameter);

                query = query.Where(lambda);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<T>?> GetByParams(Dictionary<string, object> parameters)
        {
            if (parameters == null || !parameters.Any())
            {
                return null;
            }

            var query = dbSet.Where(e => !EF.Property<bool>(e, "IsDeleted")).AsQueryable();

            var entityType = typeof(T);

            foreach (var param in parameters)
            {
                string propertyName = param.Key;
                object? value = param.Value;

                PropertyInfo? propInfo = entityType.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propInfo == null)
                {
                    Console.WriteLine($"Advertencia: La propiedad '{propertyName}' no existe en la entidad '{entityType.Name}'. Se ignorará.");
                    continue;
                }

                var parameter = Expression.Parameter(entityType, "e");
                var property = Expression.Property(parameter, propInfo);

                Expression constantExpression;

                if (value == null)
                {
                    constantExpression = Expression.Constant(null, propInfo.PropertyType);
                }
                else
                {
                    try
                    {
                        object? convertedValue = Convert.ChangeType(value, propInfo.PropertyType);
                        constantExpression = Expression.Constant(convertedValue, propInfo.PropertyType);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al convertir el valor '{value}' a tipo '{propInfo.PropertyType.Name}' para la propiedad '{propertyName}': {ex.Message}. Se ignorará este filtro.");
                        continue;
                    }
                }

                BinaryExpression equalExpression;

                if (propInfo.PropertyType == typeof(string) && value is string stringValue && value != null)
                {
                    var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                    var propertyToLower = Expression.Call(property, toLowerMethod!);
                    var constantToLower = Expression.Call(constantExpression, toLowerMethod!);
                    equalExpression = Expression.Equal(propertyToLower, constantToLower);
                }
                else
                {
                    equalExpression = Expression.Equal(property, constantExpression);
                }

                var lambda = Expression.Lambda<Func<T, bool>>(equalExpression, parameter);

                query = query.Where(lambda);
            }

            return await query.ToListAsync();
        }
    }
}
