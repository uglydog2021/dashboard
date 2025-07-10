<template>
  <div>
    <div
      id="item1550b630-3dec-11ec-a2e6-953464dcce14"
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

    <div id="leftCentershowWindow2">
      <div
        ref="selectContainer"
        class="selectContainer"
        @click="showSelect = true"
      >
        <div class="periodContainer">
          <span style="padding-left: 58%">{{
            periodOption.find((t) => displayType == t.value)?.label
          }}</span>
        </div>
        <div
          ref="popover"
          v-if="showSelect"
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
            :props="defaultProps"
            @node-click="handleNodeClick"
          />
        </div>
      </div>
      <div class="innerBox">
        <v-chart
          id="leftCenterEchartsBox21"
          class="chart"
          :option="option1"
          v-if="JSON.stringify(option1) != '{}'"
        />
      </div>
    </div>
    <div id="item0c892655-9dbb-497c-bec9-239cca87c34a">
      <img
        src="../../../assets/images/线框.png"
        alt=""
        :style="{ width: '100%', height: '100%' }"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { nextTick, ref, watch } from "vue";
import useWatchDateRange from "@/hooks/use-watch";
import { EChartsOption, SeriesOption } from "echarts/types/dist/shared";
import { useSettingStore } from "@/stores";
import { storeToRefs } from "pinia";
import { periodOption } from "@/views/ScreenDesign2/component/consts";

type MissCountPerformance = {
  Department: number;
  UserName: string;
  ReportingDate: string;
  TimePeriod: string;
  MissTaskCount?: number;
  MissJJCount?: number;
  JSTaskCount?: number;
  JSJJCount?: number;
};
const settingStore = useSettingStore();
const { dateRange } = storeToRefs(settingStore);
// import { TRAP_FOCUS_HANDLER } from "element-plus/es/directives/trap-focus"
const defaultProps = ref({
  children: "children",
  label: "label",
});
const selectContainer = ref<HTMLDivElement | null>(null);
const showSelect = ref(false);
const option1 = ref<EChartsOption>({});
const displayType = ref("Day");
const handleNodeClick = async (
  data: any,
  node: any,
  treeNode: any,
  e: Event
) => {
  if (displayType.value != data.value) {
    displayType.value = dateRange.value!.peroid = data.value;
    // const res = await currentGET('GetMakerTotalCount',  { period: dateRange.value!.peroid,
    //   group: dateRange.value!.group,
    //   team: dateRange.value!.team,
    // })
    // setEcharts(res)
  }
  showSelect.value = false;
  e.preventDefault();
};

const handleClickOutside = (e: MouseEvent) => {
  if (
    selectContainer.value &&
    !selectContainer.value.contains(e.target as Node)
  ) {
    showSelect.value = false;
  }
};

watch(showSelect, async (newVal: any) => {
  if (newVal) {
    await nextTick();
    document.addEventListener("click", handleClickOutside);
  } else {
    document.removeEventListener("click", handleClickOutside);
  }
});

const showEchartsData: (SeriesOption & { data: string[] })[] = [
  {
    name: "件数",
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
    name: "miss率",
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

const setEcharts = (datas: any) => {
  if (!datas) {
    return;
  }
  const res: MissCountPerformance[] = datas.result;
  const xData: any[] = [];
  showEchartsData[0].data = [];
  showEchartsData[1].data = [];
  if (res && res.length > 0) {
    res.forEach((item) => {
      xData.push(item.UserName);
      showEchartsData[0].type = "bar";
      showEchartsData[0].data.push(`${item.JSJJCount ?? '0'}`);
      showEchartsData[1].type = "line";
      const JSJJCount = item.JSJJCount ?? 0
      const MissJJCount = item.MissJJCount ?? 0
      let miss = '0'
      if (JSJJCount != 0) {
        miss = (MissJJCount * 100 / JSJJCount).toFixed(2)
      }
      showEchartsData[1].data.push(miss);
    });
  }
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
      data: ["件数", "miss率"],
      textStyle: {
        color: "#fff",
      },
    },
    grid: {
      containLabel: false, // 是否自适应x, y 轴文字的宽度
    },
    xAxis: [
      {
        type: "category",
        data: xData,
        axisPointer: {
          type: "shadow",
        },
        axisLabel: {
          interval: 0, // 全部展示
          rotate: 45, // x轴字体倾斜角度
        },
      },
    ],
    yAxis: [
      {
        type: "value",
        name: "件数",
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
        name: "miss率",
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
useWatchDateRange("GetMakerTotalCount", setEcharts);
/*
  banner滚动
  @param n: 当前展示的第几张图，从0开始
  @param bufferMove: 函数(能够实现元素的减速运动)
*/
// const changeBanner =
//     (n: number, bufferMove: Function) => {
//       const bannerWrap: HTMLElement = document.querySelector('.innerBox')!
//       if (n === 3) {
//         bannerWrap.style.left = '0'
//         n = 1
//       }
//       bufferMove(
//           bannerWrap,
//           {
//             left: -n * 450
//           },
//           30
//       )
//     }
// defineExpose({changeBanner})
</script>

<style scoped>
/* 背景图的位置及大小 */
#item0c892655-9dbb-497c-bec9-239cca87c34a {
  top: 320px;
  left: 20px;
  width: 530px;
  height: 310px;
}

/* 标题位置 */
#item1550b630-3dec-11ec-a2e6-953464dcce14 {
  top: 334px;
}

#leftCentershowWindow2 {
  position: absolute;
  left: 45px;
  top: 370px;
  width: 500px;
  height: 270px;
  overflow: hidden;
  z-index: 10;
}

#leftCentershowWindow2 .innerBox {
  display: flex;
  position: absolute;
  top: 8px;
  left: 0;
  height: 270px;
  width: 500px;
}

#leftCentershowWindow2 .innerBox div {
  position: absolute;
  top: 0px;
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

#leftCenterEchartsBox21 {
  left: 0px;
}

#leftCenterEchartsBox22 {
  left: 500px;
}

#leftCenterEchartsBox23 {
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
