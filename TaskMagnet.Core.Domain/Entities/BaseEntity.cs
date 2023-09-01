using TaskMagnet.Core.Domain.Interfaces;

namespace TaskMagnet.Core.Domain.Entities;

public abstract class BaseEntity<T> : IBaseEntity<T>
{
        public T Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public long? CreatedById { get; set; }
        public long? ModifiedById { get; set; }
}
