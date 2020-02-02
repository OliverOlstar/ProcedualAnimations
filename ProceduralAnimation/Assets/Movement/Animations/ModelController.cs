using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelController : MonoBehaviour
{
    private Rigidbody _rb;
    private Animator _anim;

    [HideInInspector] public bool onGround = false;
    [HideInInspector] public Vector3 acceleration;

    private ModelWeights _modelWeights;
    private ModelAnimations _modelAnimation;
    private ModelMovement _modelMovement;

    [HideInInspector] public Vector3 horizontalVelocity;
    // 0 - done attack, 1 - attacking, 2 - done attacking but on delay
    private int _AttackingState;
    private bool _AttackingDirection;
    private bool _Dodging;

    [Space]
    public SOAttack[] attacks;
    private float _doneAttackDelay = 0;
    
    void Start()
    {
        // Get Model Components
        _modelWeights = GetComponent<ModelWeights>();
        _modelAnimation = GetComponent<ModelAnimations>();
        _modelMovement = GetComponent<ModelMovement>();

        // Get Other Components
        _rb = GetComponentInParent<Rigidbody>();
        _anim = GetComponent<Animator>();

        // Setup Components
        _modelWeights.Init(this, _anim);
        _modelAnimation.Init(this, _rb, _anim);
        _modelMovement.Init(this);
    }

    void FixedUpdate()
    {
        horizontalVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);

        if (_Dodging == false)
        {
            if (_AttackingState == 1)
            {
                if (_modelAnimation.AttackingAnim(_AttackingDirection))
                {
                    StartCoroutine("DoneAttackWithDelay");
                }
            }
            else if (_AttackingState == 0)
            {
                _modelWeights.UpdateWeights();
            }
        }

        _modelWeights.LerpWeights();

        _modelMovement.TiltingParent();
        _modelMovement.FacingSelf();

        _modelAnimation.SteppingAnim();
        _modelAnimation.JumpingAnim();
    }

    #region Attacking
    public void PlayAttack(int pIndex, bool pHeavy)
    {
        SOAttack curAttack = attacks[pIndex + (pHeavy ? 3 : 0)];

        StopCoroutine("DoneAttackWithDelay");
        StopCoroutine("PlayAttackWithDelay");
        StartCoroutine("PlayAttackWithDelay", curAttack.holdStartPosTime);

        _AttackingDirection = pIndex == 1 ? false : true;
        _doneAttackDelay = curAttack.holdEndPosTime;
        _modelMovement.DisableRotation = true;
        _modelWeights.SetWeights(0, 0, 1, 0);
        _modelAnimation.StartAttack(pIndex, pHeavy);
    }

    private IEnumerator PlayAttackWithDelay(float pDelay)
    {
        _AttackingState = 2;
        yield return new WaitForSeconds(pDelay);
        _AttackingState = 1;
    }

    private IEnumerator DoneAttackWithDelay()
    {
        _AttackingState = 2;
        yield return new WaitForSeconds(_doneAttackDelay);
        DoneAttack();
    }

    public void DoneAttack()
    {
        _AttackingState = 0;
        _modelMovement.DisableRotation = false;
        _modelWeights.SetWeights(0, 0, 0, 0);
    }
    #endregion

    #region Dodging
    public void PlayDodge(Vector2 pDirection, float pSpeed)
    {
        _Dodging = true;
        _modelWeights.SetWeights(0, 0, 0, 1);
        _modelMovement.PlayFlipParent(pDirection, pSpeed);
    }

    public void DoneDodge()
    {
        _Dodging = false;
        _modelWeights.SetWeights(0, 0, 0, 0);
    }
    #endregion

    #region Crouching
    public void AddCrouching(float pValue, float pGoingToLength, float pGoingAwayLength)
    {
        _modelWeights.AddCrouching(pValue, pGoingToLength, pGoingAwayLength);
    }
    #endregion

    public Vector2 GetCatmullRomPosition(float pTime, SOGraph pGraph)
    {
        Vector2 startPoint = Vector2.zero;
        Vector2 endPoint = new Vector2(1, pGraph.EndValue);

        Vector2 a = 2f * startPoint;
        Vector2 b = endPoint - pGraph.firstBender;
        Vector2 c = 2f * pGraph.firstBender - 5f * startPoint + 4f * endPoint - pGraph.secondBender;
        Vector2 d = -pGraph.firstBender + 3f * startPoint - 3f * endPoint + pGraph.secondBender;

        Vector2 pos = 0.5f * (a + (b * pTime) + (c * pTime * pTime) + (d * pTime * pTime * pTime));

        return pos;
    }
}
