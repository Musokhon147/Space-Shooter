using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private float _speedMultiplier = 2f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;

    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isShieldActive = false;
    // variable reference to the shield visualizer
    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private GameObject _rightEngine;
    [SerializeField]
    private GameObject _leftEngine;
    [SerializeField]
    private int _score = 0;
    [SerializeField]
    private UIManager _uiManager;
    [SerializeField]
    private AudioClip _laserSoundClip;
    private AudioSource _audioSource;



    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager")?.GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas")?.GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }
        else
        {
            _spawnManager.StartSpawning();
        }
        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL.");
        }

        if (_audioSource == null)
        {
            Debug.LogError("Audio Source is NULL.");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }

    }

    void Update()
    {
        calculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            fireLaser();
        }
    }

    void calculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(direction * _speed * Time.deltaTime);

        // Vertical boundaries
        if (transform.position.y >= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        else if (transform.position.y <= -3.8f)
        {
            transform.position = new Vector3(transform.position.x, -3.8f, 0);
        }

        // Horizontal wrap-around
        if (transform.position.x >= 11)
        {
            transform.position = new Vector3(-11, transform.position.y, 0);
        }
        else if (transform.position.x <= -11)
        {
            transform.position = new Vector3(11, transform.position.y, 0);
        }
    }

    void fireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }
        _audioSource.Play(); // play the laser sound
        // play the laser sound
      
    }

    public void Damage()
    {
        if (_isShieldActive)
        {
            _isShieldActive = false;
            _shieldVisualizer.SetActive(false);
            return;
        }

        _lives--;
        if (_lives == 2)
        {
            _rightEngine.SetActive(true);
        }
        else if (_lives == 1)
        {
            _leftEngine.SetActive(true);
        }
        

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager?.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void TripleShotPowerupOn()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isTripleShotActive = false;
    }

    public void SpeedBoostPowerupOn()
    {
        _isSpeedBoostActive = true;
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isSpeedBoostActive = false;
        _speed /= _speedMultiplier;
    }

    public void ShieldsPowerupOn()
    {
        _isShieldActive = true;
        StartCoroutine(ShieldsPowerDownRoutine());
        _shieldVisualizer.SetActive(true);
    }

    IEnumerator ShieldsPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isShieldActive = false;
    }
    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
    
}
