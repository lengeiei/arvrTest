using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public Rigidbody rigid;
    public float forceMove = 4f;
    public float forceJump = 20f;
    public bool onFloor = false;

    public GameObject coinPrefab;
    public GameObject[] coinArray;
    public int coinArraySize = 3;

    public GameObject bombPrefab;
    public GameObject[] bombArray;
    public int bombArraySize = 3;

    public float coinRespawnPosY = 3f;
    public float coinRespawnPosYRNG = 1f;
    public float coinRespawnPosXRNG = 3f;



    public AudioClip clipCoinCollect;
    public AudioClip clipCoinRespawn;
    public AudioClip clipBombCollect;
    public AudioClip Jump;
    private AudioSource audioSource;

    
    public TMPro.TextMeshProUGUI tmp;
    public int score = 0;

    public float timer;
    public TMPro.TextMeshProUGUI countD;
    public float gameTime = 15;
    public Animator animator;

    public TMPro.TextMeshProUGUI Done;


    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        timer = gameTime;
    }

    private void Awake()
    {
        coinArray = new GameObject[coinArraySize];
        for (int i = 0; i < coinArraySize; i++)
        {
            coinArray[i] = Instantiate(coinPrefab, RandomizePos(), coinPrefab.transform.rotation);
        }

        bombArray = new GameObject[bombArraySize];
        for (int i = 0; i < bombArraySize; i++)
        {
            coinArray[i] = Instantiate(bombPrefab, RandomizePos(), coinPrefab.transform.rotation);
        }
    }
    // Update is called once per frame
    void Update()
    {
        tmp.text = "Score : " + score;
        timer -= Time.deltaTime;
        countD.text = "Time : " + Mathf.Round(timer);
        if (timer < 0)
        {
            StartCoroutine(GameOver());
        }
        if (Input.GetKey(KeyCode.A))
        {
            rigid.AddForce(new Vector3(-1f * forceMove, 0f, 0f));
        }
        if (Input.GetKey(KeyCode.D))
        {
            rigid.AddForce(new Vector3(1f * forceMove, 0f, 0f));
        }
        if (Input.GetKey(KeyCode.Space) && onFloor)
        {
            rigid.AddForce(new Vector3(0f, 1f * forceJump, 0f), ForceMode.Impulse);
            onFloor = false;
        }

    }

    private Vector3 RandomizePos()
    {
        Vector3 tempPos = new Vector3(Random.Range(-coinRespawnPosXRNG, coinRespawnPosXRNG), coinRespawnPosY + Random.Range(-coinRespawnPosYRNG, coinRespawnPosYRNG), 0f);
        return tempPos;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        { 
            onFloor = true;
            audioSource.PlayOneShot(Jump);
        }
       
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Coin")
        {
            StartCoroutine(RespawnCoin(other.gameObject));
        }

        if (other.gameObject.tag == "Bomb")
        {
            StartCoroutine(RespawnBomb(other.gameObject));
        }
    }

    IEnumerator RespawnCoin(GameObject coin)
    {
        coin.SetActive(false);
        score++;
        audioSource.PlayOneShot(clipCoinCollect);
        yield return new WaitForSeconds(3f);
        coin.transform.position = RandomizePos();
        coin.SetActive(true);
        audioSource.PlayOneShot(clipCoinRespawn);
    }

    IEnumerator RespawnBomb(GameObject bomb)
    {
        bomb.SetActive(false);
        score-=5;
        audioSource.PlayOneShot(clipBombCollect);
        yield return new WaitForSeconds(3f);
        bomb.transform.position = RandomizePos();
        bomb.SetActive(true);
    }

    IEnumerator GameOver()
    {
        rigid.constraints = RigidbodyConstraints.FreezeAll;
        animator.SetBool("gameOver", true);
        countD.text = ("Times Up");
        if (score >= 12)
        {
            Done.text = "YOU WIN";
        }
        if (score < 12)
        {
            Done.text = "YOU LOSE";
        }
        yield return new WaitForSeconds(6f);
        SceneManager.LoadScene("SampleScene");
        Debug.Log("2");
        
    }
}

