namespace ToDoList.Entities
{
    public abstract class Entity : IEquatable<Entity>
    {
        public int Id { get; init; }

        public override bool Equals(object other) => Equals(other as Entity);
        public bool Equals(Entity other)
        {
            if (other is null) return false;
            return ReferenceEquals(this, other) || Id.Equals(other.Id);
        }

        public static bool operator ==(Entity a, Entity b)
        {
            if (a is null) return b is null;
            return a.Equals(b);
        }

        public static bool operator !=(Entity a, Entity b)
        {
            return !(a == b);
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}
