<template>
  <div>
    <!-- 左下标题头 -->
    <!-- 组件d86d3f90-3deb-11ec-a2e6-953464dcce14开始-->
    <div
      id="itemd86d3f90-3deb-11ec-a2e6-953464dcce14"
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
      <p>个人performance</p>
    </div>
    <!-- 组件d86d3f90-3deb-11ec-a2e6-953464dcce14结束-->
    <!-- 左下echart图展示区域 -->
    <!-- 组件57e5c2f0-3e1a-11ec-9ac5-c10a0ae21a86开始-->
    <!-- <div id="item57e5c2f0-3e1a-11ec-9ac5-c10a0ae21a86"></div> -->
    <div id="leftBottomShowWindow2">
      <div
        ref="periodSelectContainer"
        class="selectContainer"
        @click="showPeriodSelect = true"
      >
        <div class="periodContainer">
          <span style="padding-left: 58%">{{
            periodOption.find((t) => displayType == t.value)?.label
          }}</span>
        </div>
        <div
          ref="popover"
          v-if="showPeriodSelect"
          tabindex="-1"
          style="padding: 3px; background-color: rgb(21 65 99 / 54%)"
          class="pop-container"
        >
          <el-tree
            ref="selectTree"
            style="width: 100%"
            :data="periodOption"
            :highlight-current="true"
            :default-expand-all="true"
            node-key="value"
            :current-node-key="displayType"
            :indent="10"
            @node-click="handleNodeClick"
          />
        </div>
      </div>
      <div class="innerBox">
        <v-chart
          id="leftBottomEchartsBox21"
          class="chart"
          :option="option1"
          v-if="JSON.stringify(option1) != '{}'"
        />
      </div>
    </div>
    <!-- 组件57e5c2f0-3e1a-11ec-9ac5-c10a0ae21a86结束-->
    <!-- 左下容器背景图 -->
    <!-- 组件85e23400-9797-4236-9be8-186a0b1f24e2开始-->
    <div id="item85e23400-9797-4236-9be8-186a0b1f24e2">
      <img
        src="../../../assets/images/线框.png"
        alt=""
        :style="{ width: '100%', height: '100%' }"
      />
    </div>
    <!-- 组件85e23400-9797-4236-9be8-186a0b1f24e2结束-->
  </div>
</template>

<script setup lang="ts">
import { nextTick, ref, watch } from "vue";
import useWatchDateRange from "@/hooks/use-watch";
import { EChartsOption, SeriesOption } from "echarts/types/dist/shared";
import { useSettingStore } from "@/stores";
import { storeToRefs } from "pinia";
import { periodOption } from "@/views/ScreenDesign2/component/consts";
import { SystemSetting } from "@/api/models";
import { getSystemSetting } from "@/utils";

type Utilization = {
  OrganizationID: number;
  User_Name: string;
  ReportingDate: string;
  MeetingDuration: number;
  TrainingDuration: number;
  BreakDuration: number;
};
type Productivity = {
  OrganizationID: number;
  User_Name: string;
  ReportingDate: string;
  TimeCycle: string;
  OCRMakerVolum: number;
  OCRCheckerVolum: number;
  TotalVolum: number;
  NotNullCount: number;
  OCRMakerHour: number;
  OCRCheckerHour: number;
};
type ResultData = {
  utilization: Utilization[];
  productivity: {
    productivity: Productivity[];
    systemSettings: SystemSetting[];
  };
};
type ChartData = {
  User_Name: string;
  productivity: string;
  utilization: string;
};
const settingStore = useSettingStore();
const { dateRange } = storeToRefs(settingStore);
const periodSelectContainer = ref<HTMLDivElement | null>(null);
const showPeriodSelect = ref(false);
const displayType = ref("Day");
const showEchartsData: (SeriesOption & { data: string[] })[] = [
  {
    name: "Productivity",
    type: "bar",
    barMaxWidth: 30, // 柱图宽度
    data: [],
    label: {
      // 显示数值的配置
      // show: true, // 开启显示
      // position: 'top', // 数值显示在柱形上方
      formatter: "{c}", // 显示数据值，{c} 表示数据值
      color: "white",
      fontSize: "8px",
    },
  },
  {
    name: "Utilization",
    type: "line",
    yAxisIndex: 1, // 关键：绑定到第二个 y 轴
    data: [],
    label: {
      // 显示数值的配置
      // show: true, // 开启显示
      // position: 'top', // 数值显示在柱形上方
      formatter: "{c}%", // 显示数据值，{c} 表示数据值
      color: "white",
      fontSize: "8px",
    },
  },
];
const handleNodeClick = async (
  data: any,
  node: any,
  treeNode: any,
  e: Event
) => {
  if (displayType.value != data.value) {
    displayType.value = dateRange.value!.cycle = data.value;
    // const res = await currentGET('GetCheckerTotalCount',  { cycle: dateRange.value!.cycle,
    //   StartData: dateRange.value!.dateArr![0],
    //   EndData: dateRange.value!.dateArr![1],
    //   databaseName: dateRange.value!.databaseName,
    //   group: dateRange.value!.group,
    //   team: dateRange.value!.team
    // })
    // setEcharts(res)
  }
  showPeriodSelect.value = false;
  e.preventDefault();
};

const handleClickOutside = (e: MouseEvent) => {
  if (
    periodSelectContainer.value &&
    !periodSelectContainer.value.contains(e.target as Node)
  ) {
    showPeriodSelect.value = false;
  }
};

watch(showPeriodSelect, async (newVal: any) => {
  if (newVal) {
    await nextTick();
    document.addEventListener("click", handleClickOutside);
  } else {
    document.removeEventListener("click", handleClickOutside);
  }
});

const option1 = ref<EChartsOption>({});
/*
  创建echarts图
  @param id: 要画echarts的容器
  @param flag: 统计项目的内容
  - flag = 1 : 统计内容为项目数量柱状图
  - flag = 0 : 统计内容为项目预算柱状图、中标金额柱状图和资金节约率折线图
*/
const setEcharts = (datas: any) => {
  // 根据id如果是图2展示项目数量和中标项目预算，否则展示中标金额和资金节约率
  // const showEchartsData: (SeriesOption & { data: number[] })[] = [
  //   {
  //     name: '作业量',
  //     type: 'bar',
  //     barWidth: 30, // 柱图宽度
  //     data: [],
  //     label: { // 显示数值的配置
  //       show: true, // 开启显示
  //       position: 'top', // 数值显示在柱形上方
  //       formatter: '{c}', // 显示数据值，{c} 表示数据值
  //       color: 'white'
  //     }
  //   }TotalMarkerAllModel
  // ]
  if (!datas) {
    return;
  }
  const res: ResultData = datas.result;
  // const rel = res.filter(t => t.TimeCycle == displayType.value)
  const systemSettings = res.productivity.systemSettings;
  const systemConfig = getSystemSetting(systemSettings);
  const productivity = res.productivity.productivity;
  const utilization = res.utilization;
  const xData: string[] = [];
  showEchartsData[0].data = [];
  showEchartsData[1].data = [];
  const nameMap = new Map<string, ChartData>();
  const chartDatas: ChartData[] = [];
  for (const p of productivity) {
    let makerVal = 0;
    if (p.TotalVolum !== 0 && p.OCRMakerHour !== 0) {
      makerVal =
        (systemConfig.standard_maker_time * 100) /
        (p.OCRMakerHour / p.TotalVolum);
    }
    let checkeVal = 0;
    if (p.TotalVolum !== 0 && p.OCRCheckerHour !== 0) {
      checkeVal =
        (systemConfig.standard_checker_time * 100) /
        (p.OCRCheckerHour / p.TotalVolum);
    }
    const pVal = ((makerVal + checkeVal) / 2).toFixed(2);
    const chartData: ChartData = {
      User_Name: p.User_Name,
      productivity: pVal,
      utilization: "0",
    };
    chartDatas.push(chartData);
    nameMap.set(p.User_Name, chartData);
  }
  for (const p of utilization) {
    const userName = p.User_Name;
    const down = 8 - p.MeetingDuration - p.TrainingDuration - p.BreakDuration;
    let pVal = "0";
    if (down !== 0) {
      pVal = (
        ((systemConfig.standard_maker_time +
          systemConfig.standard_checker_time) *
          100) /
        down
      ).toFixed(2);
    }

    if (nameMap.has(userName)) {
      const chartData = nameMap.get(userName);
      chartData!.utilization = pVal;
    } else {
      const chartData: ChartData = {
        User_Name: userName,
        productivity: "0",
        utilization: pVal,
      };
      chartDatas.push(chartData);
      nameMap.set(userName, chartData);
    }
  }
  chartDatas.forEach((item) => {
    xData.push(item.User_Name);
    showEchartsData[0].type = "bar";
    showEchartsData[0].data.push(item.productivity);
    showEchartsData[1].type = "line";
    showEchartsData[1].data.push(item.utilization);
  });
  option1.value = {
    tooltip: {
      trigger: "axis",
      axisPointer: {
        type: "cross",
        crossStyle: {
          color: "#999",
        },
      },
    },
    legend: {
      data: ["Productivity", "Utilization"],
      textStyle: {
        color: "#fff",
      },
    },
    xAxis: [
      {
        type: "category",
        data: xData,
        axisPointer: {
          type: "shadow",
        },
        axisLabel: { interval: 0, rotate: 45 },
      },
    ],
    yAxis: [
      {
        type: "value",
        name: "Productivity",
        splitNumber: 5, // 大致分为 5 段
        // interval: 50,
        axisLabel: {
          formatter: "{value}",
        },
        splitLine: {
          show: true, // 显示左侧网格线
        },
      },
      {
        type: "value",
        name: "Utilization",
        // interval: 5,
        axisLabel: {
          formatter: "{value} %",
        },
        splitLine: { show: false }, // 关闭网格线
      },
    ],
    series: showEchartsData,
  };
};
useWatchDateRange("GetCheckerTotalCount", setEcharts);
/*
  banner滚动
  @param n: 当前展示的第几张图，从0开始
  @param bufferMove: 函数(能够实现元素的减速运动)
*/
const changeBanner = (n: number, bufferMove: Function) => {
  const bannerWrap: HTMLElement = document.querySelector(
    "#leftBottomShowWindow2 .innerBox"
  )!;
  if (n === 3) {
    bannerWrap.style.left = "0";
    n = 1;
  }
  bufferMove(
    bannerWrap,
    {
      left: -n * 450,
    },
    30
  );
};
defineExpose({ changeBanner });
</script>

<style scoped>
/* 背景图位置及大小 */
#item85e23400-9797-4236-9be8-186a0b1f24e2 {
  top: 640px;
  left: 20px;
  width: 530px;
  height: 310px;
}

/* 标题位置 */
#itemd86d3f90-3deb-11ec-a2e6-953464dcce14 {
  top: 654px;
}

#leftBottomShowWindow2 {
  position: absolute;
  left: 45px;
  top: 690px;
  width: 500px;
  height: 270px;
  overflow: hidden;
  z-index: 10;
}

#leftBottomShowWindow2 .innerBox {
  display: flex;
  position: absolute;
  top: 10px;
  left: 0px;
  height: 270px;
  width: 500px;
}

#leftBottomShowWindow2 .innerBox div {
  position: absolute;
  top: 0;
  width: 500px;
  height: 270px;
  z-index: 8;
  opacity: 1;
  transform: rotate(0deg);
  background: rgba(255, 255, 255, 0);
  border: 0px solid #2ed64a;
  border-radius: 0px;
  padding: 0px;
}

#leftBottomEchartsBox21 {
  left: 0px;
}

#leftBottomEchartsBox22 {
  left: 450px;
}

#leftBottomEchartsBox23 {
  left: 900px;
}

.selectContainer {
  z-index: 11;
  display: block;
  position: absolute;
  right: 0;
  top: 0;
  padding-right: 10px;
  color: white;
}

.dropdown-menu {
  border: 1px solid red;
}

.el-tree {
  background-image: url(/src/assets/images/bg.jpg);
  background-size: 1000% 1000%;
  background-repeat: no-repeat;
  background-position: center;
  color: white;
  --el-tree-node-content-height: 28px;
  --el-tree-node-hover-bg-color: #195e93;
}

.periodContainer {
  /* border: 1px solid red;  */
  width: 46px;
  background-image: url(/src/assets/images/中蓝.png);
  background-size: 110% 150%;
  background-repeat: no-repeat;
  background-position: center;
  color: white;
  /* padding: 5px; */
  --el-tree-node-content-height: 28px;
  --el-tree-node-hover-bg-color: #195e93;
}
</style>
