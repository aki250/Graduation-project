using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFinishCollider : MonoBehaviour
{
    [SerializeField] private string achievedEndingText;
    [SerializeField] private string achievedEndingText_Chinese;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            //Ó¢Óï
            if (LanguageManager.instance.localeID == 0)
            {
                UI.instance.SwitchToThankYouForPlaying(achievedEndingText);
            }
            //ÖÐÎÄ
            else if (LanguageManager.instance.localeID == 1)
            {
                UI.instance.SwitchToThankYouForPlaying(achievedEndingText_Chinese);
            }
        }
    }
}
