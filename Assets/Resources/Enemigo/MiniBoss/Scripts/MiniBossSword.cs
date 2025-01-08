using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MiniBoss;

public class MiniBossSword : MonoBehaviour
{
    Animator ani;
    SpriteRenderer sR;
    public MiniBoss miniBoss;
    Vector2 startPos;
    // Start is called before the first frame update
    void Start()
    {
        miniBoss = transform.parent.GetComponent<MiniBoss>();
        ani = GetComponent<Animator>();
        sR = GetComponent<SpriteRenderer>();
        startPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        sR.flipX = miniBoss.flipX;
        transform.localPosition = miniBoss.flipX ? new Vector2(-startPos.x,transform.localPosition.y) : new Vector2(startPos.x, transform.localPosition.y);

        switch (miniBoss.patron)
        {
            case patterns.idle:
                SetBoolAnimator("idle");
                Debug.Log("idle");

                break;
            case patterns.change:
                SetBoolAnimator("change");
                Debug.Log("Estoy cambiando de arma");
                break;
            case patterns.follow:
                SetBoolAnimator("idle");
                Debug.Log("Estoy persiguiendote");
                break;
            case patterns.attack_sword:
                SetBoolAnimator("swordIdle");
                Debug.Log("Estoy en espada");
                break;
            case patterns.attack_charge:
                SetBoolAnimator("dismiss");
                Debug.Log("Estoy en carga");
                break;
            case patterns.attack_shoot_1:
                SetBoolAnimator("idle");
                Debug.Log("Estoy en shoot 1");
                break;
            case patterns.attack_shoot_2:
                SetBoolAnimator("idle");
                Debug.Log("Estoy en shoot 2");
                break;
            case patterns.attack_shoot_especial:
                SetBoolAnimator("idle");
                Debug.Log("Estoy en shoot espe");
                break;
        }
    }
    public void SetBoolAnimator(string name)
    {
        ani.SetBool(name, true);
        foreach(AnimatorControllerParameter parameter in ani.parameters)
        {

            if (parameter.type == AnimatorControllerParameterType.Bool && !parameter.name.Equals(name))
            {
                ani.SetBool(parameter.name, false);
            }
        }
    }

}
