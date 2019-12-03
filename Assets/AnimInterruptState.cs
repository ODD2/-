using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EAnimInterruptType
{
    Always,
    NeverMovement,
    Never,
};
public class AnimInterruptState
{
    public EAnimInterruptType IntType;
}
