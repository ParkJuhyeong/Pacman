using System.Net; // 추가해야 할 네임스페이스
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class DataSender : MonoBehaviour
{
    [System.Serializable]
    public class UserInfo
    {
        public int gameCode;
        public int score;
    }

    public void SendDataToServer(string url, UserInfo userInfo)
    {
        StartCoroutine(PostData(url, userInfo));
    }

    private IEnumerator PostData(string url, UserInfo userInfo)
    {
        // 인증서 무시 (개발용)
        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

        // 데이터를 JSON 형식으로 변환
        string jsonData = JsonUtility.ToJson(userInfo);
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);

        // UnityWebRequest로 POST 요청 생성
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json"); // JSON 요청 헤더 설정

            // 요청 전송
            yield return request.SendWebRequest();

            // 에러 체크
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                Debug.Log("Data sent successfully: " + request.downloadHandler.text);
            }
        }
    }
}
