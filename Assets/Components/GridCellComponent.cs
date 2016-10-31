using System.Collections.Generic;
using Entitas;
using UnityEngine;

[Boids]
public class GridCellComponent : IComponent
{
    public List<Entity> ContainsEntities;
    public Entity[] NeighboringCells;
}
