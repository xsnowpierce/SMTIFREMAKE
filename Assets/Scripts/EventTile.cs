using Game.Level;
using UnityEngine;

public abstract class EventTile : MapCollider
{
    public abstract void OnTileEntered(Vector3 playerRotation);
    public abstract void OnTileExited();
}