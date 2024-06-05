using Base;
using UnityEngine;

public class SecretaryProblemSettings : Singleton<SecretaryProblemSettings>
{
    public Material bestSecretaryMat;
    public Material defaultSecretaryMat;

    public Material correctSecretaryMat;
    public Material wrongSecretaryMat;

    private void Awake()
    {
        bestSecretaryMat = Resources.Load<Material>("Materials/SecretaryBest");
        defaultSecretaryMat = Resources.Load<Material>("Materials/SecretaryDefault");
        
        correctSecretaryMat = Resources.Load<Material>("Materials/SecretaryCorrect");
        wrongSecretaryMat = Resources.Load<Material>("Materials/SecretaryWrong");
    }
}