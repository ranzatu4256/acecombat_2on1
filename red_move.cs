using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using UnityEngine.UI;

public class red_move : Agent
{
    Rigidbody rBody;
    public GameObject my_Vec;
    public GameObject my_missile1;

    public GameObject en_missile1;
    public GameObject en_missile2;

    public GameObject enemy1;
    public GameObject ally;

    public GameObject stock1;
    public GameObject al_stock;
    public GameObject en_stock1;
    public GameObject en_stock2;

    public GameObject score;

    public float lapseTime1;
    public float mypos_x, mypos_y, myvec_x, myvec_y, alpos_x, alpos_y, en1_x, en1_y;

    Vector3 respawnpos;

    // 初期化時に呼ばれる
    public override void Initialize()
    {
        this.rBody = GetComponent<Rigidbody>();
        lapseTime1 = 10.0f;
    }

    // エピソード開始時に呼ばれる
    public override void OnEpisodeBegin()
    {
    }

    // 観察取得時に呼ばれる
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(Vector3.Distance(this.transform.localPosition,enemy1.transform.localPosition));
        sensor.AddObservation(Vector3.Distance(this.transform.localPosition,ally.transform.localPosition));
        sensor.AddObservation(Vector3.Distance(this.transform.localPosition,en_missile1.transform.localPosition));
        sensor.AddObservation(Vector3.Distance(this.transform.localPosition,en_missile2.transform.localPosition));
        sensor.AddObservation(mypos_x);
        sensor.AddObservation(mypos_y);
        sensor.AddObservation(myvec_x);
        sensor.AddObservation(myvec_y);
        sensor.AddObservation(alpos_x);
        sensor.AddObservation(alpos_y);
        sensor.AddObservation(en1_x);
        sensor.AddObservation(en1_y);
        sensor.AddObservation(lapseTime1);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;
        int action = actions.DiscreteActions[0];

        if (action == 1) rotateDir.y = 1f;
        if (action == 2) rotateDir.y = -1f;
        transform.Rotate(rotateDir, Time.deltaTime * 100f);

        if (stock1.CompareTag("ready"))
        {
            if (action == 3)
            {
                stock1.tag = "empty";
                lapseTime1 = 0.0f;
            }
        }
    }

    void Update()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * 80f, ForceMode.Force);
        rBody.velocity = Vector3.zero;

        if (stock1.CompareTag("ready"))
        {
            stock1.GetComponent<Renderer>().material.color = Color.blue;
        }
        else
        {
            stock1.GetComponent<Renderer>().material.color = Color.red;
            lapseTime1 += Time.deltaTime;
            if (lapseTime1 >= 10)
            {
                stock1.tag = "ready";
                lapseTime1 = 0.0f;
            }
        }

        //リスポーン位置
        if (enemy1.transform.localPosition.x >= 0 && enemy1.transform.localPosition.y >= 0)
        {
            respawnpos = new Vector3(-22f, -22f, 0.0f);
        }
        else if (enemy1.transform.localPosition.x > 0 && enemy1.transform.localPosition.y < 0)
        {
            respawnpos = new Vector3(-22f, 22f, 0.0f);
        }
        else if (enemy1.transform.localPosition.x < 0 && enemy1.transform.localPosition.y > 0)
        {
            respawnpos = new Vector3(22f, -22f, 0.0f);
        }
        else
        {
            respawnpos = new Vector3(22f, 22f, 0.0f);
        }

        //自機の一般化した位置
        if (this.transform.localPosition.x >= 0 && this.transform.localPosition.y >= 0)
        {
            mypos_x = this.transform.localPosition.x;
            mypos_y = this.transform.localPosition.y;
        }
        else if (this.transform.localPosition.x < 0 && this.transform.localPosition.y < 0)
        {
            mypos_x = -this.transform.localPosition.x;
            mypos_y = -this.transform.localPosition.y;
        }
        else if (this.transform.localPosition.x > 0 && this.transform.localPosition.y < 0)
        {
            mypos_x = -this.transform.localPosition.y;
            mypos_y = this.transform.localPosition.x;
        }
        else
        {
            mypos_x = this.transform.localPosition.y;
            mypos_y = -this.transform.localPosition.x;
        }

        //自機の一般化した向き
        if (my_Vec.transform.localPosition.x >= 0 && my_Vec.transform.localPosition.y >= 0)
        {
            myvec_x = my_Vec.transform.localPosition.x;
            myvec_y = my_Vec.transform.localPosition.y;
        }
        else if (my_Vec.transform.localPosition.x < 0 && my_Vec.transform.localPosition.y < 0)
        {
            myvec_x = -my_Vec.transform.localPosition.x;
            myvec_y = -my_Vec.transform.localPosition.y;
        }
        else if (my_Vec.transform.localPosition.x > 0 && my_Vec.transform.localPosition.y < 0)
        {
            myvec_x = -my_Vec.transform.localPosition.y;
            myvec_y = my_Vec.transform.localPosition.x;
        }
        else
        {
            myvec_x = my_Vec.transform.localPosition.y;
            myvec_y = -my_Vec.transform.localPosition.x;
        }

        //ロック中の敵の一般化した位置
        if (enemy1.transform.localPosition.x >= 0 && enemy1.transform.localPosition.y >= 0)
        {
            en1_x = enemy1.transform.localPosition.x;
            en1_y = enemy1.transform.localPosition.y;
        }
        else if (enemy1.transform.localPosition.x < 0 && enemy1.transform.localPosition.y < 0)
        {
            en1_x = -enemy1.transform.localPosition.x;
            en1_y = -enemy1.transform.localPosition.y;
        }
        else if (enemy1.transform.localPosition.x > 0 && enemy1.transform.localPosition.y < 0)
        {
            en1_x = -enemy1.transform.localPosition.y;
            en1_y = enemy1.transform.localPosition.x;
        }
        else
        {
            en1_x = enemy1.transform.localPosition.y;
            en1_y = -enemy1.transform.localPosition.x;
        }

        //味方の一般化した位置
        if (ally.transform.localPosition.x >= 0 && ally.transform.localPosition.y >= 0)
        {
            alpos_x = ally.transform.localPosition.x;
            alpos_y = ally.transform.localPosition.y;
        }
        else if (ally.transform.localPosition.x < 0 && ally.transform.localPosition.y < 0)
        {
            alpos_x = -ally.transform.localPosition.x;
            alpos_y = -ally.transform.localPosition.y;
        }
        else if (ally.transform.localPosition.x > 0 && ally.transform.localPosition.y < 0)
        {
            alpos_x = -ally.transform.localPosition.y;
            alpos_y = ally.transform.localPosition.x;
        }
        else
        {
            alpos_x = ally.transform.localPosition.y;
            alpos_y = -ally.transform.localPosition.x;
        }
    }

    //OnTriggerEnter関数
    //接触したオブジェクトが引数otherとして渡される
    void OnTriggerEnter(Collider other)
    {
        //接触したオブジェクトのタグ
        if (other.CompareTag("map_end"))
        {
            this.transform.localPosition = respawnpos;

            stock1.tag = "ready";
            al_stock.tag = "ready";
            en_stock1.tag = "ready";
            en_stock2.tag = "ready";

            score.tag = "crush_blue";

            this.AddReward(-0.2f);
            EndEpisode();
        }

        if (other.CompareTag("blue_attack"))
        {
            this.transform.localPosition = respawnpos;

            stock1.tag = "ready";
            al_stock.tag = "ready";
            en_stock1.tag = "ready";
            en_stock2.tag = "ready";

            score.tag = "crush_blue";

            this.AddReward(-1f);
            EndEpisode();
        }
    }

    // ヒューリスティックモードの行動決定時に呼ばれる
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.DiscreteActions;
        actions[0] = 0;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            actions[0] = 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            actions[0] = 2;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            actions[0] = 3;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            actions[0] = 4;
        }
    }
}

//mlagents-learn config/acecombat.yaml --run-id=acecombat --env=apps/acecombat --force