using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HitResponseUI : MonoBehaviour
{
    [SerializeField] private Image sprite;
    

    public void SpriteChange()
    {
        StartCoroutine(ChangeColor());
    }
    
    private IEnumerator ChangeColor()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(.1f);
        sprite.color = Color.white;
    } 
}
