using UnityEngine;

public class Secretary : MonoBehaviour
{
    public int ranking;                 // 실제 랭킹 [1,n]
    public int rankingAfterInterview;   // 면접관과 인터뷰 할 때 갖게 될 랭킹 [1,n]

    public int row;
    public int col;

    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetMaterial(Material material)
    {
        _meshRenderer.material = material;
    }
}
