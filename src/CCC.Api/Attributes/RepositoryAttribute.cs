// File: CCC.Api/Attributes/RepositoryAttribute.cs
using System;

namespace CCC.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RepositoryAttribute : Attribute
    {
        public Type EntityType { get; }
        public Type RepositoryType { get; }
        public bool Registrar { get; init; } = false;

        public RepositoryAttribute(Type entityType, Type repositoryType, bool registrar = false)
        {
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
            RepositoryType = repositoryType ?? throw new ArgumentNullException(nameof(repositoryType));
            Registrar = registrar;
        }
    }
}