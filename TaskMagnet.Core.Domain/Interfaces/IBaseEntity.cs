namespace TaskMagnet.Core.Domain.Interfaces;

public interface IBaseEntity<T>
{
      T Id { get; }
      DateTime CreatedDate { get; set; }
      DateTime ModifiedDate { get; set; }
      long? CreatedById { get; set; }
      long? ModifiedById { get; set; }  
}