using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Triwoinmag
{
    public interface IWeapon
    {
        void Attack(Vector3 targetPosition);
        void Damage(float damageAmount, Vector3 targetHitPosition, GameAgent sender);
        void VisualizeFiring(Vector3 targetPosition);
    }
}