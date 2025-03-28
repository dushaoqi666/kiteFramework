﻿using Domain.Aggregate;
using System.Linq.Expressions;
using Domain.System;

namespace Repository.Repositories;

public class DbContext : SugarUnitOfWork
{
    /// <summary>
    /// 用户表
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// 菜单表
    /// </summary>
    public DbSet<Menu> Menus { get; set; }

    /// <summary>
    /// 岗位表
    /// </summary>
    public DbSet<Post> Posts { get; set; }
    
    /// <summary>
    /// 角色表
    /// </summary>
    public DbSet<Role> Roles { get; set; }
    
    /// <summary>
    /// 角色菜单关联表
    /// </summary>
    public DbSet<RoleMenu> RoleMenus { get; set; }
    
    /// <summary>
    /// 用户角色关联表
    /// </summary>
    public DbSet<UserRole> UserRoles { get; set; }
    
    /// <summary>
    /// 用户岗位关联表
    /// </summary>
    public DbSet<UserPost> UserPosts { get; set; }
}

public class DbSet<T> : SimpleClient<T> where T : class, IDeleted, new()
{
    /// <summary>
    /// 逻辑删除
    /// </summary>
    public override bool Delete(T deleteObj)
    {
        deleteObj.IsDeleted = true;
        return Context.Updateable(deleteObj).ExecuteCommand() > 0;
    }

    /// <summary>
    /// 逻辑删除
    /// </summary>
    public override async Task<bool> DeleteAsync(T deleteObj)
    {
        deleteObj.IsDeleted = true;
        return await Context.Updateable(deleteObj).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 逻辑删除
    /// </summary>
    public override bool Delete(Expression<Func<T, bool>> exp)
    {
        return Context.Updateable<T>().SetColumns(it => new T() { IsDeleted = true }, true).Where(exp)
            .ExecuteCommand() > 0;
    }

    /// <summary>
    /// 逻辑删除
    /// </summary>
    public override async Task<bool> DeleteAsync(Expression<Func<T, bool>> exp)
    {
        return await Context.Updateable<T>().SetColumns(it => new T() { IsDeleted = true }, true).Where(exp)
            .ExecuteCommandAsync() > 0;
    }
}