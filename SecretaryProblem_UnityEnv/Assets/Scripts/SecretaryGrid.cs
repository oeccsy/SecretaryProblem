using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretaryGrid : MonoBehaviour
{
    private Secretary[,] _secretaryGrid;
    private List<Secretary> _secretaryList;
    
    [SerializeField]
    private int rowCount;
    [SerializeField]
    private int colCount;

    public void InitSecretaryGrid()
    {
        _secretaryGrid = new Secretary[rowCount,colCount];
        _secretaryList = new List<Secretary>();

        var secretaryPrefab = Resources.Load<GameObject>("Secretary");
        
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                Secretary newSecretary = Instantiate(secretaryPrefab, new Vector3(2*col, -2*row, -1), Quaternion.identity, transform).GetComponent<Secretary>();
                newSecretary.row = row;
                newSecretary.col = col;
                
                _secretaryGrid[row, col] = newSecretary;
                _secretaryList.Add(newSecretary);
            }
        }
    }

    public void InitSecretaryRanking() // 각각의 Secretary에게 1등부터 n등까지를 부여한다.
    {
        List<int> rankList = new List<int>();
        
        for (int i = 1; i <= rowCount * colCount; i++)
        {
            rankList.Add(i);
        }
        
        for (int i = rankList.Count - 1; i >= 0; i--)
        {
            int randomIndex = Random.Range(0, i);
            (rankList[i], rankList[randomIndex]) = (rankList[randomIndex], rankList[i]);
        }

        for (int i = 0; i < _secretaryList.Count; i++)
        {
            _secretaryList[i].ranking = rankList[i];
        }
    }

    public void InitSecretaryRankingOnInterview() // 면접관과 인터뷰할 때 갖게 될 ranking을 계산하여 미리 부여한다.
    {
        LinkedList<Secretary> tempRankings = new LinkedList<Secretary>();
        
        foreach (var secretary in _secretaryList)
        {
            LinkedListNode<Secretary> cmpSecretaryNode = tempRankings.First;

            for (int i = 1; i <= tempRankings.Count; i++)
            {
                if (secretary.ranking < cmpSecretaryNode.Value.ranking)
                {
                    tempRankings.AddBefore(cmpSecretaryNode, secretary);
                    secretary.rankingAfterInterview = i;
                    break;
                }

                cmpSecretaryNode = cmpSecretaryNode.Next;
            }
            
            if (cmpSecretaryNode == null)
            {
                tempRankings.AddLast(secretary);
                secretary.rankingAfterInterview = tempRankings.Count;
            }
        }
    }
    
    public void InitSecretaryMat()
    {
        foreach (var secretary in _secretaryList)
        {
            if (secretary.ranking == 1)
            {
                secretary.SetMaterial(SecretaryProblemSettings.Instance.bestSecretaryMat);
            }
            else
            {
                secretary.SetMaterial(SecretaryProblemSettings.Instance.defaultSecretaryMat);
            }
        }
    }

    public Secretary GetSecretary(int row, int col)
    {
        return _secretaryGrid[row, col];
    }

    public Secretary GetSecretary(int index)
    {
        return _secretaryList[index];
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
