using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Secretary : MonoBehaviour
{
    public int ranking;                 // 실제 랭킹 [1,n]
    public int rankingAfterInterview;   // 면접관과 인터뷰 할 때 갖게 될 랭킹 [1,n]

    public int row;
    public int col;
}
