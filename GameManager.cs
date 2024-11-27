using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;

    public TextMeshProUGUI scoreText; // ���� ǥ�ÿ� TextMeshPro ����
    public TextMeshProUGUI livesText; // ��� ǥ�ÿ� TextMeshPro ����

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
        SetLives(3); // ��� �ʱ�ȭ
        NewRound();
        Debug.Log("newStart");
        gameOverUI.SetActive(false);
    }

    private void NewRound()
    {
        foreach (Transform pellet in this.pellets)
        {
            pellet.gameObject.SetActive(true);  // ���� pellet�� �ٽ� Ȱ��ȭ
        }
        ResetState();
        IncreaseGhostSpeed();  // ���ο� ���忡�� Ghost�� �ӵ� ����
    }

    private void IncreaseGhostSpeed()
    {
        float speedIncreaseAmount = 0.5f; // ���ϴ� �ӵ� ������
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
        gameOverUI.SetActive(false); // ������ ���� ���� �� UI ��Ȱ��ȭ
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
        
        // ���� ���� UI Ȱ��ȭ
        gameOverUI.SetActive(true);
    }

    public void RestartGame()
    {
        Debug.Log("start");
        NewGame();  // ������ �ٽ� ����
    }

    public void QuitGame()
    {
        Debug.Log("��");

        Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString();  // ���ڸ� ǥ��
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        livesText.text = "Lives: " + lives.ToString();  // ��� �ؽ�Ʈ ������Ʈ
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

        SetLives(this.lives - 1); // ��� ����

        // ��� ������ �����Ǹ� ���� ����
        if (this.lives > 0)
        {
            Invoke(nameof(ResetState), 3.0f);  // ����� ���� ������ ������ �ٽ� ����
        }
        else
        {
            GameOver();  // ������ ��� �����Ǿ��� ���� GameOver() ȣ��
        }
    }

    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);  // Pellet ��Ȱ��ȭ
        SetScore(this.score + pellet.points);  // ���� ������Ʈ

        if (!HasRemainingPellets())  // ��� Pellet�� ������
        {
            this.pacman.gameObject.SetActive(false);  // Pacman ��Ȱ��ȭ

            if (this.lives > 0)  // ���� ����� ������
            {
                Invoke(nameof(NewRound), 3.0f);  // 3�� �� ���ο� ���� ����
            }
            else  // ����� ������ ���� ����
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
                return true;  // �����ִ� Pellet�� ������ true ��ȯ
            }
        }

        return false;  // ��� �������� false ��ȯ
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

    // ������ ����
    dataSender.SendDataToServer(serverUrl, userInfo);
    }
}
