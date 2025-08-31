using Azure;
using Dapper;
using Dapper.Contrib.Extensions;
using MES_ReportForms.Core.Entities;
using MES_ReportForms.Core.Models;
using MES_ReportForms.Utilities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MES_ReportForms.Core.Repositories.Sql
{
    public class DashboardRepository : ProcessRepositoryBase
    {
        public DashboardRepository(string connectionName = "") : base(connectionName)
        {

        }
          
        public async Task<ApiResult<IEnumerable<dynamic>>> GetVacationRecord(AttendanceRecordQuery query)
        {
            var sqlBuilder = new StringBuilder();
             
            sqlBuilder.Append($@" -- 修改后的综合出勤和休假统计
                                DECLARE @Today DATE = CONVERT(DATE, GETDATE()); 
                                SELECT
                                    @OrganizationID AS OrganizationID,
                                    (SELECT COUNT(DISTINCT GUID) FROM T_UserMaster WHERE OrganizationID = @OrganizationID) AS TotalNumberEmployees,
                                    (SELECT COUNT(DISTINCT ulr.GUID) FROM T_UserMaster um JOIN T_UserLoginRecords ulr ON um.GUID = ulr.GUID WHERE um.OrganizationID = @OrganizationID AND CONVERT(DATE, ulr.LoginTime) = @Today) AS NumberAttendance,
                                    (SELECT COUNT(DISTINCT tea.GUID) FROM T_EmployeeAttendance tea JOIN T_UserMaster um ON tea.GUID = um.GUID WHERE um.OrganizationID = @OrganizationID AND tea.LeaveType = 4 AND tea.AttendanceYear = YEAR(@Today) AND tea.AttendanceMonth = MONTH(@Today) AND tea.AttendanceDay = DAY(@Today)) AS NumberPeopleVacation,
                                    (SELECT COUNT(DISTINCT tea.GUID) FROM T_EmployeeAttendance tea JOIN T_UserMaster um ON tea.GUID = um.GUID WHERE um.OrganizationID = @OrganizationID AND tea.LeaveType = 1 AND tea.AttendanceYear = YEAR(@Today) AND tea.AttendanceMonth = MONTH(@Today) AND tea.AttendanceDay = DAY(@Today)) AS NumberAnnualRestDays,
                                    (SELECT COUNT(DISTINCT tea.GUID) FROM T_EmployeeAttendance tea JOIN T_UserMaster um ON tea.GUID = um.GUID WHERE um.OrganizationID = @OrganizationID AND tea.LeaveType = 2 AND tea.AttendanceYear = YEAR(@Today) AND tea.AttendanceMonth = MONTH(@Today) AND tea.AttendanceDay = DAY(@Today)) AS NumberSickLeavePatients,
                                    -- 其他休假人数 = 员工休假表中LeaveType=3 + 没打卡且没请假的人数
                                    (SELECT COUNT(DISTINCT tea.GUID) FROM T_EmployeeAttendance tea JOIN T_UserMaster um ON tea.GUID = um.GUID WHERE um.OrganizationID = @OrganizationID AND tea.LeaveType = 3 AND tea.AttendanceYear = YEAR(@Today) AND tea.AttendanceMonth = MONTH(@Today) AND tea.AttendanceDay = DAY(@Today)) +
                                    (SELECT COUNT(DISTINCT um.GUID) FROM T_UserMaster um LEFT JOIN T_UserLoginRecords ulr ON um.GUID = ulr.GUID AND CONVERT(DATE, ulr.LoginTime) = @Today LEFT JOIN T_EmployeeAttendance tea ON um.GUID = tea.GUID AND tea.AttendanceYear = YEAR(@Today) AND tea.AttendanceMonth = MONTH(@Today) AND tea.AttendanceDay = DAY(@Today) WHERE um.OrganizationID = @OrganizationID AND ulr.GUID IS NULL AND tea.GUID IS NULL) AS OtherNumberVacationers;

                                     ");

            var sqlParams = new DynamicParameters();

            sqlParams.Add($"OrganizationID", query.OrganizationID, DbType.Int32, ParameterDirection.Input);

            var returnData = await DBCon.QueryAsync(sqlBuilder.ToString(), sqlParams);

            return ApiResult<IEnumerable<dynamic>>.Ok(returnData);
        }
         
        public async Task<ApiResult<IEnumerable<dynamic>>> GetMissCountPerformanceVolume(MissCountQuery query)
        { 
            var sqlBuilder = new StringBuilder();

            var sqlParams = new DynamicParameters();

            var whereStr = new StringBuilder();
            if (query.OrganizationID.HasValue)
            {
                whereStr.Append($@" AND UM.OrganizationID = @OrganizationID ");
                sqlParams.Add($"OrganizationID", query.OrganizationID, DbType.String, ParameterDirection.Input);
            }
            //改变当前数据库连接的隔离级别，后续在这个连接内执行的所有 SQL 都会以 READ UNCOMMITTED（等同于 NOLOCK）的方式运行，直到连接关闭或重新设置隔离级别。
            sqlBuilder.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");

            if (query.Type == "Day")
            {
                sqlBuilder.Append($@"
                        -- 按日统计
                        WITH Latest57 AS (
                            SELECT 
                                ah.fileName,
                                ah.taskUser,
                                ah.createDate,
                                ah.status,
                                ISNULL(ah.JJCount,0) AS JJCount,
                                ROW_NUMBER() OVER (
                                    PARTITION BY ah.taskUser, ah.fileName
                                    ORDER BY ah.createDate DESC
                                ) AS rn
                            FROM dbo.T_ActionHistory ah WITH (NOLOCK)
                            WHERE ah.status = 57
                              AND CAST(ah.createDate AS DATE) = CAST(GETDATE() AS DATE)
                        ),
                        AH_Union AS (
                            SELECT fileName, taskUser, createDate, status, JJCount
                            FROM dbo.T_ActionHistory ah WITH (NOLOCK)
                            WHERE ah.status IN (60,59)
                              AND CAST(ah.createDate AS DATE) = CAST(GETDATE() AS DATE)
                            UNION ALL
                            SELECT fileName, taskUser, createDate, status, JJCount
                            FROM Latest57
                            WHERE rn = 1
                        ),
                        TaskData1 AS (
                            SELECT 
                                UM.OrganizationID AS Department,
                                UM.user_name AS UserName,
                                CAST(AH.createDate AS DATE) AS ReportingDate,
                                'Daily' AS TimePeriod,
                                COUNT_BIG(DISTINCT AH.fileName) AS TaskCount,
                                SUM(ISNULL(AH.JJCount,0)) AS JJCount
                            FROM T_ActionHistory AH
                            INNER JOIN T_UserMaster UM 
                                ON AH.taskUser COLLATE Chinese_PRC_CI_AS = UM.GUID COLLATE Chinese_PRC_CI_AS
                            WHERE AH.status = 49
                                {whereStr.ToString()}
                                AND CAST(AH.createDate AS DATE) = CAST(GETDATE() AS DATE)
                            GROUP BY UM.OrganizationID, UM.user_name, CAST(AH.createDate AS DATE)
                        ),
                        TaskData2 AS (
                            SELECT 
                                UM.OrganizationID AS Department,
                                UM.user_name AS UserName,
                                CAST(AH.createDate AS DATE) AS ReportingDate,
                                'Daily' AS TimePeriod,
                                COUNT_BIG(DISTINCT AH.fileName) AS TaskCount,
                                SUM(ISNULL(AH.JJCount,0)) AS JJCount
                            FROM AH_Union AH
                            INNER JOIN T_UserMaster UM 
                                ON AH.taskUser COLLATE Chinese_PRC_CI_AS = UM.GUID COLLATE Chinese_PRC_CI_AS
                                {whereStr.ToString()}   
                            GROUP BY UM.OrganizationID, UM.user_name, CAST(AH.createDate AS DATE)
                        ),
                        TaskData3 AS (
                            SELECT 
                                UM.OrganizationID AS Department,
                                IW.User_Name AS UserName,
                                CAST(IW.Create_Time AS DATE) AS ReportingDate,
                                'Daily' AS TimePeriod,
                                SUM(IW.Non_OCR_JJCount) AS NoOcrJJOperated,
                                SUM(IW.Non_OCR_Count) AS NoOcrOperated,
                                SUM(IW.Duration) AS TotalDurationHours
                            FROM T_IndirectWork IW
                            INNER JOIN T_UserMaster UM ON IW.GUID = UM.GUID
                            WHERE CAST(IW.Create_Time AS DATE) = CAST(GETDATE() AS DATE)
                                {whereStr.ToString()}
                            GROUP BY UM.OrganizationID, IW.User_Name, CAST(IW.Create_Time AS DATE)
                        )
                        SELECT 
                            COALESCE(t1.Department, t2.Department) AS Department,
                            COALESCE(t1.UserName, t2.UserName) AS UserName,
                            COALESCE(t1.ReportingDate, t2.ReportingDate) AS ReportingDate,
                            'Daily' AS TimePeriod,
                            COALESCE(t1.TaskCount, null) AS MissTaskCount,
                            COALESCE(t1.JJCount, null) AS MissJJCount,
                            COALESCE(t2.TaskCount, null) AS JSTaskCount,
                            COALESCE(t2.JJCount, null) AS JSJJCount,
                            COALESCE(t3.NoOcrJJOperated, null) AS NoOcrJJOperated,
                            COALESCE(t3.NoOcrOperated, null) AS NoOcrOperated,
                            COALESCE(t3.TotalDurationHours, null) AS TotalDurationHours
                        FROM TaskData1 t1
                            FULL OUTER JOIN TaskData2 t2 
                                ON t1.Department = t2.Department 
                                AND t1.UserName = t2.UserName 
                                AND t1.ReportingDate = t2.ReportingDate
                            FULL OUTER JOIN TaskData3 t3 
                                ON COALESCE(t1.Department, t2.Department) = t3.Department 
                                AND COALESCE(t1.UserName, t2.UserName) = t3.UserName 
                                AND COALESCE(t1.ReportingDate, t2.ReportingDate) = t3.ReportingDate
                        ORDER BY Department, UserName, ReportingDate; ");
                            }
            else if (query.Type == "Week")
            {
                sqlBuilder.Append($@"
                        WITH Latest57 AS (
                            SELECT 
                                ah.fileName,
                                ah.taskUser,
                                ah.createDate,
                                ah.status,
                                ISNULL(ah.JJCount,0) AS JJCount,
                                ROW_NUMBER() OVER (
                                    PARTITION BY ah.taskUser, ah.fileName
                                    ORDER BY ah.createDate DESC
                                ) AS rn
                            FROM dbo.T_ActionHistory ah WITH (NOLOCK)
                            WHERE ah.status = 57
                              AND AH.createDate >= DATEADD(DAY, 1-DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE))
                              AND AH.createDate < DATEADD(DAY, 8-DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE))
                        ),
                        AH_Union AS (
                            SELECT fileName, taskUser, createDate, status, JJCount
                            FROM dbo.T_ActionHistory ah WITH (NOLOCK)
                            WHERE ah.status IN (60,59)
                              AND AH.createDate >= DATEADD(DAY, 1-DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE))
                              AND AH.createDate < DATEADD(DAY, 8-DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE))
                            UNION ALL
                            SELECT fileName, taskUser, createDate, status, JJCount
                            FROM Latest57
                            WHERE rn = 1
                        ),
                        TaskData1 AS (
                            SELECT 
                                UM.OrganizationID AS Department,
                                UM.user_name AS UserName,
                                DATEADD(DAY, 1-DATEPART(WEEKDAY, AH.createDate), CAST(AH.createDate AS DATE)) AS ReportingDate,
                                'Weekly' AS TimePeriod,
                                COUNT_BIG(DISTINCT AH.fileName) AS TaskCount,
                                SUM(ISNULL(AH.JJCount,0)) AS JJCount
                            FROM T_ActionHistory AH
                            INNER JOIN T_UserMaster UM 
                                ON AH.taskUser COLLATE Chinese_PRC_CI_AS = UM.GUID COLLATE Chinese_PRC_CI_AS
                            WHERE AH.status = 49
                                {whereStr.ToString()}
                                AND AH.createDate >= DATEADD(DAY, 1-DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE))
                                AND AH.createDate < DATEADD(DAY, 8-DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE))
                            GROUP BY UM.OrganizationID, UM.user_name, DATEADD(DAY, 1-DATEPART(WEEKDAY, AH.createDate), CAST(AH.createDate AS DATE))
                        ),
                        TaskData2 AS (
                            SELECT 
                                UM.OrganizationID AS Department,
                                UM.user_name AS UserName,
                                DATEADD(DAY, 1-DATEPART(WEEKDAY, AH.createDate), CAST(AH.createDate AS DATE)) AS ReportingDate,
                                'Weekly' AS TimePeriod,
                                COUNT_BIG(DISTINCT AH.fileName) AS TaskCount,
                                SUM(ISNULL(AH.JJCount,0)) AS JJCount
                            FROM AH_Union AH
                            INNER JOIN T_UserMaster UM 
                                ON AH.taskUser COLLATE Chinese_PRC_CI_AS = UM.GUID COLLATE Chinese_PRC_CI_AS
                                {whereStr.ToString()}
                            GROUP BY UM.OrganizationID, UM.user_name, DATEADD(DAY, 1-DATEPART(WEEKDAY, AH.createDate), CAST(AH.createDate AS DATE))
                        ),
                        TaskData3 AS (
                            SELECT 
                                UM.OrganizationID AS Department,
                                IW.User_Name AS UserName,
                                DATEADD(DAY, 1 - DATEPART(WEEKDAY, IW.Create_Time), CAST(IW.Create_Time AS DATE)) AS ReportingDate,
                                'Weekly' AS TimePeriod,
                                SUM(IW.Non_OCR_JJCount) AS NoOcrJJOperated,
                                SUM(IW.Non_OCR_Count) AS NoOcrOperated,
                                SUM(IW.Duration) AS TotalDurationHours
                            FROM T_IndirectWork IW
                            INNER JOIN T_UserMaster UM ON IW.GUID = UM.GUID
                            WHERE IW.Create_Time >= DATEADD(DAY, 1 - DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE))
                              AND IW.Create_Time < DATEADD(DAY, 8 - DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE))
                              {whereStr.ToString()}
                            GROUP BY UM.OrganizationID, IW.User_Name, DATEADD(DAY, 1 - DATEPART(WEEKDAY, IW.Create_Time), CAST(IW.Create_Time AS DATE))
                        )
                        SELECT 
                            COALESCE(t1.Department, t2.Department) AS Department,
                            COALESCE(t1.UserName, t2.UserName) AS UserName,
                            COALESCE(t1.ReportingDate, t2.ReportingDate) AS ReportingDate,
                            'Weekly' AS TimePeriod,
                            COALESCE(t1.TaskCount, null) AS MissTaskCount,
                            COALESCE(t1.JJCount, null) AS MissJJCount,
                            COALESCE(t2.TaskCount, null) AS JSTaskCount,
                            COALESCE(t2.JJCount, null) AS JSJJCount,
                            COALESCE(t3.NoOcrJJOperated, null) AS NoOcrJJOperated,
                            COALESCE(t3.NoOcrOperated, null) AS NoOcrOperated,
                            COALESCE(t3.TotalDurationHours, null) AS TotalDurationHours
                        FROM TaskData1 t1
                            FULL OUTER JOIN TaskData2 t2 
                                ON t1.Department = t2.Department 
                                AND t1.UserName = t2.UserName 
                                AND t1.ReportingDate = t2.ReportingDate
                            FULL OUTER JOIN TaskData3 t3 
                                ON COALESCE(t1.Department, t2.Department) = t3.Department 
                                AND COALESCE(t1.UserName, t2.UserName) = t3.UserName 
                                AND COALESCE(t1.ReportingDate, t2.ReportingDate) = t3.ReportingDate
                        ORDER BY Department, UserName, ReportingDate; ");
                            }
            else if (query.Type == "Month")
            {
                sqlBuilder.Append($@"
                        WITH Latest57 AS (
                            SELECT 
                                ah.fileName,
                                ah.taskUser,
                                ah.createDate,
                                ah.status,
                                ISNULL(ah.JJCount,0) AS JJCount,
                                ROW_NUMBER() OVER (
                                    PARTITION BY ah.taskUser, ah.fileName
                                    ORDER BY ah.createDate DESC
                                ) AS rn
                            FROM dbo.T_ActionHistory ah WITH (NOLOCK)
                            WHERE ah.status = 57
                              AND AH.createDate >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
                              AND AH.createDate < DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE())+1, 1)
                        ),
                        AH_Union AS (
                            SELECT fileName, taskUser, createDate, status, JJCount
                            FROM dbo.T_ActionHistory ah WITH (NOLOCK)
                            WHERE ah.status IN (60,59)
                              AND AH.createDate >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
                              AND AH.createDate < DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE())+1, 1)
                            UNION ALL
                            SELECT fileName, taskUser, createDate, status, JJCount
                            FROM Latest57
                            WHERE rn = 1
                        ),
                        TaskData1 AS (
                            SELECT 
                                UM.OrganizationID AS Department,
                                UM.user_name AS UserName,
                                DATEFROMPARTS(YEAR(AH.createDate), MONTH(AH.createDate), 1) AS ReportingDate,
                                'Monthly' AS TimePeriod,
                                COUNT_BIG(DISTINCT AH.fileName) AS TaskCount,
                                SUM(ISNULL(AH.JJCount,0)) AS JJCount
                            FROM T_ActionHistory AH
                            INNER JOIN T_UserMaster UM 
                                ON AH.taskUser COLLATE Chinese_PRC_CI_AS = UM.GUID COLLATE Chinese_PRC_CI_AS
                            WHERE AH.status = 49
                                {whereStr.ToString()}
                                AND AH.createDate >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
                                AND AH.createDate < DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE())+1, 1)
                            GROUP BY UM.OrganizationID, UM.user_name, DATEFROMPARTS(YEAR(AH.createDate), MONTH(AH.createDate), 1)
                        ),
                        TaskData2 AS (
                            SELECT 
                                UM.OrganizationID AS Department,
                                UM.user_name AS UserName,
                                DATEFROMPARTS(YEAR(AH.createDate), MONTH(AH.createDate), 1) AS ReportingDate,
                                'Monthly' AS TimePeriod,
                                COUNT_BIG(DISTINCT AH.fileName) AS TaskCount,
                                SUM(ISNULL(AH.JJCount,0)) AS JJCount
                            FROM AH_Union AH
                            INNER JOIN T_UserMaster UM 
                                ON AH.taskUser COLLATE Chinese_PRC_CI_AS = UM.GUID COLLATE Chinese_PRC_CI_AS
                                {whereStr.ToString()}
                            GROUP BY UM.OrganizationID, UM.user_name, DATEFROMPARTS(YEAR(AH.createDate), MONTH(AH.createDate), 1)
                        ),
                        TaskData3 AS (
                            SELECT 
                                UM.OrganizationID AS Department,
                                IW.User_Name AS UserName,
                                DATEFROMPARTS(YEAR(IW.Create_Time), MONTH(IW.Create_Time), 1) AS ReportingDate,
                                'Monthly' AS TimePeriod,
                                SUM(IW.Non_OCR_JJCount) AS NoOcrJJOperated,
                                SUM(IW.Non_OCR_Count) AS NoOcrOperated,
                                SUM(IW.Duration) AS TotalDurationHours
                            FROM T_IndirectWork IW
                            INNER JOIN T_UserMaster UM ON IW.GUID = UM.GUID
                            WHERE IW.Create_Time >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
                              AND IW.Create_Time < DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE())+1, 1)
                              {whereStr.ToString()}
                            GROUP BY UM.OrganizationID, IW.User_Name, DATEFROMPARTS(YEAR(IW.Create_Time), MONTH(IW.Create_Time), 1)
                        )
                        SELECT 
                            COALESCE(t1.Department, t2.Department) AS Department,
                            COALESCE(t1.UserName, t2.UserName) AS UserName,
                            COALESCE(t1.ReportingDate, t2.ReportingDate) AS ReportingDate,
                            'Monthly' AS TimePeriod,
                            COALESCE(t1.TaskCount, null) AS MissTaskCount,
                            COALESCE(t1.JJCount, null) AS MissJJCount,
                            COALESCE(t2.TaskCount, null) AS JSTaskCount,
                            COALESCE(t2.JJCount, null) AS JSJJCount,
                            COALESCE(t3.NoOcrJJOperated, null) AS NoOcrJJOperated,
                            COALESCE(t3.NoOcrOperated, null) AS NoOcrOperated,
                            COALESCE(t3.TotalDurationHours, null) AS TotalDurationHours
                        FROM TaskData1 t1
                            FULL OUTER JOIN TaskData2 t2 
                                ON t1.Department = t2.Department 
                                AND t1.UserName = t2.UserName 
                                AND t1.ReportingDate = t2.ReportingDate
                            FULL OUTER JOIN TaskData3 t3 
                                ON COALESCE(t1.Department, t2.Department) = t3.Department 
                                AND COALESCE(t1.UserName, t2.UserName) = t3.UserName 
                                AND COALESCE(t1.ReportingDate, t2.ReportingDate) = t3.ReportingDate
                        ORDER BY Department, UserName, ReportingDate; ");
            }

            var sqlStr = sqlBuilder?.ToString();

            var returnData = await DBCon.QueryAsync(sqlStr, sqlParams);

            return ApiResult<IEnumerable<dynamic>>.Ok(returnData);
        }
        
        public async Task<ApiResult<IEnumerable<dynamic>>> GetUtilization(OrganizationalFormQuery query)
        {
            var sqlBuilder = new StringBuilder();

            var sqlParams = new DynamicParameters();

            var whereStr = new StringBuilder();
            if (query.OrganizationID.HasValue)
            {
                whereStr.Append($@" AND UM.OrganizationID = @OrganizationID ");
                sqlParams.Add($"OrganizationID", query.OrganizationID, DbType.String, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(query.GUID))
            {
                whereStr.Append($@" AND UM.GUID = @GUID ");
                sqlParams.Add($"GUID", query.GUID, DbType.String, ParameterDirection.Input);
            }

            if (query.Type == "Day")
            {
                sqlBuilder.Append($@" -- 按日统计
                                   WITH TaskData1 AS (
                                        SELECT
		                                     um.OrganizationID AS OrganizationID,
		                                     um.User_Name AS UserName,
		                                     CONVERT(DATE, ulr.LoginTime) AS ReportingDate,
		                                     SUM(ISNULL(ulr.MeetingDuration, 0)) / 3600.0 AS MeetingDuration,
		                                     SUM(ISNULL(ulr.TrainingDuration, 0)) / 3600.0 AS TrainingDuration,
		                                     SUM(ISNULL(ulr.BreakDuration, 0)) / 3600.0 AS BreakDuration
	                                     FROM
		                                     T_UserLoginRecords ulr
	                                     JOIN
		                                     T_UserMaster UM ON ulr.GUID = UM.GUID
	                                     WHERE 
		                                     CONVERT(DATE, ulr.LoginTime) = CONVERT(DATE, GETDATE())  -- 当日数据
                                                {whereStr.ToString()}
	                                     GROUP BY
		                                     um.OrganizationID,
		                                     um.User_Name,
		                                     CONVERT(DATE, ulr.LoginTime)
                                    ),
                                    TaskData2 AS (
                                       SELECT 
		                                    UM.OrganizationID AS OrganizationID,
		                                    IW.User_Name AS UserName,
		                                    CAST(IW.Create_Time AS DATE) AS ReportingDate,
		                                    SUM(IW.Duration) AS NotTotalDurationHours
	                                    FROM 
		                                    T_IndirectWork IW
	                                    INNER JOIN 
		                                    T_UserMaster UM ON IW.GUID = UM.GUID
	                                    WHERE  CAST(IW.Create_Time AS DATE) = CAST(GETDATE() AS DATE)  -- 当日数据
                                                {whereStr.ToString()}
	                                    GROUP BY 
		                                    UM.OrganizationID,
		                                    IW.User_Name,
		                                    CAST(IW.Create_Time AS DATE) 
                                    )
                                    SELECT 
                                         COALESCE(t1.OrganizationID, t2.OrganizationID) AS OrganizationID,
                                         COALESCE(t1.UserName, t2.UserName) AS UserName,
                                         COALESCE(t1.ReportingDate, t2.ReportingDate) AS ReportingDate,
                                         'Daily' AS TimePeriod,
                                         COALESCE(t1.MeetingDuration, null) AS MeetingDuration,
                                         COALESCE(t1.TrainingDuration, null) AS TrainingDuration,
                                         COALESCE(t1.BreakDuration, null) AS BreakDuration,
                                         COALESCE(t2.NotTotalDurationHours, null) AS NotTotalDurationHours
                                    FROM   TaskData1 t1
                                    FULL OUTER JOIN  TaskData2 t2  ON  t1.OrganizationID = t2.OrganizationID AND t1.UserName = t2.UserName AND t1.ReportingDate = t2.ReportingDate
                                    ORDER BY 
                                         OrganizationID, UserName, ReportingDate; ");
            }
            else if (query.Type == "Week")
            {
                sqlBuilder.Append($@" -- 按周统计（以周一为周起始）
                                   WITH TaskData1 AS (
                                         SELECT
		                                     um.OrganizationID AS OrganizationID,
		                                     um.User_Name AS UserName,
		                                     DATEADD(DAY, 1-DATEPART(WEEKDAY, ulr.LoginTime), CAST(ulr.LoginTime AS DATE)) AS ReportingDate,
		                                     SUM(ISNULL(ulr.MeetingDuration, 0)) / 3600.0 AS MeetingDuration,
		                                     SUM(ISNULL(ulr.TrainingDuration, 0)) / 3600.0 AS TrainingDuration,
		                                     SUM(ISNULL(ulr.BreakDuration, 0)) / 3600.0 AS BreakDuration
	                                     FROM
		                                     T_UserLoginRecords ulr
	                                     JOIN
		                                     T_UserMaster um ON ulr.GUID = um.GUID
	                                     WHERE  
		                                     ulr.LoginTime >= DATEADD(DAY, 1 - DATEPART(WEEKDAY, GETDATE()), CONVERT(DATE, GETDATE()))  -- 本周一
		                                     AND ulr.LoginTime < DATEADD(DAY, 8 - DATEPART(WEEKDAY, GETDATE()), CONVERT(DATE, GETDATE()))   -- 下周一（不含）
                                                {whereStr.ToString()}
	                                     GROUP BY
		                                     um.OrganizationID,
		                                     um.User_Name,
		                                     YEAR(ulr.LoginTime),
		                                      DATEADD(DAY, 1-DATEPART(WEEKDAY, ulr.LoginTime), CAST(ulr.LoginTime AS DATE))
                                    ),
                                    TaskData2 AS (
                                       SELECT 
		                                    UM.OrganizationID AS OrganizationID,
		                                    IW.User_Name AS UserName,
		                                    DATEADD(DAY, 1 - DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE)) AS ReportingDate,
		                                    SUM(IW.Duration) AS NotTotalDurationHours
	                                    FROM 
		                                    T_IndirectWork IW
	                                    INNER JOIN 
		                                    T_UserMaster UM ON IW.GUID = UM.GUID
	                                    WHERE  IW.Create_Time >= DATEADD(DAY, 1 - DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE))
		                                    AND IW.Create_Time < DATEADD(DAY, 8 - DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE))
                                                {whereStr.ToString()}
	                                    GROUP BY 
		                                    UM.OrganizationID,
		                                    IW.User_Name,
		                                    DATEADD(DAY, 1 - DATEPART(WEEKDAY,IW.Create_Time), CAST(IW.Create_Time AS DATE)) 
                                    )
                                    SELECT 
                                         COALESCE(t1.OrganizationID, t2.OrganizationID) AS OrganizationID,
                                         COALESCE(t1.UserName, t2.UserName) AS UserName,
                                         COALESCE(t1.ReportingDate, t2.ReportingDate) AS ReportingDate,
                                         'Daily' AS TimePeriod,
                                         COALESCE(t1.MeetingDuration, null) AS MeetingDuration,
                                         COALESCE(t1.TrainingDuration, null) AS TrainingDuration,
                                         COALESCE(t1.BreakDuration, null) AS BreakDuration,
                                         COALESCE(t2.NotTotalDurationHours, null) AS NotTotalDurationHours
                                    FROM   TaskData1 t1
                                    FULL OUTER JOIN  TaskData2 t2  ON  t1.OrganizationID = t2.OrganizationID AND t1.UserName = t2.UserName AND t1.ReportingDate = t2.ReportingDate
                                    ORDER BY 
                                         OrganizationID, UserName, ReportingDate;");
            }
            else if (query.Type == "Month")
            {
                sqlBuilder.Append($@"  -- 按月统计（指定部门）
                                    WITH TaskData1 AS (
                                        SELECT
		                                    um.OrganizationID AS OrganizationID,
		                                    um.User_Name AS UserName,
		                                    DATEFROMPARTS(YEAR(ulr.LoginTime), MONTH(ulr.LoginTime), 1) AS ReportingDate,
		                                    SUM(ISNULL(ulr.MeetingDuration, 0)) / 3600.0 AS MeetingDuration,
		                                    SUM(ISNULL(ulr.TrainingDuration, 0)) / 3600.0 AS TrainingDuration,
		                                    SUM(ISNULL(ulr.BreakDuration, 0)) / 3600.0 AS BreakDuration
	                                    FROM
		                                    T_UserLoginRecords ulr
	                                    JOIN
		                                    T_UserMaster um ON ulr.GUID = um.GUID
	                                    WHERE 
		                                    ulr.LoginTime >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)  -- 当月第一天
		                                    AND ulr.LoginTime < DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()) + 1, 1)  -- 下月第一天（不含）
                                                {whereStr.ToString()}
	                                    GROUP BY
		                                    um.OrganizationID,
		                                    um.User_Name,
		                                    YEAR(ulr.LoginTime),
		                                    MONTH(ulr.LoginTime)
                                    ),
                                    TaskData2 AS (
	                                    SELECT 
		                                    UM.OrganizationID AS OrganizationID,
		                                    IW.User_Name AS UserName,
		                                    DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1) AS ReportingDate,
		                                    SUM(IW.Duration) AS NotTotalDurationHours
	                                    FROM 
		                                    T_IndirectWork IW
	                                    INNER JOIN 
		                                    T_UserMaster UM ON IW.GUID = UM.GUID
	                                    WHERE  IW.Create_Time >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
		                                    AND IW.Create_Time < DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()) + 1, 1)
                                                {whereStr.ToString()}
	                                    GROUP BY 
		                                    UM.OrganizationID,
		                                    IW.User_Name,
		                                    DATEFROMPARTS(YEAR(IW.Create_Time), MONTH(IW.Create_Time), 1) 
                                    )
                                    SELECT 
                                            COALESCE(t1.OrganizationID, t2.OrganizationID) AS OrganizationID,
                                            COALESCE(t1.UserName, t2.UserName) AS UserName,
                                            COALESCE(t1.ReportingDate, t2.ReportingDate) AS ReportingDate,
                                            'Daily' AS TimePeriod,
                                            COALESCE(t1.MeetingDuration, null) AS MeetingDuration,
                                            COALESCE(t1.TrainingDuration, null) AS TrainingDuration,
                                            COALESCE(t1.BreakDuration, null) AS BreakDuration,
                                            COALESCE(t2.NotTotalDurationHours, null) AS NotTotalDurationHours
                                    FROM   TaskData1 t1
                                    FULL OUTER JOIN  TaskData2 t2  ON  t1.OrganizationID = t2.OrganizationID AND t1.UserName = t2.UserName AND t1.ReportingDate = t2.ReportingDate
                                    ORDER BY 
                                            OrganizationID, UserName, ReportingDate;");
            }
            var sqlStr = sqlBuilder?.ToString();

            var returnData = await DBCon.QueryAsync(sqlStr, sqlParams);

            return ApiResult<IEnumerable<dynamic>>.Ok(returnData);
        }

        public async Task<ApiResult<dynamic>> GetProductivity(OrganizationalFormQuery query)
        {
            var sqlBuilder = new StringBuilder();
             
            var sqlParams = new DynamicParameters();

            var whereStr = new StringBuilder();
            if (query.OrganizationID.HasValue)
            {
                //whereStr.Append($@" AND UM.OrganizationID = @OrganizationID ");
                whereStr.Append(" AND OrganizationID = @OrganizationID ");

                sqlParams.Add($"OrganizationID", query.OrganizationID, DbType.String, ParameterDirection.Input);
            }
            //改变当前数据库连接的隔离级别，后续在这个连接内执行的所有 SQL 都会以 READ UNCOMMITTED（等同于 NOLOCK）的方式运行，直到连接关闭或重新设置隔离级别。
            sqlBuilder.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ");

            //if (query.Type == "Day")
            //{
            //    sqlBuilder.Append($@" -- 按日统计
            //                    SELECT 
            //                        UM.OrganizationID ,
            //                        UM.User_Name ,
            //                        CAST(AH.createDate AS DATE) AS ReportingDate,
            //                        'Daily' AS TimeCycle,
            //                        SUM(CASE WHEN AH.status IN (49, 50, 55, 57) THEN 1 ELSE 0 END) AS OCRMakerVolum,
            //                        SUM(CASE WHEN AH.status IN (58, 59, 60) THEN 1 ELSE 0 END) AS OCRCheckerVolum,
            //                        COUNT_BIG(DISTINCT T.c.value('(DetailValue)[1]', 'NVARCHAR(MAX)')) AS TotalVolum,  -- 统计不重复的JJ号
            //                        SUM(CASE WHEN T.c.value('.', 'NVARCHAR(MAX)') IS NOT NULL AND LTRIM(RTRIM(T.c.value('.', 'NVARCHAR(MAX)'))) <> '' THEN 1 ELSE 0 END) AS NotNullCount,
            //                        SUM(CASE WHEN AH.status IN (49, 50, 55, 57) THEN DATEDIFF(SECOND, AH.ActionStartDate, AH.ActionEndDate) ELSE 0 END) / 3600.0 AS OCRMakerHour,
            //                        SUM(CASE WHEN AH.status IN (58, 59, 60) THEN DATEDIFF(SECOND, AH.ActionStartDate, AH.ActionEndDate) ELSE 0 END) / 3600.0 AS OCRCheckerHour
            //                    FROM 
            //                        T_ActionHistory AH
            //                    INNER JOIN 
            //                        T_UserMaster UM ON AH.taskUser COLLATE Chinese_PRC_CI_AS = UM.GUID COLLATE Chinese_PRC_CI_AS
            //                    LEFT JOIN 
            //                        T_OrderInfo OH ON AH.fileName COLLATE Chinese_PRC_CI_AS = OH.fileName COLLATE Chinese_PRC_CI_AS
            //                    CROSS APPLY 
            //                        OH.filecontent.nodes('//DetailItem[DetailName=""JJ""]') AS T(c)  -- 直接定位JJ节点
            //                    WHERE  
            //                        AH.ActionEndDate IS NOT NULL  -- 确保时间统计有效性
            //                        AND CAST(AH.createDate AS DATE) = CAST(GETDATE() AS DATE)  -- 当日数据
            //                        {whereStr.ToString()}
            //                    GROUP BY 
            //                        UM.OrganizationID, 
            //                        UM.User_Name,
            //                        CAST(AH.createDate AS DATE)
            //                    ORDER BY 
            //                     UM.OrganizationID, 
            //                     UM.User_Name,ReportingDate
            //                     ");
            //}
            //else if (query.Type == "Week")
            //{
            //    sqlBuilder.Append($@"  -- 按周统计（以周一为周起始）
            //                    SELECT
            //                        UM.OrganizationID ,
            //                        UM.User_Name ,
            //                        DATEADD(DAY, 1-DATEPART(WEEKDAY, AH.createDate), CAST(AH.createDate AS DATE)) AS ReportingDate,
            //                        'Weekly' AS TimeCycle,
            //                        SUM(CASE WHEN AH.status IN (49, 50, 55, 57) THEN 1 ELSE 0 END) AS OCRMakerVolum,
            //                        SUM(CASE WHEN AH.status IN (58, 59, 60) THEN 1 ELSE 0 END) AS OCRCheckerVolum,
            //                        COUNT_BIG(DISTINCT T.c.value('(DetailValue)[1]', 'NVARCHAR(MAX)')) AS TotalVolum,  -- 统计不重复的JJ号
            //                        SUM(CASE WHEN T.c.value('.', 'NVARCHAR(MAX)') IS NOT NULL AND LTRIM(RTRIM(T.c.value('.', 'NVARCHAR(MAX)'))) <> '' THEN 1 ELSE 0 END) AS NotNullCount,
            //                        SUM(CASE WHEN AH.status IN (49, 50, 55, 57) THEN DATEDIFF(SECOND, AH.ActionStartDate, AH.ActionEndDate) ELSE 0 END) / 3600.0 AS OCRMakerHour,
            //                        SUM(CASE WHEN AH.status IN (58, 59, 60) THEN DATEDIFF(SECOND, AH.ActionStartDate, AH.ActionEndDate) ELSE 0 END) / 3600.0 AS OCRCheckerHour
            //                    FROM 
            //                        T_ActionHistory AH
            //                    INNER JOIN 
            //                        T_UserMaster UM ON AH.taskUser COLLATE Chinese_PRC_CI_AS  = UM.GUID COLLATE Chinese_PRC_CI_AS 
            //                    LEFT JOIN 
            //                        T_OrderInfo OH ON AH.fileName COLLATE Chinese_PRC_CI_AS  = OH.fileName COLLATE Chinese_PRC_CI_AS 
            //                    CROSS APPLY 
            //                        OH.filecontent.nodes('//DetailItem[DetailName=""JJ""]') AS T(c)  -- 直接定位JJ节点
            //                    WHERE
            //                        AH.ActionEndDate IS NOT NULL
            //                        AND AH.createDate >= DATEADD(DAY, 1-DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE))  -- 本周一
            //                        AND AH.createDate < DATEADD(DAY, 8-DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE))   -- 下周一（不含）
            //                        {whereStr.ToString()}
            //                    GROUP BY 
            //                        UM.OrganizationID, 
            //                        UM.User_Name,
            //                        DATEADD(DAY, 1-DATEPART(WEEKDAY, AH.createDate), CAST(AH.createDate AS DATE))
            //                    ORDER BY 
            //                     UM.OrganizationID, 
            //                     UM.User_Name,ReportingDate ");
            //}
            //else if (query.Type == "Month")
            //{
            //    sqlBuilder.Append($@" -- 按月统计
            //                    SELECT 
            //                        UM.OrganizationID ,
            //                        UM.User_Name ,
            //                        DATEFROMPARTS(YEAR(AH.createDate), MONTH(AH.createDate), 1) AS ReportingDate,
            //                        'Monthly' AS TimeCycle,
            //                        SUM(CASE WHEN AH.status IN (49, 50, 55, 57) THEN 1 ELSE 0 END) AS OCRMakerVolum,
            //                        SUM(CASE WHEN AH.status IN (58, 59, 60) THEN 1 ELSE 0 END) AS OCRCheckerVolum,
            //                        COUNT_BIG(DISTINCT T.c.value('(DetailValue)[1]', 'NVARCHAR(MAX)')) AS TotalVolum,  -- 统计不重复的JJ号
            //                        SUM(CASE WHEN T.c.value('.', 'NVARCHAR(MAX)') IS NOT NULL AND LTRIM(RTRIM(T.c.value('.', 'NVARCHAR(MAX)'))) <> '' THEN 1 ELSE 0 END) AS NotNullCount,
            //                        SUM(CASE WHEN AH.status IN (49, 50, 55, 57) THEN DATEDIFF(SECOND, AH.ActionStartDate, AH.ActionEndDate) ELSE 0 END) / 3600.0 AS OCRMakerHour,
            //                        SUM(CASE WHEN AH.status IN (58, 59, 60) THEN DATEDIFF(SECOND, AH.ActionStartDate, AH.ActionEndDate) ELSE 0 END) / 3600.0 AS OCRCheckerHour
            //                    FROM 
            //                        T_ActionHistory AH
            //                    INNER JOIN 
            //                        T_UserMaster UM ON AH.taskUser COLLATE Chinese_PRC_CI_AS  = UM.GUID COLLATE Chinese_PRC_CI_AS 
            //                    LEFT JOIN 
            //                        T_OrderInfo OH ON AH.fileName  COLLATE Chinese_PRC_CI_AS = OH.fileName COLLATE Chinese_PRC_CI_AS 
            //                    CROSS APPLY 
            //                        OH.filecontent.nodes('//DetailItem[DetailName=""JJ""]') AS T(c)  -- 直接定位JJ节点
            //                    WHERE 
            //                        AH.ActionEndDate IS NOT NULL
            //                        AND AH.createDate >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)  -- 当月第一天
            //                        AND AH.createDate < DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE())+1, 1)  -- 下月第一天（不含）
            //                        {whereStr.ToString()}
            //                    GROUP BY 
            //                        UM.OrganizationID, 
            //                        UM.User_Name,
            //                        DATEFROMPARTS(YEAR(AH.createDate), MONTH(AH.createDate), 1)
            //                    ORDER BY 
            //                     UM.OrganizationID, 
            //                     UM.User_Name ,ReportingDate ");
            //}
            
            // 原来是直接拼 Day/Week/Month SQL，现在统一调用
            sqlBuilder.Append(BuildOCRStatsSql(query.Type, whereStr.ToString()));

            var systemSettings = await DBCon.QueryAsync($@" SELECT [SettingID] ,[SettingKey] ,[SettingValue] ,[ValueType]
                                                      FROM [T_SystemSettings]  WHERE SettingKey in( 'standard_maker_time' ,'standard_checker_time')");
            
            var sqlStr = sqlBuilder?.ToString();

            var returnData = await DBCon.QueryAsync(sqlStr, sqlParams);

            return ApiResult<dynamic>.Ok(new { Productivity = returnData, SystemSettings = systemSettings } );
        }

        //private string BuildOCRStatsSql(string type, string whereStr)
        //{
        //    // 配置时间范围与 ReportingDate 表达式
        //    var config = type switch
        //    {
        //        "Day" => new
        //        {
        //            TimeCycle = "Daily",
        //            ReportingDate = "CAST(AH.createDate AS DATE)",
        //            Start = "CAST(GETDATE() AS DATE)",
        //            End = "DATEADD(DAY,1,CAST(GETDATE() AS DATE))"
        //        },
        //        "Week" => new
        //        {
        //            TimeCycle = "Weekly",
        //            ReportingDate = "DATEADD(DAY, 1-DATEPART(WEEKDAY, AH.createDate), CAST(AH.createDate AS DATE))",
        //            Start = "DATEADD(DAY, 1-DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE))",
        //            End = "DATEADD(DAY, 8-DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE))"
        //        },
        //        "Month" => new
        //        {
        //            TimeCycle = "Monthly",
        //            ReportingDate = "DATEFROMPARTS(YEAR(AH.createDate), MONTH(AH.createDate), 1)",
        //            Start = "DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)",
        //            End = "DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE())+1, 1)"
        //        },
        //        _ => throw new ArgumentException("Unsupported Type")
        //    };

        //    // 统一处理 whereStr，避免 UM 前缀在不同 CTE 冲突
        //    // 替换成独立条件，不带前缀，由各 CTE 自己加别名
        //    string whereCondition = string.IsNullOrWhiteSpace(whereStr) ? "" : whereStr.Replace("UM.", "");

        //    var sqlBuilder = new StringBuilder();

        //    sqlBuilder.Append($@"
        //            -- JJCount 统计（去重 + 状态 57 / 60）
        //            WITH CTE_JJStats AS (
        //                SELECT
        //                    UM.OrganizationID,
        //                    UM.User_Name,
        //                    {config.ReportingDate} AS ReportingDate,
        //                    SUM(AH.JJCount) AS TotalVolum,
        //                    SUM(CASE 
        //                            WHEN AH.JJCount IS NOT NULL AND LTRIM(RTRIM(AH.JJCount)) <> '' THEN 1 
        //                            ELSE 0 
        //                        END) AS NotNullCount
        //                FROM (
        //                    SELECT 
        //                        AH.fileName,
        //                        AH.taskUser,
        //                        AH.createDate,
        //                        AH.JJCount,
        //                        AH.status,
        //                        ROW_NUMBER() OVER (
        //                            PARTITION BY AH.fileName
        //                            ORDER BY 
        //                                CASE WHEN AH.status = 60 THEN 1 WHEN AH.status = 57 THEN 2 ELSE 3 END,
        //                                AH.createDate DESC
        //                        ) AS rn
        //                    FROM T_ActionHistory AH
        //                    WHERE AH.status IN (57, 60)
        //                      AND AH.createDate >= {config.Start}
        //                      AND AH.createDate <  {config.End}
        //                ) AS AH
        //                INNER JOIN T_UserMaster UM 
        //                    ON AH.taskUser COLLATE Chinese_PRC_CI_AS = UM.GUID COLLATE Chinese_PRC_CI_AS
        //                WHERE AH.rn = 1
        //                  {whereCondition}
        //                GROUP BY UM.OrganizationID, UM.User_Name, {config.ReportingDate}
        //            ),

        //            -- OCR 操作统计
        //            CTE_MainStats AS (
        //                SELECT 
        //                    UM.OrganizationID,
        //                    UM.User_Name,
        //                    {config.ReportingDate} AS ReportingDate,
        //                    SUM(CASE WHEN AH.status IN (49, 50, 55, 57) THEN 1 ELSE 0 END) AS OCRMakerVolum,
        //                    SUM(CASE WHEN AH.status IN (58, 59, 60) THEN 1 ELSE 0 END) AS OCRCheckerVolum,
        //                    SUM(CASE WHEN AH.status IN (49, 50, 55, 57) AND AH.ActionStartDate IS NOT NULL AND AH.ActionEndDate IS NOT NULL THEN DATEDIFF(SECOND, AH.ActionStartDate, AH.ActionEndDate) ELSE 0 END) / 60.0 AS OCRMakerHour,
        //                    SUM(CASE WHEN AH.status IN (59, 60) AND AH.ActionStartDate IS NOT NULL AND AH.ActionEndDate IS NOT NULL THEN DATEDIFF(SECOND, AH.ActionStartDate, AH.ActionEndDate) ELSE 0 END) / 60.0 AS OCRCheckerHour
        //                FROM T_ActionHistory AH
        //                INNER JOIN T_UserMaster UM 
        //                    ON AH.taskUser COLLATE Chinese_PRC_CI_AS = UM.GUID COLLATE Chinese_PRC_CI_AS
        //                WHERE 
        //                    AH.ActionEndDate IS NOT NULL
        //                    AND AH.createDate >= {config.Start}
        //                    AND AH.createDate <  {config.End}
        //                    {whereCondition}
        //                GROUP BY UM.OrganizationID, UM.User_Name, {config.ReportingDate}
        //            )

        //            -- 合并输出
        //            SELECT 
        //                COALESCE(M.OrganizationID, J.OrganizationID) AS OrganizationID,
        //                COALESCE(M.User_Name, J.User_Name) AS User_Name,
        //                COALESCE(M.ReportingDate, J.ReportingDate) AS ReportingDate,
        //                '{config.TimeCycle}' AS TimeCycle,
        //                M.OCRMakerVolum,
        //                M.OCRCheckerVolum,
        //                ISNULL(J.TotalVolum,0) AS TotalVolum,
        //                ISNULL(J.NotNullCount,0) AS NotNullCount,
        //                M.OCRMakerHour,
        //                M.OCRCheckerHour
        //            FROM CTE_MainStats M
        //            FULL OUTER JOIN CTE_JJStats J
        //                ON M.OrganizationID = J.OrganizationID 
        //               AND M.User_Name = J.User_Name 
        //               AND M.ReportingDate = J.ReportingDate
        //            ORDER BY OrganizationID, User_Name, ReportingDate;
        //        ");

        //    return sqlBuilder.ToString();
        //}

        private string BuildOCRStatsSql(string type, string whereStr)
        {
            var config = type switch
            {
                "Day" => new
                {
                    TimeCycle = "Daily",
                    ReportingDate = "CAST(AH.createDate AS DATE)",
                    Start = "CAST(GETDATE() AS DATE)",
                    End = "DATEADD(DAY,1,CAST(GETDATE() AS DATE))"
                },
                "Week" => new
                {
                    TimeCycle = "Weekly",
                    ReportingDate = "DATEADD(DAY, 1-DATEPART(WEEKDAY, AH.createDate), CAST(AH.createDate AS DATE))",
                    Start = "DATEADD(DAY, 1-DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE))",
                    End = "DATEADD(DAY, 8-DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE))"
                },
                "Month" => new
                {
                    TimeCycle = "Monthly",
                    ReportingDate = "DATEFROMPARTS(YEAR(AH.createDate), MONTH(AH.createDate), 1)",
                    Start = "DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)",
                    End = "DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE())+1, 1)"
                },
                _ => throw new ArgumentException("Unsupported Type")
            };

            string whereCondition = string.IsNullOrWhiteSpace(whereStr) ? "" : whereStr.Replace("UM.", "");

            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append($@"
        -- 基础过滤
        WITH CTE_Base AS (
            SELECT 
                AH.fileName,
                AH.taskUser,
                AH.createDate,
                AH.JJCount,
                AH.status,
                AH.ActionStartDate,
                AH.ActionEndDate
            FROM T_ActionHistory AH
            INNER JOIN T_UserMaster UM 
                ON AH.taskUser COLLATE Chinese_PRC_CI_AS = UM.GUID COLLATE Chinese_PRC_CI_AS
            WHERE AH.createDate >= {config.Start}
              AND AH.createDate <  {config.End}
              {whereCondition}
        ),

        -- 获取每个 taskUser, fileName 最新 status=57 记录
        Latest57 AS (
            SELECT 
                fileName,
                taskUser,
                createDate,
                JJCount,
                ROW_NUMBER() OVER (
                    PARTITION BY taskUser, fileName
                    ORDER BY createDate DESC
                ) AS rn
            FROM CTE_Base
            WHERE status = 57
        ),

        CTE_JJStats AS (
            SELECT
                UM.OrganizationID,
                UM.User_Name,
                {config.ReportingDate} AS ReportingDate,
                SUM(CASE WHEN AH.status = 57 AND L57.rn = 1 THEN ISNULL(AH.JJCount,0) ELSE 0 END) AS MakerJJCount,
                SUM(CASE WHEN AH.status IN (59,60) THEN ISNULL(AH.JJCount,0) ELSE 0 END) AS CheckerJJCount
            FROM CTE_Base AH
            LEFT JOIN Latest57 L57
              ON AH.taskUser = L57.taskUser AND AH.fileName = L57.fileName AND AH.createDate = L57.createDate
            INNER JOIN T_UserMaster UM 
                ON AH.taskUser COLLATE Chinese_PRC_CI_AS = UM.GUID COLLATE Chinese_PRC_CI_AS
            GROUP BY UM.OrganizationID, UM.User_Name, {config.ReportingDate}
        ),

        CTE_MainStats AS (
            SELECT 
                UM.OrganizationID,
                UM.User_Name,
                {config.ReportingDate} AS ReportingDate,
                SUM(CASE WHEN AH.status IN (49, 50, 55, 57) THEN 1 ELSE 0 END) AS OCRMakerVolum,
                SUM(CASE WHEN AH.status IN (58, 59, 60) THEN 1 ELSE 0 END) AS OCRCheckerVolum,
                SUM(CASE WHEN AH.status IN (49, 50, 55, 57) AND AH.ActionStartDate IS NOT NULL AND AH.ActionEndDate IS NOT NULL 
                         THEN DATEDIFF(SECOND, AH.ActionStartDate, AH.ActionEndDate) ELSE 0 END) / 60.0 AS OCRMakerHour,
                SUM(CASE WHEN AH.status IN (59, 60) AND AH.ActionStartDate IS NOT NULL AND AH.ActionEndDate IS NOT NULL 
                         THEN DATEDIFF(SECOND, AH.ActionStartDate, AH.ActionEndDate) ELSE 0 END) / 60.0 AS OCRCheckerHour
            FROM CTE_Base AH
            INNER JOIN T_UserMaster UM 
                ON AH.taskUser COLLATE Chinese_PRC_CI_AS = UM.GUID COLLATE Chinese_PRC_CI_AS
            WHERE AH.ActionEndDate IS NOT NULL
            GROUP BY UM.OrganizationID, UM.User_Name, {config.ReportingDate}
        )

        SELECT 
            COALESCE(M.OrganizationID, J.OrganizationID) AS OrganizationID,
            COALESCE(M.User_Name, J.User_Name) AS User_Name,
            COALESCE(M.ReportingDate, J.ReportingDate) AS ReportingDate,
            '{config.TimeCycle}' AS TimeCycle,
            M.OCRMakerVolum,
            M.OCRCheckerVolum,
            ISNULL(J.MakerJJCount,0) AS MakerJJCount,
            ISNULL(J.CheckerJJCount,0) AS CheckerJJCount,
            M.OCRMakerHour,
            M.OCRCheckerHour
        FROM CTE_MainStats M
        FULL OUTER JOIN CTE_JJStats J
            ON M.OrganizationID = J.OrganizationID 
           AND M.User_Name = J.User_Name 
           AND M.ReportingDate = J.ReportingDate
        ORDER BY OrganizationID, User_Name, ReportingDate;
    ");

            return sqlBuilder.ToString();
        }


        public async Task<ApiResult<dynamic>> GetMonthFileCount()
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append($@" SELECT 
                                        COUNT(*) AS TotalFileCount
                                  FROM 
                                        T_FilesManagement WITH (NOLOCK)
                                  WHERE 
                                        YEAR(createDate) = YEAR(GETDATE())   
                                        AND MONTH(createDate) = MONTH(GETDATE())   
                                        AND createDate <= GETDATE()
                                ");
              
            var sqlStr = sqlBuilder?.ToString();
             
            var totalFileCount =  await DBCon.QueryFirstOrDefaultAsync<int>(sqlStr);

            var baseline = await DBCon.QueryFirstOrDefaultAsync<int>($@" SELECT  [SettingValue] FROM [T_SystemSettings] WHERE SettingKey = 'baseline' ");

            var baselineRate = (totalFileCount / (baseline == 0 ? 1 : baseline)).ToString("F2");

            return ApiResult<dynamic>.Ok(new { Baseline = baseline, TotalFileCount  = totalFileCount , BaselineRate = baselineRate });
        }

        public async Task<ApiResult<dynamic>> GetDailyTotalFileCount(DashboardQuery query)
        {
            var sqlBuilder = new StringBuilder();

            var sqlParams = new DynamicParameters();

            var whereStr = new StringBuilder();
            if (query.OrganizationID.HasValue)
            {
                whereStr.Append($@" AND OrganizeID = @OrganizationID ");
                sqlParams.Add($"OrganizationID", query.OrganizationID, DbType.Int32, ParameterDirection.Input);
            }

            sqlBuilder.Append($@" SELECT 
                                        COUNT(*) AS TotalFileCount
                                    FROM 
                                        T_FilesManagement
                                    WHERE 
                                        CONVERT(DATE, createDate) = CONVERT(DATE, GETDATE())  {whereStr.ToString()};
                                ");
              
            var sqlStr = sqlBuilder?.ToString();
             
            var organizationFileCount = await DBCon.QueryFirstOrDefaultAsync<int>(sqlStr, sqlParams);
             
            var totalFileCountSql =  $@" SELECT 
                                        COUNT(*) AS TotalFileCount
                                    FROM 
                                        T_FilesManagement
                                    WHERE 
                                        CONVERT(DATE, createDate) = CONVERT(DATE, GETDATE())" ;

            var totalFileCount = await DBCon.QueryFirstOrDefaultAsync<int>(totalFileCountSql);


            return ApiResult<dynamic>.Ok(new { TotalFileCount  = totalFileCount, OrganizationFileCount = organizationFileCount });
        }

        public async Task<ApiResult<dynamic>> GetDailyCountByStatus(DashboardQuery query)
        {
            var sqlBuilder = new StringBuilder();

            var sqlParams = new DynamicParameters();

            var whereStr = new StringBuilder();
            if (query.OrganizationID.HasValue)
            {
                whereStr.Append($@" AND OrganizeID = @OrganizationID ");
                sqlParams.Add($"OrganizationID", query.OrganizationID, DbType.Int32, ParameterDirection.Input);
            }
            //sqlBuilder.Append($@" -- WaitMaker: status=40 and priority2<>10
            //                        SELECT COUNT(*) AS WaitMaker
            //                        FROM T_FilesManagement WITH (NOLOCK)
            //                        WHERE [status] = 40
            //                          AND priority2 <> 10 
            //                          {whereStr.ToString()}
            //                          AND CONVERT(DATE, createDate) = CONVERT(DATE, GETDATE());

            //                        -- MakerUrgent: status=40 and priority2=10
            //                        SELECT COUNT(*) AS MakerUrgent
            //                        FROM T_FilesManagement WITH (NOLOCK)
            //                        WHERE [status] = 40
            //                          AND priority2 = 10 
            //                          {whereStr.ToString()}
            //                          AND CONVERT(DATE, createDate) = CONVERT(DATE, GETDATE());

            //                        -- MakerComplete: status=49 or status=50
            //                        SELECT COUNT(*) AS MakerComplete
            //                        FROM T_FilesManagement WITH (NOLOCK)
            //                        WHERE ([status] = 49 OR [status] = 50) 
            //                          {whereStr.ToString()}
            //                          AND CONVERT(DATE, createDate) = CONVERT(DATE, GETDATE());

            //                        -- WaitChecker (not urgent): status=57 and NOT ('大至急','一便')
            //                        SELECT COUNT(*) AS WaitChecker
            //                        FROM T_FilesManagement fm WITH (NOLOCK)
            //                        JOIN T_OrderInfo oi WITH (NOLOCK)
            //                          ON fm.fileName COLLATE Chinese_PRC_CI_AS = oi.fileName
            //                        WHERE [status] = 57 
            //                          AND CONVERT(DATE, fm.updateDate) = CONVERT(DATE, GETDATE())
            //                          {whereStr.ToString()}
            //                          AND NOT EXISTS (
            //                              SELECT 1 FROM T_ActionHistory ah WITH (NOLOCK)
            //                              WHERE ah.fileName COLLATE Chinese_PRC_CI_AS = fm.fileName
            //                                AND ah.status = 58
            //                                AND ah.taskUser IS NOT NULL
            //                                AND ah.ActionEndDate > fm.updateDate)
            //                          AND oi.filecontent.value('(OrderInfo/Priority/HeaderValue)[1]', 'NVARCHAR(MAX)') NOT IN ('大至急','一便');

            //                        -- CheckerUrgent: status=57 and IN ('大至急','一便')
            //                        SELECT COUNT(*) AS CheckerUrgent
            //                        FROM T_FilesManagement fm WITH (NOLOCK)
            //                        JOIN T_OrderInfo oi WITH (NOLOCK)
            //                          ON fm.fileName COLLATE Chinese_PRC_CI_AS = oi.fileName
            //                        WHERE [status] = 57 
            //                          AND CONVERT(DATE, fm.updateDate) = CONVERT(DATE, GETDATE())
            //                          {whereStr.ToString()}
            //                          AND NOT EXISTS (
            //                              SELECT 1 FROM T_ActionHistory ah WITH (NOLOCK)
            //                              WHERE ah.fileName COLLATE Chinese_PRC_CI_AS = fm.fileName
            //                                AND ah.status = 58
            //                                AND ah.taskUser IS NOT NULL
            //                                AND ah.ActionEndDate > fm.updateDate)
            //                          AND oi.filecontent.value('(OrderInfo/Priority/HeaderValue)[1]', 'NVARCHAR(MAX)') IN ('大至急','一便');

            //                        -- Checking: status=58
            //                        SELECT COUNT(*) AS Checking
            //                        FROM T_FilesManagement WITH (NOLOCK)
            //                        WHERE [status] = 58 
            //                          {whereStr.ToString()}
            //                          AND CONVERT(DATE, createDate) = CONVERT(DATE, GETDATE());

            //                        -- CheckerComplete: status=60
            //                        SELECT COUNT(*) AS CheckerComplete
            //                        FROM T_FilesManagement WITH (NOLOCK)
            //                        WHERE [status] = 60 
            //                          {whereStr.ToString()}
            //                          AND CONVERT(DATE, createDate) = CONVERT(DATE, GETDATE()); ");
            sqlBuilder.Append($@"
                    -- WaitMaker
                    SELECT COUNT(*) AS WaitMaker
                    FROM T_FilesManagement WITH (NOLOCK)
                    WHERE [status] = 40
                      AND priority2 <> 10 
                      {whereStr.ToString()}
                      AND CONVERT(DATE, createDate) = CONVERT(DATE, GETDATE());

                    -- MakerUrgent
                    SELECT COUNT(*) AS MakerUrgent
                    FROM T_FilesManagement WITH (NOLOCK)
                    WHERE [status] = 40
                      AND priority2 = 10 
                      {whereStr.ToString()}
                      AND CONVERT(DATE, createDate) = CONVERT(DATE, GETDATE());

                    -- MakerComplete
                    SELECT COUNT(*) AS MakerComplete
                    FROM T_FilesManagement WITH (NOLOCK)
                    WHERE ([status] = 49 OR [status] = 50) 
                      {whereStr.ToString()}
                      AND CONVERT(DATE, createDate) = CONVERT(DATE, GETDATE());

                    ;WITH LatestAction AS (
                        SELECT t.fileName, t.Priority
                        FROM (
                            SELECT 
                                ah.fileName,
                                ah.Priority,
                                ROW_NUMBER() OVER (
                                    PARTITION BY ah.fileName 
                                    ORDER BY ah.createDate DESC
                                ) AS rn
                            FROM T_ActionHistory ah WITH (NOLOCK)
                        ) t
                        WHERE t.rn = 1
                    ),
                    CheckerStats AS (
                        SELECT
                            CASE 
                                WHEN la.Priority IN (N'大至急', N'一便') THEN 'CheckerUrgent'
                                ELSE 'WaitChecker'
                            END AS Category,
                            COUNT(*) AS TotalCount
                        FROM T_FilesManagement fm WITH (NOLOCK)
                        JOIN LatestAction la
                          ON fm.fileName COLLATE Chinese_PRC_CI_AS = la.fileName COLLATE Chinese_PRC_CI_AS
                        WHERE fm.[status] = 57 
                          AND CONVERT(DATE, fm.updateDate) = CONVERT(DATE, GETDATE())
                          {whereStr.ToString()}
                          AND NOT EXISTS (
                              SELECT 1 FROM T_ActionHistory ah2 WITH (NOLOCK)
                              WHERE ah2.fileName COLLATE Chinese_PRC_CI_AS = fm.fileName COLLATE Chinese_PRC_CI_AS
                                AND ah2.status = 58
                                AND ah2.taskUser IS NOT NULL
                                AND ah2.ActionEndDate > fm.updateDate
                          )
                        GROUP BY
                            CASE 
                                WHEN la.Priority IN (N'大至急', N'一便') THEN 'CheckerUrgent'
                                ELSE 'WaitChecker'
                            END
                    )
                    SELECT
                        SUM(CASE WHEN Category = 'WaitChecker' THEN TotalCount ELSE 0 END) AS WaitChecker,
                        SUM(CASE WHEN Category = 'CheckerUrgent' THEN TotalCount ELSE 0 END) AS CheckerUrgent
                    FROM CheckerStats;

                    -- Checking
                    SELECT COUNT(*) AS Checking
                    FROM T_FilesManagement WITH (NOLOCK)
                    WHERE [status] = 58 
                      {whereStr.ToString()}
                      AND CONVERT(DATE, createDate) = CONVERT(DATE, GETDATE());

                    -- CheckerComplete
                    SELECT COUNT(*) AS CheckerComplete
                    FROM T_FilesManagement WITH (NOLOCK)
                    WHERE [status] = 60 
                      {whereStr.ToString()}
                      AND CONVERT(DATE, createDate) = CONVERT(DATE, GETDATE());
                ");


            var sqlStr = sqlBuilder?.ToString();
            
            var multi = await DBCon.QueryMultipleAsync(sqlStr, sqlParams);

            var waitMaker = multi.Read<int>().FirstOrDefault();
            var makerUrgent = multi.Read<int>().FirstOrDefault();
            var makerComplete = multi.Read<int>().FirstOrDefault();

            // WaitChecker + CheckerUrgent 是同一条 SELECT 的两列
            var checkerStats = multi.Read<(int WaitChecker, int CheckerUrgent)>().FirstOrDefault();
            var waitChecker = checkerStats.WaitChecker;
            var checkerUrgent = checkerStats.CheckerUrgent;

            var checking = multi.Read<int>().FirstOrDefault();
            var checkerComplete = multi.Read<int>().FirstOrDefault();

            var actionCounts = new
            {
                WaitMaker = waitMaker,
                MakerComplete = makerComplete,
                WaitChecker = waitChecker,
                CheckerComplete = checkerComplete,
                MakerUrgent = makerUrgent,
                Checking = checking,
                CheckerUrgent = checkerUrgent
            };

            return ApiResult<dynamic>.Ok(actionCounts);
        }
         
        public async Task<ApiResult<IEnumerable<dynamic>>> GetDepartmentMonthCurrentYear(DepartmentMonthCurrentYearQuery query)
        {
            var sqlBuilder = new StringBuilder();

            var sqlParams = new DynamicParameters();

            //var whereStr = new StringBuilder();
            //if (query.OrganizationID.HasValue)
            //{
            //    whereStr.Append($@" AND UM.OrganizationID = @OrganizationID ");
            //    sqlParams.Add($"OrganizationID", query.OrganizationID, DbType.String, ParameterDirection.Input);
            //}

            //if (query.CreateDate.HasValue)
            //{
            //    whereStr.Append($@" AND AH.createDate >= @CreateDate ");
            //    sqlParams.Add($"CreateDate", query.CreateDate, DbType.String, ParameterDirection.Input);
            //}

            //sqlBuilder.Append($@" -- 使用递归CTE生成当年所有月份
            //                    WITH Months AS (
            //                        SELECT 
            //                            1 AS MonthNum,
            //                            DATEFROMPARTS(YEAR(GETDATE()), 1, 1) AS MonthStart,
            //                            DATEADD(MONTH, 1, DATEFROMPARTS(YEAR(GETDATE()), 1, 1)) AS MonthEnd
            //                        UNION ALL
            //                        SELECT 
            //                            MonthNum + 1,
            //                            DATEADD(MONTH, 1, MonthStart),
            //                            DATEADD(MONTH, 1, MonthEnd)
            //                        FROM 
            //                            Months
            //                        WHERE 
            //                            MonthNum < 12
            //                    ),
            //                    -- 预先计算每个月的统计数据（合并所有组织）
            //                    MonthlyStats AS (
            //                        SELECT 
            //                            MONTH(AH.createDate) AS MonthNum,
            //                            COUNT_BIG(DISTINCT AH.fileName) AS TaskCount,
            //                            COUNT_BIG(DISTINCT T.c.value('(DetailValue)[1]', 'NVARCHAR(MAX)')) AS JJCount
            //                        FROM 
            //                            T_ActionHistory AH
            //                        INNER JOIN  
            //                            T_UserMaster UM ON AH.taskUser  COLLATE Chinese_PRC_CI_AS= UM.GUID  COLLATE Chinese_PRC_CI_AS
            //                        LEFT JOIN 
            //                            T_OrderInfo OH ON AH.fileName  COLLATE Chinese_PRC_CI_AS = OH.fileName  COLLATE Chinese_PRC_CI_AS
            //                        CROSS APPLY 
            //                            OH.filecontent.nodes('//DetailItem[DetailName=""JJ""]') AS T(c)
            //                        WHERE 
            //                            AH.status IN (57, 60)
            //                            AND AH.createDate >= DATEFROMPARTS(YEAR(GETDATE()), 1, 1)
            //                            AND AH.createDate < DATEADD(YEAR, 1, DATEFROMPARTS(YEAR(GETDATE()), 1, 1))
            //                        GROUP BY 
            //                            MONTH(AH.createDate)
            //                    )
            //                    -- 生成完整的月份数据，无数据时补0
            //                    SELECT 
            //                        M.MonthNum AS Month,
            //                        'Monthly' AS TimePeriod,
            //                        ISNULL(S.TaskCount, 0) AS TaskCount,
            //                        ISNULL(S.JJCount, 0) AS JJCount
            //                    FROM 
            //                        Months M
            //                    LEFT JOIN 
            //                        MonthlyStats S ON M.MonthNum = S.MonthNum
            //                    ORDER BY 
            //                        Month ");

            //2025-07-30 11:00:00 在 SQL 内部关键表加 WITH (NOLOCK)，防止长时间锁表
            sqlBuilder.Append($@"
                        -- 使用递归CTE生成当年所有月份
                        WITH Months AS (
                            SELECT 
                                1 AS MonthNum,
                                DATEFROMPARTS(YEAR(GETDATE()), 1, 1) AS MonthStart,
                                DATEADD(MONTH, 1, DATEFROMPARTS(YEAR(GETDATE()), 1, 1)) AS MonthEnd
                            UNION ALL
                            SELECT 
                                MonthNum + 1,
                                DATEADD(MONTH, 1, MonthStart),
                                DATEADD(MONTH, 1, MonthEnd)
                            FROM 
                                Months
                            WHERE 
                                MonthNum < 12
                        ),
                        -- 聚合每个月 57 + 60 状态的 JJCount
                        MonthlyStats AS (
                            SELECT 
                                MONTH(AH.createDate) AS MonthNum,
                                COUNT_BIG(DISTINCT AH.fileName) AS TaskCount,   -- 不重复任务数
                                SUM(ISNULL(AH.JJCount,0)) AS JJCount            -- 57 和 60 的 JJCount 合计
                            FROM 
                                T_ActionHistory AH WITH (NOLOCK)
                            INNER JOIN  
                                T_UserMaster UM WITH (NOLOCK) 
                                ON AH.taskUser COLLATE Chinese_PRC_CI_AS = UM.GUID COLLATE Chinese_PRC_CI_AS
                            WHERE 
                                AH.status IN (57, 60)
                                AND AH.createDate >= DATEFROMPARTS(YEAR(GETDATE()), 1, 1)
                                AND AH.createDate < DATEADD(YEAR, 1, DATEFROMPARTS(YEAR(GETDATE()), 1, 1))
                            GROUP BY 
                                MONTH(AH.createDate)
                        )
                        -- 生成完整的月份数据，无数据补0
                        SELECT 
                            M.MonthNum AS Month,
                            'Monthly' AS TimePeriod,
                            ISNULL(S.TaskCount, 0) AS TaskCount,
                            ISNULL(S.JJCount, 0) AS JJCount
                        FROM 
                            Months M
                        LEFT JOIN 
                            MonthlyStats S ON M.MonthNum = S.MonthNum
                        ORDER BY 
                            Month
                    ");


            var sqlStr = sqlBuilder?.ToString();

            var returnData = await DBCon.QueryAsync(sqlStr, sqlParams);

            return ApiResult<IEnumerable<dynamic>>.Ok(returnData);
        }
         
        public async Task<ApiResult<IEnumerable<dynamic>>> GetDepartmentMonthCurrentYearMissRate(DepartmentMonthCurrentYearQuery query)
        {
            var sqlBuilder = new StringBuilder();

            var sqlParams = new DynamicParameters();

            //var whereStr = new StringBuilder();
            //if (query.OrganizationID.HasValue)
            //{
            //    whereStr.Append($@" AND UM.OrganizationID = @OrganizationID ");
            //    sqlParams.Add($"OrganizationID", query.OrganizationID, DbType.String, ParameterDirection.Input);
            //}

            //sqlBuilder.Append($@" 
            //                    -- 使用递归CTE生成当年所有月份
            //                    WITH Months AS (
            //                        SELECT 
            //                            1 AS MonthNum,
            //                            DATEFROMPARTS(YEAR(GETDATE()), 1, 1) AS MonthStart,
            //                            DATEADD(MONTH, 1, DATEFROMPARTS(YEAR(GETDATE()), 1, 1)) AS MonthEnd
            //                        UNION ALL
            //                        SELECT 
            //                            MonthNum + 1,
            //                            DATEADD(MONTH, 1, MonthStart),
            //                            DATEADD(MONTH, 1, MonthEnd)
            //                        FROM 
            //                            Months
            //                        WHERE 
            //                            MonthNum < 12
            //                    ),
            //                    -- 预先计算每个月的统计数据（合并所有组织）
            //                    MonthlyStats AS (
            //                        SELECT 
            //                            MONTH(AH.createDate) AS MonthNum,
            //                            COUNT_BIG(DISTINCT AH.fileName) AS TaskCount,
            //                            COUNT_BIG(DISTINCT T.c.value('(DetailValue)[1]', 'NVARCHAR(MAX)')) AS JJCount
            //                        FROM 
            //                            T_ActionHistory AH
            //                        INNER JOIN 
            //                            T_UserMaster UM ON AH.taskUser COLLATE Chinese_PRC_CI_AS = UM.GUID COLLATE Chinese_PRC_CI_AS
            //                        LEFT JOIN 
            //                            T_OrderInfo OH ON AH.fileName COLLATE Chinese_PRC_CI_AS = OH.fileName COLLATE Chinese_PRC_CI_AS
            //                        CROSS APPLY 
            //                            OH.filecontent.nodes('//DetailItem[DetailName=""JJ""]') AS T(c)
            //                        WHERE 
            //                            AH.status = 49
            //                            AND AH.createDate >= DATEFROMPARTS(YEAR(GETDATE()), 1, 1)
            //                            AND AH.createDate < DATEADD(YEAR, 1, DATEFROMPARTS(YEAR(GETDATE()), 1, 1))
            //                        GROUP BY 
            //                            MONTH(AH.createDate)
            //                    )
            //                    -- 生成完整的月份数据，无数据时补0
            //                    SELECT 
            //                        M.MonthNum AS Month,
            //                        'Monthly' AS TimePeriod,
            //                        ISNULL(S.TaskCount, 0) AS TaskCount,
            //                        ISNULL(S.JJCount, 0) AS JJCount
            //                    FROM 
            //                        Months M
            //                    LEFT JOIN 
            //                        MonthlyStats S ON M.MonthNum = S.MonthNum
            //                    ORDER BY 
            //                        Month ");
            sqlBuilder.Append($@"
                    -- 使用递归CTE生成当年所有月份
                    WITH Months AS (
                        SELECT 
                            1 AS MonthNum,
                            DATEFROMPARTS(YEAR(GETDATE()), 1, 1) AS MonthStart,
                            DATEADD(MONTH, 1, DATEFROMPARTS(YEAR(GETDATE()), 1, 1)) AS MonthEnd
                        UNION ALL
                        SELECT 
                            MonthNum + 1,
                            DATEADD(MONTH, 1, MonthStart),
                            DATEADD(MONTH, 1, MonthEnd)
                        FROM 
                            Months
                        WHERE 
                            MonthNum < 12
                    ),
                    -- 预先计算每个月的统计数据（合并所有组织）
                    MonthlyStats AS (
                        SELECT 
                            MONTH(AH.createDate) AS MonthNum,
                            COUNT_BIG(DISTINCT AH.fileName) AS TaskCount,
                            SUM(ISNULL(AH.JJCount,0)) AS JJCount
                        FROM 
                            T_ActionHistory AH WITH (NOLOCK)
                        INNER JOIN 
                            T_UserMaster UM WITH (NOLOCK) 
                            ON AH.taskUser COLLATE Chinese_PRC_CI_AS = UM.GUID COLLATE Chinese_PRC_CI_AS
                        WHERE 
                            AH.status = 49
                            AND AH.createDate >= DATEFROMPARTS(YEAR(GETDATE()), 1, 1)
                            AND AH.createDate < DATEADD(YEAR, 1, DATEFROMPARTS(YEAR(GETDATE()), 1, 1))
                        GROUP BY 
                            MONTH(AH.createDate)
                    )
                    -- 生成完整的月份数据，无数据时补0
                    SELECT 
                        M.MonthNum AS Month,
                        'Monthly' AS TimePeriod,
                        ISNULL(S.TaskCount, 0) AS TaskCount,
                        ISNULL(S.JJCount, 0) AS JJCount
                    FROM 
                        Months M
                    LEFT JOIN 
                        MonthlyStats S ON M.MonthNum = S.MonthNum
                    ORDER BY 
                        Month
                ");


            var sqlStr = sqlBuilder?.ToString();

            var returnData = await DBCon.QueryAsync(sqlStr, sqlParams);

            return ApiResult<IEnumerable<dynamic>>.Ok(returnData);
        }

        public async Task<ApiResult<IEnumerable<dynamic>>> GetNumberItemsSamePeriodLastYear(int minWeek,int maxWeek)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append($@" SELECT
                                    Year AS Year,
                                    WeekOfYear AS WeekOfYear,
                                    SUM(TotalItemCount) AS TotalItemCount,
                                    SUM(TotalWorkingHours) AS TotalWorkingHours
                                FROM
                                    T_HistoryStatistics
                                WHERE
                                    Year = YEAR(GETDATE()) -1  -- 指定年份，默认去年
                                    AND WeekOfYear BETWEEN {minWeek} AND {maxWeek}  -- 指定周次区间
                                GROUP BY
                                    Year,
                                    WeekOfYear
                                ORDER BY
                                    WeekOfYear;  -- 按周排序   
                                ");

            var sqlStr = sqlBuilder?.ToString();

            var returnData = await DBCon.QueryAsync(sqlStr);

            return ApiResult<IEnumerable<dynamic>>.Ok(returnData);
        }

        public async Task<ApiResult<IEnumerable<dynamic>>> GetAttendanceRateDepartmentCurrentMonth()
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append($@" DECLARE @CurrentDate DATE = GETDATE();
                                DECLARE @StartDate DATE = DATEADD(WEEK, 0, @CurrentDate); -- 当前周
                                DECLARE @EndDate DATE = DATEADD(WEEK, 9, @CurrentDate);   -- 向后9周

                                -- 使用更简洁的日期范围生成方式
                                WITH DateRange AS (
                                    SELECT @StartDate AS Date
                                    UNION ALL
                                    SELECT DATEADD(DAY, 1, Date)
                                    FROM DateRange
                                    WHERE Date < @EndDate
                                ),

                                Calendar AS (
                                    SELECT
                                        Date,
                                        YEAR(Date) AS Year,
                                        DATEPART(ISO_WEEK, Date) AS Week,
                                        DATEPART(WEEKDAY, Date) AS Weekday,
                                        CASE 
                                            WHEN DATEPART(WEEKDAY, Date) IN (1, 7) THEN 0  -- 周末
                                            WHEN EXISTS (
                                                SELECT 1 
                                                FROM T_JapanHolidays h 
                                                WHERE h.Year = YEAR(Date) 
                                                  AND h.HolidayDate = Date
                                            ) THEN 0  -- 日本节假日
                                            ELSE 1 
                                        END AS IsWorkingDay
                                    FROM DateRange
                                ),

                                -- 计算全体员工数
                                TotalEmployeeCount AS (
                                    SELECT
                                        COUNT(DISTINCT um.GUID) AS EmployeeCount
                                    FROM
                                        T_UserMaster um
                                    WHERE
                                        um.OrganizationID IS NOT NULL
                                ),

                                -- 计算每周工作日总数
                                WeekWorkingDays AS (
                                    SELECT
                                        Year,
                                        Week,
                                        SUM(IsWorkingDay) AS WeekWorkingDays
                                    FROM Calendar
                                    GROUP BY Year, Week
                                ),

                                -- 计算每周总休假天数（小时转换为天）
                                TotalWeekLeave AS (
                                    SELECT
                                        YEAR(a.Date) AS AttendanceYear,
                                        DATEPART(ISO_WEEK, a.Date) AS Week,
                                        SUM(ea.LeaveDays / 8.0) AS TotalLeaveDays
                                    FROM
                                        T_EmployeeAttendance ea
                                    JOIN
                                        Calendar a ON 
                                            ea.AttendanceYear = YEAR(a.Date) AND
                                            ea.AttendanceMonth = MONTH(a.Date) AND
                                            ea.AttendanceDay = DAY(a.Date)
                                    JOIN
                                        T_UserMaster um ON ea.GUID = um.GUID
                                    WHERE
                                        a.Date BETWEEN @StartDate AND @EndDate
                                        AND ea.LeaveType IN (1, 2, 3)
                                    GROUP BY
                                        YEAR(a.Date),
                                        DATEPART(ISO_WEEK, a.Date)
                                )

                                -- 主查询：统计每周整体Capacity
                                SELECT
                                    wwd.Year AS Year,
                                    wwd.Week AS Week,
                                    tec.EmployeeCount AS EmployeeCount,
                                    wwd.WeekWorkingDays AS WeekWorkingDays,
                                    ISNULL(twl.TotalLeaveDays, 0) AS TotalLeaveDays,
                                    ISNULL(twl.TotalLeaveDays * 8, 0) AS AllTotalLeaveDays,
                                    (tec.EmployeeCount * wwd.WeekWorkingDays - ISNULL(twl.TotalLeaveDays, 0)) AS ActualWorkingDays,
                                    CONVERT(DECIMAL(10, 2),
                                        -- 调整计算顺序：先计算出勤天数，再进行单位换算
                                        (6.5 * 60.0) / 8.0 * (tec.EmployeeCount * wwd.WeekWorkingDays - ISNULL(twl.TotalLeaveDays, 0))
                                    ) AS Capacity,
                                    CONVERT(DECIMAL(5, 2),
                                        CASE 
                                            WHEN tec.EmployeeCount * wwd.WeekWorkingDays > 0 THEN 
                                                (tec.EmployeeCount * wwd.WeekWorkingDays - ISNULL(twl.TotalLeaveDays, 0)) 
                                                / CAST(tec.EmployeeCount * wwd.WeekWorkingDays AS DECIMAL(10, 2)) * 100
                                            ELSE 100
                                        END
                                    ) AS AttendancePercentage
                                FROM
                                    WeekWorkingDays wwd
                                CROSS JOIN
                                    TotalEmployeeCount tec
                                LEFT JOIN
                                    TotalWeekLeave twl 
                                        ON wwd.Year = twl.AttendanceYear
                                        AND wwd.Week = twl.Week
                                ORDER BY
                                    Year,
                                    Week
                                OPTION (MAXRECURSION 366); -- 覆盖一年的最大天数
                                ");

            var sqlStr = sqlBuilder?.ToString();

            var returnData = await DBCon.QueryAsync(sqlStr);

            return ApiResult<IEnumerable<dynamic>>.Ok(returnData);
        }

        public async Task<ApiResult<IEnumerable<dynamic>>> GetDailyPendingList(DailyPendingQuery query)
        {
            var sqlBuilder = new StringBuilder();
            var sqlParams = new DynamicParameters();

            if (query.CreateDate.HasValue)
            {
                sqlBuilder.Append($@" AND CONVERT(DATE, createDate) = @CreateDate ");
                sqlParams.Add($"CreateDate", query.CreateDate, DbType.String, ParameterDirection.Input);
            }
            var whereStr = new StringBuilder();

            if (query.OrganizationID.HasValue)
            {
                whereStr.Append($@" AND OrganizeID = @OrganizationID ");
                sqlParams.Add($"OrganizationID", query.OrganizationID, DbType.Int32, ParameterDirection.Input);
            }

            sqlBuilder.Append($@" SELECT 
                                    fm.fileName,
                                    fm.filePath,
                                    fm.updateDate,
                                    fm.processMessage,
                                    um.User_Name AS taskUser,  -- 新增用户名字段
                                    fm.status,
                                    fm.OrganizeID,
                                    fm.releaseMessage,
                                    fm.priority,
                                    fm.priority2,
                                    fm.createDate
                                FROM 
                                    T_FilesManagement fm WITH (NOLOCK)
                                LEFT JOIN 
                                    T_UserMaster um WITH (NOLOCK)
                                    ON fm.taskUser COLLATE Chinese_PRC_CI_AS = um.GUID COLLATE Chinese_PRC_CI_AS
                                WHERE 
                                    fm.[status] = 55
                                        {whereStr.ToString()}
                                    ");

            var sqlStr = sqlBuilder?.ToString();
             
            var returnData = await DBCon.QueryAsync(sqlStr, sqlParams);

            return ApiResult<IEnumerable<dynamic>>.Ok(returnData);
        }
         
        public async Task<ApiResult<IEnumerable<dynamic>>> GetDailyAttentionList(DailyAttentionQuery query)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append($@" SELECT 
                                        *
                                    FROM 
                                        T_Attention WITH (NOLOCK)

                                    WHERE   CONVERT(DATE, start_time) <= CONVERT(DATE, GETDATE())
	                                    AND CONVERT(DATE, end_time) >= CONVERT(DATE, GETDATE())
                                    ");

            var sqlParams = new DynamicParameters();

            if (query.OrganizationID.HasValue)
            {
                sqlBuilder.Append($@" AND OrganizationID = @OrganizationID ");
                sqlParams.Add($"OrganizationID", query.OrganizationID, DbType.String, ParameterDirection.Input);
            } 

            var sqlStr = sqlBuilder?.ToString();
             
            var returnData = await DBCon.QueryAsync(sqlStr, sqlParams);

            return ApiResult<IEnumerable<dynamic>>.Ok(returnData);
        }


        /// <summary>
        /// 根据团队类型获取技能矩阵数据列表
        /// </summary>
        public async Task<ApiResult<IEnumerable<dynamic>>> GetSkillMatrixListByTeam(SkillMatrixQuery query)
        {
            try
            {
                // 验证团队类型
                var validTeams = new[] { "Order Processing", "Quotation Management", "Sales Support" };
                if (!validTeams.Contains(query.TeamType))
                {
                    return (ApiResult<IEnumerable<dynamic>>)ApiResult<IEnumerable<dynamic>>.Failed($"无效的团队类型。有效值：{string.Join(", ", validTeams)}");
                }

                // 构建SQL查询
                var sqlBuilder = new StringBuilder();
                var sqlParams = new DynamicParameters();

                // 根据团队类型获取对应的技能字段
                string skillFields = GetSkillFieldsByTeamType(query.TeamType);

                // 构建SELECT语句
                sqlBuilder.Append($@"
                    SELECT 
                        t.[Team],
                        t.[Group1],
                        t.[Group2],
                        u.User_name AS [Name],
                        {skillFields},
                        t.[JapaneseCertificate],
                        t.[JapaneseAbility],
                        t.[StudyAbroadExperience],
                        t.[EnglishLevel],
                        t.[CallHandlingExperience],
                        t.[KTExperience],
                        t.[DomainExperience1],
                        t.[DomainExperience2],
                        t.[ExcelSkill],
                        t.[OtherQualifications],
                        t.[OtherSkills]
                    FROM [dbo].[T_SkillMatrix] t WITH (NOLOCK)
                    INNER JOIN t_usermaster u WITH (NOLOCK) ON t.[GUID] = u.[GUID]
                    WHERE t.[Team] = @TeamType
                ");

                // 添加参数
                sqlParams.Add("@TeamType", query.TeamType, DbType.String, ParameterDirection.Input);

                // 执行查询
                var queryResult = await DBCon.QueryAsync(sqlBuilder.ToString(), sqlParams);

                // 转换技能值（数字转形状）
                var result = ConvertSkillValues(queryResult, query.TeamType);

                return ApiResult<IEnumerable<dynamic>>.Ok(result);
            }
            catch (Exception ex)
            {
                return (ApiResult<IEnumerable<dynamic>>)ApiResult<IEnumerable<dynamic>>.Failed($"查询技能矩阵数据失败：{ex.Message}");
            }
        }
        /// <summary>
        /// 根据团队类型获取对应的技能字段
        /// </summary>
        private string GetSkillFieldsByTeamType(string teamType)
        {
            return teamType switch
            {
                "Order Processing" => @"
                    [TokyoSkill],
                    [OsakaSkill],
                    [Wave10Skill],
                    [InputASkill],
                    [InputBSkill],
                    [NewSCSkill],
                    [EFAXSkill]",

                "Quotation Management" => @"
                    [ProductQuoteSkill],
                    [GPOSecondQuoteSkill],
                    [ContractQuoteSkill],
                    [MasterRegistrationSkill],
                    [CertificateSkill],
                    [MSAPaymentSkill],
                    [PostDiscountSkill]",

                "Sales Support" => @"
                    [SampleArrangementSkill],
                    [ShortTermLoanSkill],
                    [InventoryPromotionSkill],
                    [LoanerEquipmentSkill],
                    [EquipmentArrangementSkill],
                    [IndirectSalesCaseSkill],
                    [NewSCCaseSkill],
                    [DirectSalesCaseSkill]",

                _ => throw new ArgumentOutOfRangeException(nameof(teamType))
            };
        }

        /// <summary>
        /// 转换查询结果中的技能值（数字转形状）
        /// </summary>
        private IEnumerable<dynamic> ConvertSkillValues(IEnumerable<dynamic> queryResult, string teamType)
        {
            foreach (var item in queryResult)
            {
                // 转换为可修改的动态对象
                var dictionary = (IDictionary<string, object>)item;

                // 根据团队类型转换对应的技能字段
                ConvertTeamSpecificSkills(dictionary, teamType);

                yield return item;
            }
        }

        /// <summary>
        /// 根据团队类型转换特定的技能字段
        /// </summary>
        private void ConvertTeamSpecificSkills(IDictionary<string, object> item, string teamType)
        {
            switch (teamType)
            {
                case "Order Processing":
                    ConvertSkillField(item, "TokyoSkill");
                    ConvertSkillField(item, "OsakaSkill");
                    ConvertSkillField(item, "Wave10Skill");
                    ConvertSkillField(item, "InputASkill");
                    ConvertSkillField(item, "InputBSkill");
                    ConvertSkillField(item, "NewSCSkill");
                    ConvertSkillField(item, "EFAXSkill");
                    break;

                case "Quotation Management":
                    ConvertSkillField(item, "ProductQuoteSkill");
                    ConvertSkillField(item, "GPOSecondQuoteSkill");
                    ConvertSkillField(item, "ContractQuoteSkill");
                    ConvertSkillField(item, "MasterRegistrationSkill");
                    ConvertSkillField(item, "CertificateSkill");
                    ConvertSkillField(item, "MSAPaymentSkill");
                    ConvertSkillField(item, "PostDiscountSkill");
                    break;

                case "Sales Support":
                    ConvertSkillField(item, "SampleArrangementSkill");
                    ConvertSkillField(item, "ShortTermLoanSkill");
                    ConvertSkillField(item, "InventoryPromotionSkill");
                    ConvertSkillField(item, "LoanerEquipmentSkill");
                    ConvertSkillField(item, "EquipmentArrangementSkill");
                    ConvertSkillField(item, "IndirectSalesCaseSkill");
                    ConvertSkillField(item, "NewSCCaseSkill");
                    ConvertSkillField(item, "DirectSalesCaseSkill");
                    break;
            }
        }

        /// <summary>
        /// 转换单个技能字段值（数字转形状）
        /// </summary>
        private void ConvertSkillField(IDictionary<string, object> item, string fieldName)
        {
            if (item.TryGetValue(fieldName, out object value))
            {
                string stringValue = value?.ToString() ?? "";
                item[fieldName] = GetCellValue(stringValue);
            }
        }

        /// <summary>
        /// 转换技能值（1→◎，2→〇，3→△，0或其他→空）
        /// </summary>
        private string GetCellValue(string cellValue)
        {
            switch (cellValue)
            {
                case "1":
                    return "◎";
                case "2":
                    return "〇";
                case "3":
                    return "△";
                default:
                    return "";
            }
        }

    }
}
