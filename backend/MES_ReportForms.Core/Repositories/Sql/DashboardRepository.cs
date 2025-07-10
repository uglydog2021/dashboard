using Azure;
using Dapper;
using Dapper.Contrib.Extensions;
using MES_ReportForms.Core.Models;
using MES_ReportForms.Utilities;
using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

            if (query.Type == "Day")
            {
                sqlBuilder.Append($@" -- 按日统计
                                WITH TaskData1 AS (
                                    SELECT 
                                        UM.OrganizationID AS Department,
                                        UM.user_name AS UserName,
                                        CAST(AH.createDate AS DATE) AS ReportingDate,
                                        'Daily' AS TimePeriod,
                                        COUNT_BIG(DISTINCT AH.fileName) AS TaskCount,  -- 统计不同的filename数量
                                        COUNT_BIG(DISTINCT T.c.value('(DetailValue)[1]', 'NVARCHAR(MAX)')) AS JJCount
                                    FROM 
                                        T_ActionHistory AH
                                    INNER JOIN 
                                        T_UserMaster UM ON AH.taskUser COLLATE Chinese_PRC_CI_AS = UM.GUID COLLATE Chinese_PRC_CI_AS
                                    LEFT JOIN 
                                        T_OrderInfo OH ON AH.fileName COLLATE Chinese_PRC_CI_AS = OH.fileName COLLATE Chinese_PRC_CI_AS
                                    CROSS APPLY 
                                        OH.filecontent.nodes('//DetailItem[DetailName=""JJ""]') AS T(c)
                                    WHERE 
                                        AH.status = 49 
                                         {whereStr.ToString()}
                                        AND CAST(AH.createDate AS DATE) = CAST(GETDATE() AS DATE)  -- 仅统计当日数据
                                    GROUP BY 
                                        UM.OrganizationID,
                                        UM.user_name,
                                        CAST(AH.createDate AS DATE)
                                ),
                                TaskData2 AS (
                                    SELECT 
                                        UM.OrganizationID AS Department,
                                        UM.user_name AS UserName,
                                        CAST(AH.createDate AS DATE) AS ReportingDate,
                                        'Daily' AS TimePeriod,
                                        COUNT_BIG(DISTINCT AH.fileName) AS TaskCount,  -- 统计不同的filename数量
                                        COUNT_BIG(DISTINCT T.c.value('(DetailValue)[1]', 'NVARCHAR(MAX)')) AS JJCount  -- 统计不重复的JJ号
                                    FROM 
                                        T_ActionHistory AH
                                    INNER JOIN 
                                        T_UserMaster UM ON AH.taskUser COLLATE Chinese_PRC_CI_AS = UM.GUID COLLATE Chinese_PRC_CI_AS
                                    LEFT JOIN 
                                        T_OrderInfo OH ON AH.fileName COLLATE Chinese_PRC_CI_AS= OH.fileName COLLATE Chinese_PRC_CI_AS
                                    CROSS APPLY 
                                        OH.filecontent.nodes('//DetailItem[DetailName=""JJ""]') AS T(c)
                                    WHERE 
                                        AH.status IN (57, 60,58)  -- 只统计status为57或60的记录
                                         {whereStr.ToString()}
                                        AND CAST(AH.createDate AS DATE) = CAST(GETDATE() AS DATE)  -- 仅统计当日数据
                                    GROUP BY 
                                        UM.OrganizationID, 
                                        UM.user_name,
                                        CAST(AH.createDate AS DATE)
                                )
                                SELECT 
                                     COALESCE(t1.Department, t2.Department) AS Department,
                                     COALESCE(t1.UserName, t2.UserName) AS UserName,
                                     COALESCE(t1.ReportingDate, t2.ReportingDate) AS ReportingDate,
                                     'Daily' AS TimePeriod,
                                     COALESCE(t1.TaskCount, null) AS MissTaskCount,
                                     COALESCE(t1.JJCount, null) AS MissJJCount,
                                     COALESCE(t2.TaskCount, null) AS JSTaskCount, 
                                     COALESCE(t2.JJCount, null) AS JSJJCount  
                                FROM 
                                     TaskData1 t1
                                FULL OUTER JOIN 
                                     TaskData2 t2 
                                 ON 
                                     t1.Department = t2.Department
                                     AND t1.UserName = t2.UserName
                                     AND t1.ReportingDate = t2.ReportingDate
                                ORDER BY 
                                     Department, UserName, ReportingDate; ");
            }
            else if (query.Type == "Week")
            {
                sqlBuilder.Append($@"  
                                WITH TaskData1 AS (
                                    SELECT 
                                        UM.OrganizationID AS Department,
                                        UM.user_name AS UserName,
                                        DATEADD(DAY, 1-DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE)) AS ReportingDate,
                                        'Weekly' AS TimePeriod,
                                        COUNT_BIG(DISTINCT AH.fileName) AS TaskCount,  -- 统计不同的filename数量
                                        COUNT_BIG(DISTINCT T.c.value('(DetailValue)[1]', 'NVARCHAR(MAX)')) AS JJCount
                                    FROM 
                                        T_ActionHistory AH
                                    INNER JOIN 
                                        T_UserMaster UM ON AH.taskUser COLLATE Chinese_PRC_CI_AS = UM.GUID COLLATE Chinese_PRC_CI_AS
                                    LEFT JOIN 
                                        T_OrderInfo OH ON AH.fileName COLLATE Chinese_PRC_CI_AS = OH.fileName COLLATE Chinese_PRC_CI_AS
                                    CROSS APPLY 
                                        OH.filecontent.nodes('//DetailItem[DetailName=""JJ""]') AS T(c)
                                    WHERE 
                                        AH.status = 49
                                        {whereStr.ToString()}
                                        AND AH.createDate >= DATEADD(DAY, 1-DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE))  -- 本周一
                                        AND AH.createDate < DATEADD(DAY, 8-DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE))   -- 下周一（不包含）
                                    GROUP BY 
                                        UM.OrganizationID,
                                        UM.user_name 
 
                                ),
                                TaskData2 AS (
                                    SELECT 
                                        UM.OrganizationID AS Department,
                                        UM.user_name AS UserName,
                                        DATEADD(DAY, 1-DATEPART(WEEKDAY, AH.createDate), CAST(AH.createDate AS DATE)) AS ReportingDate,
                                        'Weekly' AS TimePeriod,
                                        COUNT_BIG(DISTINCT AH.fileName) AS TaskCount,  -- 统计不同的filename数量
                                        COUNT_BIG(DISTINCT T.c.value('(DetailValue)[1]', 'NVARCHAR(MAX)')) AS JJCount  -- 统计不重复的JJ号
                                    FROM 
                                        T_ActionHistory AH
                                    INNER JOIN 
                                        T_UserMaster UM ON AH.taskUser COLLATE Chinese_PRC_CI_AS = UM.GUID COLLATE Chinese_PRC_CI_AS
                                    LEFT JOIN 
                                        T_OrderInfo OH ON AH.fileName  COLLATE Chinese_PRC_CI_AS= OH.fileName COLLATE Chinese_PRC_CI_AS
                                    CROSS APPLY 
                                        OH.filecontent.nodes('//DetailItem[DetailName=""JJ""]') AS T(c)
                                    WHERE 
                                        AH.status IN (57, 60,58)  -- 只统计status为57或60的记录
                                       {whereStr.ToString()}
                                        AND AH.createDate >= DATEADD(DAY, 1-DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE))  -- 本周一
                                        AND AH.createDate < DATEADD(DAY, 8-DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE))   -- 下周一（不包含）
                                    GROUP BY 
                                        UM.OrganizationID, 
                                        UM.user_name,
                                        DATEADD(DAY, 1-DATEPART(WEEKDAY, AH.createDate), CAST(AH.createDate AS DATE)) 
 
                                )
                                SELECT 
                                     COALESCE(t1.Department, t2.Department) AS Department,
                                     COALESCE(t1.UserName, t2.UserName) AS UserName,
                                     COALESCE(t1.ReportingDate, t2.ReportingDate) AS ReportingDate,
                                     'Weekly' AS TimePeriod,
                                     COALESCE(t1.TaskCount, null) AS MissTaskCount,
                                     COALESCE(t1.JJCount, null) AS MissJJCount,
                                     COALESCE(t2.TaskCount, null) AS JSTaskCount, 
                                     COALESCE(t2.JJCount, null) AS JSJJCount  
                                FROM 
                                     TaskData1 t1
                                FULL OUTER JOIN 
                                     TaskData2 t2 
                                 ON 
                                     t1.Department = t2.Department
                                     AND t1.UserName = t2.UserName
                                     AND t1.ReportingDate = t2.ReportingDate
                                ORDER BY 
                                     Department, UserName, ReportingDate;");
                 
            }
            else if (query.Type == "Month")
            {
                sqlBuilder.Append($@"  
                                WITH TaskData1 AS (
                                   SELECT 
                                    UM.OrganizationID AS Department,
                                    UM.user_name AS UserName,
                                    DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1) AS ReportingDate,
                                    'Monthly' AS TimePeriod,
                                    COUNT_BIG(DISTINCT AH.fileName) AS TaskCount,  -- 统计不同的filename数量
                                    COUNT_BIG(DISTINCT T.c.value('(DetailValue)[1]', 'NVARCHAR(MAX)')) AS JJCount
                                FROM 
                                    T_ActionHistory AH
                                INNER JOIN 
                                    T_UserMaster UM ON AH.taskUser COLLATE Chinese_PRC_CI_AS = UM.GUID COLLATE Chinese_PRC_CI_AS
                                LEFT JOIN 
                                    T_OrderInfo OH ON AH.fileName COLLATE Chinese_PRC_CI_AS = OH.fileName COLLATE Chinese_PRC_CI_AS
                                CROSS APPLY 
                                    OH.filecontent.nodes('//DetailItem[DetailName=""JJ""]') AS T(c)
                                WHERE 
                                    AH.status = 49
                                    {whereStr.ToString()}
                                    AND AH.createDate >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)  -- 当月第一天
                                    AND AH.createDate < DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE())+1, 1)  -- 下月第一天（不包含）
                                GROUP BY 
                                    UM.OrganizationID,
                                    UM.user_name
 
                                ),
                                TaskData2 AS (
                                   SELECT 
                                        UM.OrganizationID AS Department,
                                        UM.user_name AS UserName,
                                        DATEFROMPARTS(YEAR(AH.createDate), MONTH(AH.createDate), 1) AS ReportingDate,
                                        'Monthly' AS TimePeriod,
                                        COUNT_BIG(DISTINCT AH.fileName) AS TaskCount,  -- 统计不同的filename数量
                                        COUNT_BIG(DISTINCT T.c.value('(DetailValue)[1]', 'NVARCHAR(MAX)')) AS JJCount  -- 统计不重复的JJ号
                                    FROM 
                                        T_ActionHistory AH
                                    INNER JOIN 
                                        T_UserMaster UM ON AH.taskUser COLLATE Chinese_PRC_CI_AS = UM.GUID COLLATE Chinese_PRC_CI_AS
                                    LEFT JOIN 
                                        T_OrderInfo OH ON AH.fileName COLLATE Chinese_PRC_CI_AS= OH.fileName COLLATE Chinese_PRC_CI_AS
                                    CROSS APPLY 
                                        OH.filecontent.nodes('//DetailItem[DetailName=""JJ""]') AS T(c)
                                    WHERE 
                                        AH.status IN (57, 60,58)  -- 只统计status为57或60的记录
                                        {whereStr.ToString()}
                                    AND AH.createDate >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)  -- 当月第一天
                                    AND AH.createDate < DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE())+1, 1)  -- 下月第一天（不包含）
                                    GROUP BY 
                                        UM.OrganizationID, 
                                        UM.user_name,
                                        DATEFROMPARTS(YEAR(AH.createDate), MONTH(AH.createDate), 1)
 
                                )

                                SELECT 
                                     COALESCE(t1.Department, t2.Department) AS Department,
                                     COALESCE(t1.UserName, t2.UserName) AS UserName,
                                     COALESCE(t1.ReportingDate, t2.ReportingDate) AS ReportingDate,
                                     'Monthly' AS TimePeriod,
                                     COALESCE(t1.TaskCount, null) AS MissTaskCount,
                                     COALESCE(t1.JJCount, null) AS MissJJCount,
                                     COALESCE(t2.TaskCount, null) AS JSTaskCount, 
                                     COALESCE(t2.JJCount, null) AS JSJJCount  
                                FROM 
                                     TaskData1 t1
                                FULL OUTER JOIN 
                                     TaskData2 t2 
                                 ON 
                                     t1.Department = t2.Department
                                     AND t1.UserName = t2.UserName
                                     AND t1.ReportingDate = t2.ReportingDate
                                ORDER BY 
                                     Department, UserName, ReportingDate; ");
                 
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
             
            if (query.Type == "Day")
            {
                sqlBuilder.Append($@" -- 按日统计
                                    SELECT
                                        um.OrganizationID AS OrganizationID,
                                        um.User_Name AS User_Name,
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
                                    ORDER BY
                                        ReportingDate DESC,
                                        User_Name;
                                 ");
            }
            else if (query.Type == "Week")
            {
                sqlBuilder.Append($@" -- 按周统计（以周一为周起始）
                                    SELECT
                                        um.OrganizationID AS OrganizationID,
                                        um.User_Name AS User_Name,
                                        CONCAT(YEAR(ulr.LoginTime), '-W', DATEPART(ISO_WEEK, ulr.LoginTime)) AS ReportingDate,
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
                                        DATEPART(ISO_WEEK, ulr.LoginTime)
                                    ORDER BY
                                        ReportingDate DESC,
                                        User_Name;");
            }
            else if (query.Type == "Month")
            {
                sqlBuilder.Append($@"  -- 按月统计（指定部门）
                                    SELECT
                                        um.OrganizationID AS OrganizationID,
                                        um.User_Name AS User_Name,
                                        CONCAT(YEAR(ulr.LoginTime), '-', RIGHT('0' + CAST(MONTH(ulr.LoginTime) AS VARCHAR(2)), 2)) AS ReportingDate,
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
                                    ORDER BY
                                        ReportingDate DESC,
                                        User_Name; ");
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
                whereStr.Append($@" AND UM.OrganizationID = @OrganizationID ");
                sqlParams.Add($"OrganizationID", query.OrganizationID, DbType.String, ParameterDirection.Input);
            }
             
            if (query.Type == "Day")
            {
                sqlBuilder.Append($@" -- 按日统计
                                SELECT 
                                    UM.OrganizationID ,
                                    UM.User_Name ,
                                    CAST(AH.createDate AS DATE) AS ReportingDate,
                                    'Daily' AS TimeCycle,
                                    SUM(CASE WHEN AH.status IN (49, 50, 55, 57) THEN 1 ELSE 0 END) AS OCRMakerVolum,
                                    SUM(CASE WHEN AH.status IN (58, 59, 60) THEN 1 ELSE 0 END) AS OCRCheckerVolum,
                                    COUNT_BIG(DISTINCT T.c.value('(DetailValue)[1]', 'NVARCHAR(MAX)')) AS TotalVolum,  -- 统计不重复的JJ号
                                    SUM(CASE WHEN T.c.value('.', 'NVARCHAR(MAX)') IS NOT NULL AND LTRIM(RTRIM(T.c.value('.', 'NVARCHAR(MAX)'))) <> '' THEN 1 ELSE 0 END) AS NotNullCount,
                                    SUM(CASE WHEN AH.status IN (49, 50, 55, 57) THEN DATEDIFF(SECOND, AH.ActionStartDate, AH.ActionEndDate) ELSE 0 END) / 3600.0 AS OCRMakerHour,
                                    SUM(CASE WHEN AH.status IN (58, 59, 60) THEN DATEDIFF(SECOND, AH.ActionStartDate, AH.ActionEndDate) ELSE 0 END) / 3600.0 AS OCRCheckerHour
                                FROM 
                                    T_ActionHistory AH
                                INNER JOIN 
                                    T_UserMaster UM ON AH.taskUser COLLATE Chinese_PRC_CI_AS = UM.GUID COLLATE Chinese_PRC_CI_AS
                                LEFT JOIN 
                                    T_OrderInfo OH ON AH.fileName COLLATE Chinese_PRC_CI_AS = OH.fileName COLLATE Chinese_PRC_CI_AS
                                CROSS APPLY 
                                    OH.filecontent.nodes('//DetailItem[DetailName=""JJ""]') AS T(c)  -- 直接定位JJ节点
                                WHERE  
                                    AH.ActionEndDate IS NOT NULL  -- 确保时间统计有效性
                                    AND CAST(AH.createDate AS DATE) = CAST(GETDATE() AS DATE)  -- 当日数据
                                    {whereStr.ToString()}
                                GROUP BY 
                                    UM.OrganizationID, 
                                    UM.User_Name,
                                    CAST(AH.createDate AS DATE)
                                ORDER BY 
	                                UM.OrganizationID, 
	                                UM.User_Name,ReportingDate
                                 ");
            }
            else if (query.Type == "Week")
            {
                sqlBuilder.Append($@"  -- 按周统计（以周一为周起始）
                                SELECT
                                    UM.OrganizationID ,
                                    UM.User_Name ,
                                    DATEADD(DAY, 1-DATEPART(WEEKDAY, AH.createDate), CAST(AH.createDate AS DATE)) AS ReportingDate,
                                    'Weekly' AS TimeCycle,
                                    SUM(CASE WHEN AH.status IN (49, 50, 55, 57) THEN 1 ELSE 0 END) AS OCRMakerVolum,
                                    SUM(CASE WHEN AH.status IN (58, 59, 60) THEN 1 ELSE 0 END) AS OCRCheckerVolum,
                                    COUNT_BIG(DISTINCT T.c.value('(DetailValue)[1]', 'NVARCHAR(MAX)')) AS TotalVolum,  -- 统计不重复的JJ号
                                    SUM(CASE WHEN T.c.value('.', 'NVARCHAR(MAX)') IS NOT NULL AND LTRIM(RTRIM(T.c.value('.', 'NVARCHAR(MAX)'))) <> '' THEN 1 ELSE 0 END) AS NotNullCount,
                                    SUM(CASE WHEN AH.status IN (49, 50, 55, 57) THEN DATEDIFF(SECOND, AH.ActionStartDate, AH.ActionEndDate) ELSE 0 END) / 3600.0 AS OCRMakerHour,
                                    SUM(CASE WHEN AH.status IN (58, 59, 60) THEN DATEDIFF(SECOND, AH.ActionStartDate, AH.ActionEndDate) ELSE 0 END) / 3600.0 AS OCRCheckerHour
                                FROM 
                                    T_ActionHistory AH
                                INNER JOIN 
                                    T_UserMaster UM ON AH.taskUser COLLATE Chinese_PRC_CI_AS  = UM.GUID COLLATE Chinese_PRC_CI_AS 
                                LEFT JOIN 
                                    T_OrderInfo OH ON AH.fileName COLLATE Chinese_PRC_CI_AS  = OH.fileName COLLATE Chinese_PRC_CI_AS 
                                CROSS APPLY 
                                    OH.filecontent.nodes('//DetailItem[DetailName=""JJ""]') AS T(c)  -- 直接定位JJ节点
                                WHERE
                                    AH.ActionEndDate IS NOT NULL
                                    AND AH.createDate >= DATEADD(DAY, 1-DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE))  -- 本周一
                                    AND AH.createDate < DATEADD(DAY, 8-DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE))   -- 下周一（不含）
                                    {whereStr.ToString()}
                                GROUP BY 
                                    UM.OrganizationID, 
                                    UM.User_Name,
                                    DATEADD(DAY, 1-DATEPART(WEEKDAY, AH.createDate), CAST(AH.createDate AS DATE))
                                ORDER BY 
	                                UM.OrganizationID, 
	                                UM.User_Name,ReportingDate ");
            }
            else if (query.Type == "Month")
            {
                sqlBuilder.Append($@" -- 按月统计
                                SELECT 
                                    UM.OrganizationID ,
                                    UM.User_Name ,
                                    DATEFROMPARTS(YEAR(AH.createDate), MONTH(AH.createDate), 1) AS ReportingDate,
                                    'Monthly' AS TimeCycle,
                                    SUM(CASE WHEN AH.status IN (49, 50, 55, 57) THEN 1 ELSE 0 END) AS OCRMakerVolum,
                                    SUM(CASE WHEN AH.status IN (58, 59, 60) THEN 1 ELSE 0 END) AS OCRCheckerVolum,
                                    COUNT_BIG(DISTINCT T.c.value('(DetailValue)[1]', 'NVARCHAR(MAX)')) AS TotalVolum,  -- 统计不重复的JJ号
                                    SUM(CASE WHEN T.c.value('.', 'NVARCHAR(MAX)') IS NOT NULL AND LTRIM(RTRIM(T.c.value('.', 'NVARCHAR(MAX)'))) <> '' THEN 1 ELSE 0 END) AS NotNullCount,
                                    SUM(CASE WHEN AH.status IN (49, 50, 55, 57) THEN DATEDIFF(SECOND, AH.ActionStartDate, AH.ActionEndDate) ELSE 0 END) / 3600.0 AS OCRMakerHour,
                                    SUM(CASE WHEN AH.status IN (58, 59, 60) THEN DATEDIFF(SECOND, AH.ActionStartDate, AH.ActionEndDate) ELSE 0 END) / 3600.0 AS OCRCheckerHour
                                FROM 
                                    T_ActionHistory AH
                                INNER JOIN 
                                    T_UserMaster UM ON AH.taskUser COLLATE Chinese_PRC_CI_AS  = UM.GUID COLLATE Chinese_PRC_CI_AS 
                                LEFT JOIN 
                                    T_OrderInfo OH ON AH.fileName  COLLATE Chinese_PRC_CI_AS = OH.fileName COLLATE Chinese_PRC_CI_AS 
                                CROSS APPLY 
                                    OH.filecontent.nodes('//DetailItem[DetailName=""JJ""]') AS T(c)  -- 直接定位JJ节点
                                WHERE 
                                    AH.ActionEndDate IS NOT NULL
                                    AND AH.createDate >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)  -- 当月第一天
                                    AND AH.createDate < DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE())+1, 1)  -- 下月第一天（不含）
                                    {whereStr.ToString()}
                                GROUP BY 
                                    UM.OrganizationID, 
                                    UM.User_Name,
                                    DATEFROMPARTS(YEAR(AH.createDate), MONTH(AH.createDate), 1)
                                ORDER BY 
	                                UM.OrganizationID, 
	                                UM.User_Name ,ReportingDate ");
            }

            var systemSettings = await DBCon.QueryAsync($@" SELECT [SettingID] ,[SettingKey] ,[SettingValue] ,[ValueType]
                                                      FROM [T_SystemSettings]  WHERE SettingKey in( 'standard_maker_time' ,'standard_checker_time')");
            
            var sqlStr = sqlBuilder?.ToString();

            var returnData = await DBCon.QueryAsync(sqlStr, sqlParams);

            return ApiResult<dynamic>.Ok(new { Productivity = returnData, SystemSettings = systemSettings } );
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

        public async Task<ApiResult<dynamic>> GetDailyTotalFileCount()
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append($@" SELECT 
                                        COUNT(*) AS TotalFileCount
                                    FROM 
                                        T_FilesManagement
                                    WHERE 
                                        CONVERT(DATE, createDate) = CONVERT(DATE, GETDATE());
                                ");
              
            var sqlStr = sqlBuilder?.ToString();
             
            var totalFileCount =  await DBCon.QueryFirstOrDefaultAsync<int>(sqlStr);
             
            return ApiResult<dynamic>.Ok(new { TotalFileCount  = totalFileCount });
        }

        public async Task<ApiResult<dynamic>> GetDailyCountByStatus(Dashboard6Query query)
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
                                    COUNT(*) AS WaitMaker
                                FROM 
                                    T_FilesManagement WITH (NOLOCK)
                                WHERE 
	                                [status] = 40 
									{whereStr.ToString()}
	                                AND CONVERT(DATE, createDate) = CONVERT(DATE, GETDATE());

	                                SELECT 
                                    COUNT(*) AS MakerComplete
                                FROM 
                                    T_FilesManagement WITH (NOLOCK)
                                WHERE 
	                                [status] = 57 
									{whereStr.ToString()}
	                                AND CONVERT(DATE, createDate) = CONVERT(DATE, GETDATE());
	 SELECT 
                                    COUNT(*) AS WaitChecker
                                FROM 
                                    T_FilesManagement fm WITH (NOLOCK)
                                WHERE 
	                                [status] = 57 
									{whereStr.ToString()}
	                                AND CONVERT(DATE, createDate) = CONVERT(DATE, GETDATE())
	AND NOT EXISTS (
	        SELECT 1 
	        FROM T_ActionHistory ah WITH (NOLOCK)
	        WHERE 
	             ah.fileName COLLATE Chinese_PRC_CI_AS = fm.fileName
	            AND ah.status =58
	            AND ah.taskUser IS NOT NULL    )
	;
	
	                                SELECT 
                                    COUNT(*) AS CheckerComplete
                                FROM 
                                    T_FilesManagement WITH (NOLOCK)
                                WHERE 
	                                [status] = 60
									{whereStr.ToString()}
	                                AND CONVERT(DATE, createDate) = CONVERT(DATE, GETDATE());

	                     SELECT
	                                 COUNT(*)  AS MakerUrgent 
                                FROM 
	                                T_FilesManagement fm WITH (NOLOCK)
                                WHERE 
	                                priority2 = 20
									{whereStr.ToString()}
	                                AND CONVERT(DATE, createDate) = CONVERT(DATE, GETDATE()) 
	 AND NOT EXISTS (
	        SELECT 1 
	        FROM T_ActionHistory ah WITH (NOLOCK)
	        WHERE 
	             ah.fileName COLLATE Chinese_PRC_CI_AS = fm.fileName
	            AND ah.status > 40
	            AND ah.taskUser IS NOT NULL    );   
	
	                      SELECT 
	                                COUNT(DISTINCT OH.filename) AS CheckerUrgent 
                                FROM 
                                    T_OrderInfo OH WITH (NOLOCK)
                                WHERE 
	                                filecontent.value('(OrderInfo/Priority/HeaderValue)[1]', 'NVARCHAR(MAX)')  = '大至急'
	AND CONVERT(DATE, createTime) = CONVERT(DATE, GETDATE()) 
	AND NOT EXISTS (
	SELECT 1 
	        FROM T_ActionHistory ah WITH (NOLOCK)
	        WHERE 
	             ah.fileName COLLATE Chinese_PRC_CI_AS = OH.fileName
	            AND ah.status = 60
	            AND ah.taskUser IS NOT NULL   
	 ); 
                                
            ");
              
            var sqlStr = sqlBuilder?.ToString();
             
            var multi = await DBCon.QueryMultipleAsync(sqlStr, sqlParams);
             
            var waitMaker = multi.Read<int>().FirstOrDefault();
            var makerComplete = multi.Read<int>().FirstOrDefault();
            var waitChecker = multi.Read<int>().FirstOrDefault();
            var checkerComplete = multi.Read<int>().FirstOrDefault();
            var makerUrgent = multi.Read<int>().FirstOrDefault();
            var checkerUrgent = multi.Read<int>().FirstOrDefault();

            var actionCounts = new
            {
                WaitMaker = waitMaker,
                MakerComplete = makerComplete,
                WaitChecker = waitChecker,
                CheckerComplete = checkerComplete,
                MakerUrgent = makerUrgent,
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

            sqlBuilder.Append($@" -- 使用递归CTE生成当年所有月份
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
                                        COUNT_BIG(DISTINCT T.c.value('(DetailValue)[1]', 'NVARCHAR(MAX)')) AS JJCount
                                    FROM 
                                        T_ActionHistory AH
                                    INNER JOIN 
                                        T_UserMaster UM ON AH.taskUser  COLLATE Chinese_PRC_CI_AS= UM.GUID  COLLATE Chinese_PRC_CI_AS
                                    LEFT JOIN 
                                        T_OrderInfo OH ON AH.fileName  COLLATE Chinese_PRC_CI_AS = OH.fileName  COLLATE Chinese_PRC_CI_AS
                                    CROSS APPLY 
                                        OH.filecontent.nodes('//DetailItem[DetailName=""JJ""]') AS T(c)
                                    WHERE 
                                        AH.status IN (57, 60)
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
                                    Month ");

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
                                        COUNT_BIG(DISTINCT T.c.value('(DetailValue)[1]', 'NVARCHAR(MAX)')) AS JJCount
                                    FROM 
                                        T_ActionHistory AH
                                    INNER JOIN 
                                        T_UserMaster UM ON AH.taskUser COLLATE Chinese_PRC_CI_AS = UM.GUID COLLATE Chinese_PRC_CI_AS
                                    LEFT JOIN 
                                        T_OrderInfo OH ON AH.fileName COLLATE Chinese_PRC_CI_AS = OH.fileName COLLATE Chinese_PRC_CI_AS
                                    CROSS APPLY 
                                        OH.filecontent.nodes('//DetailItem[DetailName=""JJ""]') AS T(c)
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
                                    Month ");

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
                                    Year = YEAR(GETDATE())  -- 指定年份，默认当前年
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

            sqlBuilder.Append($@" SELECT 
                                        *
                                    FROM 
                                        T_FilesManagement WITH (NOLOCK)
                                    WHERE 
	                                    [status] = 55
                                    ");

            var sqlParams = new DynamicParameters();
            
            if (query.CreateDate.HasValue)
            {
                sqlBuilder.Append($@" AND CONVERT(DATE, createDate) = @CreateDate ");
                sqlParams.Add($"CreateDate", query.CreateDate, DbType.String, ParameterDirection.Input);
            }

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
    }
}
