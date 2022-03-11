using Kingdee.BOS;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.ServiceHelper;
using Kingdee.BOS.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZSJ_PJBJD
{
    [HotUpdate]
    [Description("配件报价单")]
    public class PJBJD : AbstractDynamicFormPlugIn
    {
        public override void AfterButtonClick(AfterButtonClickEventArgs e)
        {
            base.AfterButtonClick(e);
            if (e.Key.EqualsIgnoreCase("F_SZSJ_Button")) 
            {
                //throw new Exception("123");
                string FMaterialID = "";
                string FMaterialNumber = "";
                if (this.Model.GetValue("F_SZSJ_MaterialId").IsNullOrEmptyOrWhiteSpace() == false)
                {
                    DynamicObject dy_Material = this.Model.GetValue("F_SZSJ_MATERIALID") as DynamicObject;
                    FMaterialID = dy_Material[0].ToString();//物料内码
                    FMaterialNumber = dy_Material["Number"].ToString();//物料编码
                }
                var jgxs = ReadSysParameter(this.Context, this.Context.CurrentOrganizationInfo.ID, "SAL_SystemParameter", "F_SZSJ_CL");

                //string sql = string.Format("EXEC NEWPJBJD  '{0}','{1}'", FMaterialNumber, jgxs);
                string sql = string.Format("EXEC NEWPJBJD  '{0}'", FMaterialNumber);
                DataSet ds = DBServiceHelper.ExecuteDataSet(Context, sql);
                DataTable dt = ds.Tables[0];
                this.View.Model.DeleteEntryData("FEntity");
                var rEntity = this.View.Model.BusinessInfo.GetEntity("FEntity");

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        this.Model.CreateNewEntryRow(rEntity, i);

                        this.Model.SetValue("FMaterialID", dt.Rows[i]["物料内码"], i);
                        this.Model.SetValue("F_SZSJ_AMOUNT", dt.Rows[i]["销售单价"], i);
                        this.Model.SetValue("F_SZSJ_HSAmount", dt.Rows[i]["含税单价"], i);
                    }
                    this.View.UpdateView("FEntity");
                }
            }
        }



        /// <summary>
        /// 读取系统参数
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="orgId"></param>
        /// <param name="parameterObjId">系统参数的业务对象标识</param>
        /// <param name="parameterName">系统参数的实体属性名</param>
        /// <returns></returns>
        public static object ReadSysParameter(Context ctx, long orgId, string parameterObjId, string parameterName)
        {
            // 读取系统参数数据包            
            var parameterData = SystemParameterServiceHelper.Load(ctx, orgId, 0, parameterObjId);
            // 从系统参数数据包中获取某一个参数            
            if (parameterData != null && parameterData.DynamicObjectType.Properties.ContainsKey(parameterName))
            {
                return parameterData[parameterName];
            }
            return null;
        }
    }


}
