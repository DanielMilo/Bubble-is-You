using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mutetoggle : MonoBehaviour
{
    public void MuteToggle()
    {
        music.mute = !music.mute;
    }
}
