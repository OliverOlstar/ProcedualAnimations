using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelAnimations : MonoBehaviour
{
    private ModelController _modelController;
    private Rigidbody _rb;
    private Animator _anim;

    [Header("Animation Lengths")]
    [SerializeField] private float _stepMult = 1;
    [SerializeField] private float _fallMult = 1;

    private float _stepProgress = 0;
    private float _fallProgress = 0;
    private float _attackProgress = 0;

    [Header("Interpolation Graphs")]
    [SerializeField] private SOGraph _stepGraph;
    [SerializeField] private SOGraph _fallGraph;
    private SOGraph _attackGraph;
    
    [Space]
    [SerializeField] private float _jumpingTransitionWidth = 1;

    //Attack
    private float _attackLength = 1;
    private float _attackTransitionRate = 0;

    private int _doneAttackTransition = 0;

    public void Init(ModelController pController, Rigidbody pRb, Animator pAnim)
    {
        _modelController = pController;
        _rb = pRb;
        _anim = pAnim;
    }

    #region Attacking
    // True = go towards 1, False = towards 0
    public bool AttackingAnim(bool pDirection)
    {
        // Return if still transtioning to next attack
        if (_doneAttackTransition < 2)
            return false;

        // Increase progress value
        _attackProgress = Mathf.Min(1, _attackProgress + Time.fixedDeltaTime / _attackLength);

        // Set progress value
        if (pDirection)
            _anim.SetFloat("Attacking Progress", _modelController.GetCatmullRomPosition(_attackProgress, _attackGraph).y);
        else
            _anim.SetFloat("Attacking Progress", 1 - _modelController.GetCatmullRomPosition(_attackProgress, _attackGraph).y);

        // Return true if reached anim goal
        return _attackProgress == 1;
    }

    public void StartAttack(int pIndex, bool pHeavy)
    {
        // Reset attack progress
        _attackProgress = 0;
        _anim.SetFloat("Attacking Progress", pIndex == 1 ? 1 : 0);

        // Get SOAttack values
        SOAttack curAttack = _modelController.attacks[pIndex + (pHeavy ? 3 : 0)];
        _attackLength = curAttack.attackTime;
        _attackGraph = curAttack.attackGraph;

        if (pIndex == 0)
        {
            // Snap to first attack
            _anim.SetFloat("Attacking Index", 0);
            _anim.SetFloat("Attacking Type", pHeavy ? 1 : 0);
            _doneAttackTransition = 2;
        }
        else
        {
            // Start transitions to next attack
            _attackTransitionRate = 1 / curAttack.transitionToTime;
            _doneAttackTransition = 0;

            StopCoroutine("AttackingTypeTransition");
            StartCoroutine("AttackingTypeTransition", pHeavy ? 1 : 0);

            StopCoroutine("AttackingIndexTransition");
            StartCoroutine("AttackingIndexTransition", pIndex);
        }
    }

    // Transition between attacks in combo
    private IEnumerator AttackingIndexTransition(float pIndex)
    {
        // Starting Value
        float curIndex = _anim.GetFloat("Attacking Index");

        // Transition to Target Value
        while (curIndex < pIndex)
        {
            curIndex += _attackTransitionRate * Time.deltaTime;
            _anim.SetFloat("Attacking Index", curIndex);
            yield return null;
        }

        // Snap to Target Value
        _anim.SetFloat("Attacking Index", pIndex);
        _doneAttackTransition++;
    }

    // Transition between heavy & light attacks
    private IEnumerator AttackingTypeTransition(float pType)
    {
        // Starting Value
        float curType = _anim.GetFloat("Attacking Type");

        // Transition to Target Value
        while ((curType < pType && pType == 1) || (curType > pType && pType == 0))
        {
            curType += _attackTransitionRate * Time.deltaTime * (pType == 0 ? -1 : 1);
            _anim.SetFloat("Attacking Type", curType);
            yield return null;
        }

        // Snap to Target Value
        _anim.SetFloat("Attacking Type", pType);
        _doneAttackTransition++;
    }
    #endregion

    #region Jumping
    public void JumpingAnim()
    {
        // Jumping Speed
        _anim.SetFloat("Jumping Speed", _rb.velocity.y / _jumpingTransitionWidth);

        // Increase Falling Animation
        _fallProgress = increaseProgress(_fallProgress, _fallMult);
        _anim.SetFloat("Falling Progress", _modelController.GetCatmullRomPosition(_fallProgress, _fallGraph).y);
    }
    #endregion

    #region Stepping
    public void SteppingAnim()
    {
        // Increase Stepping Animation
        _stepProgress = increaseProgress(_stepProgress, _stepMult);

        float secondStep = (_stepProgress <= 0.5f) ? 0 : 0.5f;
        float time = (_stepProgress - secondStep) * 2;

        float steppingSpeed = _modelController.horizontalVelocity.magnitude / GetComponentInParent<MovementComponent>().maxSpeed;

        // Set Anim Stepping values
        _anim.SetFloat("Stepping Progress", _modelController.GetCatmullRomPosition(time, _stepGraph).y + secondStep);
        _anim.SetFloat("Stepping Speed", (steppingSpeed > 1) ? 1 : steppingSpeed);

        // Set Anim Direction
        Vector3 relDirection = transform.rotation * new Vector3(-_modelController.horizontalVelocity.x, 0, _modelController.horizontalVelocity.z);
        relDirection.y = 0;
        relDirection.Normalize();

        _anim.SetFloat("Move Direction X", relDirection.x);
        _anim.SetFloat("Move Direction Z", relDirection.z);
    }
    #endregion

    private float increaseProgress(float pProgress, float pMult)
    {
        pProgress += Time.fixedDeltaTime * pMult;
        if (pProgress >= 1)
            pProgress -= 1;

        return pProgress;
    }
}
