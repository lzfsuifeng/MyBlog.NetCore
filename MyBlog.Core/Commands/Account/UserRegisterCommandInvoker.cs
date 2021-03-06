﻿using System;
using MyExtensionsLib;
using MyBlog.Models;

namespace MyBlog.Core.Commands.Account
{
    /// <summary>
    /// 用户注册命令执行
    /// </summary>
    public class UserRegisterCommandInvoker : ICommandInvoker<UserRegisterCommand, UserLoginCommandResult>
    {
        private readonly IDbSession _db;

        public UserRegisterCommandInvoker(IDbSession db)
        {
            this._db = db;
        }

        /// <summary>
        /// 执行注册命令
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public UserLoginCommandResult Execute(UserRegisterCommand command)
        {
            try
            {
                if (command.Password != command.ConfirmPassword)
                    return new UserLoginCommandResult("两次输入密码不一致");

                // 开始事务
                this._db.GetSession().BeginTran();
                user_tb user = new user_tb();
                user.user_account = command.UserAccount;
                user.user_pwd = command.Password.GetMd5Hash();

                this._db.GetSession().Insert(user);
                // 提交事务
                this._db.GetSession().CommitTran();
                return new UserLoginCommandResult() { UserInfo = user };
            }
            catch (Exception e)
            {
                // 回滚事务
                this._db.GetSession().RollbackTran();
                return new UserLoginCommandResult(e.ToString());
            }
        }
    }
}
