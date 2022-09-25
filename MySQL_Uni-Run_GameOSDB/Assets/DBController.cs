using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

public class DBController : MonoBehaviour
{
    public Text scoreText;


    // Start is called before the first frame update
    void Start()
    {

    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetData();
        }
    }

    public void GetData()
    {
        Debug.Log("DB Connection..!");
        // 데이터 베이스 연결 하기
        string ipAddress = "localhost";
        string db_id = "root";
        string db_pw = "unitytest1234";
        string db_name = "gamedb";

        string strConn = string.Format("server={0};uid={1};pwd={2};database={3};charset=utf8 ;",
                               ipAddress, db_id, db_pw, db_name);

        MySqlConnection conn = new MySqlConnection(strConn);
        conn.Open();

        MySqlCommand command = new MySqlCommand("select *  from producttbl where gunName=@NAME;", conn);
        string gunName = "M4A1";
        command.Parameters.AddWithValue("@NAME", gunName);

        MySqlDataReader rdr = command.ExecuteReader();
        string temp = string.Empty;
        if (rdr == null) temp = "No return";
        else
        {
            Debug.Log("select..!");
            while (rdr.Read())
            {
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    if (i != rdr.FieldCount - 1)
                        temp += rdr[i] + ";";    // parser 넣어주기
                    else if (i == rdr.FieldCount - 1)
                        temp += rdr[i] + "\n";
                }
            }
        }

        scoreText.text = temp;

        conn.Close();
    }
}