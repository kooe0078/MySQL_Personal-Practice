using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isGameover = false;
    public Text scoreText;
    public GameObject gameoverUI;

    public bool isLogIn = false;
    string memberID;
    public Text idText;
    public InputField passText;

    public GameObject loginUI;
    public GameObject selectUI;
    public GameObject loginLogUI;
    public GameObject cdLogUI;
    public GameObject rankUI;

    public GameObject M4A1;
    public GameObject WA2000;
    public GameObject G41;
    public GameObject CreateM4A1;
    public GameObject CreateWA2000;
    public GameObject CreateG41;
    public GameObject DeleteM4A1;
    public GameObject DeleteWA2000;
    public GameObject DeleteG41;

    public Text loginLogText;
    public Text cdLogText;
    public Text rankText;

    List<string> gunList = new List<string>();

    private int score = 0;

    public string thisGunName;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("씬에 두 개 이상의 게임 매니저가 존재합니다!");
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    // 인스턴스가 없는 경우에 접근하려하면 인스턴스를 할당
    public static GameManager Instance
    {
        get
        {
            if(!instance)
            {
                instance = FindObjectOfType(typeof(GameManager)) as GameManager;

                if (instance == null)
                    Debug.Log("no singleton obj");
            }
            return instance;
        }
    }

    public void LogIn() // 로그인 처리 함수
    {
        string id = idText.text;
        string pass = passText.text;

        // id는 memberid
        // pass는 이름을 pass로 설정
        string ipAddress = "localhost";
        string db_id = "root";
        string db_pw = "unitytest1234";
        string db_name = "gamedb";

        string strConn = string.Format("server={0};uid={1};pwd={2};database={3};charset=utf8 ;", ipAddress, db_id, db_pw, db_name);

        using (MySqlConnection conn = new MySqlConnection(strConn)) // 로그인 처리
        {
            conn.Open();

            MySqlCommand command = new MySqlCommand("select * from membertbl where memberid=@memberid and membername=@membername;", conn);
            command.Parameters.AddWithValue("@memberid", id);
            command.Parameters.AddWithValue("@membername", pass);
            MySqlDataReader rdr = command.ExecuteReader();
            string temp = string.Empty;
            if (rdr == null)
            {
                temp = "No return";
                Debug.Log("로그인에 실패했습니다!");
            }
            else
            {
                int idCounter = 0;

                while (rdr.Read())
                {
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        Debug.Log(rdr[i]);
                    }
                    idCounter++;
                }

                if (idCounter <= 0)
                {
                    Debug.Log("로그인에 실패했습니다!");
                }
                else
                {
                    Debug.Log("로그인이 완료 되었습니다!");
                    isLogIn = true;
                }
            }
        }

        using (MySqlConnection conn = new MySqlConnection(strConn)) // 접속 로그 처리
        {
            conn.Open();

            if (isLogIn == true) // 로그인 성공 시
            {
                MySqlCommand command = new MySqlCommand("insert into loginlog values (@memberid, now(), 'LoginSuccess!!');", conn);
                command.Parameters.AddWithValue("@memberid", id);
                MySqlDataReader rdr = command.ExecuteReader();

                while (rdr.Read())
                {
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        Debug.Log(rdr[i].ToString());
                        gunList.Add(rdr[i].ToString());
                    }
                }
                                
                memberID = id;
                SelectGuns();
                loginUI.SetActive(false);
                selectUI.SetActive(true);
            }
            else                 // 로그인 실패 시
            {
                MySqlCommand command = new MySqlCommand("insert into loginlog values (@memberid, now(), 'LoginFail..');", conn);
                command.Parameters.AddWithValue("@memberid", id);
                MySqlDataReader rdr = command.ExecuteReader();

                while (rdr.Read())
                {
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        Debug.Log(rdr[i].ToString());
                        gunList.Add(rdr[i].ToString());
                    }
                }

                return;
            }
        }
        
    }

    public void SelectGuns() // 캐릭터 선택 함수
    {
        if (!isLogIn)
            return;

        string id = idText.text;
        string pass = passText.text;

        // id는 memberid
        // pass는 이름을 pass로 설정
        string ipAddress = "localhost";
        string db_id = "root";
        string db_pw = "unitytest1234";
        string db_name = "gamedb";

        string strConn = string.Format("server={0};uid={1};pwd={2};database={3};charset=utf8 ;", ipAddress, db_id, db_pw, db_name);

        MySqlConnection conn = new MySqlConnection(strConn);
        conn.Open();

        MySqlCommand command = new MySqlCommand(
            "select gunName from membertbl inner join producttbl on membertbl.memberid = producttbl.memberid where membertbl.memberid =@memberid; ", conn);
        command.Parameters.AddWithValue("@memberid", id);
        MySqlDataReader rdr = command.ExecuteReader();

        while (rdr.Read())
        {
            for (int i = 0; i < rdr.FieldCount; i++)
            {
                Debug.Log(rdr[i].ToString());
                gunList.Add(rdr[i].ToString());
            }
        }

        CharacterActiveTrue();

        conn.Close();
    }    

    public void CreateGuns(string gunID) // 캐릭터 생성 함수
    {
        if (!isLogIn)
            return;

        string id = idText.text;
        string pass = passText.text;

        // id는 memberid
        // pass는 이름을 pass로 설정
        string ipAddress = "localhost";
        string db_id = "root";
        string db_pw = "unitytest1234";
        string db_name = "gamedb";

        string strConn = string.Format("server={0};uid={1};pwd={2};database={3};charset=utf8 ;", ipAddress, db_id, db_pw, db_name);

        using (MySqlConnection conn = new MySqlConnection(strConn)) // 캐릭터 생성
        {
            conn.Open();

            MySqlCommand command = new MySqlCommand("Call selectGun(@gunName, @memberid);", conn);
            command.Parameters.AddWithValue("@memberid", id);
            command.Parameters.AddWithValue("@gunName", gunID);
            MySqlDataReader rdr = command.ExecuteReader();

            while (rdr.Read())
            {
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    gunList.Add(rdr[i].ToString());
                }
            }
        }

        using (MySqlConnection conn = new MySqlConnection(strConn)) // 생성 로그 작성
        {
            conn.Open();

            MySqlCommand command = new MySqlCommand("insert into createlog values (now(), @gunName, @memberid);", conn);
            command.Parameters.AddWithValue("@memberid", id);
            command.Parameters.AddWithValue("@gunName", gunID);
            MySqlDataReader rdr = command.ExecuteReader();

            while (rdr.Read())
            {
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    gunList.Add(rdr[i].ToString());
                }
            }
        }

        SelectGuns();
    }

    public void DeleteGuns(string gunID) // 캐릭터 삭제 함수
    {
        if (!isLogIn)
            return;

        string id = idText.text;
        string pass = passText.text;

        // id는 memberid
        // pass는 이름을 pass로 설정
        string ipAddress = "localhost";
        string db_id = "root";
        string db_pw = "unitytest1234";
        string db_name = "gamedb";

        string strConn = string.Format("server={0};uid={1};pwd={2};database={3};charset=utf8 ;", ipAddress, db_id, db_pw, db_name);

        using (MySqlConnection conn = new MySqlConnection(strConn)) // 캐릭터 삭제
        {
            conn.Open();

            MySqlCommand command = new MySqlCommand("delete from producttbl where gunName = @gunName and memberID = @memberid;", conn);
            command.Parameters.AddWithValue("@memberid", id);
            command.Parameters.AddWithValue("@gunName", gunID);
            MySqlDataReader rdr = command.ExecuteReader();

            while (rdr.Read())
            {
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    gunList.Add(rdr[i].ToString());
                }
            }
        }

        using (MySqlConnection conn = new MySqlConnection(strConn)) // 삭제 로그 작성
        {
            conn.Open();

            MySqlCommand command = new MySqlCommand("insert into deletelog values (now(), @gunName, @memberid);", conn);
            command.Parameters.AddWithValue("@memberid", id);
            command.Parameters.AddWithValue("@gunName", gunID);
            MySqlDataReader rdr = command.ExecuteReader();

            while (rdr.Read())
            {
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    gunList.Add(rdr[i].ToString());
                }
            }
        }

        gunList.Remove(gunID);
        CharacterActiveFalse();
        SelectGuns();
    }

    public void loginLogCheck() // 접속 로그 확인 함수
    {
        if (!isLogIn)
            return;

        string id = idText.text;
        string pass = passText.text;

        // id는 memberid
        // pass는 이름을 pass로 설정
        string ipAddress = "localhost";
        string db_id = "root";
        string db_pw = "unitytest1234";
        string db_name = "gamedb";

        string strConn = string.Format("server={0};uid={1};pwd={2};database={3};charset=utf8 ;", ipAddress, db_id, db_pw, db_name);

        using (MySqlConnection conn = new MySqlConnection(strConn)) // 사용자 정보와 접속 기록 출력
        {
            conn.Open();

            MySqlCommand command = new MySqlCommand(
                "select memberID, memberName, memberAddress, loginDate, loginType from membertbl " +
                "inner join loginlog on membertbl.memberID = loginlog.loginMemberID where membertbl.memberID = @memberid;", conn);
            command.Parameters.AddWithValue("@memberid", id);
            MySqlDataReader rdr = command.ExecuteReader();

            string temp = string.Empty;
            if (rdr == null) temp = "No return";
            else
            {
                while (rdr.Read())
                {
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        if (i != rdr.FieldCount - 1)
                            temp += rdr[i] + "\t\t";
                        else if (i == rdr.FieldCount - 1)
                            temp += rdr[i] + "\n";
                    }
                }
            }
            loginLogText.text = temp;
        }
    }

    public void characterLogCheck(string type) // 캐릭터 생성, 삭제 로그 확인 함수
    {
        if (!isLogIn)
            return;

        string id = idText.text;
        string pass = passText.text;

        // id는 memberid
        // pass는 이름을 pass로 설정
        string ipAddress = "localhost";
        string db_id = "root";
        string db_pw = "unitytest1234";
        string db_name = "gamedb";

        string strConn = string.Format("server={0};uid={1};pwd={2};database={3};charset=utf8 ;", ipAddress, db_id, db_pw, db_name);

        using (MySqlConnection conn = new MySqlConnection(strConn)) // 캐릭터 생성, 삭제 기록 출력
        {
            conn.Open();

            MySqlCommand command = new MySqlCommand("Call logManager(@type, @memberid);", conn);
            command.Parameters.AddWithValue("@type", type);
            command.Parameters.AddWithValue("@memberid", id);
            MySqlDataReader rdr = command.ExecuteReader();

            string temp = string.Empty;
            if (rdr == null) temp = "No return";
            else
            {
                while (rdr.Read())
                {
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        if (i != rdr.FieldCount - 1)
                            temp += rdr[i] + "\t\t";
                        else if (i == rdr.FieldCount - 1)
                            temp += rdr[i] + "\n";
                    }
                }
            }
            cdLogText.text = temp;
        }
    }

    public void RankCheck() // 랭크 정렬 함수
    {
        if (!isLogIn)
            return;

        string id = idText.text;
        string pass = passText.text;

        // id는 memberid
        // pass는 이름을 pass로 설정
        string ipAddress = "localhost";
        string db_id = "root";
        string db_pw = "unitytest1234";
        string db_name = "gamedb";

        string strConn = string.Format("server={0};uid={1};pwd={2};database={3};charset=utf8 ;", ipAddress, db_id, db_pw, db_name);

        using (MySqlConnection conn = new MySqlConnection(strConn)) // 랭크 정렬
        {
            conn.Open();

            MySqlCommand command = new MySqlCommand("select *, rank() over (order by score desc) As RankNum from ranktbl;", conn);
            MySqlDataReader rdr = command.ExecuteReader();

            string temp = string.Empty;
            if (rdr == null) temp = "No return";
            else
            {
                while (rdr.Read())
                {
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        if (i != rdr.FieldCount - 1)
                            temp += rdr[i] + "\t\t";
                        else if (i == rdr.FieldCount - 1)
                            temp += rdr[i] + "\n";
                    }
                }
            }
            rankText.text = temp;
        }
    }

    public void CharacterActiveTrue() // 캐릭터 활성화
    {
        for (int i = 0; i < gunList.Count; i++)
        {
            if (gunList[i] == "M4A1")
            {
                M4A1.SetActive(true);
                CreateM4A1.SetActive(false);
            }
            else if (gunList[i] == "WA2000")
            {
                WA2000.SetActive(true);
                CreateWA2000.SetActive(false);
            }
            else if (gunList[i] == "G41")
            {
                G41.SetActive(true);
                CreateG41.SetActive(false);
            }
        }
    }

    public void CharacterActiveFalse() // 캐릭터 비활성화
    {
        M4A1.SetActive(false);
        CreateM4A1.SetActive(true);
        WA2000.SetActive(false);
        CreateWA2000.SetActive(true);
        G41.SetActive(false);
        CreateG41.SetActive(true);
    }

    public void CreateButton(string gunID) // 생성 버튼 클릭 이벤트
    {
        Debug.Log("캐릭터를 생성합니다.");
        CreateGuns(gunID);
    }

    public void DeleteButton(string gunID) // 삭제 버튼 클릭 이벤트
    {
        Debug.Log("캐릭터를 삭제합니다.");
        DeleteGuns(gunID);
    }

    public void LogCheckButton() // 접속로그 확인 버튼 클릭 이벤트
    {
        Debug.Log("사용자 접속 기록을 확인합니다.");
        loginLogCheck();
        selectUI.SetActive(false);
        loginLogUI.SetActive(true);
    }

    public void ReturnButton() // 접속로그창 돌아가기 버튼 클릭 이벤트
    {
        Debug.Log("캐릭터 선택창으로 돌아갑니다.");
        loginLogUI.SetActive(false);
        selectUI.SetActive(true);
    }

    public void cdLogCheckButton(string type) // 캐릭터 생성, 삭제 로그 확인 버튼 클릭 이벤트
    {
        Debug.Log("캐릭터 생성, 삭제 기록을 확인합니다.");
        characterLogCheck(type);
        selectUI.SetActive(false);
        cdLogUI.SetActive(true);
    }

    public void cdReturnButton() // 캐릭터 생성, 삭제 로그창 돌아가기 버튼 클릭 이벤트
    {
        Debug.Log("캐릭터 선택창으로 돌아갑니다.");
        cdLogUI.SetActive(false);
        selectUI.SetActive(true);
    }

    public void rankButton() // 접속로그 확인 버튼 클릭 이벤트
    {
        Debug.Log("랭킹을 확인합니다.");
        RankCheck();
        selectUI.SetActive(false);
        rankUI.SetActive(true);
    }

    public void rankReturnButton() // 접속로그창 돌아가기 버튼 클릭 이벤트
    {
        Debug.Log("캐릭터 선택창으로 돌아갑니다.");
        rankUI.SetActive(false);
        selectUI.SetActive(true);
    }

    public void StartGun(string id) // 씬 넘기기 이벤트
    {
        thisGunName = id;
        SceneManager.LoadScene("SampleScene");
    }
}

