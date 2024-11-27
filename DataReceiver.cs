using UnityEngine;

public class DataReceiver : MonoBehaviour
{
    public static DataReceiver instance = null;

    // 공개 프로퍼티
    public string College { get; private set; }
    public string Major { get; private set; }
    public string Name { get; private set; }
    
    void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    // 리액트에서 값을 받을 메서드
    public void ReceiveData(string jsonData)
    {
        UserInfo userInfo = JsonUtility.FromJson<UserInfo>(jsonData);
        // 프로퍼티에 값 할당
        College = userInfo.userCollege;
        Major = userInfo.userMajor;
        Name = userInfo.userName;
        Debug.Log("Received Username: " + College);
        Debug.Log("Received Email: " + Major);
        Debug.Log("Received Score: " + Name);

    }

    void Start() {
        Debug.Log("Gamme started. you can now send data from JS");
    }
}

[System.Serializable]
public class UserInfo
{
    public string userCollege;
    public string userMajor;
    public string userName;
}
