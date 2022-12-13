public interface IEntityComponent
{
    bool Enabled { get; set; }
    void Update(float dt);
    void Draw(float dt);
}