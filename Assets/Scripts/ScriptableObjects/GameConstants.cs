using UnityEngine;

[CreateAssetMenu(fileName = "GameConstants", menuName = "ScriptableObjects/GameConstants", order = 1)]
public class GameConstants : ScriptableObject
{
    // lives
    public int maxLives;

    // Mario's movement
    public int speed;
    public int maxSpeed;
    public int deceleration;
    public float jumpVelocity;
    public float lowGravityScale;
    public float highGravityScale;
    public float maxFallSpeed;
    public float stompHeightFraction;
    public float stompBounceVelocity;
    public float deathGravityScale;
    public int deathImpulse;
}