using SA.Foundation.Templates;
using SA.iOS.GameKit;
using UnityEngine;
using UnityEngine.UI;

public class ISN_GameKitUseExample_TvOS : MonoBehaviour
{
    [SerializeField] private Button m_SingInButton = null;
    [SerializeField] private Button m_LeaderboardsUI = null;
    [SerializeField] private Button m_AchievementsUI = null;
    
    private void Start()
    {
        m_SingInButton.onClick.AddListener(() =>
        {
            Debug.Log("Click");
            ISN_GKLocalPlayer.Authenticate((SA_Result result) => {
                if (result.IsSucceeded) {
                    Debug.Log("Authenticate is succeeded!");

                    var player = ISN_GKLocalPlayer.LocalPlayer;
                    Debug.Log(player.PlayerID);
                    Debug.Log(player.Alias);
                    Debug.Log(player.DisplayName);
                    Debug.Log(player.Authenticated);
                    Debug.Log(player.Underage);


                    player.GenerateIdentityVerificationSignatureWithCompletionHandler((signatureResult) => {
                        if(signatureResult.IsSucceeded) {
                            Debug.Log("signatureResult.PublicKeyUrl: " + signatureResult.PublicKeyUrl);
                            Debug.Log("signatureResult.Timestamp: " + signatureResult.Timestamp);
                            Debug.Log("signatureResult.Salt.Length: " + signatureResult.Salt.Length);
                            Debug.Log("signatureResult.Signature.Length: " + signatureResult.Signature.Length);
                        } else {
                            Debug.Log("IdentityVerificationSignature has failed: " + signatureResult.Error.FullMessage);
                        }
                    });

                }
                else {
                    Debug.Log("Authenticate is failed! Error with code: " + result.Error.Code + " and description: " + result.Error.Message);
                }
            });
        });   
        
        
        m_LeaderboardsUI.onClick.AddListener(() =>
        {
            Debug.Log("m_LeaderboardsUI");
            ISN_GKGameCenterViewController viewController = new ISN_GKGameCenterViewController();
            viewController.ViewState = ISN_GKGameCenterViewControllerState.Leaderboards;
            viewController.Show();
            
        });
        
        m_AchievementsUI.onClick.AddListener(() =>
        {
            Debug.Log("m_LeaderboardsUI");
            ISN_GKGameCenterViewController viewController = new ISN_GKGameCenterViewController();
            viewController.ViewState = ISN_GKGameCenterViewControllerState.Achievements;
            viewController.Show();
        });
        
    }
}
