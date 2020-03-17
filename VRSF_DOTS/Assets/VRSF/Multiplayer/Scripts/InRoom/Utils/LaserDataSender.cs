using Photon.Pun;
using UnityEngine;
using VRSF.Core.LaserPointer;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;

namespace VRSF.Multiplayer
{
    /// <summary>
    /// Send the Laser data to the other user's when requested
    /// This component replace the LaserWidthSetter and the LaserLengthSetter component when in multiplayer
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    public class LaserDataSender : MonoBehaviourPun, IPunObservable
    {
        [SerializeField] private ERayOrigin _rayOrigin;

        private LineRenderer _lineRenderer;

        private float _startWidthToSend;
        private float _endWidthToSend;
        private Vector3 _endPosToSend;

        private bool _needToSendData;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        private void Start()
        {
            EDevice device = (EDevice)photonView.Owner.CustomProperties[VRSFPlayer.DEVICE_USED];
            if (photonView.IsMine)
            {
                if (device != EDevice.NONE && device != EDevice.SIMULATOR)
                {
                    OnLaserWidthChanged.Listeners += UpdateLineRenderWidth;
                    OnLaserLengthChanged.Listeners += UpdateLineRenderLength;
                    Destroy(_lineRenderer);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                if (device != EDevice.NONE && device != EDevice.SIMULATOR)
                    this._lineRenderer.SetPosition(0, Vector3.zero);
                else
                    Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (OnLaserWidthChanged.IsCallbackRegistered(UpdateLineRenderWidth))
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
                _needToSendData = true;
            }
        }

        private void UpdateLineRenderLength(OnLaserLengthChanged info)
        {
            if (info.LaserOrigin == _rayOrigin)
            {
                _endPosToSend = info.NewEndPos;
                _needToSendData = true;
            }
        }

        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (!stream.IsWriting && !photonView.IsMine)
            {
                this._lineRenderer.startWidth = (float)stream.ReceiveNext();
                this._lineRenderer.endWidth = (float)stream.ReceiveNext();
                this._lineRenderer.SetPosition(1, (Vector3)stream.ReceiveNext());
            }
            else if (_needToSendData && stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(_startWidthToSend);
                stream.SendNext(_endWidthToSend);
                stream.SendNext(_endPosToSend);
                _needToSendData = false;
            }
        }

        #endregion
    }
}