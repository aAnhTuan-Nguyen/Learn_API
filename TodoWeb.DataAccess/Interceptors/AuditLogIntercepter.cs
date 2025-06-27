using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;
using TodoWeb.Domain.Entities;

namespace TodoWeb.Infrastructures.Interceptors
{
    public class AuditLogIntercepter : SaveChangesInterceptor
    {
        //private HashSet<object> addSet = new HashSet<object>();
        private List<EntityEntry> addEntities = new List<EntityEntry>();
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            var context = eventData.Context as ApplicationDbContext;// laays context ra
            var auditLogs = new List<AuditLog>();
            foreach (var entry in context.ChangeTracker.Entries())
            {
                if(entry.Entity is AuditLog)
                {
                    continue;
                }
                var log = new AuditLog
                {
                    EntityName = entry.Entity.GetType().Name,
                    CreatedAt = DateTime.Now,
                    Action = entry.State.ToString(),
                };
                if (entry.State == EntityState.Added)
                {
                    addEntities.Add(entry);
                    //log.NewValue = JsonSerializer.Serialize(entry.CurrentValues.ToObject());
                }
                if (entry.State == EntityState.Modified)
                {
                    log.OldValue = JsonSerializer.Serialize(entry.OriginalValues.ToObject());
                    log.NewValue = JsonSerializer.Serialize(entry.CurrentValues.ToObject());
                    auditLogs.Add(log);
                }
                if (entry.State == EntityState.Deleted)
                {
                    // gox sai thong cam
                    log.OldValue = JsonSerializer.Serialize(entry.OriginalValues.ToObject());
                    auditLogs.Add(log);
                }

            }
            if (auditLogs.Any())
            {
                context.AuditLog.AddRange(auditLogs);
            }
            return base.SavingChanges(eventData, result); // commit data
        }

        public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            var context = eventData.Context as ApplicationDbContext;// laays context ra
            if (addEntities.Any())
            {
                var auditLogs = addEntities.Select(entity => new AuditLog
                {
                    EntityName = entity.Entity.GetType().Name,
                    CreatedAt = DateTime.Now,
                    Action = entity.State.ToString(),
                    NewValue = JsonSerializer.Serialize(entity.CurrentValues.ToObject()),
                });
                context.AuditLog.AddRange(auditLogs);
                addEntities.Clear();
                context.SaveChanges();
            }
            return base.SavedChanges(eventData, result);
        }
    }
}
