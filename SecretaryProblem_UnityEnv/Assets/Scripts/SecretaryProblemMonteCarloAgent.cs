using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

public class SecretaryProblemMonteCarloControlAgent : Agent
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

    //초기화 작업을 위해 한번 호출되는 메소드
    public override void Initialize()
    {
        Debug.Log("Initialize");
        SecretaryProblemSettings.Instance.InitMaterialSettigns();
        
        secretaryGrid.InitSecretaryGrid();
        secretaryGrid.InitSecretaryRanking();
        secretaryGrid.InitSecretaryRankingOnInterview();
        secretaryGrid.InitSecretaryMat();
        
        ResetAgent();
    }

    //에피소드(학습단위)가 시작할때마다 호출
    public override void OnEpisodeBegin()
    {
        Debug.Log("OnEpisodeBegin");
        
        secretaryGrid.InitSecretaryRanking();
        secretaryGrid.InitSecretaryRankingOnInterview();
        secretaryGrid.InitSecretaryMat();
        
        ResetAgent();
    }

    //환경 정보를 관측 및 수집해 정책 결정을 위해 브레인에 전달하는 메소드
    public override void CollectObservations(Unity.MLAgents.Sensors.VectorSensor sensor)
    {
        Debug.Log("CollectObservations");
        
        // 지원자 총원
        int totalSecretaryCount = secretaryGrid.GetTotalSecretaryCount();
        
        // 현재 지원자를 포함하여 면접을 진행한 면접자 수
        int totalCheckCount = rowPos * secretaryGrid.GetRowCount() + colPos + 1;

        // 현재 면접자의 지금까지 면접자중의 순위 
        int curSecretaryRanking = secretaryGrid.GetSecretary(rowPos, colPos).rankingAfterInterview;

        // 관측 정보 전달
        sensor.AddObservation(totalSecretaryCount);     // 지원자 총원 == n
        sensor.AddObservation(totalCheckCount);         // 지금까지 검사한 인원 [1, n]
        sensor.AddObservation(curSecretaryRanking);     // 현재 지원자의 순위 [1, n]            
    }

    //브레인으로 부터 전달받은 액션(행위)를 실행하는 메소드
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Debug.Log($"OnActionReceived : {actionBuffers.DiscreteActions[0]}");
        
        int action = actionBuffers.DiscreteActions[0];
        
        switch (action)
        {
            case 0:
                // Select : 선택한 Secretary가 실제로 1순위인 경우 +1, 아닌 경우 -1로 종료
                Secretary selectedSecretary = secretaryGrid.GetSecretary(rowPos, colPos);

                if (selectedSecretary.ranking == 1)
                {
                    SetReward(1.0f);
                    EndEpisode();
                }
                else
                {
                    SetReward(0.0f);
                    EndEpisode();
                }
                break;
            case 1 :
                // Pass : 더이상 움직일 수 없으면 -1로 종료, 아닌 경우 이동하여 계속 진행
                if ((rowPos + 1) * (colPos + 1) == secretaryGrid.GetTotalSecretaryCount())
                {
                    SetReward(0.0f);
                    EndEpisode();
                }
                else
                {
                    colPos++;
                    if(colPos == secretaryGrid.GetColCount())
                    {
                        colPos = 0;
                        rowPos++;
                    }
                    
                    // Agent 이동
                    transform.position = new Vector3(2 * colPos, -2 * rowPos, 0);
                }
                break;
            case 2 :
                // do nothing
                SetReward(-0.01f);
                break;
        }
    }

    //개발자(사용자)가 직접 명령을 내릴때 호출하는 메소드(주로 테스트용도 또는 모방학습에 사용)
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        Debug.Log("Heuristic");
        var discreteActionsOut = actionsOut.DiscreteActions;
        
        discreteActionsOut[0] = 2; // default : pass
        
        if (Input.GetKey(KeyCode.S))
        {
            Debug.Log("Select");
            discreteActionsOut[0] = 0;
        }
        else if (Input.GetKey(KeyCode.P))
        {
            Debug.Log("Pass");
            discreteActionsOut[0] = 1;
        }
    }

    // agent의 정보를 reset하는 로직
    public void ResetAgent()
    {
        // Agent Pos
        transform.position = Vector3.zero;
        rowPos = 0;
        colPos = 0;
    }

    // action을 진행하는 주기를 결정하는 로직
    private void Update()
    {
        WaitTimeInference();
    }

    private void WaitTimeInference()
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
                m_timeSinceDecision += Time.deltaTime;
            }
        }
    }
}
