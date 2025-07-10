<template>
  <div>
    <!-- 右下标题开始-->
    <div
        id="itemd0cac130-3e05-11ec-a2e6-953464dcce14"
        :style="{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        fontSize: '18px',
        lineHeight: '1.5',
        color: '#ffffff',
        textAlign: 'center',
        textShadow: false ? '0px 0px 14px rgba(15, 146, 255,1)' : '',
      }"
        class="centertitle"
    >
      <p>Attention</p>
    </div>
    <!-- 右下标题结束-->

    <!-- 右下echarts开始-->
    <div id="item93b724c0-3e03-11ec-a2e6-953464dcce14"  class="left_boottom_wrap beautify-scroll-def" :class="{ 'overflow-y-auto': !indexConfig.leftBottomSwiper }">
      <component
          :is="comName"
          :list="state.list"
          v-model="state.scroll"
          :singleHeight="state.defaultOption.singleHeight"
          :step="state.defaultOption.step"
          :limitScrollNum="state.defaultOption.limitScrollNum"
          :hover="state.defaultOption.hover"
          :singleWaitTime="state.defaultOption.singleWaitTime"
          :wheel="state.defaultOption.wheel"
      >
        <ul class="left_boottom">
          <li class="left_boottom_item" v-for="(item, i) in state.list" :key="i">
            <span class="orderNum doudong">{{ i + 1 }}</span>
            <div class="inner_right">
              <div class="dibu"></div>
              <div class="flex">
                <div class="info">
                  <span class="labels">Subject：</span>
                  <span class="text-content zhuyao doudong wangguan">
                  {{ item.subject }}</span
                  >
                </div>
                <!-- <div class="info">
                  <span class="labels">时间：</span>
                  <span class="text-content" style="font-size: 12px">
                  {{ item.createTime }}</span
                  >
                </div> -->
              </div>

              <!-- <span
                  class="types doudong"
                  :class="{
                typeRed: item.onlineState == 0,
                typeGreen: item.onlineState == 1,
              }"
              >{{ item.onlineState == 1 ? "上线" : "下线" }}</span> -->

              <div class="info addresswrap">
                <span class="labels">Message：</span>
                <span class="text-content ciyao" style="font-size: 12px">
                  {{ item.message }}</span>
                <!-- {{ addressHandle(item) }}</span> -->
              </div>
            </div>
          </li>
        </ul>
      </component>
    </div>
    <!-- 右下echarts结束-->

    <!-- 右下背景图开始-->
    <div id="item71b4b580-3deb-11ec-a2e6-953464dcce14">
      <img src="../../../assets/images/线框.png" alt="" :style="{ width: '100%', height: '100%' }"/>
    </div>
    <!-- 右下背景图结束-->
  </div>
</template>

<script setup lang="ts">
import {reactive, ref, onMounted, type Ref, computed} from "vue";
import {SeriesEchartData} from "@/views/ScreenDesign2/ScreenDesign2.vue";
import {useSettingStore} from "@/stores";
import {storeToRefs} from "pinia";
import {currentGET} from "@/api";
import SeamlessScroll from "@/components/seamless-scroll";
import EmptyCom from "@/components/empty-com";
import useWatchDateRange from "@/hooks/use-watch";

const settingStore = useSettingStore();
const {defaultOption, indexConfig} = storeToRefs(settingStore);
const state = reactive<any>({
  list: [],
  defaultOption: {
    ...defaultOption.value,
    singleHeight: 156,
    limitScrollNum: 3,
  },
  scroll: true,
});
// const addressHandle = (item: any) => {
//   let name = item.provinceName;
//   if (item.cityName) {
//     name += "/" + item.cityName;
//     if (item.countyName) {
//       name += "/" + item.countyName;
//     }
//   }
//   return name;
// };
const getData = () => {
  currentGET("getDailyAttentionList", {OrganizationID: 2}).then((res) => {
    console.log("设备提醒", res);
    if (res.success) {
      state.list = res.data.list;
    }
  });
};
const comName = computed(() => {
  if (indexConfig.value.leftBottomSwiper) {
    return SeamlessScroll
  } else {
    return EmptyCom
  }
})

// onMounted(() => {
//   getData()
// })
// 模拟后端传来的数据
const resData = reactive([
  {
    companyName: '神州数码信息系统有限公司', // 厂商名称
    companyPencent: 0.046, // 厂商项目数量占比
    projectNums: 21 // 厂商项目数量
  },
  {
    companyName: '中国软件与技术服务股份有限公司',
    companyPencent: 0.0438,
    projectNums: 20
  },
  {
    companyName: '北京华宇信息技术有限公司',
    companyPencent: 0.0372,
    projectNums: 17
  },
  {
    companyName: '税友软件集团股份有限公司',
    companyPencent: 0.035,
    projectNums: 16
  },
  {
    companyName: '长城计算机软件与系统有限公司',
    companyPencent: 0.0306,
    projectNums: 14
  },
  {
    companyName: '京瓷办公信息系统（中国）有限公司',
    companyPencent: 0.0263,
    projectNums: 12
  },
  {
    companyName: '联想（北京）有限公司',
    companyPencent: 0.0219,
    projectNums: 10
  }
])
const showData = reactive<any[]>([]) // 用于展示的数据
// 处理接收数据，用于展示
const dealWithData = () => {
  resData.forEach((item, index) => {
    showData.push({
      no: index + 1,
      companyName: item.companyName,
      companyPencent: Number(item.companyPencent * 100).toFixed(2) + '%',
      projectNums: item.projectNums
    })
  })
}
const setEcharts = (datas: any) => {
  state.list = datas.result
}
useWatchDateRange('getDailyAttentionList', setEcharts)
</script>

<style scoped lang="scss">
/* 背景图位置及大小 */
#item71b4b580-3deb-11ec-a2e6-953464dcce14 {
  top: 640px;
  left: 20px;
  width: 530px;
  height: 310px;
}

/* 标题位置 */
#itemd0cac130-3e05-11ec-a2e6-953464dcce14 {
  top: 654px;
}

#item93b724c0-3e03-11ec-a2e6-953464dcce14 {
  top: 694px;
}

.projectNumTable {
  width: 100%;
  height: 86%;
  font-size: 16px;
  color: #fff;
}

.projectNumTable thead tr th:nth-child(1) {
  width: 60px;
}

.projectNumTable thead tr th:nth-child(2) {
  text-align: left;
}

.projectNumTable tbody tr td {
  text-align: center;
}

.projectNumTable tbody tr td:nth-child(2) {
  text-align: left;
}

.projectNumTable tbody tr {
  background: linear-gradient(to right, rgba(28, 103, 161, 1), rgba(28, 103, 161, 0.1));
}

.projectNumTable tbody tr:nth-child(2n) {
  background: linear-gradient(to right, rgba(4, 36, 94, 1), rgba(4, 36, 94, 0.1));
}

.left_boottom_wrap {
  overflow: hidden;
  width: 100%;
  height: 100%;
}

.doudong {
  overflow: hidden;
  backface-visibility: hidden;
}

.overflow-y-auto {
  overflow-y: auto;
}

.left_boottom {
  width: 100%;
  height: 100%;

  .left_boottom_item {
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 8px;
    font-size: 14px;
    margin: 10px 0;

    .orderNum {
      color: #fff;
      margin: 0 16px 0 -20px;
    }

    .info {
      margin-right: 10px;
      display: flex;
      align-items: center;
      color: #fff;

      .labels {
        flex-shrink: 0;
        font-size: 12px;
        color: rgba(255, 255, 255, 0.6);
      }

      .zhuyao {
        color: $primary-color;
        font-size: 15px;
      }

      .ciyao {
        color: rgba(255, 255, 255, 0.8);
      }

      .warning {
        color: #e6a23c;
        font-size: 15px;
      }
    }

    .inner_right {
      position: relative;
      height: 100%;
      width: 380px;
      flex-shrink: 0;
      line-height: 1;
      display: flex;
      align-items: center;
      justify-content: space-between;
      flex-wrap: wrap;

      .dibu {
        position: absolute;
        height: 2px;
        width: 104%;
        background-image: url("@/assets/img/zuo_xuxian.png");
        bottom: -10px;
        left: -2%;
        background-size: cover;
      }

      .addresswrap {
        width: 100%;
        display: flex;
        margin-top: 8px;
      }
    }

    .wangguan {
      color: #1890ff;
      font-weight: 900;
      font-size: 15px;
      width: 80px;
      flex-shrink: 0;
    }

    .time {
      font-size: 12px;
      // color: rgba(211, 210, 210,.8);
      color: #fff;
    }

    .address {
      font-size: 12px;
      cursor: pointer;
      // @include text-overflow(1);
    }

    .types {
      width: 30px;
      flex-shrink: 0;
    }

    .typeRed {
      color: #fc1a1a;
    }

    .typeGreen {
      color: #29fc29;
    }
  }
}
</style>
