using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class Player : MonoBehaviour
{
    [SerializeField]
    AudioClip audio1;

    [SerializeField]
    AudioClip audio2;

    public AudioSource source;

    public int hp = 20;
    private float startFallHeight; // Almacena la altura al inicio de la caída
    private bool isFalling; // Indica si el jugador está cayendo

    private bool isInvincible;

    [SerializeField]
    TextMeshProUGUI text;
    [SerializeField] private LayerMask groundLayers;

    // Start is called before the first frame update
    void Start()
    {
        startFallHeight = transform.position.y; // Inicializa la altura inicial
        isFalling = false;
        text.text = "HP: " + hp;
    }

    // Update is called once per frame
    void Update()
    {
        bool _isGrounded = IsGrounded();

        if (!_isGrounded && !isFalling)
        {
            startFallHeight = transform.position.y;
            isFalling = true;
        }

        if(_isGrounded && isFalling)
        {
            float fallDistance = startFallHeight - transform.position.y;
            if (fallDistance > 8)
            {
                takeDamage(5, 2);
            }
            else if (fallDistance > 5)
            {
                takeDamage(3, 2);
            }
            else if (fallDistance > 3)
            {
                takeDamage(1, 1);
            }
            isFalling = false;
        }

        if (hp < 1)
        {
            SceneManager.LoadScene("LostScene");
        }
    }

    public void takeDamage(int damage, int audionum)
    {
        

        if (!isInvincible)
        {
            hp -= damage;
            
            Debug.Log("Daño");
            text.text = "HP: " + hp;

            if (audionum == 1)
            {
                source.PlayOneShot(audio1);
            }
            else if (audionum == 2)
            {
                source.PlayOneShot(audio2);
            }

            
            isInvincible = true;
            StartCoroutine(Invincibility());
        }

        IEnumerator Invincibility()
        {
            yield return new WaitForSeconds(1);
            isInvincible = false;
        }
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position, 0.2f, groundLayers);
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.layer == LayerMask.NameToLayer("Lava"))
        {
            takeDamage(1,1);
            //source.PlayOneShot(audio1);


        }

        else if (hit.gameObject.layer == LayerMask.NameToLayer("Uranium"))
        {
            takeDamage(3,2);
            //source.PlayOneShot(audio2);
            
        }
    }

    /*public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Lava"))
        {
            takeDamage(1);
            source.PlayOneShot(audio1);

        }

        else if (collision.gameObject.layer == LayerMask.NameToLayer("Uranium"))
        {
            takeDamage(3);
            source.PlayOneShot(audio2);
        }

        else
        {
            // Si el jugador está cayendo y colisiona con algo, verifica la altura de caída
            if (isFalling && collision.gameObject.layer != LayerMask.NameToLayer("WaterV"))
            {
                float fallDistance = startFallHeight - transform.position.y;
                if (fallDistance >= fallThreshold)
                {
                    takeDamage(fallDamage);
                }
                isFalling = false; // El jugador ha aterrizado, deja de caer
            }
        }
    }*/

    /*public void OnCollisionExit(Collision collision)
    {
        // Si el jugador deja de colisionar con algo (es decir, empieza a caer), marca el inicio de la caída
         startFallHeight = transform.position.y;
         isFalling = true;
        
    }*/
}
