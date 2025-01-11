using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class camara : MonoBehaviour
{
    public Player player;
    public Enemy enemigo;
    GenLight gN;

    public bool isAuxCamera = false;

    // cosas del shake
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.1f;
    private float dampingSpeed = 1.0f;

    private Vector3 initialPosition;
    private Transform cameraTrans;
    private static Vector3 randShake = Vector3.zero;
    private static List<camara> instances = new List<camara>();

    // Start is called before the first frame update
    void Start()
    {
        gN = gameObject.AddComponent<GenLight>();
        gN.luz = Resources.Load<GameObject>("Luces/LuzBossSwordEpico");
        cameraTrans = transform;
        instances.Add(this);
    }

    // Update is called once per frame
    void Update()
    {   
        if(!isAuxCamera){
            if (Input.GetKeyUp(KeyCode.P))
            {
                Instantiate(enemigo.gameObject, new Vector2(player.transform.position.x +5, player.transform.position.y),Quaternion.identity);
            }
            if (Input.GetKeyUp(KeyCode.N))
            {
                Instantiate(Resources.Load<GameObject>("Enemigo/MiniBoss/Miniboss"), new Vector2(player.transform.position.x + 8, player.transform.position.y), Quaternion.identity);
            }
            if (Input.GetKeyUp(KeyCode.M))
            {
                gN.GenerateLight(new Vector2(player.transform.position.x + 8, player.transform.position.y));
            }
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y,-20);
            if (Input.GetKeyUp(KeyCode.O))
            {
                Instantiate(Resources.Load<GameObject>("Proyectiles/arrow"), new Vector2(player.transform.position.x + 5, player.transform.position.y), Quaternion.identity);
            }
        }
        if (shakeDuration > 0){
            // random ffset

            if(this == instances[0]){
                Debug.Log("true");
                randShake = Random.insideUnitSphere;
            }
            cameraTrans.localPosition += randShake * shakeMagnitude;

            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else{
            shakeDuration = 0f;
        }
    }

        // shake shake
    public void TriggerShake(float intensity){
        Debug.Log("SHAKEI");
        shakeMagnitude = Mathf.Clamp(intensity, 0.1f,0.3f);

        shakeDuration = intensity * 0.6f;
    }

    public static void shakeAllCameras(float intensity){
        Debug.Log(instances.Count);
        int i = 0;
        foreach(camara c in instances){
            Debug.Log("INSTANCE" + i++);
            c.TriggerShake(intensity*2.2f);
        }
    }
}
