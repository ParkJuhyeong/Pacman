using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;

    public TextMeshProUGUI scoreText; // 점수 표시용 TextMeshPro 변수
    public TextMeshProUGUI livesText; // 목숨 표시용 TextMeshPro 변수

    public int ghostMultiplier { get; private set; } = 1;
    public int score { get; private set; } = 0;
    public int lives { get; private set; }

    public GameObject gameOverUI;

    private void Start()
    {
        gameOverUI.SetActive(false);
        NewGame();
    }

    /*private void Update()
    {
        if (this.lives <= 0)
        {
            NewGame();
        }
    }*/

    private void NewGame()
    {
        SetScore(0);
        SetLives(3); // 목숨 초기화
        NewRound();
        Debug.Log("newStart");
        gameOverUI.SetActive(false);
    }

    private void NewRound()
    {
        foreach (Transform pellet in this.pellets)
        {
            pellet.gameObject.SetActive(true);  // 개별 pellet을 다시 활성화
        }
        ResetState();
        IncreaseGhostSpeed();  // 새로운 라운드에서 Ghost의 속도 증가
    }

    private void IncreaseGhostSpeed()
    {
        float speedIncreaseAmount = 0.5f; // 원하는 속도 증가량
        foreach (Ghost ghost in ghosts)
        {
            ghost.IncreaseSpeed(speedIncreaseAmount);
        }
    }

    private void ResetState()
    {
        ResetGhostMultiplier();

        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].ResetState();
        }
        this.pacman.ResetState();
        gameOverUI.SetActive(false); // 게임이 진행 중일 때 UI 비활성화
    }


    private void GameOver()
    {
        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].gameObject.SetActive(false);
        }
        this.pacman.gameObject.SetActive(false);

        Debug.Log("over");

        Sender();
        
        // 게임 오버 UI 활성화
        gameOverUI.SetActive(true);
    }

    public void RestartGame()
    {
        Debug.Log("start");
        NewGame();  // 게임을 다시 시작
    }

    public void QuitGame()
    {
        Debug.Log("끝");

        Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString();  // 숫자만 표시
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        livesText.text = "Lives: " + lives.ToString();  // 목숨 텍스트 업데이트
    }

    public void GhostEaten(Ghost ghost)
    {
        int points = ghost.points * this.ghostMultiplier;
        SetScore(this.score + points);
        this.ghostMultiplier++;
    }

    public void pacmanEaten()
    {
        this.pacman.gameObject.SetActive(false);

        SetLives(this.lives - 1); // 목숨 감소

        // 모든 생명이 소진되면 게임 종료
        if (this.lives > 0)
        {
            Invoke(nameof(ResetState), 3.0f);  // 목숨이 남아 있으면 게임을 다시 시작
        }
        else
        {
            GameOver();  // 생명이 모두 소진되었을 때만 GameOver() 호출
        }
    }

    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);  // Pellet 비활성화
        SetScore(this.score + pellet.points);  // 점수 업데이트

        if (!HasRemainingPellets())  // 모든 Pellet을 먹으면
        {
            this.pacman.gameObject.SetActive(false);  // Pacman 비활성화

            if (this.lives > 0)  // 남은 목숨이 있으면
            {
                Invoke(nameof(NewRound), 3.0f);  // 3초 후 새로운 라운드 시작
            }
            else  // 목숨이 없으면 게임 종료
            {
                GameOver();
            }
        }
    }



    public void PowerPelletEaten(PowerPellet pellet)
    {
        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].frightened.Enable(pellet.duration);
        }

        PelletEaten(pellet);
        CancelInvoke();
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);
    }

    private bool HasRemainingPellets()
    {
        foreach (Transform pellet in this.pellets)
        {
            if (pellet.gameObject.activeSelf)
            {
                return true;  // 남아있는 Pellet이 있으면 true 반환
            }
        }

        return false;  // 모두 먹혔으면 false 반환
    }


    private void ResetGhostMultiplier()
    {
        this.ghostMultiplier = 1;
    }

    private string serverUrl = "https://hyunverse.kro.kr:3001/sendResult";
    public void Sender()
    {
        Debug.Log("Sender()");
        DataSender dataSender = gameObject.AddComponent<DataSender>();

        DataSender.UserInfo userInfo = new DataSender.UserInfo
        {
            gameCode = 4,
            score = score
    };

    // 데이터 전송
    dataSender.SendDataToServer(serverUrl, userInfo);
    }
}
