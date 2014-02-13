using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SystemBuilder.SB_SSOScheduler.Entities;
using System.Data;
using DotNetNuke.Security.Roles;
using DotNetNuke.Entities.Users;

namespace SystemBuilder.SB_SSOScheduler
{
    public class SSOScheduler : DotNetNuke.Services.Scheduling.SchedulerClient
    {
        SSOSechdulerController objSSOSechCtrl = new SSOSechdulerController();
        public SSOScheduler(DotNetNuke.Services.Scheduling.ScheduleHistoryItem objScheduleHistoryItem)
            : base()
        {

            this.ScheduleHistoryItem = objScheduleHistoryItem;

        }
        public override void DoWork()
        {
            try
            {
                this.Progressing();
                PushDNNRolesToSSO();
                this.ScheduleHistoryItem.Succeeded = true;                
            }
            catch (Exception ex)
            {
                this.ScheduleHistoryItem.Succeeded = false;
                this.ScheduleHistoryItem.AddLogNote("Records Not Added" + ex.ToString());
                // this.Errored(ex);
            }
        }
        protected void PushDNNRolesToSSO()
        {
            DataTable dtSSORoles = new DataTable();

            dtSSORoles = objSSOSechCtrl.GetLatestSBUserRoles();

            if (dtSSORoles.Rows.Count > 0)
            //means sso user has been added to a new dnnrole
            //so we need to add this role in module table and push role to SBSite
            {
                foreach (DataRow drSSORole in dtSSORoles.Rows)
                {

                    string strRoleCreatedDate = drSSORole["CreatedOnDate"].ToString();
                    DateTime objRoleCreatedDate = new DateTime();
                    objRoleCreatedDate = DateTime.Parse(strRoleCreatedDate);
                    DateTime objRoleEndDate = new DateTime();
                    objRoleEndDate = objRoleCreatedDate.AddYears(20);
                    int _portalid=Convert.ToInt32(drSSORole["portalid"].ToString());
                    int _siteid = Convert.ToInt32(drSSORole["SiteId"].ToString());
                    string _uid = drSSORole["UID"].ToString();
                    string _token = drSSORole["Token"].ToString();
                    string DNNRoleName = drSSORole["RoleName"].ToString();

                    if (drSSORole["RoleName"].ToString() != "Registered Users" && drSSORole["RoleName"].ToString() != "Subscribers" && drSSORole["RoleName"].ToString() != "Administrators")
                    {
                        objSSOSechCtrl.AddSSOUserRole(Convert.ToInt32(drSSORole["UserId"].ToString()), drSSORole["RoleName"].ToString(), drSSORole["RoleName"].ToString(),_portalid,_siteid,_uid,_token);
                        //now to push dnn role to SBSite                      

                        SB.UI.Facade.SSO.PushDNNRole(_siteid, new Guid(_uid), _token, DNNRoleName, objRoleCreatedDate, objRoleEndDate, decimal.Parse("100"), SB.Common.PaymentMethod.Cash);
                    }

                }

            }
            else //here we check that any of the module added role is modified in DNNRole i.e removed from any of the Existing role that has been Pushed
            { 
              //first get all UserRoles in DNN and compare to Temp Role table
                DataTable dtAllPushedRoles=new DataTable();
                dtAllPushedRoles = objSSOSechCtrl.GetAllPushedRoles();
                RoleController objRoleCtrl = new RoleController();
                RoleInfo objRoleInfo = new RoleInfo();
                UserController objUserCtrl=new UserController();
                UserInfo objUserInfo=new UserInfo();

                if (dtAllPushedRoles.Rows.Count > 0)
                {
                    foreach (DataRow drPushedRole in dtAllPushedRoles.Rows)
                    {
                        int portalid = Convert.ToInt32(drPushedRole["portalid"].ToString());
                        string strdnnRoleName=drPushedRole["DNNRole"].ToString();
                        int userid=Convert.ToInt32(drPushedRole["UserId"].ToString());
                        RoleInfo objTempRoleinfo = objRoleCtrl.GetRoleByName(portalid,strdnnRoleName);
                        if (objTempRoleinfo != null)
                        {
                           UserRoleInfo objUseRoleInfo= objRoleCtrl.GetUserRole(portalid, userid, objTempRoleinfo.RoleID);
                           if (objUseRoleInfo == null)
                           { 
                             //now delete the role from temp table
                               objSSOSechCtrl.DeleteModifiedPushedRole(userid, strdnnRoleName);

                           }
                           
                        }
                    }
                
                }
            }
        
        }
    
    
    }
}