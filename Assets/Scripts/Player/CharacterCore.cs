using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Triwoinmag
{
    public class CharacterCore : NetworkBehaviour
    {
        public GameAgent Agent;

        [SerializeField] private string LayerRemotePlayer = "PlayerRemote";
        [SerializeField] private List<Collider> _colliders;

        protected List<Collider> Colliders
        {
            get
            {
                if (_colliders == null)
                {
                    _colliders = new List<Collider>();
                    foreach (Collider col in GetComponentsInChildren<Collider>())
                    {
                        //if (col.gameObject.layer != someSpecialLayerOnCharacter)
                        _colliders.Add(col);
                    }
                }

                return _colliders;
            }
        }

        private void Start()
        {
            // Universal code
            gameObject.name = "Player" + NetworkObjectId;


            // Owner code
            if (IsOwner)
            {
                return;
            }


            // Not Owner code
            // Set layer for other Player prefabs
            gameObject.layer = LayerMask.NameToLayer(LayerRemotePlayer);
            foreach (var col in _colliders)
            {
                col.gameObject.layer = LayerMask.NameToLayer(LayerRemotePlayer);
            }
        }
    }
}