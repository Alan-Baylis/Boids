using Entitas;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private Systems systems;

    private void Start()
    {
        var pools = Pools.sharedInstance;
        pools.SetAllPools();

        systems = CreateSystems(pools);
        systems.Initialize();
    }

    private void OnDestroy()
    {
        systems.TearDown();
    }

    private void Update()
    {
        systems.Execute();
        systems.Cleanup();
    }

    private Systems CreateSystems(Pools pools)
    {
        return new Feature("Systems").
            Add(pools.boids.CreateSystem(new FlockingSystem()));
    }
}