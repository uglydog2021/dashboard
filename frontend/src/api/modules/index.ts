export default {
    'leftTop':'/bigscreen/countDeviceNum',//左上
    'leftCenter':'/bigscreen/countUserNum',//左中
    "centerMap":"/bigscreen/centerMap",
    "centerBottom":"/bigscreen/installationPlan",

    'leftBottom':"/bigscreen/leftBottom", //坐下
    'rightTop':"/bigscreen/alarmNum", //报警次数
    'rightBottom':'/bigscreen/rightBottom',//右下 
    'rightCenter':'/bigscreen/ranking',// 报警排名
    'GetMakerTotalCount':'/api/dashboard/getMissCountPerformanceVolume',// 报警排名
    'GetCheckerTotalCount':'/api/dashboard/getOrganizationalForm',// 报警排名
    'getDepartmentMonthCurrentYear':'/api/dashboard/getDepartmentMonthCurrentYear',// 报警排名
    'getNumberItemsSamePeriodLastYear':'/api/dashboard/getNumberItemsSamePeriodLastYear',// 报警排名
    'GetStatusTotalCount':'/api/GetStatusTotalCount',// 报警排名
    'getMonthFileCount':'/api/dashboard/getMonthFileCount',// 报警排名
    'getDailyTotalFileCount':'/api/dashboard/getDailyTotalFileCount',// 报警排名
    'getDailyCountByStatus':'/api/dashboard/getDailyCountByStatus',// 报警排名
    'getDailyPendingList':'/api/dashboard/getDailyPendingList',// 报警排名
    'GetSLA':'/api/dashboard/getAttendanceRecord',// 报警排名
    'GetFilesManagement':'/api/filesManagement/GetFilesManagement',// 单据主表分页查询
    'GetOrderInfo':'/api/orderInfo/GetOrderInfo',
    'OrderInfoExportExcel':'/api/orderInfo/exportExcel',
    'ExportFilesManagement':'/api/filesManagement/exportExcel',// 单据主表分页查询
    'GetActionHistory':'/api/actionHistory/GetActionHistory',// GetActionHistory
    'GetActionHistoryList':'/api/actionHistory/GetActionHistoryList',// GetActionHistory
    'GetAttention':'/api/attention/GetAttention',// 获取Attention信息
    'AddAttention':'/api/attention/AddAttention',// 获取Attention信息
    'UpdateAttention':'/api/attention/UpdateAttention',// 获取Attention信息
    'RemoveAttention':'/api/attention/RemoveAttention',// 获取Attention信息
    'GetBackDataList':'/api/actionHistory/GetActionGroupUserHistory',// 获取Attention信息
    'GetPermissionManagement':'/api/userMaster/GetUserMaster',// 获取权限信息
    'CreateUserMaster':'/api/userMaster/CreateUserMaster',// 获取权限信息
    'RemoveUserMaster':'/api/userMaster/RemoveUserMaster',// 获取权限信息
    'UpdateUserMaster':'/api/userMaster/UpdateUserMaster',// 获取权限信息
    'GetDBConnection':'/api/dbConnection',// 读取数据库备份配置
    'GetFilesDetail':'/api/GetFilesDetail',// 读取数据库备份配置
    'GetDataBase':'/api/dbConnection',// 读取数据库备份配置
    'addDBConnection':'/api/dbConnection',// 读取数据库备份配置
    'GetUserInfo':'/api/GetUserInfo',// 读取数据库备份配置
    'GetDeployment':'/api/GetDeployment',// 读取数据库备份配置
    'GetLeaveRecord':'/api/GetLeaveRecord',// 读取数据库备份配置
    'GetOrg':'/api/organization/GetOrganization',// 读取数据库备份配置
    'getDailyAttentionList':'/api/dashboard/getDailyAttentionList',// 获取Attention信息
    'RemoveOrganization':'/api/organization/RemoveOrganization',// 获取Attention信息
    'AddOrganization':'/api/organization/AddOrganization',// 获取Attention信息
    'UpdateOrganization':'/api/organization/UpdateOrganization',// 获取Attention信息
    'ImportEmployeeAttendanceData':'/api/importFile/ImportEmployeeAttendanceData',// 获取Attention信息
    'ImportSkillMatrixData':'/api/importFile/ImportSkillMatrixData',// 获取Attention信息
    'ImportJapanHolidaysData':'/api/importFile/ImportJapanHolidaysData',// 获取Attention信息
}