// File: Assets/Scripts/Physics/PhysicsMode.cs
// Purpose: Defines physics modes for hybrid implementation during migration

namespace BlockBall.Physics
{
    public enum PhysicsMode
    {
        UnityPhysics,    // Use existing Unity physics system
        CustomPhysics,   // Use custom BlockBall physics system
        Hybrid           // Transitional mode using both systems with custom taking precedence
    }
}
