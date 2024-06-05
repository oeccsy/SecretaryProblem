using System.Collections;
using System.Collections.Generic;
using Base;
using UnityEngine;

public class Secretary_Grid : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField]
    private int rowCount;
    [SerializeField]
    private int colCount;

    [Header("Prefab Settings")]
    [SerializeField]
    private GameObject secretaryPrefab;

    private Transform gridTransform;

    private Secretary[,] secretaryGrid;
    
    private void Awake()
    {
        gridTransform = transform;
        
        InitSecretaryGrid();
        InitSecretaryRanking();
        InitSecretaryRankingOnInterview();
    }

    private void InitSecretaryGrid()
    {
        secretaryGrid = new Secretary[rowCount,colCount];
        
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                Secretary newSecretary = Instantiate(secretaryPrefab, new Vector3(2*col, -2*row, -1), Quaternion.identity, gridTransform).GetComponent<Secretary>();
                secretaryGrid[row, col] = newSecretary;
            }
        }
    }

    public void InitSecretaryRanking() // 각각의 Secretary에게 1등부터 n등까지를 부여한다.
    {
        List<int> availableRankList = new List<int>();
        
        for (int i = 1; i <= rowCount * colCount; i++)
        {
            availableRankList.Add(i);
        }

        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                int randomIndex = Random.Range(0, availableRankList.Count);
                secretaryGrid[row, col].real_ranking = availableRankList[randomIndex];
                availableRankList.RemoveAt(randomIndex);
            }
        }
    }

    public void InitSecretaryRankingOnInterview()   // 면접관과 인터뷰할 때 갖게 될 ranking을 계산하여 미리 부여한다.
    {
        List<int> rankList = new List<int>();

        Secretary secretary = secretaryGrid[0, 0];
        rankList.Add(secretary.real_ranking);
        secretary.ranking_on_interview = 1;
        
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                if (row == 0 && col == 0) continue;
                
                secretary = secretaryGrid[row, col];
                int ranking = secretary.real_ranking;
                int listCount = rankList.Count;

                for (int i = 0; i < listCount; i++)
                {
                    if (ranking < rankList[i])
                    {
                        rankList.Insert(i, ranking);
                        secretary.ranking_on_interview = i + 1;
                        break;
                    }
                    else if (i == listCount - 1)
                    {
                        rankList.Add(ranking);
                        secretary.ranking_on_interview = i + 2;
                    }
                }
            }
        }
    }

    public Secretary GetSecretary(int row, int col)
    {
        return secretaryGrid[row, col];
    }

    public int GetTotalSecretaryCount()
    {
        return rowCount * colCount;
    }

    public int GetRowCount()
    {
        return rowCount;
    }

    public int GetColCount()
    {
        return colCount;
    }
}
