using UnityEngine;
using Entitas;

[Boids]
public class VelocityComponent : IComponent
{
    public Vector3 Direction;
    public float Magnitude;
}
