<template>
  <!-- 外框、设置背景图 -->
  <div class="containerSelf">
    <!-- 头部通栏 -->
    <div id="item88b51490-3e05-11ec-a2e6-953464dcce14">
      <!-- 头部标题 -->
      <!-- 组件4ec4a890-3dec-11ec-a2e6-953464dcce14开始-->
      <div id="item4ec4a890-3dec-11ec-a2e6-953464dcce14" class="centertitle">
         <h1>{{headerTitle}}</h1>
      </div>
      <!-- 组件4ec4a890-3dec-11ec-a2e6-953464dcce14结束-->

      <!-- 头部通栏背景图 -->
      <!-- 组件139cfda0-3dea-11ec-a2e6-953464dcce14开始-->
      <div id="item139cfda0-3dea-11ec-a2e6-953464dcce14">
        <img src="../../assets/images/top.png" alt="" :style="{ width: '100%', height: '50%', marginTop: '40px' }"/>
      </div>
      <!-- 组件139cfda0-3dea-11ec-a2e6-953464dcce14结束-->
    </div>
    <!-- 左边三个控件 -->
    <div id="item8389fda0-3e05-11ec-a2e6-953464dcce14">
      <!-- 左上组件 -->
      <left-top/>

      <!-- 左中组件 -->
      <left-center ref="leftCenterComponent2"/>

      <!-- 左下组件 -->
      <left-bottom ref="leftBottomComponent2"/>
    </div>

    <!-- 中间 -->
    <div id="itemc2579760-3e0d-11ec-9df4-83999b721ef6">
      <!-- 中间组件 -->
      <center-content ref="cnetentComponent2"/>
    </div>

    <!-- 右边三个控件 -->
    <div id="item7a111060-3e05-11ec-a2e6-953464dcce14">
      <!-- 右上组件 -->
      <right-top ref="rightTopComponent2"/>

      <!-- 右中组件 -->
      <right-center/>

      <!-- 右下组件 -->
      <right-bottom/>
    </div>
  </div>
  <!-- <el-dialog
      v-model="configDialogShow"
      title="设置数据库和时间区间"
      width="500"
      align-center
      :close-on-click-modal="false"
      :close-on-press-escape="false"
      :show-close="false"
  >
    <el-form ref="configRef" :model="configData" :rules="rules" label-width="auto">
      <el-form-item label="Database List:" prop="databaseName">
        <el-select v-model="configData.databaseName" placeholder="please select database" style="width: 100%">
          <el-option label="Zone one" value="shanghai"/>
          <el-option label="Zone two" value="beijing"/>
        </el-select>
      </el-form-item>
      <el-form-item label="Date Range:" prop="dateArr">
        <el-date-picker
            v-model="configData.dateArr"
            type="daterange"
            start-placeholder="Start Date"
            end-placeholder="End Date"
        />
      </el-form-item>
    </el-form>
    <template #footer>
      <div class="dialog-footer">
        <el-button type="primary" @click="setDataConfig(configRef!)">
          確定
        </el-button>
      </div>
    </template>
  </el-dialog> -->
</template>

<script setup lang="ts">
import {storeToRefs} from "pinia";
const headerTitle = computed(() => {
  if (dateRange.value && dateRange.value.team) {
    return teamNames[dateRange.value.team]
  }
  return ''
})
export interface SeriesEchartData {
  name: string
  type: string
  barMaxWidth?: number // 柱最大宽度
  barWidth?: number // 柱最大宽度
  yAxisIndex?: number
  data: Array<number>
  label?: object
}

export interface TotalMarkerAllModel {
  week?: string
  taskUser?: string
  userNameDisplay?: string
  count?: number
  miss?: number
  prod?: number
  utilz?: number
}

export interface StatusTotalCountModel {
  Status?: string
  TotalCount?: number
}

export interface SLADataModel {
  TimeRange?: string
  Count?: number
  CountPencent?: number
}

export interface StatusCodeTotalCountModel {
  makerComplete?: number
  checkerComplete?: number
  waitMaker?: number
  makerUrgent?: number
  waitChecker?: number
  checkerUrgent?: number
  baseline?: number
  totalFileCount?: number
  totalFileCount2?: number
  baselineRate?: number
}

import '../../assets/css/reset.css'
import '../../assets/css/custom-animation.css'
import '../../assets/css/index.css'
// 引入组件
import leftTop from '@/views/ScreenDesign2/component/LeftTop.vue'
import leftCenter from '@/views/ScreenDesign2/component/LeftCenter.vue'
import leftBottom from '@/views/ScreenDesign2/component/LeftBottom.vue'
import rightTop from '@/views/ScreenDesign2/component/RightTop.vue'
import rightCenter from '@/views/ScreenDesign2/component/RightCenter.vue'
import rightBottom from '@/views/ScreenDesign2/component/RightBottom.vue'
import centerContent from '@/views/ScreenDesign2/component/CenterContent.vue'
import {computed, nextTick, onBeforeUnmount, ref, reactive, onMounted} from "vue"
import {type DateRangeDTO, useSettingStore} from "@/stores"
import {FormInstance, FormRules} from "element-plus";
import {teamNames} from "@/views/ScreenDesign2/component/consts";

const settingStore = useSettingStore()
const {dateRange} = storeToRefs(settingStore);
const leftCenterComponent2 = ref()
const leftBottomComponent2 = ref()
const rightTopComponent2 = ref()
const cnetentComponent2 = ref()
const timer = ref()
const dataTimer = ref()
const dataInterval = ref(10000)
// const configDialogShow = computed(() => !dateRange.value)
const configData = ref<DateRangeDTO>({
  team: '0',
  group: '0',
  interval: 0,
  peroid: '日',
  cycle: '日'
})
// const configRef = ref<FormInstance>()
// const rules = reactive<FormRules<DateRangeDTO>>({
//   databaseName: [{required: true, message: 'Please select database', trigger: ['change', 'blur']}],
//   dateArr: [{required: true, message: 'Please select date range', trigger: 'blur'}],
// })
// 组件实现出场动画
nextTick(() => {
  //  头部
  playAnimation('#item88b51490-3e05-11ec-a2e6-953464dcce14', [
    {
      type: 'fadeInDown',
      name: '向下移入',
      delayed: 0,
      loop: false,
      frequency: 1,
      duration: 1,
      isPlayer: false,
      isDisabled: false
    }
  ])
  //  左
  playAnimation('#item8389fda0-3e05-11ec-a2e6-953464dcce14', [
    {
      type: 'fadeInLeft',
      name: '向右移入',
      delayed: 0,
      loop: false,
      frequency: 1,
      duration: 1,
      isPlayer: false,
      isDisabled: false
    }
  ])
  // 中间
  playAnimation('#itemc2579760-3e0d-11ec-9df4-83999b721ef6', [
    {
      type: 'bounceInDown',
      name: '向下弹入',
      delayed: 0.5,
      loop: false,
      frequency: 1,
      duration: 1,
      isPlayer: false,
      isDisabled: false
    }
  ])
  // 右
  playAnimation('#item7a111060-3e05-11ec-a2e6-953464dcce14', [
    {
      type: 'fadeInRight',
      name: '向左移入',
      delayed: 0,
      loop: false,
      frequency: 1,
      duration: 1,
      isPlayer: false,
      isDisabled: false
    }
  ])

  /**
   *
   * @param {*} id
   * @param {*} animations
   * 声明初始值
   * 判断动画是否结束
   */
  function playAnimation(id: string, animations: any[]) {
    let index = 0
    const ele = document.querySelector(id)
    setAnimation(ele, animations[index])
    ele?.addEventListener('webkitAnimationEnd', function () {
      if (index < animations.length - 1) {
        index++
        setAnimation(ele, animations[index])
      }
    })
  }

  /**
   *
   * @param {*} element
   * @param {*} animation
   * 取index对应的动画
   * 给元素animation重新赋值
   */
  function setAnimation(element: any, animation: any) {
    const action = `${animation.duration * 1000}ms ease ${animation.delayed * 1000}ms ${
        animation.loop ? 'infinite' : animation.frequency
    } normal both running ${animation.type}`
    element.style.animation = action
  }

  // 自适应宽度处理
  (document.getElementsByClassName('containerSelf')[0] as any).style.zoom = document.body.clientWidth / 1920

  // 窗口发生改变自动刷新页面
  function fandou(time: number) {
    let timeout: any = null
    return function () {
      clearTimeout(timeout)
      timeout = setTimeout(() => {
        location.reload()
      }, time)
    }
  }

  window.onresize = fandou(1000)
})
onBeforeUnmount(() => {
  // 销毁定时器
  clearInterval(timer.value)
  clearInterval(dataTimer.value)
  console.log("// 销毁定时器", dataTimer.value)
})

onMounted(() => {
  // settingStore.setDateRange({...configData.value})
  dataTimer.value = setInterval(() => {
    dateRange.value!.interval ++;
    console.log("dateRange.value", dateRange.value)
  }, dataInterval.value); 
})
// 设置定时切换
/*
减速运动
@param eleObj:元素对象
@param styleObj:样式对象集合  属性：目标值
@param interval:时间间隔
@param callBack:回调函数
*/
const bufferMove =
    (eleObj: any, styleObj: any, interval: any, callBack: any) => {
      window.clearInterval(eleObj.timer)
      eleObj.timer = window.setInterval(function () {
        // 默认值是true
        var flag = true
        for (var styleAttr in styleObj) {
          const getStyle = eleObj.currentStyle
              ? eleObj.currentStyle[styleAttr]
              : (window.getComputedStyle(eleObj) as any)[styleAttr]
          var cur = styleAttr === 'opacity' ? parseFloat((getStyle * 100).toString()) : parseFloat(getStyle)
          var speed = (styleObj[styleAttr] - cur) / 10
          speed = speed > 0 ? Math.ceil(speed) : Math.floor(speed)

          if (cur !== styleObj[styleAttr]) {
            flag = false
          }

          if (styleAttr === 'opacity') {
            eleObj.style.opacity = (cur + speed) / 100
            eleObj.style.filter = 'alpha(opacity=' + (cur + speed) + ')'
          } else {
            eleObj.style[styleAttr] = cur + speed + 'px'
          }
        }

        if (flag) {
          window.clearInterval(eleObj.timer)
          callBack && callBack.call(eleObj)
        }
      }, interval)
    }
// const setDataConfig = (form: FormInstance) => {
//   form.validate((valid) => {
//     if (valid) {
//       settingStore.setDateRange({...configData.value})
//       dataTimer.value = setInterval(() => {
//         dateRange.value!.interval ++;
//         console.log("dateRange.value", dateRange.value)
//       }, dataInterval.value); 
//     }
//   })
// }
</script>

<style scoped>
.containerSelf {
  z-index: 9;
  background: rgba(5, 11, 22, 1) url(../../assets/images/bg.jpg) no-repeat;
  background-size: 100% 100%;
}

.centertitle h1 {
  margin-top: 10px;
  font-size: 26px;
}

/* 左边外框 */
#item8389fda0-3e05-11ec-a2e6-953464dcce14 {
  height: 980px;
}

/* 右边外框 */
#item7a111060-3e05-11ec-a2e6-953464dcce14 {
  height: 980px;
}

#item4ec4a890-3dec-11ec-a2e6-953464dcce14 {
  top: 6px;
}

#itemc2579760-3e0d-11ec-9df4-83999b721ef6 {
  top: 90px;
  height: 940px;
}

.el-dialog__body {
    color: #ffffff !important;
    font-size: var(--el-dialog-content-font-size);
}

/* 关键样式 */
.custom-bg-table {
  position: relative;
  overflow: hidden;
  color: white;
}

/* 通过伪元素添加背景图 */
.custom-bg-table::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  width: 120%;
  height: 120%;
  background-image: url('../../assets/images/bg.jpg'); /* 替换为你的图片路径 */
  background-size: cover;
  background-repeat: no-repeat;
  opacity: 0.89;
  z-index: 1;
}

/* 调整表格内容的层级 */
.custom-bg-table .el-table__inner-wrapper,
.custom-bg-table .el-table__header-wrapper,
.custom-bg-table .el-table__body-wrapper {
  position: relative;
  z-index: 1; /* 确保内容在背景之上 */
  color: #ffffff;
}

/* 处理滚动区域 */
.custom-bg-table .el-table__body-wrapper {
  background-color: transparent !important;
  color: #ffffff;
}

.custom-title {
  color: rgb(255, 255, 255);  /* 蓝色 */
  margin: 0;
}
/* 
.el-table tr:hover {
  color: red;
} */

:deep(.el-table__body tr:hover > td)  {
  background-color: #1e56a0 !important; /* 修改为你想要的颜色 */
}

:deep(.el-dialog__headerbtn .el-dialog__close) {
  color: #fff !important; 
}

</style>
