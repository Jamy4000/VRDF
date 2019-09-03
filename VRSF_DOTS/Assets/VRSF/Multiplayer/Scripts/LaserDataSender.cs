using Photon.Pun;
using UnityEngine;
using VRSF.Core.LaserPointer;
using VRSF.Core.Raycast;

namespace VRSF.Multiplayer
{
    /// <summary>
    /// Send the Laser data to the other user's when requested
    /// This component replace the LaserWidthSetter and the LaserLengthSetter component when in multiplayer
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    public class LaserDataSender : MonoBehaviour, IPunObservable
    {
        [SerializeField] private ERayOrigin _rayOrigin;

        private bool _isSendingData;
        private LineRenderer _lineRenderer;
        private PhotonView _punComp;

        private float _startWidthToSend;
        private float _endWidthToSend;
        private Vector3 _startPosToSend;
        private Vector3 _endPosToSend;

        private void Awake()
        {
            _punComp = GetComponent<PhotonView>();
            _lineRenderer = GetComponent<LineRenderer>();
        }

        private void Start()
        {
            if (_punComp.IsMine)
            {
                OnLaserWidthChanged.Listeners += UpdateLineRenderWidth;
                OnLaserLengthChanged.Listeners += UpdateLineRenderLength;
                Destroy(_lineRenderer);
            }
        }

        private void OnDestroy()
        {
            if (_punComp.IsMine)
            {
                OnLaserWidthChanged.Listeners -= UpdateLineRenderWidth;
                OnLaserLengthChanged.Listeners -= UpdateLineRenderLength;
            }
        }

        private void UpdateLineRenderWidth(OnLaserWidthChanged info)
        {
            if (info.LaserOrigin == _rayOrigin)
            {
                _startWidthToSend = info.NewWidth;
                _endWidthToSend = info.NewWidth;

                _isSendingData = true;
            }
        }

        private void UpdateLineRenderLength(OnLaserLengthChanged info)
        {
            if (info.LaserOrigin == _rayOrigin)
            {
                _startPosToSend = Vector3.zero;
                _endPosToSend = info.NewEndPos;

                _isSendingData = true;
            }
        }

        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (!stream.IsWriting)
            {
                // Network player, receive data
                this._lineRenderer.startWidth = (float)stream.ReceiveNext();
                this._lineRenderer.endWidth = (float)stream.ReceiveNext();
                this._lineRenderer.SetPosition(0, (Vector3)stream.ReceiveNext());
                this._lineRenderer.SetPosition(1, (Vector3)stream.ReceiveNext());
            }
            else if (_isSendingData)
            {
                // We own this player: send the others our data
                stream.SendNext(_startWidthToSend);
                stream.SendNext(_endWidthToSend);
                stream.SendNext(_startPosToSend);
                stream.SendNext(_endPosToSend);
                _isSendingData = false;
            }
        }

        #endregion
    }
}