using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DroneAI : MonoBehaviour
{
    //����� ���� ��� ����

    //private �Ӽ������� �����Ϳ� ����ȴ�.
    [SerializeField]
    //ü��
    int hp = 3;
    //���� ȿ��
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

    DroneState state = DroneState.Idle;//�ʱ� ���´� ��� ����
    //��� ������ ���� �ð�
    public float idleDelayTime = 5;
    //��� �ð�
    float currentTIme;
    //�̵��ӵ� 
    public float moveSpeed = 1;
    //Ÿ�� ��ġ
    Transform tower;
    //���� �������� �̵��ϱ� 

    //�� ã�⸦ ������ ������̼� �޽� ������Ʈ
    NavMeshAgent agent;
    //���� ����
    public float attackRange = 200;
    //���� ���� �ð�
    public float attackDelayTime = 2;



    void Start()
    {
        //Ÿ�� ã��
        tower = GameObject.Find("Tower").transform;
        //NavMeshAGent ������Ʈ ��������
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
        //agent�� �ӵ� ����
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
        // ������̼��� ������ ����
        if (agent.enabled)
        {
            agent.SetDestination(tower.position);
        }

        // ���� ���� �ȿ� ������ ��� ���·� ��ȯ
        if (Vector3.Distance(transform.position, tower.position) < 6)
        {

            agent.enabled = false; // agent ���� ����
            //state = DroneState.Attack;
            StartCoroutine(MoveUp()); // Coroutine ����
        }
    }
    
    private IEnumerator MoveUp()
    {
        // ����� y ��ǥ�� 10�� ������ ������ õõ�� ���
        while (transform.position.y < 10)
        {
            Vector3 newPosition = transform.position;
            newPosition.y += 1 * Time.deltaTime; // õõ�� ���
            transform.position = newPosition;

            yield return null; // ���� �����ӱ��� ���
        }

        // y ��ǥ�� 10�� �����ϸ� ���� ���·� ��ȯ
        state = DroneState.Attack;
    }


    private void Attack()
    {

        //1.�ð��� �帥��.
        currentTIme += Time.deltaTime;
        //2.��� �ð��� ���������� �ʱ�ȭ�ϸ�
        if (currentTIme > attackDelayTime)
        {
            Tower.Instance.HP--;
            Debug.Log("Tower Hp : "+Tower.Instance.HP);
            //3.����
            
            
            //4.��� �ð� �ʱ�ȭ
            currentTIme = 0;
        }
    }
    public void OnDamageProcess()
    {
        //ü���� ���ҽ�Ű�� ���� �ʾҴٸ� ���¸� �������� ��ȯ�ϰ� �ʹ�.
        //1. ü�� ����
        hp--;
        //2.���� ���� �ʾҴٸ�
        if (hp > 0)
        {
            //3.���¸� �������� ��ȯ
            state = DroneState.Damage;
            //StopAllCoroutines();
            StartCoroutine(Damage());
        }
        else
        {
            //������ ��ġ ����
            explosion.position = transform.position;
            //����Ʈ ���
            expEffect.Play();
            expAudio.Play();
            Destroy(gameObject); 
        }
    }
    IEnumerator Damage()
    {
        //1.��ã�� ����
        agent.enabled = false;
        //2.�ڽ� ��ü�� MeshRenderer���� ���� ������
        Material mat = GetComponentInChildren<MeshRenderer>().material;
        //3.���� ���� ����
        Color originalColor = mat.color;
        //4.������ �� ����
        mat.color = Color.blue;
        //5. 0,1�� ��ٸ���
        yield return new WaitForSeconds(1);
        //6.������ ���� �������
        mat.color=originalColor;
        //7. ���¸� Idle�� ��ȯ
        state = DroneState.Idle;
        //8. ��� �ð� �ʱ�ȭ
        currentTIme = 0;
    }
    private void Die() { }

    
}
