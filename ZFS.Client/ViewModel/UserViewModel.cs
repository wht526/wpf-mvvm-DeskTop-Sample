﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZFS.Client.LogicCore.Common;
using ZFS.Client.LogicCore.Configuration;
using ZFS.Client.LogicCore.Enums;
using ZFS.Client.ViewModel.VMBase;
using ZFS.Core.Interfaces;
using ZFS.Model.Entity;
using ZFS.Model.Query;

namespace ZFS.Client.ViewModel
{
    /// <summary>
    /// 用户列表
    /// </summary>
    [Module(ModuleType.BasicData, "UserDlg", "用户管理")]
    public class UserViewModel : DataProcess<User>
    {
        private readonly IUserService service;
        public UserViewModel()
        {
            service = ServiceProvider.Instance.Get<IUserService>();
            this.Init();
        }

        public override async void GetPageData(int pageIndex)
        {
            try
            {
                var r = await service.GetUsersAsync(new UserParameters()
                {
                    PageIndex = pageIndex,
                    PageSize = PageSize,
                    Search = SearchText,
                });
                if (r.success)
                {
                    TotalCount = r.TotalRecord;
                    GridModelList.Clear();
                    r.users.ForEach((arg) => GridModelList.Add(arg));
                    base.SetPageCount();
                }
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }

        public override async void Del<TModel>(TModel model)
        {
            var user = model as User;
            var delResult = await service.DeleteUserAsync(user.Id);
            if (delResult)
            {
                GetPageData(this.PageIndex);
            }
        }

        public override async void Save()
        {
            if (!this.IsValid)
            {
                return;
            }

            if (Mode == ActionMode.Add)
            {
                var addResult = await service.AddUserAsync(Model);
                if (addResult)
                {
                    GridModelList.Add(Model);
                }
            }
            else
            {
                var updateResult = await service.UpdateUserAsync(Model);
                if (updateResult)
                {
                    var model = GridModelList.FirstOrDefault(t => t.Id == Model.Id);
                    if (model != null)
                    {
                        GridModelList.Remove(model);
                        GridModelList.Add(Model);
                    }
                }
            }
            base.Save();
        }
    }
}
