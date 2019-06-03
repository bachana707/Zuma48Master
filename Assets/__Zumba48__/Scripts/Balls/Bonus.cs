using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bonus : MonoBehaviour
{
    // Start is called before the first frame update
    public abstract void doBonusAction(Ball collidedBall);

}
