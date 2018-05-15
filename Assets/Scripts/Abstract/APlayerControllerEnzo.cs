using Assets.Scripts.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class APlayerControllerEnzo : APlayerController
{

    protected bool looserDrunk = false;
    public bool LooserDrunk { get { return looserDrunk; } }

    [Command]
    public void CmdLooserDrunk()
    {
        looserDrunk = true;
    }
}
