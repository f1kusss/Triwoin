using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HelloWorld;
using IG;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Triwoinmag
{
    public class ManagerScore : SingletonManager<ManagerScore>
    {
        private Dictionary<ulong, int> _playersClientIDToPlayerScore = new Dictionary<ulong, int>();
        [SerializeField] private List<ulong> listClientsID = new List<ulong>();
        [SerializeField] private List<int> listPlayersScore = new List<int>();


        public Action<ulong, int> OnAddScore;
        public Action<ulong, int> OnNewScore;

        public UnityEvent<ulong, int> EventAddScore;

        private void Start()
        {
            // if (ScoreText == null)
            //     Debug.LogError("ManagerScore. ScoreText null");
            // else
            //     ScoreText.text = Score.ToString();
        }

        private void OnEnable()
        {
            OnAddScore += IncreaseScore;
        }

        private void OnDisable()
        {
            OnAddScore -= IncreaseScore;
        }

        public void AddScore(ulong id, int scoreDelta)
        {
            if (Debugging)
                Debug.Log("AddScore", this);
            OnAddScore?.Invoke(id, scoreDelta);
        }

        public void IncreaseScore(ulong id, int scoreDelta)
        {
            if (!_playersClientIDToPlayerScore.TryGetValue(id, out int score))
                _playersClientIDToPlayerScore[id] = scoreDelta;
            else
                _playersClientIDToPlayerScore[id] += scoreDelta;
            foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (uid == id)
                {
                    NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<ClientCharScore>()
                        .ReceiveNewScore(_playersClientIDToPlayerScore[id]);
                }
            }


            if (Debugging)
            {
                listClientsID = _playersClientIDToPlayerScore.Select(x => x.Key).ToList();
                listPlayersScore = _playersClientIDToPlayerScore.Select(x => x.Value).ToList();
            }

            OnNewScore?.Invoke(id, _playersClientIDToPlayerScore[id]);

            EventAddScore.Invoke(id, scoreDelta);
        }

        public void Achtung()
        {
            return;
        }
    }
}