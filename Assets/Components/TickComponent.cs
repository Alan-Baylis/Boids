using Entitas;
using Entitas.CodeGenerator;

[Boids,SingleEntity]
public class TickComponent : IComponent {
	public ulong value;
	public float deltatime;
}
