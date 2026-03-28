using UnityEngine;

public class BrainCloudInterface
{
    public void AuthenticateAnonymous()
    {
        BCConfig.GetBrainCloud().AuthenticateAnonymous(SuccessCallback, FailureCallback);
    }

    private void SuccessCallback(string jsonResponse, object cbObject)
    {

    }

    private void FailureCallback(int status, int reasonCode, string jsonError, object cbObject)
    {

    }
}
