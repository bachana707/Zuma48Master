using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class X2BonusBall : Bonus
{
   
    public Ball ball { private set; get; }
    
    void Awake()
    {
        ball = GetComponent<Ball>();
    }

    public override void doBonusAction(Ball collidedBall)
    {
        collidedBall.UpdateNuberValue(collidedBall.value * 2);
        GameManager.Instance.CheckNumbersValueDelayed(collidedBall.index);
        //collidedBall.ThrowParticle();
        //collidedBall.GetComponent<Animation>().Play();
        Destroy(gameObject);
    }

}
