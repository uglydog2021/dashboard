<template>
  <div>
    <!-- 右中标题开始-->
    <div
      id="itemd00a0940-3e05-11ec-a2e6-953464dcce14"
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
      <p>Resource Plan</p>
    </div>
    <!-- 右中标题结束-->

    <!-- 右中echarts开始-->
    <div id="item40b1de80-3e05-11ec-a2e6-953464dcce14">
      <v-chart
        class="chart"
        :option="option"
        v-if="JSON.stringify(option) != '{}'"
      />
    </div>
    <!-- 右中echarts结束-->

    <!-- 右中背景图开始-->
    <div id="item677df680-3deb-11ec-a2e6-953464dcce14">
      <img
        src="../../../assets/images/线框.png"
        alt=""
        :style="{ width: '100%', height: '100%' }"
      />
    </div>
    <!-- 右中背景图结束-->
  </div>
</template>

<script setup lang="ts">
import { reactive, ref, onMounted, type Ref } from "vue";
import { TotalMarkerAllModel } from "@/views/ScreenDesign2/ScreenDesign2.vue";
import { EChartsOption, SeriesOption } from "echarts/types/dist/shared";
import useWatchDateRange from "@/hooks/use-watch";

const option = ref<EChartsOption>({});
const setEcharts = (datas: any) => {
  // 对数据进行处理
  const showEchartsData: (SeriesOption & { data: number[] })[] = [
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
      name: "Capacity",
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
  const xData: any[] = [];
  showEchartsData[0].data = [];
  showEchartsData[1].data = [];
  if (datas) {
    const res = datas.result;
    const chartDatas: TotalMarkerAllModel[] = [];
    const samePeriod = res.samePeriod;
    if (samePeriod && samePeriod.length > 0) {
      samePeriod.forEach((item: any) => {
        chartDatas.push({
          week: item.WeekOfYear,
          count: item.TotalItemCount,
        });
      });
    }
    const attendanceRate = res.attendanceRate;
    attendanceRate.forEach((item: any) => {
      const orginal = chartDatas.find((t: any) => item.Week == t.week);
      if (orginal) {
        orginal.miss = item.Capacity;
      } else {
        chartDatas.push({
          week: item.Week,
          miss: item.Capacity,
        });
      }
    });
    chartDatas
      .sort((a, b) => Number(a.week) - Number(b.week))
      .forEach((item) => {
        xData.push(item.week);
        showEchartsData[0].type = "bar";
        showEchartsData[0].data.push(item.count ?? 0);
        showEchartsData[1].type = "line";
        showEchartsData[1].data.push(item.miss ?? 0);
      });
  }
  option.value = {
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
      data: ["件数", "Capacity"],
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
        splitNumber: 4, // 大致分为 5 段
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
        name: "Capacity",
        // interval: 5,
        axisLabel: {
          formatter: "{value}",
        },
        splitLine: { show: false }, // 关闭网格线
      },
    ],
    series: showEchartsData,
  };
};
useWatchDateRange("getNumberItemsSamePeriodLastYear", setEcharts);
</script>

<style scoped>
/* 背景位置及大小 */
#item677df680-3deb-11ec-a2e6-953464dcce14 {
  top: 320px;
  left: 20px;
  width: 530px;
  height: 310px;
}

/* 标题位置 */
#itemd00a0940-3e05-11ec-a2e6-953464dcce14 {
  top: 334px;
}

#item40b1de80-3e05-11ec-a2e6-953464dcce14 {
  top: 378px;
  width: 520px;
  height: 268px;
}
</style>
