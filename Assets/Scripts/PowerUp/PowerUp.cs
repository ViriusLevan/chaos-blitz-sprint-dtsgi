using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    public abstract void Activate(PlayerController player);
    public abstract void Deactivate();
}
