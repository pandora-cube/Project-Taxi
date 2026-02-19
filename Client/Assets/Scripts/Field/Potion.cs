using Field;
using UnityEngine;

[CreateAssetMenu(fileName = "Potion", menuName = "Scriptable Objects/ItemEffect/Potion")]
public class Potion : ItemEffect
{
    public override bool CanUseOn(GameObject target)
    {
        return true;
    }

    public override void Execute(Character user, GameObject target)
    {
        user.hp += 100;
        Debug.Log("Potion");
    }
}
