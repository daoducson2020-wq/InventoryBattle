using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private int id;
    [SerializeField]
    private string name;
    [SerializeField]
    private int damage;
    [SerializeField]
    private float atkCoolDown;

    public int Damage
    {
        get { return damage; }
    }

    public float AtkCoolDown
    {
        get {return atkCoolDown; }
    }

}
