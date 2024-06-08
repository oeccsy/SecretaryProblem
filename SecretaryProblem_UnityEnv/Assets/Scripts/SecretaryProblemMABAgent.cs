using System.Collections;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine.Serialization;

public class SecretaryProblemMABAgent : Agent
{
    [Header("Grid World")]
    public SecretaryGrid secretaryGrid;

    [Header("Decision 진행 속도")]
    public float renderDelay;
    
    [Header("Agent Pos")]
    [SerializeField]
    private int rowPos;                     // [0, secretaryGrid.GetRowCount)
    [SerializeField]
    private int colPos;                     // [0, secretaryGrid.GetColCount)
    
    //초기화 작업을 위해 한번 호출되는 메소드
    public override void Initialize()
    {
        ResetAgent();
        StartCoroutine(RequestDecisionRoutine());
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
        // MAB는 관찰할 State가 존재하지 않음
        // sensor.AddObservation(0);
    }

    //브레인으로 부터 전달받은 액션(행위)를 실행하는 메소드
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Debug.Log($"OnActionReceived : {actionBuffers.DiscreteActions[0]}");
        
        int skipAmount = actionBuffers.DiscreteActions[0];  // 무조건 탈락시킬 면접자 수
        Secretary selectedSecretary = null;
        
        for (int i = 0; i < secretaryGrid.GetTotalSecretaryCount(); i++)
        {
            // k명의 면접자는 무조건 탈락
            if (i < skipAmount) continue;
                
            // 이전 면접자들보다 뛰어난 면접자를 발견하면 인터뷰 중단
            Secretary interviewTarget = secretaryGrid.GetSecretary(i);
            if (interviewTarget.rankingAfterInterview == 1)
            {
                selectedSecretary = interviewTarget;
                break;
            }
        }

        if (selectedSecretary == null)
        {
            int row = secretaryGrid.GetRowCount() - 1;
            int col = secretaryGrid.GetColCount() - 1;
            selectedSecretary = secretaryGrid.GetSecretary(row, col);
        }
        
        rowPos = selectedSecretary.row;
        colPos = selectedSecretary.col;
        transform.position = new Vector3(2 * colPos, -2 * rowPos, 0);
        
        if (selectedSecretary.ranking == 1)
        {
            selectedSecretary.SetMaterial(SecretaryProblemSettings.Instance.correctSecretaryMat);
            SetReward(1.0f);
        }
        else
        {
            selectedSecretary.SetMaterial(SecretaryProblemSettings.Instance.wrongSecretaryMat);
            SetReward(-1.0f);
        }
    }

    //개발자(사용자)가 직접 명령을 내릴때 호출하는 메소드(주로 테스트용도 또는 모방학습에 사용)
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        Debug.Log("Heuristic");
        var discreteActionsOut = actionsOut.DiscreteActions;

        discreteActionsOut[0] = Mathf.RoundToInt(secretaryGrid.GetTotalSecretaryCount() * 0.368f);
    }

    // agent의 정보를 reset하는 로직
    private void ResetAgent()
    {
        // Agent Pos
        rowPos = 0;
        colPos = 0;
        transform.position = new Vector3(2 * colPos, -2 * rowPos, 0);
    }
    
    // action을 진행하는 주기를 결정하는 로직
    private IEnumerator RequestDecisionRoutine()
    {
        if (Academy.Instance.IsCommunicatorOn)
        {
            while (true)
            {
                RequestDecision();
                yield return null;
                EndEpisode();
                yield return null;
            }
        }
        else
        {
            WaitForSeconds delay = new WaitForSeconds(renderDelay);
            
            while (true)
            {
                RequestDecision();
                yield return delay;
                EndEpisode();
                yield return delay;
            }
        }
    }
}
