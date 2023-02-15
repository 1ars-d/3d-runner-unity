using UnityEngine;
using Cinemachine;

/// <summary>
/// An add-on module for Cinemachine Virtual Camera that locks the camera's Y co-ordinate
/// </summary>
[SaveDuringPlay]
[AddComponentMenu("")] // Hide in menu
public class LockCameraY : CinemachineExtension
{
    [Tooltip("Lock the camera's X position to this value")]
    public float max_XPosition = 2f;
    public float min_YPosition = 2f;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Finalize)
        {
            var pos = state.RawPosition;
            pos.x = Mathf.Clamp(pos.x, -max_XPosition, max_XPosition);
            pos.y = Mathf.Clamp(pos.y, min_YPosition, 100);
            state.RawPosition = pos;
        }
    }
}