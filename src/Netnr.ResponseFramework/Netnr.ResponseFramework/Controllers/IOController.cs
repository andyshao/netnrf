using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Netnr.Data;
using Netnr.Func;
using Netnr.Func.ViewModel;

namespace Netnr.ResponseFramework.Controllers
{
    /// <summary>
    /// 输入输出
    /// </summary>
    //[Authorize]
    public class IOController : Controller
    {
        #region 导出

        [Description("公共导出")]
        public ActionResultVM Export(QueryDataInputVM ivm, string title = "export")
        {
            var vm = new ActionResultVM();

            //文件路径
            string path = "/upload/temp/";
            var vpath = GlobalTo.WebRootPath + path;

            if (!Directory.Exists(vpath))
            {
                Directory.CreateDirectory(vpath);
            }

            //文件名
            string filename = title.Replace(" ", "").Trim() + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

            //导出的表数据
            var dtReport = new DataTable();

            try
            {
                switch (ivm.tableName?.ToLower())
                {
                    default:
                        vm.Set(ARTag.invalid);
                        break;

                    //角色
                    case "sysrole":
                        {
                            using var ctl = new SettingController();
                            dtReport = ExportAid.ModelsMapping(ivm, ctl.QuerySysRole(ivm));
                        }
                        break;

                    //用户
                    case "sysuser":
                        {
                            using var ctl = new SettingController();
                            dtReport = ExportAid.ModelsMapping(ivm, ctl.QuerySysUser(ivm));
                        }
                        break;

                    //日志
                    case "syslog":
                        {
                            using var ctl = new SettingController();
                            dtReport = ExportAid.ModelsMapping(ivm, ctl.QuerySysLog(ivm));
                        }
                        break;

                    //字典
                    case "sysdictionary":
                        {
                            using var ctl = new SettingController();
                            dtReport = ExportAid.ModelsMapping(ivm, ctl.QuerySysDictionary(ivm));
                        }
                        break;
                }

                if (vm.msg != ARTag.invalid.ToString())
                {
                    //生成
                    if (Fast.NpoiTo.DataTableToExcel(dtReport, vpath + filename))
                    {
                        vm.data = path + filename;

                        //生成的Excel继续操作
                        ExportAid.ExcelDraw(vpath + filename, ivm);

                        vm.Set(ARTag.success);
                    }
                    else
                    {
                        vm.Set(ARTag.fail);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                vm.Set(ex);
            }

            return vm;
        }

        [Description("导出下载")]
        public void ExportDown(string path)
        {
            path = GlobalTo.ContentRootPath + path;
            new Fast.DownTo(Response).Stream(path, "");
        }

        #endregion
    }
}