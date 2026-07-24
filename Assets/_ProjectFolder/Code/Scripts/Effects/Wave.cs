using UnityEngine;

public struct Wave
{
    public Vector2 Position;
    public float CurrentRadius;
    public float LifetimeLeft;
    public float MaxLifetime;

    public Wave(Vector2 position, float radius, float lifetime)
    {
        Position = position;
        CurrentRadius = radius;
        LifetimeLeft = MaxLifetime = lifetime;
    }
}