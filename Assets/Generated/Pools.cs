//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGenerator.PoolsGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace Entitas {

    public partial class Pools {

        public static Pool CreateBoidsPool() {
            return CreatePool("Boids", BoidsComponentIds.TotalComponents, BoidsComponentIds.componentNames, BoidsComponentIds.componentTypes);
        }

        public Pool[] allPools { get { return new [] { boids }; } }

        public Pool boids;

        public void SetAllPools() {
            boids = CreateBoidsPool();
        }
    }
}