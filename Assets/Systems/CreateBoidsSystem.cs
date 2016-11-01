using Entitas;

public sealed class CreateBoidsSystem : ISetPools, IInitializeSystem {

    public int NumberOfBoids = 100;
    public int RandomSeed = 42;
    public float RangeToCenter = 60;

    Pools _pools;

    public void SetPools(Pools pools) {
        _pools = pools;
    }

    public void Initialize() {
        UnityEngine.Random.InitState(RandomSeed);
        for(int boidIndex=0; boidIndex < NumberOfBoids; ++boidIndex){
            _pools.boids.CreateEntity().
                AddPosition(UnityEngine.Random.insideUnitSphere * RangeToCenter).
                AddVelocity(UnityEngine.Random.insideUnitSphere, 1).
                IsBoid(true);
        }
    }
}