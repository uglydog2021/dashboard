<template>
  <div>
    <!-- 右上标题开始-->
    <div
      id="item9c61daa0-3e05-11ec-a2e6-953464dcce14"
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
      <p>做单量统计</p>
    </div>
    <!-- 右上标题结束-->

    <!-- 右上echarts开始-->
    <!-- <div id="item62c8d410-3e05-11ec-a2e6-953464dcce14"></div> -->
    <div id="leftBottomShowWindow3">
      <div class="innerBox">
        <v-chart
          id="leftBottomEchartsBox31"
          class="chart"
          :option="option1"
          v-if="JSON.stringify(option1) != '{}'"
        />
      </div>
    </div>
    <!-- 右上echarts结束-->

    <!-- 右上背景图开始-->
    <div id="item63811bd0-3dea-11ec-a2e6-953464dcce14">
      <img
        src="../../../assets/images/线框.png"
        alt=""
        :style="{ width: '100%', height: '100%' }"
      />
    </div>
    <!-- 右上背景图结束-->
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import useWatchDateRange from "@/hooks/use-watch";
import { EChartsOption, SeriesOption } from "echarts/types/dist/shared";

type DepartmentMonthCurrentYear = {
  Month: number;
  TimePeriod: string;
  TaskCount: number;
  JJCount: number;
};

type ResultData = {
  departmentMonthCurrentYear: DepartmentMonthCurrentYear[];
  missRate: DepartmentMonthCurrentYear[];
};
type ChartData = {
  Month: string;
  totalCount: string;
  rate: string;
};
const option1 = ref<EChartsOption>({});
const setEcharts = (datas: any) => {
  // 根据flag不同定义不同的数据用来echarts的展示
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
      name: "正确率",
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

  const xData: string[] = [];
  showEchartsData[0].data = [];
  showEchartsData[1].data = [];
  if (datas) {
    const res: ResultData = datas.result;
    const departmentMonthCurrentYear = res.departmentMonthCurrentYear;
    const missRate = res.missRate;
    const map = new Map<string, ChartData>();
    const chartDatas: ChartData[] = [];
    departmentMonthCurrentYear.forEach((item) => {
      const Month = `${item.Month}`;
      const chartData: ChartData = {
        Month,
        totalCount: `${item.JJCount}`,
        rate: '0',
      };
      map.set(Month, chartData);
      chartDatas.push(chartData);
    });
    missRate.forEach((item) => {
      const Month = `${item.Month}`;
      const JJCount = item.JJCount;
      if (JJCount !== 0) {
        if (map.has(Month)) {
          const chartData = map.get(Month);
          chartData!.rate = ((item.TaskCount * 100) / JJCount).toFixed(2);
        }
      }
    });
    chartDatas.forEach((item) => {
      xData.push(item.Month);
      showEchartsData[0].type = "bar";
      showEchartsData[0].data.push(item.totalCount);
      showEchartsData[1].type = "line";
      showEchartsData[1].data.push(item.rate);
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
      data: ["件数", "正确率"],
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
          rotate: 0, // x轴字体倾斜角度
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
        name: "正确率",
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
useWatchDateRange("getDepartmentMonthCurrentYear", setEcharts);
/*
  banner滚动
  @param n: 当前展示的第几张图，从0开始
  @param bufferMove: 函数(能够实现元素的减速运动)
*/
// const changeBanner =
//     (n: number, bufferMove: Function) => {
//       const bannerWrap: HTMLElement = document.querySelector('#leftBottomShowWindow3 .innerBox')!
//       if (n === -1) {
//         bannerWrap.style.left = '-1000px'
//         n = 1
//       }
//       bufferMove(
//           bannerWrap,
//           {
//             left: -n * 500
//           },
//           30
//       )
//     }
// defineExpose({changeBanner})
</script>

<style scoped>
/* 背景图位置及大小 */
#item63811bd0-3dea-11ec-a2e6-953464dcce14 {
  left: 20px;
  width: 530px;
  height: 310px;
}

/* 标题位置 */
#item9c61daa0-3e05-11ec-a2e6-953464dcce14 {
  top: 14px;
}

#leftBottomShowWindow3 {
  position: absolute;
  left: 35px;
  top: 60px;
  width: 500px;
  height: 270px;
  overflow: hidden;
  z-index: 10;
}

#leftBottomShowWindow3 .innerBox {
  display: flex;
  position: absolute;
  top: 0;
  height: 270px;
  width: 500px;
}

#leftBottomShowWindow3 .innerBox div {
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

#leftBottomEchartsBox31 {
  left: 0px;
}

#leftBottomEchartsBox32 {
  left: 500px;
}

#leftBottomEchartsBox33 {
  left: 1000px;
}
</style>
