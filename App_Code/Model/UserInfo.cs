using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 系统用户基本信息
/// </summary>
public class UserInfo
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName;

    /// <summary>
    /// 密码
    /// </summary>
    public string Password;

    public UserInfo()
    {
            
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="userName">用户名</param>
    /// <param name="password">密码</param>
    /// <returns>
    /// 正常输入用户名密码返回true，否则返回false
    /// </returns>
    public static bool CheckUserIsValid(string userName,string password)
    {
        bool isCorrect = false;

        if(userName== "test" && password == "test")
        {
            isCorrect = true;
        }

        return isCorrect;
    }
}