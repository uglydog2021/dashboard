import {createRouter, createWebHistory, createWebHashHistory} from 'vue-router'
import type {RouteRecordRaw} from "vue-router"
import Login from '@/views/LoginPage.vue'
import ScreenDesign2 from '@/views/ScreenDesign2/ScreenDesign2.vue'
import MainPage from '@/views/MainPage.vue'
import FirstPage from '@/views/FirstPage.vue'
import MyInfo from '@/views/MyInfo.vue'
import Bill from '@/views/Bill.vue'
import ActionHistory from '@/views/ActionHistory.vue'
import OrderInfo from '@/views/OrderInfo.vue'
import Permission from '@/views/Permission.vue'
import Attention from '@/views/Attention.vue'
import BackData from '@/views/BackData.vue'
import BillDetail from '@/views/BillDetail.vue'
import DataBase from '@/views/DataBase.vue'
import {token} from '@/router/guard'
import User from '@/views/User.vue'
import Deployment from '@/views/Deployment.vue'
import LeaveManage from '@/views/LeaveManage.vue'
import LeaveRequest from '@/views/LeaveRequest.vue'
import LeaveRecord from '@/views/leaveRecord.vue'
import ProcessManagement from '@/views/ProcessManagement.vue'

const routes: Array<RouteRecordRaw> = [
    {
        path: '/',
        redirect: '/first-page',
    },
    {
        path: '/',
        component: MainPage, // 父路由，包含左侧菜单
        children: [
            {path: '/first-page', component: FirstPage}, // 默认子路由，首页
            {
                path: '/bill', component: Bill,
                name: 'Bill',
                meta: {
                    permission: [1, 2]
                }
            },
            {
                path: '/bill/:fileName', component: BillDetail,
                name: 'billDetail',
                meta: {
                    permission: [1, 2],
                    shouldCacheFrom: 'Bill,ActionHistory'
                }
            },
            {
                path: '/action-history', component: ActionHistory,
                name: 'ActionHistory',
                meta: {
                    permission: [1, 2],
                    KeepAlive: true
                }
            },
            {
                path: '/document', component: OrderInfo,
                meta: {
                    permission: [1, 2],
                }
            },
            {
                path: '/database', component: DataBase,
                meta: {
                    permission: [1],
                }
            },
            {
                path: '/permission', component: Permission,
                meta: {
                    permission: [1],
                }
            },
            {
                path: '/attention', component: Attention,
                meta: {
                    permission: [1],
                }
            },
            {
                path: '/backData', component: BackData,
                meta: {
                    permission: [1, 2],
                }
            },
            {path: '/user', component: User},
            {
                path: '/deployment', component: Deployment,
                meta: {
                    permission: [1],
                }
            },
            {
                path: '/uploadFile', component: LeaveManage,
                meta: {
                    permission: [1],
                }
            },
            {
                path: '/leaveRequest', component: LeaveRequest,
                meta: {
                    permission: [1],
                }
            },
            {
                path: '/leaveRecord', component: LeaveRecord,
                meta: {
                    permission: [1],
                }
            },
            {
                path: '/processManagement', component: ProcessManagement,
                meta: {
                    permission: [1],
                }
            },
        ],
    },
    {
        path: '/index',
        name: 'index',
        component: ScreenDesign2,
    },
    {
        path: '/main',
        name: 'main',
        component: MainPage
    },
    {
        path: '/index2',
        name: 'HomeView',
        component: () => import('@/views/HomeView.vue'),
        children: [
            {
                path: '/index2',
                name: 'index2',
                component: () => import('@/views/index/index.vue'),
            }
        ]
    },
    {
        path: '/login',
        name: 'Login',
        component: Login,
        meta: {
            isPublic: true,
        },
    },
    {
        path: '/first-page',
        name: 'FirstPage',
        component: FirstPage
    },
    {
        path: '/my',
        name: 'my',
        component: MyInfo
    },
    // {
    //   path: '/bill-detail/:id', // 动态路由，支持传递 ID
    //   component: BillDetail,
    //   props: true, // 将路由参数作为 props 传递给组件
    // },
]
const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes,
})

router.beforeEach(token)

export default router
