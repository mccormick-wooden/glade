using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface IPlayerControls
    {
        Vector2 movement { get; }
        Vector2 cameraMovement { get; }
        bool isAttackPerformed { get; }
    }
}
