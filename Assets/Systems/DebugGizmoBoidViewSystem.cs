using Entitas;
using UnityEngine;

public sealed class DebugGizmoBoidViewSystem : ISetPools, IInitializeSystem {

    Pools _pools;

    public void SetPools(Pools pools) {
        _pools = pools;
    }

    public void Initialize() {
        var allBoids = _pools.boids.GetGroup(Matcher.AllOf(BoidsMatcher.Boid, BoidsMatcher.Position, BoidsMatcher.Velocity));
        var gameObject = new GameObject();
        var view = gameObject.AddComponent<DebugGizmoBoidView>();
        view.AllBoids = allBoids;
    }
}