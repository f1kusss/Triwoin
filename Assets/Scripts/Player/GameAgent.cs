using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Triwoinmag
{
    public class GameAgent : NetworkBehaviour
    {
        public enum Faction
        {
            Player,
            Allies,
            SeventhStar
        }

        public Faction ShipFaction;
    }
}