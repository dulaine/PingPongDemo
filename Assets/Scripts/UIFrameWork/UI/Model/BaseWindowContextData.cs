using UnityEngine;
using System.Collections;

//data context for Refresh window or show window
//e.g. equipment refining UI, need equipment details to show the UI
public class WindowContextDataBase
{
    public bool executeNavLogic = true; // 打开UI的时候, 是否一定执行导航相关操作
}

// Example
//context data pass to Refine UI to show the equipment detail
//public class WindowContextData_EquipmentRefine : BaseWindowContextData
//{
//         int equipmentID ;        
//}

// Skill window context data pass to UISkillWindow
//public class WindowContextData_Skill : BaseWindowContextData
//{
//         int skillID;
//}