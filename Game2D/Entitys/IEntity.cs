public interface IEntity
{
    Dictionary<Type, IEntityComponent> Components { get; set; }

    void Update(float dt);

    void Draw(float dt);
}