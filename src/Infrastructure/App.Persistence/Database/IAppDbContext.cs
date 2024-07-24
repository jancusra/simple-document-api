using System;
using Microsoft.EntityFrameworkCore;
using App.Domain;

namespace App.Persistence.Database
{
    public partial interface IAppDbContext : IDisposable
    {
        DbSet<TEntity> Table<TEntity>() where TEntity : BaseEntity;
    }
}
