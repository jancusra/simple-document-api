using System;
using Microsoft.EntityFrameworkCore;
using App.Domain;

namespace App.Persistence.Database
{
    public partial interface IAppDbContext : IDisposable
    {
        /// <summary>
        /// Access to the database table according to the defined type
        /// </summary>
        /// <typeparam name="TEntity">entity type</typeparam>
        /// <returns>specific database set by the entity type</returns>
        DbSet<TEntity> Table<TEntity>() where TEntity : BaseEntity;
    }
}
