import Mock from "mockjs";
//处理路径传参
import {parameteUrl} from "@/utils/query-param"

function ArrSet(Arr: any[], id: string): any[] {
    let obj: any = {}
    const arrays = Arr.reduce((setArr, item) => {
        obj[item[id]] ? '' : (obj[item[id]] = true && setArr.push(item))
        return setArr
    }, [])
    return arrays
}

/**
 * @description: min ≤ r ≤ max  随机数
 * @param {*} Min
 * @param {*} Max
 * @return {*}
 */
function RandomNumBoth(Min: any, Max: any) {
    var Range = Max - Min;
    var Rand = Math.random();
    var num = Min + Math.round(Rand * Range); //四舍五入
    return num;
}

//左中
export default [
    // {
    //     url: "/api/login",
    //     type: "post",
    //     response: () => {
    //         const a = Mock.mock({
    //             result: '@string(50)'
    //         })
    //         return a
    //     },
    // },
    {
        url: "/api/GetDBConnection",
        type: "get",
        response: () => {
            const a = Mock.mock([
                'DB-2025-01-01',
                'DB-2025-02-01',
                'DB-2025-02-02',
                'DB-2025-03-01',
                'DB-2025-04-01',
                'DB-2025-05-01',
                'DB-2025-06-01'
                ])
            return a
        },
    },
    {
        url: "/api/GetFilesDetail",
        type: "get",
        response: () => {
            const a = Mock.mock([{
                Id:'1',
                status:'-1',
                fileName:'20241106-0276576333_81345632107_xsi-53904998_1.pdf',
                createDate:'2024-11-07 08:35:42',
                taskUser:'10099875',
                ActionStartDate:'2024-11-07 08:35:42.410',
                ActionEndDate:'2024-11-07 08:35:42.587',
                HistoryFilePath:'HistoryFilePath',
                WorkTimeH:'0',
                WorkTimeM:'0',
                WorkTimeS:'46',
                }])
            return a
        },
    },
    {
        url: "/api/GetMakerTotalCount",
        type: "get",
        response: () => {
            const a = Mock.mock([{
                        taskUser: '10113085',
                        userNameDisplay: '杨亭亭',
                        count: "@natural(1, 5)"+'',
                        miss: '@float(1, 99, 2, 2)'
                    }, {
                        taskUser: '10108655',
                        userNameDisplay: '韩姣',
                        count: "@natural(1, 5)"+'',
                        miss: '@float(1, 99, 2, 2)'
                    }, {
                        taskUser: '10112085',
                        userNameDisplay: '王明',
                        count: "@natural(1, 5)"+'',
                        miss: '@float(1, 99, 2, 2)'
                    },
                    {
                        taskUser: '10208625',
                        userNameDisplay: '丁广智',
                        count: "@natural(1, 5)"+'',
                        miss: '@float(1, 99, 2, 2)'
                    },
                    {
                        taskUser: '10108655',
                        userNameDisplay: '韩明明',
                        count: "@natural(1, 5)"+'',
                        miss: '@float(1, 99, 2, 2)'
                    },
                ]
            )
            return a
        },
    },
    {
        url: "/api/GetFilesManagement",
        type: "get",
        response: () => {
            const a = Mock.mock([{
                        fileName:'20241106-153716_88_81345632107_xsi-53884767_1.pdf',
                        // filePath:'\\10.202.194.49\smbshare\DataProcessing\\20241107',
                        updateDate:'2024-11-07 09:42:48.000',
                        // processMessage:'',
                        taskUser:'10102946',
                        status:'-1',
                        // OrganizeID:'20',
                        // releaseMessage:'',
                        // priority:'0',
                        // priority2:'0',
                        createDate:'20'
                    },
                    {
                        fileName:'20241106-153716_88_81345632107_xsi-53884767_1.pdf',
                        // filePath:'\\10.202.194.49\smbshare\DataProcessing\\20241107',
                        updateDate:'2024-11-07 09:42:48.000',
                        // processMessage:'',
                        taskUser:'10102946',
                        status:'-1',
                        // OrganizeID:'20',
                        // releaseMessage:'',
                        // priority:'0',
                        // priority2:'0',
                        createDate:'20'
                    }
                ]
            )
            return a
        },
    },
    {
        url: "/api/GetBackDataList",
        type: "get",
        response: () => {
            const a = Mock.mock([{
                        taskUser: '10113085',
                        userNameDisplay: '杨亭亭',
                        TotalMarkerAll: "@natural(100, 500)"+'',
                        TotalCheckerAll: "@natural(100, 500)"+'',
                    }, {
                        taskUser: '10108655',
                        userNameDisplay: '韩姣',
                        TotalMarkerAll: "@natural(100, 500)"+'',
                        TotalCheckerAll: "@natural(100, 500)"+'',
                    }, {
                        taskUser: '10112085',
                        userNameDisplay: '王明',
                        TotalMarkerAll: "@natural(100, 500)"+'',
                        TotalCheckerAll: "@natural(100, 500)"+'',
                    },
                    {
                        taskUser: '10208625',
                        userNameDisplay: '丁广智',
                        TotalMarkerAll: "@natural(100, 500)"+'',
                        TotalCheckerAll: "@natural(100, 500)"+'',
                    },
                    {
                        taskUser: '10108655',
                        userNameDisplay: '韩明明',
                        TotalMarkerAll: "@natural(100, 500)"+'',
                        TotalCheckerAll: "@natural(100, 500)"+'',
                    },
                ]
            )
            return a
        },
    },
    {
        url: "/api/GetDataBase",
        type: "get",
        response: () => {
            const a = Mock.mock([{
                        id: '10113085',
                        DBName: '杨亭亭',
                        Account: "@natural(100, 500)"+'',
                        Pwd: "@natural(100, 500)"+'',
                    }, {
                        id: '10108655',
                        DBName: '韩姣',
                        Account: "@natural(100, 500)"+'',
                        Pwd: "@natural(100, 500)"+'',
                    }, {
                        id: '10112085',
                        DBName: '王明',
                        Account: "@natural(100, 500)"+'',
                        Pwd: "@natural(100, 500)"+'',
                    },
                    {
                        id: '10208625',
                        DBName: '丁广智',
                        Account: "@natural(100, 500)"+'',
                        Pwd: "@natural(100, 500)"+'',
                    },
                    {
                        id: '10108655',
                        DBName: '韩明明',
                        Account: "@natural(100, 500)"+'',
                        Pwd: "@natural(100, 500)"+'',
                    },
                ]
            )
            return a
        },
    },
    {
        url: "/api/GetMakerCheckerCount",
        type: "get",
        response: () => {
            const a = Mock.mock([{
                    taskUser: '10102946',
                    userNameDisplay: '赵浩成',
                    count: "@natural(1, 5)"+'',
                    miss: '@float(1, 99, 2, 2)'
                },
                    {
                        taskUser: '10100870',
                        userNameDisplay: '刘爽',
                        count: "@natural(1, 5)"+'',
                        miss: '@float(1, 99, 2, 2)'
                    }, {
                        taskUser: '10102946',
                        userNameDisplay: '王鑫',
                        count: "@natural(1, 5)"+'',
                        miss: '@float(1, 99, 2, 2)'
                    },
                    {
                        taskUser: '10100870',
                        userNameDisplay: '李闯',
                        count: "@natural(1, 5)"+'',
                        miss: '@float(1, 99, 2, 2)'
                    }, {
                        taskUser: '10102946',
                        userNameDisplay: '查文凤',
                        count: "@natural(1, 5)"+'',
                        miss: '@float(1, 99, 2, 2)'
                    },
                    {
                        taskUser: '10100870',
                        userNameDisplay: '刘丽',
                        count: "@natural(1, 5)"+'',
                        miss: '@float(1, 99, 2, 2)'
                    }, {
                        taskUser: '10102946',
                        userNameDisplay: '王勇',
                        count: "@natural(1, 5)"+'',
                        miss: '@float(1, 99, 2, 2)'
                    },
                    {
                        taskUser: '10100870',
                        userNameDisplay: '李明卓',
                        count: "@natural(1, 5)"+'',
                        miss: '@float(1, 99, 2, 2)'
                    }, {
                        taskUser: '10102946',
                        userNameDisplay: '刘明佳',
                        count: "@natural(1, 5)"+'',
                        miss: '@float(1, 99, 2, 2)'
                    },
                    {
                        taskUser: '10100870',
                        userNameDisplay: '葛玄',
                        count: "@natural(1, 5)"+'',
                        miss: '@float(1, 99, 2, 2)'
                    },
                ]
            )
            return a
        },
    },
    {
        url: "/api/GetPermissionManagement",
        type: "get",
        response: () => {
            const a = Mock.mock([{
                        id: 1,
                        UserId: '10102946',
                        UserName: '赵浩成',
                        Department: "@natural(1, 5)"+'部门',
                        WebPermission: "@natural(1, 3)",
                        InputPermission: "@natural(1, 3)",
                        createDate: "@natural(1, 5)"+'日',
                    },
                    {
                        id: 2,
                        UserId: '10100870',
                        UserName: '刘爽',
                        Department: "@natural(1, 5)"+'部门',
                        WebPermission: "@natural(1, 3)",
                        InputPermission: "@natural(1, 3)",
                        createDate: "@natural(1, 5)"+'日',
                    }, {
                        id: 3,
                        UserId: '10102946',
                        UserName: '王鑫',
                        Department: "@natural(1, 5)"+'部门',
                        WebPermission: "@natural(1, 3)",
                        InputPermission: "@natural(1, 3)",
                        createDate: "@natural(1, 5)"+'日',
                    },
                    {
                        id: 4,
                        UserId: '10100870',
                        UserName: '李闯',
                        Department: "@natural(1, 5)"+'部门',
                        WebPermission: "@natural(1, 3)",
                        InputPermission: "@natural(1, 3)",
                        createDate: "@natural(1, 5)"+'日',
                    }, {
                        id: 5,
                        UserId: '10102946',
                        UserName: '查文凤',
                        Department: "@natural(1, 5)"+'部门',
                        WebPermission: "@natural(1, 3)",
                        InputPermission: "@natural(1, 3)",
                        createDate: "@natural(1, 5)"+'日',
                    },
                    {
                        id: 6,
                        UserId: '10100870',
                        UserName: '刘丽',
                        Department: "@natural(1, 5)"+'部门',
                        WebPermission: "@natural(1, 3)",
                        InputPermission: "@natural(1, 3)",
                        createDate: "@natural(1, 5)"+'日',
                    }, {
                        id: 7,
                        UserId: '10102946',
                        UserName: '王勇',
                        Department: "@natural(1, 5)"+'部门',
                        WebPermission: "@natural(1, 3)",
                        InputPermission: "@natural(1, 3)",
                        createDate: "@natural(1, 5)"+'日',
                    },
                    {
                        id: 8,
                        UserId: '10100870',
                        UserName: '李明卓',
                        Department: "@natural(1, 5)"+'部门',
                        WebPermission: "@natural(1, 3)",
                        InputPermission: "@natural(1, 3)",
                        createDate: "@natural(1, 5)"+'日',
                    }, {
                        id: 9,
                        UserId: '10102946',
                        UserName: '刘明佳',
                        Department: "@natural(1, 5)"+'部门',
                        WebPermission: "@natural(1, 3)",
                        InputPermission: "@natural(1, 3)",
                        createDate: "@natural(1, 5)"+'日',
                    },
                    {
                        id: 10,
                        UserId: '10100870',
                        UserName: '葛玄',
                        Department: "@natural(1, 5)"+'部门',
                        WebPermission: "@natural(1, 3)",
                        InputPermission: "@natural(1, 3)",
                        createDate: "@natural(1, 5)"+'日',
                    },
                ]
            )
            return a
        },
    },
    {
        url: "/api/GetAttention",
        type: "get",
        response: () => {
            const a = Mock.mock([{
                        id: 1,
                        Organization: "@natural(1, 5)"+'部门',
                        StartDate: "@natural(1, 5)"+'日',
                        EndDate: "@natural(1, 5)"+'日',
                        Publisher: "@natural(1, 5)"+'小红',
                    },
                    {
                        id: 2,
                        Organization: "@natural(1, 5)"+'部门',
                        StartDate: "@natural(1, 5)"+'日',
                        EndDate: "@natural(1, 5)"+'日',
                        Publisher: "@natural(1, 5)"+'小红',
                    }, {
                        id: 3,
                        Organization: "@natural(1, 5)"+'部门',
                        StartDate: "@natural(1, 5)"+'日',
                        EndDate: "@natural(1, 5)"+'日',
                        Publisher: "@natural(1, 5)"+'小红',
                    },
                    {
                        id: 4,
                        Organization: "@natural(1, 5)"+'部门',
                        StartDate: "@natural(1, 5)"+'日',
                        EndDate: "@natural(1, 5)"+'日',
                        Publisher: "@natural(1, 5)"+'小红',
                    }, {
                        id: 5,
                        Organization: "@natural(1, 5)"+'部门',
                        StartDate: "@natural(1, 5)"+'日',
                        EndDate: "@natural(1, 5)"+'日',
                        Publisher: "@natural(1, 5)"+'小红',
                    },
                    {
                        id: 6,
                        Organization: "@natural(1, 5)"+'部门',
                        StartDate: "@natural(1, 5)"+'日',
                        EndDate: "@natural(1, 5)"+'日',
                        Publisher: "@natural(1, 5)"+'小红',
                    }, {
                        id: 7,
                        Organization: "@natural(1, 5)"+'部门',
                        StartDate: "@natural(1, 5)"+'日',
                        EndDate: "@natural(1, 5)"+'日',
                        Publisher: "@natural(1, 5)"+'小红',
                    },
                    {
                        id: 8,
                        Organization: "@natural(1, 5)"+'部门',
                        StartDate: "@natural(1, 5)"+'日',
                        EndDate: "@natural(1, 5)"+'日',
                        Publisher: "@natural(1, 5)"+'小红',
                    }, {
                        id: 9,
                        Organization: "@natural(1, 5)"+'部门',
                        StartDate: "@natural(1, 5)"+'日',
                        EndDate: "@natural(1, 5)"+'日',
                        Publisher: "@natural(1, 5)"+'小红',
                    },
                    {
                        id: 10,
                        Organization: "@natural(1, 5)"+'部门',
                        StartDate: "@natural(1, 5)"+'日',
                        EndDate: "@natural(1, 5)"+'日',
                        Publisher: "@natural(1, 5)"+'小红',
                    },
                ]
            )
            return a
        },
    },
    {
        url: "/api/GetCheckerTotalCount",
        type: "get",
        response: () => {
            const a = Mock.mock([{
                        taskUser: '10100870',
                        userNameDisplay: '刘爽',
                        prod: "@natural(1, 5)"+'',
                        utilz: '@float(1, 99, 2, 2)'
                    },
                    {
                        taskUser: '10102672',
                        userNameDisplay: '王清',
                        prod: "@natural(1, 5)"+'',
                        utilz: '@float(1, 99, 2, 2)'
                    },{
                        taskUser: '10100870',
                        userNameDisplay: '黎明',
                        prod: "@natural(1, 5)"+'',
                        utilz: '@float(1, 99, 2, 2)'
                    },
                    {
                        taskUser: '10102672',
                        userNameDisplay: '戴悦',
                        prod: "@natural(1, 5)"+'',
                        utilz: '@float(1, 99, 2, 2)'
                    }
                ]
            )
            return a
        },
    },
    {
        url: "/api/GetMakerCheckerTotalCount",
        type: "get",
        response: () => {
            const a = Mock.mock([
                    {
                        week: '1',
                        userNameDisplay: '赵浩成',
                        count: "@natural(100, 500)"+'',
                        miss: '@float(1, 99, 2, 2)'
                    },
                    {
                        week: '2',
                        userNameDisplay: '安玉国',
                        count: "@natural(100, 500)"+'',
                        miss: '@float(1, 99, 2, 2)'
                    }, {
                        week: '3',
                        userNameDisplay: '韩明明',
                        count: "@natural(100, 500)"+'',
                        miss: '@float(1, 99, 2, 2)'
                    },
                    {
                        week: '4',
                        userNameDisplay: '曹操',
                        count: "@natural(100, 500)"+'',
                        miss: '@float(1, 99, 2, 2)'
                    }
                ]
            )
            return a
        },
    },
    {
        url: "/api/GetUserInfo",
        type: "get",
        response: () => {
            const a = Mock.mock([
                    {
                        id: '1',
                        UserName: '赵浩成',
                        Phone: "@natural(100, 500)"+'',
                        Email: '@float(1, 99, 2, 2)'
                    },
                    {
                        id: '2',
                        UserName: '安玉国',
                        Phone: "@natural(100, 500)"+'',
                        Email: '@float(1, 99, 2, 2)'
                    }, {
                        id: '3',
                        UserName: '韩明明',
                        Phone: "@natural(100, 500)"+'',
                        Email: '@float(1, 99, 2, 2)'
                    },
                    {
                        id: '4',
                        UserName: '曹操',
                        Phone: "@natural(100, 500)"+'',
                        Email: '@float(1, 99, 2, 2)'
                    }
                ]
            )
            return a
        },
    },
    {
        url: "/api/GetDeployment",
        type: "get",
        response: () => {
            const a = Mock.mock([
                    {
                        DeploymentId: '1',
                        DeploymentName: '赵浩成',
                        MakerCount: "@natural(100, 500)"+'',
                        CheckerCount: "@natural(100, 500)"+'',
                    },
                    {
                        DeploymentId: '2',
                        DeploymentName: '安玉国',
                        MakerCount: "@natural(100, 500)"+'',
                        CheckerCount: "@natural(100, 500)"+'',
                    }, {
                        DeploymentId: '3',
                        DeploymentName: '韩明明',
                        MakerCount: "@natural(100, 500)"+'',
                        CheckerCount: "@natural(100, 500)"+'',
                    },
                    {
                        DeploymentId: '4',
                        DeploymentName: '曹操',
                        MakerCount: "@natural(100, 500)"+'',
                        CheckerCount: "@natural(100, 500)"+'',
                    }
                ]
            )
            return a
        },
    },
    {
        url: "/api/GetLeaveRecord",
        type: "get",
        response: () => {
            const a = Mock.mock([
                    {
                        id: 1,
                        taskUser: '张杰',
                        department: "@natural(100, 500)"+'部门',
                        interval: "@natural(100, 500)"+'',
                        type: "1",
                    },
                    {
                        id: 2,
                        department: "@natural(100, 500)"+'部门',
                        taskUser: '安玉国',
                        interval: "@natural(100, 500)"+'',
                        type: "2",
                    }, {
                        id: 3,
                        department: "@natural(100, 500)"+'部门',
                        taskUser: '韩明明',
                        interval: "@natural(100, 500)"+'',
                        type: "3",
                    },
                    {
                        id: 4,
                        department: "@natural(100, 500)"+'部门',
                        taskUser: '曹操',
                        interval: "@natural(100, 500)"+'',
                        type: "4",
                    }
                ]
            )
            return a
        },
    },
    {
        url: "/api/GetStatusTotalCount",
        type: "get",
        response: () => {
            const a = Mock.mock(
                [
                    {Status: '-1', TotalCount: '@integer(1, 3)'},
                    {Status: '10', TotalCount: '@integer(1, 10)'},
                    {Status: '20', TotalCount: '@integer(1, 50)'},
                    {Status: '30', TotalCount: '@integer(1, 60)'},
                    {Status: '40', TotalCount: '@integer(1, 50)'},
                    {Status: '49', TotalCount: '@integer(1, 70)'},
                    {Status: '50', TotalCount: '@integer(1, 60)'},
                    {Status: '55', TotalCount: '@integer(1, 60)'},
                    {Status: '57', TotalCount: '@integer(1, 60)'},
                    {Status: '58', TotalCount: '@integer(1, 60)'},
                    {Status: '59', TotalCount: '@integer(1, 60)'},
                    {Status: '60', TotalCount: '@integer(3500, 4000)'},
                    {Status: '9999', TotalCount: '@integer(5000, 5100)'},
                    {Status: '9999', TotalCount: '@integer(5100, 5200)'},
                    {Status: '99990', TotalCount: '@integer(50, 60)'},
                    {Status: '99991', TotalCount: '@integer(5100, 5600)'},
                ]
            )
            return a
        },
    },
    {
        url: "/api/GetSLA",
        type: "get",
        response: () => {
            const a = Mock.mock(
                [
                    {
                        TimeRange: 'Less3Hours',
                        Count: "@natural(1, 5)"+''
                    },
                    {
                        TimeRange: 'Over3Hours',
                        Count: "@natural(1, 5)"+''
                    },
                    {
                        TimeRange: 'Over6Hours',
                        Count: "@natural(1, 5)"+''
                    },
                    {
                        TimeRange: 'Over12Hours',
                        Count: "@natural(1, 5)"+''
                    },
                    {
                        TimeRange: 'Over24Hours',
                        Count: "@natural(1, 5)"+''
                    }
                ]
            )
            return a
        },
    },
    {
        url: "/bigscreen/countUserNum",
        type: "get",
        response: () => {
            const a = Mock.mock({
                success: true,
                data: {
                    offlineNum: '@integer(50, 100)',
                    alarmNum: '@integer(20, 100)',
                    lockNum: '@integer(10, 50)',
                    totalNum: 368
                }
            })
            a.data.onlineNum = a.data.totalNum - a.data.offlineNum - a.data.lockNum - a.data.alarmNum
            return a
        },
    },
    {
        url: "/bigscreen/countDeviceNum",
        type: "get",
        response: () => {
            const a = Mock.mock({
                success: true,
                data: {
                    alarmNum: '@integer(100, 1000)',
                    offlineNum: '@integer(0, 50)',
                    totalNum: 698
                }
            })
            a.data.onlineNum = a.data.totalNum - a.data.offlineNum
            return a
        }
    },
    //左下
    {
        url: "/bigscreen/leftBottom",
        type: "get",
        response: () => {
            const a = Mock.mock({
                success: true,
                data: {
                    "list|20": [
                        {
                            provinceName: "@province()",
                            cityName: '@city()',
                            countyName: "@county()",
                            createTime: "@datetime('yyyy-MM-dd HH:mm:ss')",
                            deviceId: "6c512d754bbcd6d7cd86abce0e0cac58",
                            "gatewayno|+1": 10000,
                            "onlineState|1": [0, 1],

                        }
                    ]
                }
            })
            return a
        }
    },
    //右上
    {
        url: "/bigscreen/alarmNum",
        type: "get",
        response: () => {
            const a = Mock.mock({
                success: true,
                data: {
                    dateList: ['2021-11', '2021-12', '2022-01', '2022-02', '2022-03', "2022-04"],
                    "numList|6": [
                        '@integer(0, 1000)'
                    ],
                    "numList2|6": [
                        '@integer(0, 1000)'
                    ]
                }
            })
            return a
        }
    },
    //右中
    {
        url: "/bigscreen/ranking",
        type: "get",
        response: () => {
            let num = Mock.mock({"list|80": [{value: "@integer(50,1000)", name: "@city()"}]}).list
            //   console.log("ranking",num);
            let newNum: any = [], numObj: any = {}
            num.map((item: any) => {
                if (!numObj[item.name] && newNum.length < 8) {
                    numObj[item.name] = true
                    newNum.push(item)
                }
            })
            let arr = newNum.sort((a: any, b: any) => {
                return b.value - a.value
            })
            let a = {
                success: true,
                data: arr
            }
            return a
        }
    },
    //右下
    {
        url: "/bigscreen/rightBottom",
        type: "get",
        response: () => {
            const a = Mock.mock({
                success: true,
                data: {
                    "list|40": [{
                        alertdetail: "@csentence(5,10)",
                        "alertname|1": ["水浸告警", "各种报警"],
                        alertvalue: "@float(60, 200)",
                        createtime: "2022-04-19 08:38:33",
                        deviceid: null,
                        "gatewayno|+1": 10000,
                        phase: "A1",
                        sbInfo: "@csentence(10,18)",
                        "terminalno|+1": 100,
                        provinceName: "@province()",
                        cityName: '@city()',
                        countyName: "@county()",
                    }],

                }
            })
            return a
        }
    },
    //安装计划
    {
        url: "/bigscreen/installationPlan",
        type: "get",
        response: () => {

            let num = RandomNumBoth(26, 32);
            const a = Mock.mock({
                ["category|" + num]: ["@city()"],
                ["barData|" + num]: ["@integer(10, 100)"],
            })
            let lineData = [], rateData = [];
            for (let index = 0; index < num; index++) {
                let lineNum = Mock.mock('@integer(0, 100)') + a.barData[index]
                lineData.push(lineNum)
                let rate = a.barData[index] / lineNum;
                rateData.push((rate * 100).toFixed(0))
            }
            a.lineData = lineData
            a.rateData = rateData
            // barData category lineData rateData
            return {
                success: true,
                data: a
            }
        }
    },
    {
        url: "/bigscreen/centerMap",
        type: "get",
        response: (options: any) => {
            let params = parameteUrl(options.url)
            //不是中国的时候
            if (params.regionCode && !["china"].includes(params.regionCode)) {
                const a = Mock.mock({
                    success: true,
                    data: {
                        "dataList|100": [
                            {
                                name: "@city()",
                                value: '@integer(1, 1000)'
                            }
                        ],
                        regionCode: params.regionCode,//-代表中国
                    }
                })
                return a
            } else {
                const a = Mock.mock({
                    success: true,
                    data: {
                        "dataList|12": [
                            {
                                name: "@province()",
                                value: '@integer(1, 1100)'
                            }
                        ],
                        regionCode: 'china',
                    }
                })
                // 去重
                a.data.dataList = ArrSet(a.data.dataList, "name")
                return a
            }
        }
    }
];

