using Entitas;

public sealed class IncrementTickSystem : ISetPools, IInitializeSystem, IExecuteSystem {

    Pools _pools;

    public void SetPools(Pools pools) {
        _pools = pools;
    }

    public void Initialize() {
        _pools.boids.SetTick(0, UnityEngine.Time.fixedDeltaTime);
    }

    public void Execute() {
        _pools.boids.ReplaceTick(_pools.boids.tick.value + 1, UnityEngine.Time.fixedDeltaTime);
    }
}