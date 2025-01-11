using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBossHitmarker : MonoBehaviour
{
    public MiniBoss miniBoss;
    Animator ani;
    SpriteRenderer sR;

    [SerializeField] float bossAngle;
    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
        sR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
         bossAngle = Mathf.Rad2Deg * (Mathf.Atan2(miniBoss.direction.y, miniBoss.direction.x));
         transform.localEulerAngles = new Vector3(0, 0, bossAngle);
    }
    public void Comenzar()
    {
        ani.SetTrigger("slash");
        sR.flipY = !sR.flipY;
    }
}
