using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectParameters :Singleton <ProjectParameters>
{
    //10点
    public int dailyreward_hour = 10;
    //30分钟 回复
    public int refilling_time = 30;
    //最多回复5滴
    public int lifes_limit = 5;
    public float music_volume_max = 0.4f;
    public float slot_offset = 0.7f;

}
