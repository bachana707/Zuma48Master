using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PathCreation;


public class Ball : MonoBehaviour
{

    float travaledDistance;
    public Transform target;
    public bool canMove = false;
    public bool isBonus = false;
    [HideInInspector] public bool inserted = false;

    private Coroutine shootCoroutine;
    public GameObject particlePrefab;

    public int index;

    public int value;
    public TextMeshProUGUI valueText;

    public Canvas canvas;
    public SpriteRenderer spriteRenderer { private set; get; }

    public CircleCollider2D collider { private set; get; }

    void Awake()
    {
        collider = GetComponent<CircleCollider2D>();

        canvas.worldCamera = Camera.main;

        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    void Start()
    {
        if (!isBonus)
            valueText.text = value.ToString();
    }


    void Update()
    {


        if (canMove && GameManager.Instance.gameActive)
        {
            travaledDistance += GameManager.Instance.ballSpeed * Time.deltaTime;
            transform.position = GameManager.Instance.levelPath.path.GetPointAtDistance(travaledDistance);


            //Vector3 difference = target.position - transform.position;
            //float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            //transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
        }
    }

    public void ChildrenMovementSwitch(bool move)
    {

        for (int i = index + 1; i < GameManager.Instance.balls.Count; i++)
        {
            GameManager.Instance.balls[i].canMove = move;
        }
    }
    public void ParentMovementSwitch(bool move)
    {
        if (GameManager.Instance.balls.Count == 0)
            return;

        for (int i = index - 1; i >= 0; i--)
        {
            GameManager.Instance.balls[i].canMove = move;
        }
    }


    public void Throw(Vector2 targetPos, float throwPower)
    {
        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }

        GameManager.Instance.freeBall = null;
        shootCoroutine = StartCoroutine(shoot(targetPos, throwPower));
        AudioManager.Instance.PlayShootSound();

        GameManager.Instance.DoHaptic(1);


    }
    public IEnumerator shoot(Vector2 targetPos, float throwPower)
    {

        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;


        while (!inserted)
        {
            transform.Translate(direction * throwPower * Time.deltaTime);

            if (!GetComponent<Renderer>().isVisible)
            {
                Destroy(gameObject);

                GameManager.Instance.ReloadNumber();
            }

            yield return new WaitForEndOfFrame();
        }

        shootCoroutine = null;
    }

    public void UpdateNuberValue(int value)
    {
        this.value = value;
        valueText.text = value.ToString();

        spriteRenderer.color = GameManager.Instance.getElementColor(value);
    }

    public void MoveParent(int direction)
    {

        StartCoroutine(MoveParentBehavior(direction));
    }

    public IEnumerator MoveParentBehavior(int direction)
    {

        float timePassed = 0;
        float MoveSpeed = Mathf.Sign(direction) * (collider.radius * 2 * transform.lossyScale.x) / GameManager.Instance.insertAnimDuration;

        float endTraveledDistance = travaledDistance + Mathf.Sign(direction) * collider.radius * 2 * transform.lossyScale.x;

        while (timePassed <= GameManager.Instance.insertAnimDuration)
        {

            travaledDistance += MoveSpeed * Time.deltaTime;
            transform.position = GameManager.Instance.levelPath.path.GetPointAtDistance(travaledDistance);


            yield return new WaitForEndOfFrame();

            timePassed += Time.deltaTime;
        }

        travaledDistance = endTraveledDistance;
        transform.position = GameManager.Instance.levelPath.path.GetPointAtDistance(travaledDistance);
    }


    public void InsertBall(float travDist, Vector3 target, int index)
    {

        this.index = index;
        GameManager.Instance.balls.Insert(index, this);

        for (int i = index - 1; i >= 0; i--)
        {
            GameManager.Instance.balls[i].MoveParent(1);
        }

        StartCoroutine(Insert(travDist, target));
    }

    public IEnumerator Insert(float travDist, Vector3 target)
    {

        Destroy(GetComponent<Rigidbody2D>());
        collider.isTrigger = true;
        float timePassed = 0;
        float MoveSpeed = (Vector2.Distance(transform.position, target)) / GameManager.Instance.insertAnimDuration;
        Vector2 dir = (target - transform.position).normalized;

        while (timePassed <= GameManager.Instance.insertAnimDuration)
        {

            transform.Translate(dir * MoveSpeed * Time.deltaTime);

            yield return new WaitForEndOfFrame();

            timePassed += Time.deltaTime;
        }

        travaledDistance = travDist;
        transform.position = GameManager.Instance.levelPath.path.GetPointAtDistance(travaledDistance);
        transform.position = target;

        gameObject.tag = Constants.NumberTag;
        gameObject.layer = 0;



        for (int i = index + 1; i < GameManager.Instance.balls.Count; i++)
        {
            GameManager.Instance.balls[i].index++;
        }


        GameManager.Instance.CheckNumbersValue(index);


    }


    public void ThrowParticle()
    {
        GameObject particleObject = Instantiate(
            particlePrefab,
            new Vector3(transform.position.x, transform.position.y, transform.position.z - 1),
            particlePrefab.transform.rotation
            );

        ParticleSystem particle = particleObject.GetComponent<ParticleSystem>();

        ParticleSystem.MainModule mainPart = particle.main;
        mainPart.startColor = spriteRenderer.color;

        GameManager.Instance.destroyAfterDelay(particleObject, particle.main.duration);

        GameManager.Instance.DoHaptic(0);

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.CompareTag(Constants.BonusTag))
        {
            //FIXMEE
            //Ball collidedBall = collision.GetComponent<Ball>();
            //if (collidedBall.inserted)
            //    return;

            //collidedBall.inserted = true;
            
            //collision.GetComponent<Bonus>().doBonusAction(this);
        }


        if (collision.CompareTag(Constants.BallTag) || collision.CompareTag(Constants.BonusTag))
        {

            Ball collidedBall = collision.GetComponent<Ball>();

            if (collidedBall.inserted)
                return;

            collidedBall.inserted = true;

            GameManager.Instance.paused = true;

            canMove = false;
            ParentMovementSwitch(false);
            ChildrenMovementSwitch(false);

            if (collision.CompareTag(Constants.BonusTag))
            {
                collidedBall.value = value;
            }

            if (
                index == 0 &&
                GameManager.Instance.balls.Count > 1 &&
                Vector2.Distance(collision.transform.position, GameManager.Instance.balls[1].transform.position) > Mathf.Sqrt(2) * collider.radius * 2 * transform.lossyScale.x
                )
            {
                Vector3 newPos = GameManager.Instance.levelPath.path.GetPointAtDistance(travaledDistance + collider.radius * 2 * transform.lossyScale.x);
                collidedBall.InsertBall(travaledDistance + collider.radius * 2 * transform.lossyScale.x, newPos, index);
            }
            else
            {
                collidedBall.InsertBall(travaledDistance, transform.position, index + 1);
            }

        }
    }
}



