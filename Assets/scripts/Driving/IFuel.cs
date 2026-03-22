public interface IFuel
{
    float CurrentFuel { get; }
    void Consume(float amount);
    bool HasFuel();
}
