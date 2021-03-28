using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetable {
    void DestroySelf ();
    void Focus ();
    void Unfocus ();
}
