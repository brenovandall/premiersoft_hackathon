namespace Hackathon.Premiersoft.API.Models.Abstractions
{
    public abstract class Entity<T> : IEntity<T>
    {
        public T Id { get; set; } = default!;
    }
}
