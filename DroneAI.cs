using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DroneAI : MonoBehaviour
{
    //드론의 상태 상수 정의

    //private 속성이지만 에디터에 노출된다.
    [SerializeField]
    //체력
    int hp = 3;
    //폭발 효과
    Transform explosion;
    ParticleSystem expEffect;
    AudioSource expAudio;


    
    enum DroneState
    {
        Idle,
        Move,
        Attack,
        Damage,
        Die
    }

    DroneState state = DroneState.Idle;//초기 상태는 대기 상태
    //대기 상태의 지속 시간
    public float idleDelayTime = 5;
    //경과 시간
    float currentTIme;
    //이동속도 
    public float moveSpeed = 1;
    //타워 위치
    Transform tower;
    //공격 지점으로 이동하기 

    //길 찾기를 수행할 내비게이션 메시 에이전트
    NavMeshAgent agent;
    //공격 범위
    public float attackRange = 200;
    //공격 지연 시간
    public float attackDelayTime = 2;



    void Start()
    {
        //타워 찾기
        tower = GameObject.Find("Tower").transform;
        //NavMeshAGent 컴포넌트 가져오기
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
        //agent의 속도 설정
        agent.speed = moveSpeed;

        explosion = GameObject.Find("Explosion").transform;
        expEffect=explosion.GetComponent<ParticleSystem>(); 
        expAudio=explosion.GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        print("current State : " + state);
        switch (state)
        {
            case DroneState.Idle:
                Idle();
                break;
            
            case DroneState.Move:
                Move();
                break;
            
            case DroneState.Attack:
                Attack();
                break;
            case DroneState.Damage:
                //Damage();
                break;
            case DroneState.Die:
                
                break;
        }
        
    }
    private void Idle()
    {
        currentTIme += Time.deltaTime;
        if (currentTIme < idleDelayTime)
        {
            state = DroneState.Move;
            agent.enabled = true;
        }

    }
    
    private void Move()
    {
        // 내비게이션할 목적지 설정
        if (agent.enabled)
        {
            agent.SetDestination(tower.position);
        }

        // 공격 범위 안에 들어오면 상승 상태로 전환
        if (Vector3.Distance(transform.position, tower.position) < 6)
        {

            agent.enabled = false; // agent 동작 정지
            //state = DroneState.Attack;
            StartCoroutine(MoveUp()); // Coroutine 시작
        }
    }
    
    private IEnumerator MoveUp()
    {
        // 드론의 y 좌표가 10에 도달할 때까지 천천히 상승
        while (transform.position.y < 10)
        {
            Vector3 newPosition = transform.position;
            newPosition.y += 1 * Time.deltaTime; // 천천히 상승
            transform.position = newPosition;

            yield return null; // 다음 프레임까지 대기
        }

        // y 좌표가 10에 도달하면 공격 상태로 전환
        state = DroneState.Attack;
    }


    private void Attack()
    {

        //1.시간이 흐른다.
        currentTIme += Time.deltaTime;
        //2.경과 시간이 공격지연을 초기화하면
        if (currentTIme > attackDelayTime)
        {
            Tower.Instance.HP--;
            Debug.Log("Tower Hp : "+Tower.Instance.HP);
            //3.공격
            
            
            //4.경과 시간 초기화
            currentTIme = 0;
        }
    }
    public void OnDamageProcess()
    {
        //체력을 감소시키고 죽지 않았다면 상태를 데미지로 전환하고 싶다.
        //1. 체력 감소
        hp--;
        //2.만약 죽지 않았다면
        if (hp > 0)
        {
            //3.상태를 데미지로 전환
            state = DroneState.Damage;
            //StopAllCoroutines();
            StartCoroutine(Damage());
        }
        else
        {
            //폭발의 위치 지정
            explosion.position = transform.position;
            //이펙트 재생
            expEffect.Play();
            expAudio.Play();
            Destroy(gameObject); 
        }
    }
    IEnumerator Damage()
    {
        //1.길찾기 중지
        agent.enabled = false;
        //2.자식 객체의 MeshRenderer에서 재질 얻어오기
        Material mat = GetComponentInChildren<MeshRenderer>().material;
        //3.원래 색을 저장
        Color originalColor = mat.color;
        //4.재질의 색 변경
        mat.color = Color.blue;
        //5. 0,1초 기다리기
        yield return new WaitForSeconds(1);
        //6.재질의 색을 원래대로
        mat.color=originalColor;
        //7. 상태를 Idle로 전환
        state = DroneState.Idle;
        //8. 경과 시간 초기화
        currentTIme = 0;
    }
    private void Die() { }

    
}
