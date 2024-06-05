using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

public class SecretaryProblemMABAgent : Agent
{
    [Header("Grid World")]
    // Secretary Grid
    public SecretaryGrid secretaryGrid;
    
    [Header("Decision 진행 속도")]
    public float timeBetweenDecisionsAtInference;
    private float m_timeSinceDecision;

    [Header("Agent Pos")]
    [SerializeField]
    private int rowPos;                     // [0, secretaryGrid.GetRowCount)
    [SerializeField]
    private int colPos;                     // [0, secretaryGrid.GetColCount)

    private enum State
    {
        BeforeInterview,
        SelectBestSecretary,
        FailSelectBestSecretary
    }

    private State state = State.BeforeInterview; 
    
    //초기화 작업을 위해 한번 호출되는 메소드
    public override void Initialize()
    {
        ResetAgent();
    }

    //에피소드(학습단위)가 시작할때마다 호출
    public override void OnEpisodeBegin()
    {
        Debug.Log("OnEpisodeBegin");
        
        secretaryGrid.InitSecretaryRanking();
        secretaryGrid.InitSecretaryRankingOnInterview();

        ResetAgent();
    }

    //환경 정보를 관측 및 수집해 정책 결정을 위해 브레인에 전달하는 메소드
    public override void CollectObservations(Unity.MLAgents.Sensors.VectorSensor sensor)
    {
        Debug.Log("CollectObservations");
        
        sensor.AddObservation((int)state);
        // state 0 면접 진행 전
        // state 1 면접 진행 후 best 찾음
        // state 2 면접 진행 후 best 못찾음
        
        
        if (state == State.SelectBestSecretary || state == State.FailSelectBestSecretary)
        {
            Debug.Log(state.ToString());
        }
    }

    //브레인으로 부터 전달받은 액션(행위)를 실행하는 메소드
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Debug.Log($"OnActionReceived : {actionBuffers.DiscreteActions[0]}");

        Secretary selectedSecretary = null;
        int action = actionBuffers.DiscreteActions[0]; // 무조건 탈락시킬 면접자 수

        for (int row = 0; row < secretaryGrid.GetRowCount(); row++)
        {
            for (int col = 0; col < secretaryGrid.GetColCount(); col++)
            {
                // k명의 면접자는 무조건 탈락
                if (row * secretaryGrid.GetRowCount() + col < action)
                {
                    // Debug.Log($"{row * secretaryGrid.GetRowCount() + col} 탈락!");
                    continue;
                }
                
                // 이후 등장한 면접자 중 가장 뛰어난 면접자를 발견하면 선택
                Secretary curSecretary = secretaryGrid.GetSecretary(row, col);
                if (curSecretary.rankingAfterInterview == 1)
                {
                    // Debug.Log($"발견! {row * secretaryGrid.GetRowCount() + col}");
                    selectedSecretary = curSecretary;
                    transform.position = new Vector3(2 * col, -2 * row, 0);
                    break;
                }
            }

            if (selectedSecretary != null) break;
        }

        if (selectedSecretary == null || selectedSecretary.ranking != 1)
        {
            SetReward(-1.0f);
            state = State.FailSelectBestSecretary;
            EndEpisode();
        }
        else if (selectedSecretary.ranking == 1)
        {
            SetReward(1.0f);
            state = State.SelectBestSecretary;
            EndEpisode();
        }
    }

    //개발자(사용자)가 직접 명령을 내릴때 호출하는 메소드(주로 테스트용도 또는 모방학습에 사용)
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        Debug.Log("Heuristic");
        var discreteActionsOut = actionsOut.DiscreteActions;

        //discreteActionsOut[0] = (int)(secretaryGrid.GetTotalSecretaryCount() * 0.368f);
        discreteActionsOut[0] = 37;
    }

    // agent의 정보를 reset하는 로직
    public void ResetAgent()
    {
        // Agent Pos
        transform.position = Vector3.zero;
        rowPos = 0;
        colPos = 0;
        state = State.BeforeInterview;
    }

    // action을 진행하는 주기를 결정하는 로직
    public void FixedUpdate()
    {
        WaitTimeInference();
    }

    void WaitTimeInference()
    {
        if (Academy.Instance.IsCommunicatorOn)
        {
            RequestDecision();    
        }
        else
        {
            if (m_timeSinceDecision >= timeBetweenDecisionsAtInference)
            {
                m_timeSinceDecision = 0f;
                RequestDecision();
            }
            else
            {
                m_timeSinceDecision += Time.fixedDeltaTime;
            }
        }

    }
}
