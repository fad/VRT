using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HandsSynchronization : MonoBehaviour, IPunObservable
{
    private PhotonView photonView;

    public Transform leftHandTransform;

    private float _Distance_LeftHand;

    private Vector3 _Direction_LeftHand;
    private Vector3 _NetworkPosition_LeftHand;
    private Vector3 _StoredPosition_LeftHand;

    private Quaternion _NetworkRotation_LeftHand;
    private float _Angle_LeftHand;

    bool _FirstTake = false;

    private void OnEnable()
    {
        _FirstTake = true;
    }

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        _StoredPosition_LeftHand = leftHandTransform.localPosition;
        _NetworkPosition_LeftHand = Vector3.zero;
        _NetworkRotation_LeftHand = Quaternion.identity;
    }


    private void Update()
    {
        if (!photonView.IsMine)
        { 
            leftHandTransform.localPosition = Vector3.MoveTowards(leftHandTransform.localPosition,
            _NetworkPosition_LeftHand, _Distance_LeftHand * Time.deltaTime * PhotonNetwork.SerializationRate);
            leftHandTransform.localRotation = Quaternion.RotateTowards(leftHandTransform.localRotation,
                _NetworkRotation_LeftHand, _Angle_LeftHand * Time.deltaTime * PhotonNetwork.SerializationRate);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            _Direction_LeftHand = leftHandTransform.localPosition - _StoredPosition_LeftHand;
            _StoredPosition_LeftHand = leftHandTransform.localPosition;

            stream.SendNext(leftHandTransform.localPosition);
            stream.SendNext(_Direction_LeftHand);

            stream.SendNext(leftHandTransform.localRotation);
        }
        else
        {
            _NetworkPosition_LeftHand = (Vector3)stream.ReceiveNext();
            _Direction_LeftHand = (Vector3)stream.ReceiveNext();

            if (_FirstTake)
            {
                leftHandTransform.localPosition = _NetworkPosition_LeftHand;
                _Distance_LeftHand = 0;
            }
            else
            {
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                _NetworkPosition_LeftHand += _Direction_LeftHand * lag;

                _Distance_LeftHand = Vector3.Distance(leftHandTransform.localPosition, _NetworkPosition_LeftHand);

            }

            _NetworkRotation_LeftHand = (Quaternion)stream.ReceiveNext();
            if (_FirstTake)
            {
                _Angle_LeftHand = 0f;

                leftHandTransform.localRotation = _NetworkRotation_LeftHand ; 
            }
            else
            {
                _Angle_LeftHand = Quaternion.Angle(leftHandTransform.localRotation, _NetworkRotation_LeftHand);

            }

            if (_FirstTake)
                _FirstTake = false;
        }
    }

    

}
