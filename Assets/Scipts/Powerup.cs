using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] 
    private float _speed = 3f; 
    [SerializeField]
    private int _powerupID;
    [SerializeField]
    private AudioClip _clip; 
    void Start()
    {
        
    }

   
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            
            Player player = other.transform.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_clip, transform.position); 

            if (player != null)
            {
                switch (_powerupID)
                {
                    case 0:
                        player.TripleShotPowerupOn();
                        break;
                    case 1:
                        player.SpeedBoostPowerupOn();
                        break;
                    case 2:
                        player.ShieldsPowerupOn();
                        break;
                    default:
                        Debug.Log("Invalid Powerup ID");
                        break;
                }
                
                
                
            }
          
            Destroy(gameObject);
            
        }
    }
}
