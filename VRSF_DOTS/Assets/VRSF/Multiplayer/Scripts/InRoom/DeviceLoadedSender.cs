using Photon.Pun;
using UnityEngine;
using VRSF.Core.LaserPointer;
using VRSF.Core.Raycast;

namespace VRSF.Multiplayer
{
    /// <summary>
    /// Send the device that was loaded to the other users
    /// </summary>
    [RequireComponent(typeof(LineRenderer), typeof(PhotonView))]
    public class DeviceLoadedSender : MonoBehaviour, IPunObservable
    {
        private bool _isSendingData;
        private LineRenderer _lineRenderer;
        private ERayOrigin _rayOrigin;
        private PhotonView _punComp;

        private void Awake()
        {
            _punComp = GetComponent<PhotonView>();
            _lineRenderer = GetComponent<LineRenderer>();
            // VRRaycastAuthoring is necessaraly on this GameObject; as the LaserPointerStateAuthoring needs it.
            _rayOrigin = GetComponent<VRRaycastAuthoring>().RayOrigin;
            OnLaserWidthChanged.Listeners += UpdateLineRenderWidth;
            OnLaserLengthChanged.Listeners += UpdateLineRenderLength;
        }

        private void OnDestroy()
        {
            OnLaserWidthChanged.Listeners -= UpdateLineRenderWidth;
            OnLaserLengthChanged.Listeners -= UpdateLineRenderLength;
        }

        private void UpdateLineRenderWidth(OnLaserWidthChanged info)
        {
            if (info.LaserOrigin == _rayOrigin && _punComp.IsMine)
            {
                _lineRenderer.startWidth = info.NewWidth;
                _lineRenderer.endWidth = info.NewWidth;

                _isSendingData = true;
            }
        }

        private void UpdateLineRenderLength(OnLaserLengthChanged info)
        {
            if (info.LaserOrigin == _rayOrigin && _punComp.IsMine)
            {
                _lineRenderer.SetPositions(new Vector3[]
                {
                    transform.position,
                    info.NewEndPos
                });

                _isSendingData = true;
            }
        }

        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                if (_isSendingData)
                {
                    // We own this player: send the others our data
                    stream.SendNext(this._lineRenderer.startWidth);
                    stream.SendNext(this._lineRenderer.endWidth);
                    stream.SendNext(this._lineRenderer.GetPosition(0));
                    stream.SendNext(this._lineRenderer.GetPosition(1));
                    _isSendingData = false;
                }
            }
            else
            {
                // Network player, receive data
                this._lineRenderer.startWidth = (float)stream.ReceiveNext();
                this._lineRenderer.endWidth = (float)stream.ReceiveNext();
                this._lineRenderer.SetPosition(0, (Vector3)stream.ReceiveNext());
                this._lineRenderer.SetPosition(1, (Vector3)stream.ReceiveNext());
            }
        }

        #endregion
    }
}